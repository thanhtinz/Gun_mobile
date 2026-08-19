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
        public static int Compute(LivingStats attacker, LivingStats defender, int bombHurt, float distancePx, bool isCrit, bool armorPierce = false)
        {
            float atk = Mathf.Max(1f, attacker.Attack);
            float def = Mathf.Max(0f, defender.Defence);
            float denom = armorPierce ? 800f : 400f;
            float mitigation = def / (def + denom);
            if (armorPierce)
            {
                mitigation *= 0.55f;
            }

            float dist = Mathf.Clamp01(1f - distancePx / 220f);
            float crit = isCrit ? 1.5f + attacker.Luck / 800f : 1f;
            float raw = bombHurt * (atk / 40f) * (1f - mitigation) * (0.55f + 0.45f * dist) * crit;
            raw *= 1f + attacker.Agility / 800f;
            int dmg = Mathf.Max(1, Mathf.RoundToInt(raw));
            return Mathf.Min(dmg, defender.Hp);
        }

        public static int ComputeBombHurt(BallPhysics ball, float propDmgMult = 1f)
        {
            float power = ball != null ? ball.Power : 1f;
            int bombHurt = 80 + Mathf.RoundToInt(Mathf.Abs(power) * 80f);
            if (bombHurt < 40)
            {
                bombHurt = 140;
            }

            return Mathf.Max(1, Mathf.RoundToInt(bombHurt * propDmgMult));
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
        public int Seed { get; private set; }

        readonly List<LivingStats> _livings = new List<LivingStats>();
        System.Random _rng = new System.Random();

        public IReadOnlyList<LivingStats> Livings => _livings;

        public void Reset(IEnumerable<LivingStats> livings, float turnSeconds = 20f, int seed = 0)
        {
            Seed = seed != 0 ? seed : Environment.TickCount;
            _rng = new System.Random(Seed);
            _livings.Clear();
            _livings.AddRange(livings);
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
