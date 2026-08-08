namespace IAFahim.Geometry.Advanced.Tests
{
    using IAFahim.Geometry.Advanced;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class PolygonBooleanTests
    {
        [Test]
        public void Intersection_MatchesGridAreaAndContainment_Random()
        {
            Random rng = new Random(91);
            for (int t = 0; t < 120; t++)
            {
                ConvexPoly a = RandConvex(rng), b = RandConvex(rng);
                double minX = Math.Min(a.MinX, b.MinX), maxX = Math.Max(a.MaxX, b.MaxX);
                double minY = Math.Min(a.MinY, b.MaxY), maxY = Math.Max(a.MinY, b.MaxY);
                // brute area via grid sampling
                int G = 80;
                double dx = (maxX - minX) / G; if (dx <= 0) dx = 1;
                double dy = (maxY - minY) / G; if (dy <= 0) dy = 1;
                double cell = dx * dy;
                double bruteArea = 0;
                for (int gi = 0; gi <= G; gi++)
                    for (int gj = 0; gj <= G; gj++)
                    {
                        double px = minX + gi * dx, py = minY + gj * dy;
                        if (PointInConvex(a, px, py) && PointInConvex(b, px, py)) bruteArea += cell;
                    }

                double* sx = (double*)Marshal.AllocHGlobal(sizeof(double) * a.N);
                double* sy = (double*)Marshal.AllocHGlobal(sizeof(double) * a.N);
                double* cx = (double*)Marshal.AllocHGlobal(sizeof(double) * b.N);
                double* cy = (double*)Marshal.AllocHGlobal(sizeof(double) * b.N);
                int cap = 2 * (a.N + b.N) + 8;
                double* ox = (double*)Marshal.AllocHGlobal(sizeof(double) * cap);
                double* oy = (double*)Marshal.AllocHGlobal(sizeof(double) * cap);
                try
                {
                    for (int i = 0; i < a.N; i++) { sx[i] = a.X[i]; sy[i] = a.Y[i]; }
                    for (int i = 0; i < b.N; i++) { cx[i] = b.X[i]; cy[i] = b.Y[i]; }
                    int on = PolygonBoolean.Intersection(sx, sy, a.N, cx, cy, b.N, ox, oy);

                    // every output vertex must be inside both inputs (within epsilon)
                    for (int i = 0; i < on; i++)
                    {
                        Assert.IsTrue(PointInConvex(a, ox[i], oy[i]) || NearBoundary(a, ox[i], oy[i]), $"t={t} vert {i} not in A");
                        Assert.IsTrue(PointInConvex(b, ox[i], oy[i]) || NearBoundary(b, ox[i], oy[i]), $"t={t} vert {i} not in B");
                    }
                    // shoelace area of output must be close to grid area (within one grid cell band)
                    double implArea = on >= 3 ? Shoelace(ox, oy, on) : 0;
                    double tol = cell * (G + 2);
                    Assert.AreEqual(bruteArea, implArea, tol, $"t={t} on={on} brute={bruteArea:F2} impl={implArea:F2}");
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)sx); Marshal.FreeHGlobal((nint)sy);
                    Marshal.FreeHGlobal((nint)cx); Marshal.FreeHGlobal((nint)cy);
                    Marshal.FreeHGlobal((nint)ox); Marshal.FreeHGlobal((nint)oy);
                }
            }
        }

        private struct ConvexPoly { public double[] X, Y; public int N; public double MinX, MaxX, MinY, MaxY; }

        private static ConvexPoly RandConvex(Random rng)
        {
            // random points -> convex hull (Andrew monotone chain) guarantees a CCW convex polygon
            int npts = rng.Next(4, 12);
            double cx = rng.NextDouble() * 50, cy = rng.NextDouble() * 50;
            double R = 8 + rng.NextDouble() * 20;
            double[] px = new double[npts], py = new double[npts];
            for (int i = 0; i < npts; i++) { px[i] = cx + (rng.NextDouble() * 2 - 1) * R; py[i] = cy + (rng.NextDouble() * 2 - 1) * R; }
            Hull(px, py, npts, out double[] hx, out double[] hy, out int hn);
            ConvexPoly p = new ConvexPoly { X = hx, Y = hy, N = hn, MinX = double.MaxValue, MaxX = double.MinValue, MinY = double.MaxValue, MaxY = double.MinValue };
            for (int i = 0; i < hn; i++)
            {
                if (hx[i] < p.MinX) p.MinX = hx[i]; if (hx[i] > p.MaxX) p.MaxX = hx[i];
                if (hy[i] < p.MinY) p.MinY = hy[i]; if (hy[i] > p.MaxY) p.MaxY = hy[i];
            }
            return p;
        }

        private static void Hull(double[] px, double[] py, int n, out double[] hx, out double[] hy, out int hn)
        {
            int[] idx = new int[n]; for (int i = 0; i < n; i++) idx[i] = i;
            Array.Sort(idx, (a, b) => px[a] != px[b] ? px[a].CompareTo(px[b]) : py[a].CompareTo(py[b]));
            double[] X = new double[n], Y = new double[n];
            for (int i = 0; i < n; i++) { X[i] = px[idx[i]]; Y[i] = py[idx[i]]; }
            int[] stack = new int[2 * n];
            int k = 0;
            for (int i = 0; i < n; i++) { while (k >= 2 && Cross(X, Y, stack[k - 2], stack[k - 1], i) <= 0) k--; stack[k++] = i; }
            int lower = k + 1;
            for (int i = n - 2; i >= 0; i--) { while (k >= lower && Cross(X, Y, stack[k - 2], stack[k - 1], i) <= 0) k--; stack[k++] = i; }
            hn = k - 1;
            hx = new double[hn]; hy = new double[hn];
            for (int i = 0; i < hn; i++) { hx[i] = X[stack[i]]; hy[i] = Y[stack[i]]; }
        }

        private static double Cross(double[] X, double[] Y, int o, int a, int b)
            => (X[a] - X[o]) * (Y[b] - Y[o]) - (Y[a] - Y[o]) * (X[b] - X[o]);

        private static bool PointInConvex(ConvexPoly p, double px, double py)
        {
            for (int i = 0; i < p.N; i++)
            {
                double ax = p.X[i], ay = p.Y[i];
                double bx = p.X[(i + 1) % p.N], by = p.Y[(i + 1) % p.N];
                if ((bx - ax) * (py - ay) - (by - ay) * (px - ax) < -1e-9) return false;
            }
            return true;
        }

        private static bool NearBoundary(ConvexPoly p, double px, double py)
        {
            for (int i = 0; i < p.N; i++)
            {
                double ax = p.X[i], ay = p.Y[i];
                double bx = p.X[(i + 1) % p.N], by = p.Y[(i + 1) % p.N];
                double c = (bx - ax) * (py - ay) - (by - ay) * (px - ax);
                if (Math.Abs(c) < 1e-6) return true;
            }
            return false;
        }

        private static double Shoelace(double* x, double* y, int n)
        {
            double s = 0;
            for (int i = 0; i < n; i++) { int j = (i + 1) % n; s += x[i] * y[j] - x[j] * y[i]; }
            return Math.Abs(s) * 0.5;
        }

        [Test]
        public void Union_TwoOverlappingUnitSquares_AreaIs1_75()
        {
            double* ax = stackalloc double[4] { 0, 1, 1, 0 };
            double* ay = stackalloc double[4] { 0, 0, 1, 1 };
            double* bx = stackalloc double[4] { 0.5, 1.5, 1.5, 0.5 };
            double* by = stackalloc double[4] { 0.5, 0.5, 1.5, 1.5 };
            double* ox = stackalloc double[32];
            double* oy = stackalloc double[32];
            int n = PolygonBoolean.Union(ax, ay, 4, bx, by, 4, ox, oy, 32);
            Assert.IsTrue(n >= 6, $"n={n}");
            double area = Shoelace(ox, oy, n);
            Assert.AreEqual(1.75, area, 1e-6, $"area={area} n={n}");
        }

        [Test]
        public void Difference_PartialOverlap_AreaIs0_75()
        {
            double* ax = stackalloc double[4] { 0, 1, 1, 0 };
            double* ay = stackalloc double[4] { 0, 0, 1, 1 };
            double* bx = stackalloc double[4] { 0.5, 1.5, 1.5, 0.5 };
            double* by = stackalloc double[4] { 0.5, 0.5, 1.5, 1.5 };
            double* ox = stackalloc double[32];
            double* oy = stackalloc double[32];
            int n = PolygonBoolean.Difference(ax, ay, 4, bx, by, 4, ox, oy, 32);
            Assert.IsTrue(n >= 4, $"n={n}");
            double area = Shoelace(ox, oy, n);
            Assert.AreEqual(0.75, area, 1e-6, $"area={area} n={n}");
        }

        [Test]
        public void Difference_ContainedSquare_ReturnsOuterA()
        {
            double* ax = stackalloc double[4] { 0, 2, 2, 0 };
            double* ay = stackalloc double[4] { 0, 0, 2, 2 };
            double* bx = stackalloc double[4] { 0.5, 1.5, 1.5, 0.5 };
            double* by = stackalloc double[4] { 0.5, 0.5, 1.5, 1.5 };
            double* ox = stackalloc double[32];
            double* oy = stackalloc double[32];
            int n = PolygonBoolean.Difference(ax, ay, 4, bx, by, 4, ox, oy, 32);
            Assert.IsTrue(n >= 4);
            double area = Shoelace(ox, oy, n);
            Assert.AreEqual(4.0, area, 1e-6, $"area={area}");
        }

        [Test]
        public void Xor_PartialOverlap_FirstComponentArea0_75()
        {
            double* ax = stackalloc double[4] { 0, 1, 1, 0 };
            double* ay = stackalloc double[4] { 0, 0, 1, 1 };
            double* bx = stackalloc double[4] { 0.5, 1.5, 1.5, 0.5 };
            double* by = stackalloc double[4] { 0.5, 0.5, 1.5, 1.5 };
            double* ox = stackalloc double[32];
            double* oy = stackalloc double[32];
            int n = PolygonBoolean.Xor(ax, ay, 4, bx, by, 4, ox, oy, 32);
            Assert.IsTrue(n >= 4, $"n={n}");
            double area = Shoelace(ox, oy, n);
            Assert.AreEqual(0.75, area, 1e-6, $"area={area}");
        }

        [Test]
        public void Union_Disjoint_ReturnsFirstPoly()
        {
            double* ax = stackalloc double[4] { 0, 1, 1, 0 };
            double* ay = stackalloc double[4] { 0, 0, 1, 1 };
            double* bx = stackalloc double[4] { 3, 4, 4, 3 };
            double* by = stackalloc double[4] { 3, 3, 4, 4 };
            double* ox = stackalloc double[16];
            double* oy = stackalloc double[16];
            int n = PolygonBoolean.Union(ax, ay, 4, bx, by, 4, ox, oy, 16);
            Assert.AreEqual(4, n);
            Assert.IsTrue(Math.Abs(Shoelace(ox, oy, n) - 1.0) < 1e-9);
        }

        [Test]
        public void Xor_Identical_EmptyOrZeroArea()
        {
            double* ax = stackalloc double[4] { 0, 1, 1, 0 };
            double* ay = stackalloc double[4] { 0, 0, 1, 1 };
            double* ox = stackalloc double[16];
            double* oy = stackalloc double[16];
            int n = PolygonBoolean.Xor(ax, ay, 4, ax, ay, 4, ox, oy, 16);
            // identical → xor empty (0) or degenerate
            if (n >= 3)
                Assert.IsTrue(Shoelace(ox, oy, n) < 1e-6);
            else
                Assert.IsTrue(n == 0 || n < 3);
        }
    }
}
