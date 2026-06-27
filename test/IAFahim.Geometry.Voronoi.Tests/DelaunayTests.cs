namespace IAFahim.Geometry.Voronoi.Tests
{
    using IAFahim.Geometry.Voronoi;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class DelaunayTests
    {
        [Test]
        public void Build_SatisfiesEmptyCircleAndEuler_Random()
        {
            Random rng = new Random(2026);
            for (int t = 0; t < 60; t++)
            {
                int n = rng.Next(3, 60);
                double[] sx = new double[n], sy = new double[n];
                for (int i = 0; i < n; i++)
                {
                    sx[i] = Math.Round(rng.NextDouble() * 1000, 1);
                    sy[i] = Math.Round(rng.NextDouble() * 1000, 1);
                }

                Delaunay.Triangle* tri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * (n * 2 + 16));
                double* px = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                double* py = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                try
                {
                    for (int i = 0; i < n; i++) { px[i] = sx[i]; py[i] = sy[i]; }
                    int cnt = Delaunay.Build(px, py, n, tri);

                    Assert.IsTrue(cnt >= 1, $"t={t}: no triangles for n={n}");
                    Assert.IsTrue(cnt <= 2 * n - 2, $"t={t}: too many triangles {cnt} for n={n}");

                    for (int i = 0; i < cnt; i++)
                    {
                        int a = tri[i].A, b = tri[i].B, c = tri[i].C;
                        Assert.IsTrue(a >= 0 && a < n && b >= 0 && b < n && c >= 0 && c < n, $"t={t} tri {i} vertex OOB");
                        Assert.AreNotEqual(a, b, $"t={t} tri {i} degenerate AB");
                        Assert.AreNotEqual(b, c, $"t={t} tri {i} degenerate BC");
                        Assert.AreNotEqual(a, c, $"t={t} tri {i} degenerate AC");
                        for (int p = 0; p < n; p++)
                        {
                            if (p == a || p == b || p == c) continue;
                            Assert.IsFalse(InCircle(px[a], py[a], px[b], py[b], px[c], py[c], px[p], py[p]),
                                $"t={t} tri {i}: point {p} inside circumcircle (non-Delaunay)");
                        }
                    }
                    AssertEdgesUsedOnce(tri, cnt);
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)tri);
                    Marshal.FreeHGlobal((nint)px);
                    Marshal.FreeHGlobal((nint)py);
                }
            }
        }

        [Test]
        public void Build_TriangleCount_MatchesEulerInvariant()
        {
            Random rng = new Random(5);
            for (int t = 0; t < 40; t++)
            {
                int n = rng.Next(3, 30);
                double[] sx = new double[n], sy = new double[n];
                for (int i = 0; i < n; i++) { sx[i] = Math.Round(rng.NextDouble() * 200, 1) + i * 0.7; sy[i] = Math.Round(rng.NextDouble() * 200, 1) + (i % 3) * 0.3; }

                Delaunay.Triangle* tri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * (n * 2 + 16));
                double* px = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                double* py = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                try
                {
                    for (int i = 0; i < n; i++) { px[i] = sx[i]; py[i] = sy[i]; }
                    int cnt = Delaunay.Build(px, py, n, tri);
                    int h = ConvexHullSize(sx, sy, n);
                    int expected = 2 * n - 2 - h;
                    Assert.AreEqual(expected, cnt, $"t={t}: Euler invariant 2n-2-h violated (n={n} h={h}) -> non-maximal or buggy triangulation");
                    for (int i = 0; i < cnt; i++)
                    {
                        int a = tri[i].A, b = tri[i].B, c = tri[i].C;
                        for (int p = 0; p < n; p++)
                        {
                            if (p == a || p == b || p == c) continue;
                            Assert.IsFalse(InCircle(px[a], py[a], px[b], py[b], px[c], py[c], px[p], py[p]), $"t={t} tri {i} non-Delaunay");
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)tri);
                    Marshal.FreeHGlobal((nint)px);
                    Marshal.FreeHGlobal((nint)py);
                }
            }
        }

        [Test]
        public void Fortune_VoronoiVertices_EquidistantFromTriangle()
        {
            Random rng = new Random(77);
            for (int t = 0; t < 30; t++)
            {
                int n = rng.Next(3, 30);
                double[] sx = new double[n], sy = new double[n];
                for (int i = 0; i < n; i++) { sx[i] = Math.Round(rng.NextDouble() * 100, 1); sy[i] = Math.Round(rng.NextDouble() * 100, 1); }

                Delaunay.Triangle* tri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * (n * 2 + 16));
                double* px = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                double* py = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                double* vx = (double*)Marshal.AllocHGlobal(sizeof(double) * (n * 2 + 16));
                double* vy = (double*)Marshal.AllocHGlobal(sizeof(double) * (n * 2 + 16));
                int* size = (int*)Marshal.AllocHGlobal(sizeof(int));
                try
                {
                    for (int i = 0; i < n; i++) { px[i] = sx[i]; py[i] = sy[i]; }
                    int triCount = Delaunay.Build(px, py, n, tri);
                    int vorCount = Fortune.Build(px, py, n, vx, vy, size);
                    Assert.IsTrue(vorCount >= 1, $"t={t}: no voronoi vertices");
                    Assert.IsTrue(vorCount <= triCount, $"t={t}: voronoi vertices {vorCount} exceed triangles {triCount}");
                    for (int i = 0; i < vorCount; i++)
                    {
                        double dx = vx[i], dy = vy[i];
                        int a = tri[i].A, b = tri[i].B, c = tri[i].C;
                        double da = SqDist(dx, dy, sx[a], sy[a]);
                        double db = SqDist(dx, dy, sx[b], sy[b]);
                        double dc = SqDist(dx, dy, sx[c], sy[c]);
                        Assert.AreEqual(da, db, 1e-3, $"t={t} v{i}: circumcenter not equidistant a-b");
                        Assert.AreEqual(da, dc, 1e-3, $"t={t} v{i}: circumcenter not equidistant a-c");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)tri);
                    Marshal.FreeHGlobal((nint)px);
                    Marshal.FreeHGlobal((nint)py);
                    Marshal.FreeHGlobal((nint)vx);
                    Marshal.FreeHGlobal((nint)vy);
                    Marshal.FreeHGlobal((nint)size);
                }
            }
        }

        private static double SqDist(double ax, double ay, double bx, double by)
        { double dx = ax - bx, dy = ay - by; return dx * dx + dy * dy; }

        private static int ConvexHullSize(double[] xs, double[] ys, int n)
        {
            if (n < 3) return n;
            int[] idx = new int[n];
            for (int i = 0; i < n; i++) idx[i] = i;
            Array.Sort(idx, (a, b) => xs[a] != xs[b] ? xs[a].CompareTo(xs[b]) : ys[a].CompareTo(ys[b]));
            int[] hull = new int[2 * n];
            int k = 0;
            for (int ii = 0; ii < n; ii++)
            {
                int p = idx[ii];
                while (k >= 2 && Cross(xs, ys, hull[k - 2], hull[k - 1], p) <= 0) k--;
                hull[k++] = p;
            }
            int lower = k + 1;
            for (int ii = n - 2; ii >= 0; ii--)
            {
                int p = idx[ii];
                while (k >= lower && Cross(xs, ys, hull[k - 2], hull[k - 1], p) <= 0) k--;
                hull[k++] = p;
            }
            int h = k - 1;
            return h < 3 ? 3 : h;
        }

        private static double Cross(double[] xs, double[] ys, int a, int b, int c)
            => (xs[b] - xs[a]) * (ys[c] - ys[a]) - (ys[b] - ys[a]) * (xs[c] - xs[a]);

        [Test]
        public void Build_DegenerateInput_NoCrash()
        {
            Delaunay.Triangle* tri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * 8);
            try
            {
                Assert.AreEqual(0, Delaunay.Build(null, null, 0, tri), "n=0");
                double* cx = stackalloc double[3], cy = stackalloc double[3];
                cx[0] = 0; cy[0] = 0; cx[1] = 1; cy[1] = 0; cx[2] = 2; cy[2] = 0;
                Assert.AreEqual(0, Delaunay.Build(cx, cy, 3, tri), "all collinear -> 0");
            }
            finally { Marshal.FreeHGlobal((nint)tri); }
        }

        private static void AssertEdgesUsedOnce(Delaunay.Triangle* tri, int cnt)
        {
            var counts = new System.Collections.Generic.Dictionary<long, int>();
            for (int i = 0; i < cnt; i++)
            {
                Inc(counts, EdgeKey(tri[i].A, tri[i].B));
                Inc(counts, EdgeKey(tri[i].B, tri[i].C));
                Inc(counts, EdgeKey(tri[i].C, tri[i].A));
            }
            foreach (var kv in counts)
            {
                Assert.IsTrue(kv.Value == 1 || kv.Value == 2, $"edge {kv.Key} used {kv.Value} times");
            }
        }

        private static void Inc(System.Collections.Generic.Dictionary<long, int> d, long k)
        { if (d.ContainsKey(k)) d[k]++; else d[k] = 1; }

        private static long EdgeKey(int a, int b) => a < b ? ((long)a << 32) | (uint)b : ((long)b << 32) | (uint)a;

        private static long CanonKey(int a, int b, int c)
        {
            int x = a, y = b, z = c;
            if (x > y) { int t = x; x = y; y = t; }
            if (y > z) { int t = y; y = z; z = t; }
            if (x > y) { int t = x; x = y; y = t; }
            return ((long)x << 32) | ((long)y << 16) | (uint)z;
        }

        private static (long,long,long) DecodeCanon(long key)
        {
            long z = key & 0xFFFF; long y = (key >> 16) & 0xFFFF; long x = (key >> 32) & 0xFFFF;
            return (x, y, z);
        }

        private static System.Collections.Generic.HashSet<long> BruteDelaunaySet(double[] xs, double[] ys, int n)
        {
            var set = new System.Collections.Generic.HashSet<long>();
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    for (int k = j + 1; k < n; k++)
                    {
                        double cr = (xs[j] - xs[i]) * (ys[k] - ys[i]) - (ys[j] - ys[i]) * (xs[k] - xs[i]);
                        if (Math.Abs(cr) < 1e-9) continue;
                        bool ok = true;
                        for (int p = 0; p < n && ok; p++)
                        {
                            if (p == i || p == j || p == k) continue;
                            if (InCircle(xs[i], ys[i], xs[j], ys[j], xs[k], ys[k], xs[p], ys[p])) ok = false;
                        }
                        if (ok) set.Add(CanonKey(i, j, k));
                    }
            return set;
        }

        private static bool InCircle(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            double adx = ax - dx, ady = ay - dy, bdx = bx - dx, bdy = by - dy, cdx = cx - dx, cdy = cy - dy;
            double abdet = adx * bdy - bdx * ady;
            double bcdet = bdx * cdy - cdx * bdy;
            double cadet = cdx * ady - adx * cdy;
            double det = (adx * adx + ady * ady) * bcdet + (bdx * bdx + bdy * bdy) * cadet + (cdx * cdx + cdy * cdy) * abdet;
            double orient = (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
            if (orient > 0) return det > 1e-7;
            if (orient < 0) return det < -1e-7;
            return false;
        }
    }
}
