using System;
using UnityEngine;

namespace GunMobile.Logic
{
    [Serializable]
    public struct ProjectileState
    {
        public float X;
        public float Y;
        public float Vx;
        public float Vy;
        public float Time;
        public bool Alive;
    }

    /// <summary>
    /// Turn-based artillery close to the PC 25fps loop (config.xml GAME_FRAME_CONFIG).
    /// Constants are tunable so they can be calibrated against a recorded PC shot.
    /// </summary>
    [Serializable]
    public sealed class ProjectileSimulator
    {
        public const float FrameDt = 1f / 25f;

        [Tooltip("Pixels per power unit at 25fps.")]
        public float SpeedScale = 5.5f;

        [Tooltip("Pixels / sec^2. PC gravity is applied once per 40ms frame.")]
        public float Gravity = 175f;

        [Tooltip("How strongly wind (-100..100) pushes vx.")]
        public float WindScale = 1.15f;

        public ProjectileState Launch(float x, float y, float angleDeg, float power, int facing)
        {
            float p = Mathf.Clamp(power, 1f, 100f);
            float rad = angleDeg * Mathf.Deg2Rad;
            float speed = p * SpeedScale;
            int dir = facing >= 0 ? 1 : -1;
            return new ProjectileState
            {
                X = x,
                Y = y,
                Vx = Mathf.Cos(rad) * speed * dir,
                Vy = Mathf.Sin(rad) * speed,
                Time = 0f,
                Alive = true
            };
        }

        public ProjectileState Step(ProjectileState state, float wind, float dt)
        {
            if (!state.Alive)
            {
                return state;
            }

            state.X += state.Vx * dt;
            state.Y += state.Vy * dt;
            state.Vy -= Gravity * dt;
            state.Vx += wind * WindScale * dt;
            state.Time += dt;
            return state;
        }

        public ProjectileState StepFrame(ProjectileState state, float wind)
        {
            return Step(state, wind, FrameDt);
        }

        /// <summary>
        /// Integrate until terrain hit, bounds leave, or timeout. Y grows upward in Unity;
        /// MapCollision Y grows downward like the PC bitmap, so pass a converter.
        /// </summary>
        public ProjectileState FlyUntil(
            ProjectileState state,
            float wind,
            Func<float, float, bool> isSolid,
            Func<float, float, bool> outOfBounds,
            float maxTime = 12f)
        {
            while (state.Alive && state.Time < maxTime)
            {
                ProjectileState next = StepFrame(state, wind);
                if (outOfBounds(next.X, next.Y))
                {
                    next.Alive = false;
                    return next;
                }

                if (isSolid(next.X, next.Y))
                {
                    next.Alive = false;
                    return next;
                }

                state = next;
            }

            state.Alive = false;
            return state;
        }
    }
}
