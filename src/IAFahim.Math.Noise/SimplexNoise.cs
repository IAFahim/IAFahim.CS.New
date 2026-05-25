namespace IAFahim.Math.Noise
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class SimplexNoise
    {
        private const float Skew2D = 0.366025403f; // (Sqrt(3)-1)/2
        private const float Unskew2D = 0.211324865f; // (3-Sqrt(3))/6
        private const float G2 = 0.211324865f;
        private const float Norm2D = 99.2068907f;
        private const float Zero = 0.0f;
        private const float Half = 0.5f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Hash2D(int x, int y)
        {
            int h = x * 374761393 + y * 668265263;
            h = (h ^ (h >> 13)) * 1274126177;
            return (float)(h & 0x7fffffff) / 2147483647.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float2 Grad2D(int x, int y)
        {
            float angle = Hash2D(x, y) * 2.0f * math.PI;
            return new float2(math.cos(angle), math.sin(angle));
        }

        public static float Noise2D(float2 p)
        {
            if (math.isnan(p.x) || math.isnan(p.y))
            {
                return float.NaN;
            }

            float s = (p.x + p.y) * Skew2D;
            float2 ips = p + s;
            int ipx = (int)math.floor(ips.x);
            int ipy = (int)math.floor(ips.y);

            float t = (float)(ipx + ipy) * Unskew2D;
            float2 cellOrigin = new float2((float)ipx - t, (float)ipy - t);
            float2 v0 = p - cellOrigin;

            int i1x, i1y;
            if (v0.x > v0.y)
            {
                i1x = 1;
                i1y = 0;
            }
            else
            {
                i1x = 0;
                i1y = 1;
            }

            float2 v1 = v0 - new float2((float)i1x, (float)i1y) + G2;
            float2 v2 = v0 - new float2(1.0f, 1.0f) + 2.0f * G2;

            float n0 = Zero;
            float n1 = Zero;
            float n2 = Zero;

            float t0 = Half - math.dot(v0, v0);
            if (t0 > Zero)
            {
                t0 *= t0;
                n0 = t0 * t0 * math.dot(Grad2D(ipx, ipy), v0);
            }

            float t1 = Half - math.dot(v1, v1);
            if (t1 > Zero)
            {
                t1 *= t1;
                n1 = t1 * t1 * math.dot(Grad2D(ipx + i1x, ipy + i1y), v1);
            }

            float t2 = Half - math.dot(v2, v2);
            if (t2 > Zero)
            {
                t2 *= t2;
                n2 = t2 * t2 * math.dot(Grad2D(ipx + 1, ipy + 1), v2);
            }

            return Norm2D * (n0 + n1 + n2);
        }
    }
}
