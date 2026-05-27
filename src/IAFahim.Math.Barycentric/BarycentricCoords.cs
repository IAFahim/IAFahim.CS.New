namespace IAFahim.Math.Barycentric
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class BarycentricCoords
    {
        private const float Epsilon = 1e-8f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Compute(float3 a, float3 b, float3 c, float3 p)
        {
            float3 v0 = b - a;
            float3 v1 = c - a;
            float3 v2 = p - a;

            float d00 = math.dot(v0, v0);
            float d01 = math.dot(v0, v1);
            float d11 = math.dot(v1, v1);
            float d20 = math.dot(v2, v0);
            float d21 = math.dot(v2, v1);

            float denom = d00 * d11 - d01 * d01;

            if (math.abs(denom) < Epsilon)
            {
                return new float3(1.0f / 3.0f, 1.0f / 3.0f, 1.0f / 3.0f);
            }

            float invDenom = 1.0f / denom;
            float v = (d11 * d20 - d01 * d21) * invDenom;
            float w = (d00 * d21 - d01 * d20) * invDenom;
            float u = 1.0f - v - w;

            return new float3(u, v, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Interpolate(float3 a, float3 b, float3 c, float3 bary)
        {
            return bary.x * a + bary.y * b + bary.z * c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InterpolateScalar(float va, float vb, float vc, float3 bary)
        {
            return bary.x * va + bary.y * vb + bary.z * vc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInside(float3 bary)
        {
            return bary.x >= 0.0f && bary.y >= 0.0f && bary.z >= 0.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Compute2D(float2 a, float2 b, float2 c, float2 p)
        {
            float3 bary = Compute(new float3(a, 0.0f), new float3(b, 0.0f), new float3(c, 0.0f), new float3(p, 0.0f));
            return new float2(bary.y, bary.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ProjectOntoTriangle(float3 a, float3 b, float3 c, float3 p)
        {
            const float Zero = 0.0f;
            const float One = 1.0f;

            float3 ab = b - a;
            float3 ac = c - a;
            float3 ap = p - a;

            float d1 = math.dot(ab, ap);
            float d2 = math.dot(ac, ap);
            if (d1 <= Zero && d2 <= Zero)
            {
                return a;
            }

            float3 bp = p - b;
            float d3 = math.dot(ab, bp);
            float d4 = math.dot(ac, bp);
            if (d3 >= Zero && d4 <= d3)
            {
                return b;
            }

            float vc = d1 * d4 - d3 * d2;
            if (vc <= Zero && d1 >= Zero && d3 <= Zero)
            {
                float v = d1 / (d1 - d3);
                return a + v * ab;
            }

            float3 cp = p - c;
            float d5 = math.dot(ab, cp);
            float d6 = math.dot(ac, cp);
            if (d6 >= Zero && d5 <= d6)
            {
                return c;
            }

            float vb = d5 * d2 - d1 * d6;
            if (vb <= Zero && d2 >= Zero && d6 <= Zero)
            {
                float w = d2 / (d2 - d6);
                return a + w * ac;
            }

            float va = d3 * d6 - d5 * d4;
            if (va <= Zero && (d4 - d3) >= Zero && (d5 - d6) >= Zero)
            {
                float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                return b + w * (c - b);
            }

            float denom = One / (va + vb + vc);
            float nv = vb * denom;
            float nw = vc * denom;
            return a + nv * ab + nw * ac;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedArea(float3 a, float3 b, float3 c)
        {
            float3 ab = b - a;
            float3 ac = c - a;
            return math.length(math.cross(ab, ac)) * 0.5f;
        }
    }
}
