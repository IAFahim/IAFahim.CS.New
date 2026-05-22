namespace IAFahim.Optimization.Geometric
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class MinEnclosingBall
    {
        public struct Circle { public double X, Y, R; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Contains(Circle c, double px, double py)
        {
            double dx = px - c.X;
            double dy = py - c.Y;
            return dx * dx + dy * dy <= c.R * c.R + 1e-9;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Circle Construct(double x1, double y1, double x2, double y2)
        {
            double cx = (x1 + x2) / 2.0;
            double cy = (y1 + y2) / 2.0;
            double dx = x1 - cx, dy = y1 - cy;
            return new Circle { X = cx, Y = cy, R = Math.Sqrt(dx * dx + dy * dy) };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Circle Construct(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double bx = x2 - x1, by = y2 - y1;
            double cx = x3 - x1, cy = y3 - y1;
            double B = bx * bx + by * by;
            double C = cx * cx + cy * cy;
            double D = bx * cy - by * cx;
            
            if (Math.Abs(D) < 1e-9)
            {
                Circle c12 = Construct(x1, y1, x2, y2);
                Circle c13 = Construct(x1, y1, x3, y3);
                Circle c23 = Construct(x2, y2, x3, y3);
                Circle best = c12;
                if (c13.R > best.R) best = c13;
                if (c23.R > best.R) best = c23;
                return best;
            }
            
            double x = (cy * B - by * C) / (2 * D);
            double y = (bx * C - cx * B) / (2 * D);
            
            return new Circle { X = x1 + x, Y = y1 + y, R = Math.Sqrt(x * x + y * y) };
        }

        public static Circle Welzl(double* xs, double* ys, int n)
        {
            if (n == 0) return new Circle { X = 0, Y = 0, R = 0 };
            if (n == 1) return new Circle { X = xs[0], Y = ys[0], R = 0 };

            int* p = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) p[i] = i;
                
                ulong seed = 123456789;
                for (int i = n - 1; i > 0; i--)
                {
                    seed = seed * 6364136223846793005UL + 1442695040888963407UL;
                    int j = (int)(seed % (ulong)(i + 1));
                    int t = p[i]; p[i] = p[j]; p[j] = t;
                }

                Circle c = new Circle { X = xs[p[0]], Y = ys[p[0]], R = 0 };

                for (int i = 1; i < n; i++)
                {
                    int pi = p[i];
                    if (!Contains(c, xs[pi], ys[pi]))
                    {
                        c = new Circle { X = xs[pi], Y = ys[pi], R = 0 };
                        for (int j = 0; j < i; j++)
                        {
                            int pj = p[j];
                            if (!Contains(c, xs[pj], ys[pj]))
                            {
                                c = Construct(xs[pi], ys[pi], xs[pj], ys[pj]);
                                for (int k = 0; k < j; k++)
                                {
                                    int pk = p[k];
                                    if (!Contains(c, xs[pk], ys[pk]))
                                    {
                                        c = Construct(xs[pi], ys[pi], xs[pj], ys[pj], xs[pk], ys[pk]);
                                    }
                                }
                            }
                        }
                    }
                }

                return c;
            }
            finally
            {
                Marshal.FreeHGlobal((nint)p);
            }
        }
    }
}
