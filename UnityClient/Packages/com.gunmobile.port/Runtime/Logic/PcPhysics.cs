using System;
using UnityEngine;

namespace GunMobile.Logic
{
    /// <summary>
    /// Frame-step artillery from Fight <c>game.logic.dll</c>
    /// (<c>Game.Logic.Phy.Object.Physics</c> / <c>SimpleBomb</c>).
    /// PC ticks at 25fps; Y on the bitmap grows downward. Unity callers use Y-up
    /// and this helper flips gravity.
    /// </summary>
    public static class PcPhysics
    {
        public const float FrameDt = 1f / 25f;

        /// <summary>ldc.r4 0.7 in game.logic.dll — gravity pixels per 40ms frame.</summary>
        public const float GravityPerFrame = 0.7f;

        /// <summary>ldc.r4 0.04 in game.logic.dll — wind acceleration scale.</summary>
        public const float WindAccelPerFrame = 0.04f;

        /// <summary>
        /// Launch speed in pixels/frame at power 100, matching the PC force slider
        /// (protocol force ≈ power, one pixel per power unit per frame at 0°).
        /// </summary>
        public const float PowerToSpeed = 1f;

        public static float GravityFactor(float ballWeight)
        {
            // BallList Weight=70 → 0.70 of map gravity.
            return ballWeight <= 0f ? 1f : ballWeight / 100f;
        }

        public static float WindFactor(float ballWind)
        {
            // BallList Wind=240 → 2.40, then combined with 0.04 → mild push.
            return ballWind <= 0f ? 1f : ballWind / 240f;
        }
    }

    [Serializable]
    public struct BallPhysics
    {
        public int Id;
        public float Power;
        public int Radii;
        public int Crater;
        public float Wind;
        public float Weight;
        public float Mass;
        public int FlyingPartical;
        public int BombPartical;
        public int Amount;

        public static BallPhysics Default => new BallPhysics
        {
            Id = 1,
            Power = 1f,
            Radii = 60,
            Crater = 0,
            Wind = 240f,
            Weight = 70f,
            Mass = 10f,
            Amount = 1
        };
    }
}
