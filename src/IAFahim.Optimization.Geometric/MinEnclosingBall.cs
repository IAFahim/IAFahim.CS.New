namespace IAFahim.Optimization.Geometric
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinEnclosingBall
    {
        public struct Circle
        {
            public double X, Y, R;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Circle Welzl(double* xs, double* ys, int n)
        {
            Circle c = default;
            c.X = 0; c.Y = 0; c.R = 0;
            if (n == 0) return c;
            c.X = xs[0]; c.Y = ys[0]; c.R = 1e-9;
            for (int i = 0; i < n && i < 3; i++)
            {
                if (Dist2(c, xs[i], ys[i]) > c.R * c.R)
                {
                    c.X = xs[i]; c.Y = ys[i]; c.R = 1e-9;
                    for (int j = 0; j < i; j++)
                    {
                        if (Dist2(c, xs[j], ys[j]) > c.R * c.R)
                        {
                            c.X = (xs[i] + xs[j]) / 2;
                            c.Y = (ys[i] + ys[j]) / 2;
                            c.R = Math.Sqrt(Dist2(c, xs[j], ys[j]));
                            for (int k = 0; k < j; k++)
                            {
                                if (Dist2(c, xs[k], ys[k]) > c.R * c.R)
                                {
                                    double bx = (xs[i] + xs[j] + xs[k]) / 3;
                                    double by = (ys[i] + ys[j] + ys[k]) / 3;
                                    double br = 0;
                                    br = Math.Max(br, Math.Sqrt((bx - xs[i]) * (bx - xs[i]) + (by - ys[i]) * (by - ys[i])));
                                    br = Math.Max(br, Math.Sqrt((bx - xs[j]) * (bx - xs[j]) + (by - ys[j]) * (by - ys[j])));
                                    br = Math.Max(br, Math.Sqrt((bx - xs[k]) * (bx - xs[k]) + (by - ys[k]) * (by - ys[k])));
                                    c.X = bx; c.Y = by; c.R = br;
                                }
                            }
                        }
                    }
                }
            }
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Dist2(Circle c, double x, double y)
        {
            double dx = c.X - x, dy = c.Y - y;
            return dx * dx + dy * dy;
        }
    }
}
