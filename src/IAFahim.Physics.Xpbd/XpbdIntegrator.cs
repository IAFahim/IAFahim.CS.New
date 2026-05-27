namespace IAFahim.Physics.Xpbd
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class XpbdIntegrator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PredictPosition(float3* pos, float3* vel, float3 externalForce, float invMass, float dt)
        {
            *vel = *vel + externalForce * (invMass * dt);
            *pos = *pos + *vel * dt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateVelocity(float3* vel, float3* oldPos, float3* newPos, float dt)
        {
            if (dt < 1e-8f)
            {
                return;
            }

            float invDt = 1.0f / dt;
            *vel = (*newPos - *oldPos) * invDt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ApplyDamping(float3* vel, float damping, float dt)
        {
            *vel = *vel * math.max(0.0f, 1.0f - damping * dt);
        }

        public static void SolveDistanceConstraints(
            float3* positions, float3* velocities, float* invMasses,
            int* constraintA, int* constraintB, float* restLengths,
            float* compliances, int constraintCount, float dt)
        {
            for (int i = 0; i < constraintCount; i++)
            {
                int idxA = constraintA[i];
                int idxB = constraintB[i];
                float restLen = restLengths[i];
                float compliance = compliances[i];

                DistanceConstraint.Solve(
                    &positions[idxA], &positions[idxB],
                    &velocities[idxA], &velocities[idxB],
                    invMasses[idxA], invMasses[idxB],
                    restLen, compliance, dt);
            }
        }
    }
}
