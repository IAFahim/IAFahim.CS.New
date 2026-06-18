namespace IAFahim.Geometry.Advanced
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Circumcenter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long ax, long ay, long bx, long by, long cx, long cy, long* ox, long* oy)
        {
            long d = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
            if (d == 0) return;
            long a2 = ax * ax + ay * ay;
            long b2 = bx * bx + by * by;
            long c2 = cx * cx + cy * cy;
            *ox = (a2 * (by - cy) + b2 * (cy - ay) + c2 * (ay - by)) / d;
            *oy = (a2 * (cx - bx) + b2 * (ax - cx) + c2 * (bx - ax)) / d;
        }
    }

    public static unsafe class MinimumEnclosingCircle
    {
        public static void Run(int n, long* x, long* y, long* cx, long* cy, long* r)
        {
            *cx = x[0]; *cy = y[0]; *r = 0;
            for (int i = 1; i < n; i++)
            {
                long dx = x[i] - *cx;
                long dy = y[i] - *cy;
                if (dx * dx + dy * dy > *r)
                {
                    *cx = x[i]; *cy = y[i]; *r = 0;
                    for (int j = 0; j < i; j++)
                    {
                        long dx2 = x[j] - *cx;
                        long dy2 = y[j] - *cy;
                        if (dx2 * dx2 + dy2 * dy2 > *r)
                        {
                            *cx = (x[i] + x[j]) >> 1;
                            *cy = (y[i] + y[j]) >> 1;
                            *r = ((*cx - x[i]) * (*cx - x[i]) + (*cy - y[i]) * (*cy - y[i]));
                            for (int k = 0; k < j; k++)
                            {
                                long dx3 = x[k] - *cx;
                                long dy3 = y[k] - *cy;
                                if (dx3 * dx3 + dy3 * dy3 > *r)
                                {
                                    Circumcenter.Run(x[i], y[i], x[j], y[j], x[k], y[k], cx, cy);
                                    *r = (*cx - x[i]) * (*cx - x[i]) + (*cy - y[i]) * (*cy - y[i]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static unsafe class IntegerPointCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long* x, long* y)
        {
            long area = 0;
            long boundary = 0;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                long dx = Math.Abs(x[j] - x[i]);
                long dy = Math.Abs(y[j] - y[i]);
                boundary += Gcd(dx, dy);
            }
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += x[i] * y[j];
                area -= x[j] * y[i];
            }
            if (area < 0) area = -area;
            return (area - boundary + 2) / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Gcd(long a, long b)
        {
            while (b != 0) { long t = b; b = a % b; a = t; }
            return a;
        }
    }

    public static unsafe class PickTheorem
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long* x, long* y)
        {
            return IntegerPointCount.Run(n, x, y);
        }
    }
}