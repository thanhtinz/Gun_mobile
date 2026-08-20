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
        /// <summary>PC BaseDamage — weapon + AddDamage (game.logic Living.BaseDamage).</summary>
        public int BaseDamage;
        /// <summary>PC BaseGuard — armor stat for DR1 (game.logic Living.BaseGuard).</summary>
        public int BaseGuard;
        public int Grade;
        public int Hp;
        public int MaxHp;
        public int Team;
    }

    /// <summary>
    /// Damage from decompiled DDTank 3.0 game.logic SimpleBomb.MakeDamage / Living.MakeDamage.
    /// </summary>
    public static class DamageCalculator
    {
        public static int Compute(
            LivingStats attacker,
            LivingStats defender,
            int bombHurt,
            float distancePx,
            bool isCrit,
            bool armorPierce = false,
            BattleDamageMods attackerMods = default,
            BattleDamageMods defenderMods = default,
            float blastRadius = 0f)
        {
            if (attackerMods.DamageMult <= 0f)
            {
                attackerMods.DamageMult = 1f;
            }

            double baseDamage = bombHurt > 0 ? bombHurt : (attacker.BaseDamage > 0 ? attacker.BaseDamage : 140);
            double baseGuard = defender.BaseGuard + defenderMods.DefenceFlat;
            double defence = defender.Defence;
            double attack = attacker.Attack + attackerMods.AttackFlat;
            int grade = attacker.Grade > 0 ? attacker.Grade : 1;

            if (armorPierce)
            {
                baseGuard = 0;
                defence = 0;
            }

            float damagePlus = attackerMods.DamageMult;
            float shootMinus = 1f;

            double dr1Denom = 500 + baseGuard - 3 * grade;
            double dr1 = dr1Denom <= 0 ? 0 : 0.95 * (baseGuard - 3 * grade) / dr1Denom;

            double dr2 = 0;
            int lucky = attacker.Luck;
            double defMinusLuck = defence - lucky;
            if (defMinusLuck >= 0)
            {
                double dr2Denom = 600 + defence - lucky;
                dr2 = dr2Denom <= 0 ? 0 : 0.95 * defMinusLuck / dr2Denom;
            }

            double mitigation = dr1 + dr2 - dr1 * dr2;
            double damage = baseDamage * (1 + attack * 0.001) * (1 - mitigation) * damagePlus * shootMinus;

            if (attacker.MagicAttack > 0)
            {
                double mDef = defender.MagicDefence + defenderMods.DefenceFlat / 2;
                double mMit = mDef <= 0 ? 0 : 0.95 * mDef / (600 + mDef);
                damage += attacker.MagicAttack * (1 - mMit) * 0.01;
            }

            if (blastRadius > 0f)
            {
                if (distancePx >= blastRadius)
                {
                    return 0;
                }

                damage *= 1 - distancePx / blastRadius / 4f;
            }
            else
            {
                float dist = Mathf.Clamp01(1f - distancePx / 220f);
                damage *= 0.55f + 0.45f * dist;
            }

            damage *= 1f + attacker.Agility / 800f;

            if (damage < 1)
            {
                damage = 1;
            }

            int baseDmg = Mathf.RoundToInt((float)damage);
            if (isCrit)
            {
                int critBonus = ComputeCritical(lucky, baseDmg);
                baseDmg += Mathf.RoundToInt(critBonus * (1f + attackerMods.CritDamageAdd));
            }

            return Mathf.Min(Mathf.Max(1, baseDmg), defender.Hp);
        }

        public static int ComputeHeal(int bombHurt, float distancePx, float blastRadius)
        {
            if (blastRadius <= 0f || distancePx >= blastRadius)
            {
                return 0;
            }

            float factor = 1f - distancePx / blastRadius / 4f;
            return Mathf.Max(1, Mathf.RoundToInt(bombHurt * factor));
        }

        public static int ComputeCritical(int lucky, int baseDamage)
        {
            return Mathf.RoundToInt((0.5f + lucky * 0.0003f) * baseDamage);
        }

        public static int ComputeBombHurt(BallPhysics ball, float propDmgMult = 1f)
        {
            float power = ball.Power;
            if (Mathf.Abs(power) < 0.001f)
            {
                power = 1f;
            }

            int bombHurt = 80 + Mathf.RoundToInt(Mathf.Abs(power) * 80f);
            if (bombHurt < 40)
            {
                bombHurt = 140;
            }

            return Mathf.Max(1, Mathf.RoundToInt(bombHurt * propDmgMult));
        }

        /// <summary>PC SimpleBomb.MakeCriticalDamage — lucky * 75 / (800 + lucky) &gt; roll.</summary>
        public static bool RollCrit(int lucky, int seed)
        {
            var rng = new System.Random(seed);
            double threshold = lucky * 75.0 / (800 + lucky);
            return threshold > rng.Next(100);
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
        public int Seed { get; private set; }

        readonly List<LivingStats> _livings = new List<LivingStats>();
        System.Random _rng = new System.Random();

        public BattleEffectTracker Effects { get; } = new BattleEffectTracker();

        public List<(int seat, int heal, int dmg)> LastTickPulses { get; private set; }

        public IReadOnlyList<LivingStats> Livings => _livings;

        public void Reset(IEnumerable<LivingStats> livings, float turnSeconds = 20f, int seed = 0)
        {
            Seed = seed != 0 ? seed : Environment.TickCount;
            _rng = new System.Random(Seed);
            _livings.Clear();
            _livings.AddRange(livings);
            Effects.Clear();
            TurnIndex = 0;
            CurrentLiving = 0;
            TurnTimeLeft = turnSeconds;
            Wind = NextWind();
            Phase = BattlePhase.Aiming;
        }

        public void SyncTurn(int turnIndex, int currentLiving, float wind, float turnSeconds = 20f)
        {
            // Server authoritative sync for online play.
            TurnIndex = Mathf.Max(0, turnIndex);
            int maxLiving = Mathf.Max(0, _livings.Count - 1);
            CurrentLiving = Mathf.Clamp(currentLiving, 0, maxLiving);
            Wind = wind;
            TurnTimeLeft = turnSeconds;
            Phase = BattlePhase.Aiming;
        }

        public void SyncLivingHp(int[] hp, int[] maxHp = null)
        {
            if (hp == null) return;

            int n = Mathf.Min(hp.Length, _livings.Count);
            for (int i = 0; i < n; i++)
            {
                LivingStats s = _livings[i];
                s.Hp = Mathf.Max(0, hp[i]);
                if (maxHp != null && i < maxHp.Length && maxHp[i] > 0)
                {
                    s.MaxHp = maxHp[i];
                }
                _livings[i] = s;
            }
        }

        public void SyncMatchOverIfNeeded()
        {
            // Reconnect continuity: if snapshot indicates only one (or zero) team alive,
            // force UI to MatchOver even if we missed the last damage events.
            if (CountAliveTeams() <= 1)
            {
                Phase = BattlePhase.MatchOver;
            }
        }

        public void EndMatchTimeout()
        {
            Phase = BattlePhase.MatchOver;
        }

        public void FinishSettleOnline(float turnSeconds = 20f)
        {
            // Online: server owns turn advance + wind. Client only ends shot/settle UI state.
            if (CountAliveTeams() <= 1)
            {
                Phase = BattlePhase.MatchOver;
                return;
            }

            TurnTimeLeft = turnSeconds;
            Phase = BattlePhase.Aiming;
        }

        public bool WouldTeamWin(int seatIndex)
        {
            if (seatIndex < 0 || seatIndex >= _livings.Count) return false;

            int myTeam = _livings[seatIndex].Team;
            var aliveTeams = new HashSet<int>();
            foreach (LivingStats s in _livings)
            {
                if (s.Hp > 0) aliveTeams.Add(s.Team);
            }

            return aliveTeams.Count == 1 && aliveTeams.Contains(myTeam);
        }

        public void TickClockDisplay(float dt)
        {
            if (Phase != BattlePhase.Aiming) return;
            TurnTimeLeft = Mathf.Max(0f, TurnTimeLeft - dt);
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

        public List<(int seat, int heal, int dmg)> TickTurnEffects()
        {
            var hp = new int[_livings.Count];
            var arr = new LivingStats[_livings.Count];
            for (int i = 0; i < _livings.Count; i++)
            {
                arr[i] = _livings[i];
                hp[i] = _livings[i].Hp;
            }

            var pulses = Effects.TickTurn(arr, hp);
            for (int i = 0; i < hp.Length && i < _livings.Count; i++)
            {
                LivingStats s = _livings[i];
                s.Hp = hp[i];
                _livings[i] = s;
            }

            return pulses;
        }

        public LivingStats EffectiveLiving(int seat)
        {
            if (seat < 0 || seat >= _livings.Count)
            {
                return default;
            }

            return Effects.ApplyDefence(_livings[seat], seat);
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

        public void ApplyHeal(int livingIndex, int amount)
        {
            if (livingIndex < 0 || livingIndex >= _livings.Count || amount <= 0)
            {
                return;
            }

            LivingStats s = _livings[livingIndex];
            s.Hp = Mathf.Min(s.MaxHp, s.Hp + amount);
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
            LastTickPulses = TickTurnEffects();
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
