namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Delaunay
    {
        public struct Triangle { public int A, B, C; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InCircle(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            double adx = ax - dx, ady = ay - dy;
            double bdx = bx - dx, bdy = by - dy;
            double cdx = cx - dx, cdy = cy - dy;

            double abdet = adx * bdy - bdx * ady;
            double bcdet = bdx * cdy - cdx * bdy;
            double cadet = cdx * ady - adx * cdy;
            double alift = adx * adx + ady * ady;
            double blift = bdx * bdx + bdy * bdy;
            double clift = cdx * cdx + cdy * cdy;

            return alift * bcdet + blift * cadet + clift * abdet > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Triangle* outTriangles)
        {
            if (n < 3) return 0;
            int count = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    for (int k = j + 1; k < n; k++)
                    {
                        // Check if CCW
                        double cross = (xs[j] - xs[i]) * (ys[k] - ys[i]) - (ys[j] - ys[i]) * (xs[k] - xs[i]);
                        if (Math.Abs(cross) < 1e-9) continue;

                        int u = i, v = j, w = k;
                        if (cross < 0) { u = j; v = i; } // Force CCW

                        bool valid = true;
                        for (int t = 0; t < n; t++)
                        {
                            if (t == i || t == j || t == k) continue;
                            if (InCircle(xs[u], ys[u], xs[v], ys[v], xs[w], ys[w], xs[t], ys[t]))
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (valid)
                        {
                            outTriangles[count].A = u;
                            outTriangles[count].B = v;
                            outTriangles[count].C = w;
                            count++;
                        }
                    }
                }
            }
            return count;
        }
    }
}
