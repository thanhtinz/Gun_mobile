using System;
using System.Collections.Generic;
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
        public const float FrameDt = PcPhysics.FrameDt;

        [Tooltip("Pixels per power unit per 25fps frame (PC force slider).")]
        public float SpeedScale = PcPhysics.PowerToSpeed;

        [Tooltip("Pixels per frame². game.logic.dll gravity is 0.7 at 25fps.")]
        public float GravityPerFrame = PcPhysics.GravityPerFrame;

        [Tooltip("How strongly displayed wind (-40..40) pushes vx each frame.")]
        public float WindScale = PcPhysics.WindAccelPerFrame;

        public float GravityFactor = 1f;
        public float WindFactor = 1f;

        public void ApplyBall(BallPhysics ball)
        {
            GravityFactor = PcPhysics.GravityFactor(ball.Weight);
            WindFactor = PcPhysics.WindFactor(ball.Wind);
        }

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

        /// <summary>Integrate one 40ms PC frame. Velocities are pixels/frame.</summary>
        public ProjectileState StepFrame(ProjectileState state, float wind)
        {
            if (!state.Alive)
            {
                return state;
            }

            state.X += state.Vx;
            state.Y += state.Vy;
            state.Vy -= GravityPerFrame * GravityFactor;
            state.Vx += wind * WindScale * WindFactor;
            state.Time += FrameDt;
            return state;
        }

        public ProjectileState Step(ProjectileState state, float wind, float dt)
        {
            if (Mathf.Abs(dt - FrameDt) < 0.0001f)
            {
                return StepFrame(state, wind);
            }

            int frames = Mathf.Max(1, Mathf.RoundToInt(dt / FrameDt));
            for (int i = 0; i < frames; i++)
            {
                state = StepFrame(state, wind);
            }

            return state;
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

        /// <summary>Fly until hit; sample map-space points every N frames for net replay.</summary>
        public ProjectileState FlyUntilSampled(
            ProjectileState state,
            float wind,
            Func<float, float, bool> isSolid,
            Func<float, float, bool> outOfBounds,
            int mapHeight,
            List<int> mapSamples,
            int sampleEvery = 4,
            float maxTime = 12f)
        {
            int frame = 0;
            while (state.Alive && state.Time < maxTime)
            {
                ProjectileState next = StepFrame(state, wind);
                frame++;
                if (mapSamples != null && frame % sampleEvery == 0)
                {
                    mapSamples.Add(Mathf.RoundToInt(next.X));
                    mapSamples.Add(mapHeight - 1 - Mathf.RoundToInt(next.Y));
                }

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
