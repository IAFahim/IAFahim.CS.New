namespace IAFahim.Physics.Xpbd
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class VolumeConstraint
    {
        private const float OneSixth = 0.16666667f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Solve(float3* p0, float3* p1, float3* p2, float3* p3,
            float invMass0, float invMass1, float invMass2, float invMass3,
            float restVolume, float compliance, float dt)
        {
            float3 d1 = *p1 - *p0;
            float3 d2 = *p2 - *p0;
            float3 d3 = *p3 - *p0;

            float volume = math.dot(math.cross(d1, d2), d3) * OneSixth;

            float3 grad1 = math.cross(d2, d3) * OneSixth;
            float3 grad2 = math.cross(d3, d1) * OneSixth;
            float3 grad3 = math.cross(d1, d2) * OneSixth;
            float3 grad0 = -(grad1 + grad2 + grad3);

            float c = volume - restVolume;

            float alpha = compliance / (dt * dt);

            float w = invMass0 * math.lengthsq(grad0)
                    + invMass1 * math.lengthsq(grad1)
                    + invMass2 * math.lengthsq(grad2)
                    + invMass3 * math.lengthsq(grad3);

            if (w + alpha < 1e-12f)
            {
                return;
            }

            float lambda = -c / (w + alpha);

            *p0 = *p0 + invMass0 * lambda * grad0;
            *p1 = *p1 + invMass1 * lambda * grad1;
            *p2 = *p2 + invMass2 * lambda * grad2;
            *p3 = *p3 + invMass3 * lambda * grad3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ComputeRestVolume(float3 p0, float3 p1, float3 p2, float3 p3)
        {
            float3 d1 = p1 - p0;
            float3 d2 = p2 - p0;
            float3 d3 = p3 - p0;
            return math.dot(math.cross(d1, d2), d3) * OneSixth;
        }
    }
}
