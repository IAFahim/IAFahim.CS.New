namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Delaunay
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Flip(double* xs, double* ys, int n, int* outA, int* outB, int* outC)
        {
            int t = 0;
            for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            for (int k = j + 1; k < n; k++)
            {
                double x1 = xs[i], y1 = ys[i], x2 = xs[j], y2 = ys[j], x3 = xs[k], y3 = ys[k];
                double cx = (x1 + x2 + x3) / 3, cy = (y1 + y2 + y3) / 3;
                double r = Math.Sqrt((x1 - cx) * (x1 - cx) + (y1 - cy) * (y1 - cy));
                bool inside = false;
                for (int p = 0; p < n; p++)
                {
                    if (p == i || p == j || p == k) continue;
                    double d = Math.Sqrt((xs[p] - cx) * (xs[p] - cx) + (ys[p] - cy) * (ys[p] - cy));
                    if (d < r - 1e-9) { inside = true; break; }
                }
                if (!inside)
                {
                    outA[t] = i; outB[t] = j; outC[t++] = k;
                }
            }
            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Constrained(double* xs, double* ys, int n, int* edges, int ec, int* outA, int* outB, int* outC)
        {
            return Flip(xs, ys, n, outA, outB, outC);
        }
    }
}
