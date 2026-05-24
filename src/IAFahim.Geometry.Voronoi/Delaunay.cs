namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Delaunay
    {
        public struct Triangle { public int A, B, C; }

        public static int Build(double* xs, double* ys, int n, Triangle* outTri)
        {
            if (n < 3) return 0;
            int cnt = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    for (int k = j + 1; k < n; k++)
                        if (TryAddTriangle(i, j, k, xs, ys, n, outTri, ref cnt)) { }
            return cnt;
        }

        private static bool TryAddTriangle(int i, int j, int k, double* xs, double* ys, int n, Triangle* outTri, ref int cnt)
        {
            double cross = (xs[j] - xs[i]) * (ys[k] - ys[i]) - (ys[j] - ys[i]) * (xs[k] - xs[i]);
            if (Math.Abs(cross) < 1e-9) return false;
            int u = i, v = j, w = k; if (cross < 0) { u = j; v = i; }
            if (IsDelaunay(u, v, w, xs, ys, n)) { outTri[cnt++] = new Triangle { A = u, B = v, C = w }; return true; }
            return false;
        }

        private static bool IsDelaunay(int u, int v, int w, double* xs, double* ys, int n)
        {
            for (int t = 0; t < n; t++)
                if (t != u && t != v && t != w && InCircle(xs[u], ys[u], xs[v], ys[v], xs[w], ys[w], xs[t], ys[t])) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InCircle(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            double adx = ax - dx, ady = ay - dy, bdx = bx - dx, bdy = by - dy, cdx = cx - dx, cdy = cy - dy;
            double abdet = adx * bdy - bdx * ady, bcdet = bdx * cdy - cdx * bdy, cadet = cdx * ady - adx * cdy;
            return (adx * adx + ady * ady) * bcdet + (bdx * bdx + bdy * bdy) * cadet + (cdx * cdx + cdy * cdy) * abdet > 1e-9;
        }

        public static void Flip(Triangle* triangles, int* adj, int t1, int t2) { } // Placeholder for flip
    }
}
