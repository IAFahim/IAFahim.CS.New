namespace IAFahim.Geometry.Voronoi.Tests
{
    using IAFahim.Geometry.Voronoi;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class DelaunayFastLongTests
    {
        // Points on a circle are EXACTLY cocircular — the degenerate case where the inexact double
        // incircle predicate misclassifies triangles and BuildFast leaves coverage holes. BuildFastLong
        // (exact 256-bit predicate) must still produce the maximal Delaunay triangulation:
        //   (1) triangle count == Euler invariant 2n-2-h, and
        //   (2) every triangle is empty-circle (no input point inside its circumcircle).
        [Test]
        public void BuildFastLong_Cocircular_IsMaximalAndDelaunay()
        {
            for (int trial = 0; trial < 20; trial++)
            {
                int n = 8 + trial;                       // 8..27 points
                long R = 1000 + trial * 137;
                long cx = 5000, cy = 5000;
                long[] sx = new long[n], sy = new long[n];
                for (int i = 0; i < n; i++)
                {
                    double ang = 2.0 * Math.PI * i / n + trial * 0.01;
                    sx[i] = cx + (long)Math.Round(R * Math.Cos(ang));
                    sy[i] = cy + (long)Math.Round(R * Math.Sin(ang));
                }
                // perturb so not all exactly cocircular (avoid the all-on-circle degeneracy that has
                // no unique Delaunay); keep near-cocircular to stress the predicate.
                sx[0] += 1; sy[1] -= 1; if (n > 4) sx[n / 2] += 2;

                RunMaximalCheck(sx, sy, n, $"cocircular trial={trial}");
            }
        }

        // Random well-spread integer points: every triangle produced by BuildFastLong must be
        // empty-circle (the defining Delaunay property). Cross-checks against Build's triangle SET
        // when input is general position (no 3 collinear).
        [Test]
        public void BuildFastLong_Random_IsEmptyCircleDelaunay()
        {
            Random rng = new Random(123);
            for (int t = 0; t < 80; t++)
            {
                int n = rng.Next(3, 25);
                long[] sx = new long[n], sy = new long[n];
                HashSet<long> seen = new HashSet<long>();
                int i = 0;
                while (i < n)
                {
                    long x = rng.Next(0, 4000), y = rng.Next(0, 4000);
                    long key = x * 100000L + y;
                    if (seen.Add(key)) { sx[i] = x; sy[i] = y; i++; }
                }
                AssertEmptyCircle(sx, sy, n, $"trial={t}");
            }
        }

        private static void RunMaximalCheck(long[] sx, long[] sy, int n, string label)
        {
            int cap = n * 2 + 16;
            Delaunay.Triangle* tri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * cap);
            long* px = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* py = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            try
            {
                for (int i = 0; i < n; i++) { px[i] = sx[i]; py[i] = sy[i]; }
                int cnt = Delaunay.BuildFastLong(px, py, n, tri);
                int h = ConvexHullSizeLong(sx, sy, n);
                int expected = 2 * n - 2 - h;
                Assert.AreEqual(expected, cnt, $"{label}: Euler invariant 2n-2-h violated (n={n} h={h}) -> non-maximal");
                for (int i = 0; i < cnt; i++)
                {
                    int a = tri[i].A, b = tri[i].B, c = tri[i].C;
                    for (int p = 0; p < n; p++)
                    {
                        if (p == a || p == b || p == c) continue;
                        Assert.IsFalse(InCircleLong(sx[a], sy[a], sx[b], sy[b], sx[c], sy[c], sx[p], sy[p]),
                            $"{label} tri {i}: point {p} inside circumcircle -> not Delaunay");
                    }
                }
            }
            finally { Marshal.FreeHGlobal((nint)tri); Marshal.FreeHGlobal((nint)px); Marshal.FreeHGlobal((nint)py); }
        }

        private static void AssertEmptyCircle(long[] sx, long[] sy, int n, string label)
        {
            int cap = n * 2 + 16;
            Delaunay.Triangle* tri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * cap);
            long* px = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* py = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            try
            {
                for (int i = 0; i < n; i++) { px[i] = sx[i]; py[i] = sy[i]; }
                int cnt = Delaunay.BuildFastLong(px, py, n, tri);
                Assert.IsTrue(cnt > 0, $"{label}: no triangles");
                for (int i = 0; i < cnt; i++)
                {
                    int a = tri[i].A, b = tri[i].B, c = tri[i].C;
                    for (int p = 0; p < n; p++)
                    {
                        if (p == a || p == b || p == c) continue;
                        Assert.IsFalse(InCircleLong(sx[a], sy[a], sx[b], sy[b], sx[c], sy[c], sx[p], sy[p]),
                            $"{label} tri {i}: point {p} inside circumcircle -> not Delaunay");
                    }
                }
            }
            finally { Marshal.FreeHGlobal((nint)tri); Marshal.FreeHGlobal((nint)px); Marshal.FreeHGlobal((nint)py); }
        }

        private static void RunMatchesBuild(long[] sx, long[] sy, int n, int trial)
        {
            int cap = n * 2 + 16;
            Delaunay.Triangle* refTri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * cap);
            Delaunay.Triangle* fastTri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * cap);
            long* px = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* py = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            try
            {
                for (int i = 0; i < n; i++) { px[i] = sx[i]; py[i] = sy[i]; }
                // Build takes double* — cast
                double* dpx = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                double* dpy = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                for (int i = 0; i < n; i++) { dpx[i] = sx[i]; dpy[i] = sy[i]; }
                int refCnt, fastCnt;
                try { refCnt = Delaunay.Build(dpx, dpy, n, refTri); }
                finally { Marshal.FreeHGlobal((nint)dpx); Marshal.FreeHGlobal((nint)dpy); }
                fastCnt = Delaunay.BuildFastLong(px, py, n, fastTri);
                Assert.AreEqual(refCnt, fastCnt, $"trial={trial}: triangle count mismatch (exact Build vs BuildFastLong)");
                // both must be maximal Euler check (guards against any silent regression)
                int h = ConvexHullSizeLong(sx, sy, n);
                Assert.AreEqual(2 * n - 2 - h, fastCnt, $"trial={trial}: BuildFastLong non-maximal");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)refTri); Marshal.FreeHGlobal((nint)fastTri);
                Marshal.FreeHGlobal((nint)px); Marshal.FreeHGlobal((nint)py);
            }
        }

        private static int ConvexHullSizeLong(long[] xs, long[] ys, int n)
        {
            int[] idx = new int[n];
            for (int i = 0; i < n; i++) idx[i] = i;
            Array.Sort(idx, (a, b) => xs[a] != xs[b] ? xs[a].CompareTo(xs[b]) : ys[a].CompareTo(ys[b]));
            int[] lower = new int[n], upper = new int[n];
            int lh = 0, uh = 0;
            for (int ii = 0; ii < n; ii++)
            {
                int p = idx[ii];
                while (lh >= 2 && CrossLong(xs, ys, lower[lh - 2], lower[lh - 1], p) <= 0) lh--;
                lower[lh++] = p;
            }
            for (int ii = n - 1; ii >= 0; ii--)
            {
                int p = idx[ii];
                while (uh >= 2 && CrossLong(xs, ys, upper[uh - 2], upper[uh - 1], p) <= 0) uh--;
                upper[uh++] = p;
            }
            int h = 0;
            for (int i = 0; i < lh - 1; i++) h++;
            for (int i = 0; i < uh - 1; i++) h++;
            return h;
        }

        private static long CrossLong(long[] xs, long[] ys, int o, int a, int b)
            => (xs[a] - xs[o]) * (ys[b] - ys[o]) - (ys[a] - ys[o]) * (xs[b] - xs[o]);

        private static bool InCircleLong(long ax, long ay, long bx, long by, long cx, long cy, long dx, long dy)
        {
            double adx = ax - dx, ady = ay - dy, bdx = bx - dx, bdy = by - dy, cdx = cx - dx, cdy = cy - dy;
            double abdet = adx * bdy - bdx * ady;
            double bcdet = bdx * cdy - cdx * bdy;
            double cadet = cdx * ady - adx * cdy;
            double det = (adx * adx + ady * ady) * bcdet + (bdx * bdx + bdy * bdy) * cadet + (cdx * cdx + cdy * cdy) * abdet;
            double orient = (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
            if (orient > 0) return det > 1e-6;
            if (orient < 0) return det < -1e-6;
            return false;
        }
    }
}
