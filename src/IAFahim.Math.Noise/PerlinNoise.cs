namespace IAFahim.Math.Noise
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class PerlinNoise
    {
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Six = 6.0f;
        private const float Fifteen = 15.0f;
        private const float Ten = 10.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Hash2D(int x, int y)
        {
            int h = x * 1619 + y * 31337;
            h = (h ^ (h >> 15)) * 97;
            h ^= (h >> 11);
            return (float)(h & 0x7fffffff) / 2147483647.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float2 Grad2D(int x, int y)
        {
            float val = Hash2D(x, y) * 2.0f * math.PI;
            return new float2(math.cos(val), math.sin(val));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Fade(float t)
        {
            return t * t * t * (t * (t * Six - Fifteen) + Ten);
        }

        public static float Noise2D(float2 p)
        {
            if (math.isnan(p.x) || math.isnan(p.y))
            {
                return float.NaN;
            }

            float fx = math.floor(p.x);
            float fy = math.floor(p.y);
            int ix = (int)fx;
            int iy = (int)fy;

            float tx = p.x - fx;
            float ty = p.y - fy;

            float2 g00 = Grad2D(ix, iy);
            float2 g10 = Grad2D(ix + 1, iy);
            float2 g01 = Grad2D(ix, iy + 1);
            float2 g11 = Grad2D(ix + 1, iy + 1);

            float d00 = math.dot(g00, new float2(tx, ty));
            float d10 = math.dot(g10, new float2(tx - One, ty));
            float d01 = math.dot(g01, new float2(tx, ty - One));
            float d11 = math.dot(g11, new float2(tx - One, ty - One));

            float u = Fade(tx);
            float v = Fade(ty);

            float x1 = math.lerp(d00, d10, u);
            float x2 = math.lerp(d01, d11, u);

            return math.lerp(x1, x2, v);
        }
    }
}
