namespace IAFahim.Collision.Gjk
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class MinkowskiDifference
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 SphereSupport(float3 direction, float3 center, float radius)
        {
            float len = math.length(direction);
            if (len < 1e-6f)
            {
                return center;
            }

            return center + (direction / len) * radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 BoxSupport(float3 direction, float3 center, float3 halfExtents)
        {
            float3 n = math.normalizesafe(direction);
            return center + halfExtents * math.sign(n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 CapsuleSupport(float3 direction, float3 a, float3 b, float radius)
        {
            float3 ba = b - a;
            float t = math.dot(direction, ba);
            float lenSq = math.dot(ba, ba);

            if (lenSq < 1e-6f)
            {
                return SphereSupport(direction, a, radius);
            }

            float param = math.clamp(t / lenSq, 0.0f, 1.0f);
            float3 closest = a + ba * param;
            return SphereSupport(direction, closest, radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ConvexHullSupport(float3 direction, float3* points, int count)
        {
            if (count <= 0)
            {
                return float3.zero;
            }

            float3 best = points[0];
            float maxDot = math.dot(direction, best);

            for (int i = 1; i < count; i++)
            {
                float d = math.dot(direction, points[i]);
                if (d > maxDot)
                {
                    maxDot = d;
                    best = points[i];
                }
            }

            return best;
        }
    }
}
