using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using GunMobile.Res;
using UnityEngine;

namespace GunMobile.Logic
{
    public enum BattleEffectKind
    {
        DefenceFlat,
        DamagePercent,
        CritDamagePercent,
        DotMaxHpPercent,
        HotMaxHpPercent
    }

    [Serializable]
    public struct BattleEffect
    {
        public int ElementId;
        public int SourceSeat;
        public int TargetSeat;
        public BattleEffectKind Kind;
        public int Value;
        public int TurnsLeft;
    }

    public struct BattleDamageMods
    {
        public int AttackFlat;
        public int DefenceFlat;
        public float DamageMult;
        public float CritDamageAdd;
        public bool ForceCrit;
    }

    public sealed class BattleEffectTracker
    {
        readonly List<BattleEffect> _active = new List<BattleEffect>();

        public IReadOnlyList<BattleEffect> Active => _active;

        public void Clear()
        {
            _active.Clear();
        }

        public void Add(BattleEffect effect)
        {
            if (effect.TurnsLeft <= 0 && effect.Kind != BattleEffectKind.DotMaxHpPercent &&
                effect.Kind != BattleEffectKind.HotMaxHpPercent)
            {
                return;
            }

            _active.Add(effect);
        }

        public void AddRange(IEnumerable<BattleEffect> effects)
        {
            if (effects == null)
            {
                return;
            }

            foreach (BattleEffect e in effects)
            {
                Add(e);
            }
        }

        public BattleDamageMods GetMods(int seat)
        {
            var mods = new BattleDamageMods { DamageMult = 1f };
            for (int i = 0; i < _active.Count; i++)
            {
                BattleEffect e = _active[i];
                if (e.TurnsLeft <= 0)
                {
                    continue;
                }

                if (e.TargetSeat != seat)
                {
                    continue;
                }

                switch (e.Kind)
                {
                    case BattleEffectKind.DefenceFlat:
                        mods.DefenceFlat += e.Value;
                        break;
                    case BattleEffectKind.DamagePercent:
                        mods.DamageMult += e.Value / 100f;
                        break;
                    case BattleEffectKind.CritDamagePercent:
                        mods.CritDamageAdd += e.Value / 100f;
                        break;
                }
            }

            return mods;
        }

        public BattleDamageMods GetOutgoingMods(int seat)
        {
            var mods = new BattleDamageMods { DamageMult = 1f };
            for (int i = 0; i < _active.Count; i++)
            {
                BattleEffect e = _active[i];
                if (e.TurnsLeft <= 0 || e.SourceSeat != seat)
                {
                    continue;
                }

                if (e.Kind == BattleEffectKind.DamagePercent)
                {
                    mods.DamageMult += e.Value / 100f;
                }
            }

            return mods;
        }

        public LivingStats ApplyDefence(LivingStats stats, int seat)
        {
            BattleDamageMods mods = GetMods(seat);
            if (mods.DefenceFlat != 0)
            {
                stats.Defence = Mathf.Max(0, stats.Defence + mods.DefenceFlat);
            }

            return stats;
        }

        /// <summary>Tick start-of-turn HoT/DoT. Returns (seat, heal, dmg) pulses.</summary>
        public List<(int seat, int heal, int dmg)> TickTurn(LivingStats[] livings, int[] hp)
        {
            var pulses = new List<(int, int, int)>();
            if (livings == null || hp == null)
            {
                return pulses;
            }

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                BattleEffect e = _active[i];
                if (e.TurnsLeft <= 0)
                {
                    _active.RemoveAt(i);
                    continue;
                }

                int t = e.TargetSeat;
                if (t < 0 || t >= livings.Length || hp[t] <= 0)
                {
                    e.TurnsLeft--;
                    _active[i] = e;
                    if (e.TurnsLeft <= 0)
                    {
                        _active.RemoveAt(i);
                    }

                    continue;
                }

                if (e.Kind == BattleEffectKind.DotMaxHpPercent && e.Value > 0)
                {
                    int dmg = Mathf.Max(1, livings[t].MaxHp * e.Value / 100);
                    dmg = Mathf.Min(dmg, hp[t]);
                    if (dmg > 0)
                    {
                        pulses.Add((t, 0, dmg));
                    }
                }
                else if (e.Kind == BattleEffectKind.HotMaxHpPercent && e.Value > 0)
                {
                    int heal = Mathf.Max(1, livings[t].MaxHp * e.Value / 100);
                    pulses.Add((t, heal, 0));
                }

                e.TurnsLeft--;
                _active[i] = e;
                if (e.TurnsLeft <= 0)
                {
                    _active.RemoveAt(i);
                }
            }

            return pulses;
        }
    }

    public static class BattleEffectParser
    {
        static readonly Regex PercentRegex = new Regex(@"(\d+)\s*%", RegexOptions.Compiled);
        static readonly Regex FlatRegex = new Regex(@"(\d+)\s*点", RegexOptions.Compiled);
        static readonly Regex DurationRegex = new Regex(@"持续\s*(\d+)\s*回合", RegexOptions.Compiled);
        static readonly Regex DotRegex = new Regex(@"减少\s*(\d+)\s*%\s*生命.*?持续\s*(\d+)\s*回合", RegexOptions.Compiled);
        static readonly Regex HotTurnRegex = new Regex(@"每回合回复\s*(\d+)\s*%", RegexOptions.Compiled);
        static readonly Regex CritDmgRegex = new Regex(@"(\d+)\s*%\s*的\s*暴击伤害", RegexOptions.Compiled);

        public static List<BattleEffect> FromPetSkill(
            PetSkillInfo skill,
            IReadOnlyDictionary<int, PetSkillElementInfo> elements,
            int sourceSeat,
            int targetSeat)
        {
            var list = new List<BattleEffect>();
            if (skill == null)
            {
                return list;
            }

            int defaultTurns = ParseDuration(skill.Description, 2);

            if (TryParseFlat(skill.Description, "护甲", out int armour))
            {
                list.Add(new BattleEffect
                {
                    SourceSeat = sourceSeat,
                    TargetSeat = sourceSeat,
                    Kind = BattleEffectKind.DefenceFlat,
                    Value = armour,
                    TurnsLeft = defaultTurns
                });
            }

            Match hotTurn = HotTurnRegex.Match(skill.Description ?? "");
            if (hotTurn.Success &&
                int.TryParse(hotTurn.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int hotPct))
            {
                list.Add(new BattleEffect
                {
                    SourceSeat = sourceSeat,
                    TargetSeat = sourceSeat,
                    Kind = BattleEffectKind.HotMaxHpPercent,
                    Value = hotPct,
                    TurnsLeft = defaultTurns
                });
            }

            if (TryParsePercentAfter(skill.Description, "暴击伤害", out int critDmgPct) ||
                TryParseCritDamage(skill.Description, out critDmgPct))
            {
                list.Add(new BattleEffect
                {
                    SourceSeat = sourceSeat,
                    TargetSeat = sourceSeat,
                    Kind = BattleEffectKind.CritDamagePercent,
                    Value = critDmgPct,
                    TurnsLeft = defaultTurns
                });
            }

            if (skill.ElementIds == null)
            {
                return list;
            }

            foreach (int eid in skill.ElementIds)
            {
                if (eid <= 0 || elements == null || !elements.TryGetValue(eid, out PetSkillElementInfo el))
                {
                    continue;
                }

                string name = el.Name ?? "";
                string desc = el.Description ?? "";

                if (name.IndexOf("伤害增加", StringComparison.Ordinal) >= 0 &&
                    TryParsePercentFromText(name, out int dmgPct))
                {
                    list.Add(new BattleEffect
                    {
                        ElementId = eid,
                        SourceSeat = sourceSeat,
                        TargetSeat = sourceSeat,
                        Kind = BattleEffectKind.DamagePercent,
                        Value = dmgPct,
                        TurnsLeft = defaultTurns
                    });
                }

                if (name.IndexOf("护甲提升", StringComparison.Ordinal) >= 0 &&
                    TryParseFlatFromText(name, out int defFlat))
                {
                    list.Add(new BattleEffect
                    {
                        ElementId = eid,
                        SourceSeat = sourceSeat,
                        TargetSeat = sourceSeat,
                        Kind = BattleEffectKind.DefenceFlat,
                        Value = defFlat,
                        TurnsLeft = defaultTurns
                    });
                }

                Match dot = DotRegex.Match(desc);
                if (dot.Success &&
                    int.TryParse(dot.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int dotPct) &&
                    int.TryParse(dot.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int dotTurns))
                {
                    list.Add(new BattleEffect
                    {
                        ElementId = eid,
                        SourceSeat = sourceSeat,
                        TargetSeat = targetSeat,
                        Kind = BattleEffectKind.DotMaxHpPercent,
                        Value = dotPct,
                        TurnsLeft = dotTurns
                    });
                }

                if (desc.IndexOf("回复生命值", StringComparison.Ordinal) >= 0 &&
                    name.IndexOf("慢生回复", StringComparison.Ordinal) >= 0 &&
                    TryParsePercentFromText(skill.Description, out int regenPct))
                {
                    list.Add(new BattleEffect
                    {
                        ElementId = eid,
                        SourceSeat = sourceSeat,
                        TargetSeat = sourceSeat,
                        Kind = BattleEffectKind.HotMaxHpPercent,
                        Value = regenPct,
                        TurnsLeft = defaultTurns
                    });
                }
            }

            return list;
        }

        static int ParseDuration(string text, int fallback)
        {
            if (string.IsNullOrEmpty(text))
            {
                return fallback;
            }

            Match m = DurationRegex.Match(text);
            if (m.Success && int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) && n > 0)
            {
                return n;
            }

            return fallback;
        }

        static bool TryParseCritDamage(string text, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            Match m = CritDmgRegex.Match(text);
            if (!m.Success || !int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return false;
            }

            return value > 0;
        }

        static bool TryParseFlat(string text, string keyword, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            int idx = text.IndexOf(keyword, StringComparison.Ordinal);
            if (idx < 0)
            {
                return false;
            }

            return TryParseFlatFromText(text.Substring(idx), out value);
        }

        static bool TryParsePercentAfter(string text, string keyword, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            int idx = text.IndexOf(keyword, StringComparison.Ordinal);
            if (idx < 0)
            {
                return false;
            }

            return TryParsePercentFromText(text.Substring(idx), out value);
        }

        static bool TryParsePercentFromText(string text, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            Match m = PercentRegex.Match(text);
            if (!m.Success || !int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return false;
            }

            return value > 0;
        }

        static bool TryParseFlatFromText(string text, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            Match m = FlatRegex.Match(text);
            if (!m.Success || !int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return false;
            }

            return value > 0;
        }
    }
}
