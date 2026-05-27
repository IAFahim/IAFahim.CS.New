namespace IAFahim.Math.Sdf
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class SdfBoolean
    {
        private const float Zero = 0.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Union(float d1, float d2)
        {
            return math.min(d1, d2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Intersection(float d1, float d2)
        {
            return math.max(d1, d2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Difference(float d1, float d2)
        {
            return math.max(d1, -d2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothUnion(float d1, float d2, float k)
        {
            float h = math.clamp(0.5f + 0.5f * (d2 - d1) / k, Zero, 1.0f);
            return math.lerp(d2, d1, h) - k * h * (1.0f - h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothIntersection(float d1, float d2, float k)
        {
            float h = math.clamp(0.5f - 0.5f * (d2 - d1) / k, Zero, 1.0f);
            return math.lerp(d2, d1, h) + k * h * (1.0f - h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothDifference(float d1, float d2, float k)
        {
            float h = math.clamp(0.5f - 0.5f * (d2 + d1) / k, Zero, 1.0f);
            return math.lerp(d2, -d1, h) + k * h * (1.0f - h);
        }
    }
}
