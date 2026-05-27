namespace IAFahim.Math.Sdf
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class SdfRayMarch
    {
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Epsilon = 1e-6f;
        private const float NormalEpsilon = 1e-4f;

        public delegate float SdfFunction(float3 p);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EstimateNormal(SdfFunction sdf, float3 p)
        {
            float d = sdf(p);
            float3 grad = new float3(
                sdf(p + new float3(NormalEpsilon, Zero, Zero)) - d,
                sdf(p + new float3(Zero, NormalEpsilon, Zero)) - d,
                sdf(p + new float3(Zero, Zero, NormalEpsilon)) - d);
            return math.normalize(grad);
        }

        public static bool March(SdfFunction sdf, float3 origin, float3 direction, float maxDistance, int maxSteps, out float t, out float3 hitPoint)
        {
            t = Zero;
            hitPoint = origin;

            for (int i = 0; i < maxSteps; i++)
            {
                float3 p = origin + direction * t;
                float d = sdf(p);

                if (d < Epsilon)
                {
                    hitPoint = p;
                    return true;
                }

                t += d;

                if (t > maxDistance)
                {
                    return false;
                }
            }

            return false;
        }

        public static float AmbientOcclusion(SdfFunction sdf, float3 p, float3 normal, int steps, float stepSize)
        {
            float occlusion = Zero;
            float scale = One;

            for (int i = 1; i <= steps; i++)
            {
                float dist = (float)i * stepSize;
                float d = sdf(p + normal * dist);
                occlusion += (dist - d) * scale;
                scale *= 0.5f;
            }

            return math.clamp(One - 1.5f * occlusion, Zero, One);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 SafeNormalize(float3 v)
        {
            float len = math.length(v);
            return len > Epsilon ? v / len : new float3(Zero, One, Zero);
        }
    }
}
