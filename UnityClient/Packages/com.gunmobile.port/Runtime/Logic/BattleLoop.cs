using System;
using System.Collections.Generic;
using GunMobile.Core;
using UnityEngine;

namespace GunMobile.Logic
{
    public sealed class BombInfo
    {
        public int TemplateId;
        public int Common;
        public int CommonAddWound;
        public int CommonMultiBall;
        public int Special;
    }

    public static class BombTable
    {
        public static Dictionary<int, BombInfo> Load(XmlResultTable table)
        {
            var map = new Dictionary<int, BombInfo>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var info = new BombInfo
                {
                    TemplateId = GetInt(row, "TemplateID"),
                    Common = GetInt(row, "Common"),
                    CommonAddWound = GetInt(row, "CommonAddWound"),
                    CommonMultiBall = GetInt(row, "CommonMultiBall"),
                    Special = GetInt(row, "Special")
                };
                map[info.TemplateId] = info;
            }

            return map;
        }

        static int GetInt(IReadOnlyDictionary<string, string> row, string key)
        {
            if (!row.TryGetValue(key, out string raw))
            {
                return 0;
            }

            int.TryParse(raw, out int n);
            return n;
        }
    }

    [Serializable]
    public struct LivingStats
    {
        public int Attack;
        public int Defence;
        public int Agility;
        public int Luck;
        public int MagicAttack;
        public int MagicDefence;
        public int Hp;
        public int MaxHp;
        public int Team;
    }

    public static class DamageCalculator
    {
        public static int Compute(LivingStats attacker, LivingStats defender, int bombHurt, float distancePx, bool isCrit)
        {
            float atk = Mathf.Max(1f, attacker.Attack);
            float def = Mathf.Max(0f, defender.Defence);
            float mitigation = def / (def + 400f);
            float dist = Mathf.Clamp01(1f - distancePx / 220f);
            float crit = isCrit ? 1.5f + attacker.Luck / 800f : 1f;
            float raw = bombHurt * (atk / 40f) * (1f - mitigation) * (0.55f + 0.45f * dist) * crit;
            raw *= 1f + attacker.Agility / 800f;
            int dmg = Mathf.Max(1, Mathf.RoundToInt(raw));
            return Mathf.Min(dmg, defender.Hp);
        }

        public static bool RollCrit(int luck, int seed)
        {
            var rng = new System.Random(seed);
            int chance = Mathf.Clamp(5 + luck / 50, 5, 45);
            return rng.Next(0, 100) < chance;
        }
    }

    public enum BattlePhase
    {
        Waiting,
        Aiming,
        Flying,
        Settling,
        NextTurn,
        MatchOver
    }

    public sealed class BattleLoop
    {
        public BattlePhase Phase { get; private set; } = BattlePhase.Waiting;
        public int TurnIndex { get; private set; }
        public int CurrentLiving { get; private set; }
        public float Wind { get; private set; }
        public float TurnTimeLeft { get; private set; } = 20f;

        readonly List<LivingStats> _livings = new List<LivingStats>();
        readonly System.Random _rng = new System.Random();

        public IReadOnlyList<LivingStats> Livings => _livings;

        public void Reset(IEnumerable<LivingStats> livings, float turnSeconds = 20f)
        {
            _livings.Clear();
            _livings.AddRange(livings);
            TurnIndex = 0;
            CurrentLiving = 0;
            TurnTimeLeft = turnSeconds;
            Wind = NextWind();
            Phase = BattlePhase.Aiming;
        }

        public void TickClock(float dt)
        {
            if (Phase != BattlePhase.Aiming)
            {
                return;
            }

            TurnTimeLeft -= dt;
            if (TurnTimeLeft <= 0f)
            {
                SkipTurn();
            }
        }

        public void BeginShot()
        {
            if (Phase == BattlePhase.Aiming)
            {
                Phase = BattlePhase.Flying;
            }
        }

        public void EndShot()
        {
            if (Phase == BattlePhase.Flying)
            {
                Phase = BattlePhase.Settling;
            }
        }

        public void FinishSettle(float turnSeconds = 20f)
        {
            if (CountAliveTeams() <= 1)
            {
                Phase = BattlePhase.MatchOver;
                return;
            }

            AdvanceTurn();
            TurnTimeLeft = turnSeconds;
            Wind = NextWind();
            Phase = BattlePhase.Aiming;
        }

        public void ApplyDamage(int livingIndex, int amount)
        {
            if (livingIndex < 0 || livingIndex >= _livings.Count)
            {
                return;
            }

            LivingStats s = _livings[livingIndex];
            s.Hp = Mathf.Max(0, s.Hp - amount);
            _livings[livingIndex] = s;
        }

        void SkipTurn()
        {
            AdvanceTurn();
            TurnTimeLeft = 20f;
            Wind = NextWind();
        }

        void AdvanceTurn()
        {
            TurnIndex++;
            int n = _livings.Count;
            for (int i = 1; i <= n; i++)
            {
                int idx = (CurrentLiving + i) % n;
                if (_livings[idx].Hp > 0)
                {
                    CurrentLiving = idx;
                    return;
                }
            }
        }

        int CountAliveTeams()
        {
            var teams = new HashSet<int>();
            foreach (LivingStats s in _livings)
            {
                if (s.Hp > 0)
                {
                    teams.Add(s.Team);
                }
            }

            return teams.Count;
        }

        float NextWind()
        {
            return _rng.Next(-3, 4) * 10;
        }
    }
}
