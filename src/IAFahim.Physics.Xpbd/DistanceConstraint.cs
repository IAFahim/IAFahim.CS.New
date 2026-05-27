namespace IAFahim.Physics.Xpbd
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class DistanceConstraint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Solve(float3* posA, float3* posB, float3* velA, float3* velB,
            float invMassA, float invMassB, float restLength, float compliance, float dt)
        {
            float3 delta = *posB - *posA;
            float dist = math.length(delta);

            if (dist < 1e-8f)
            {
                return;
            }

            float3 n = delta / dist;
            float c = dist - restLength;

            float alpha = compliance / (dt * dt);
            float w = invMassA + invMassB;

            if (w + alpha < 1e-12f)
            {
                return;
            }

            float lambda = -c / (w + alpha);
            float3 correction = lambda * n;

            *posA = *posA - invMassA * correction;
            *posB = *posB + invMassB * correction;
        }
    }
}
