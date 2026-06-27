namespace IAFahim.Geometry.Voronoi.Tests
{
    using IAFahim.Geometry.Voronoi;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class DelaunayFlipTests
    {
        // For each convex quadrilateral, there are two triangulations (one diagonal each). Exactly one
        // is Delaunay. We build the non-Delaunay one, run Flip on the shared edge, and check the result
        // equals the Delaunay triangulation from Build. Also checks adjacency stays consistent.
        [Test]
        public void Flip_ConvertsToDelaunay_Random()
        {
            Random rng = new Random(555);
            for (int t = 0; t < 200; t++)
            {
                // 4 points on a strictly convex curve y=x^2 (slight noise to avoid cocircularity):
                // in t-sorted order the polygon is convex, so diagonals are (0,2) and (1,3).
                double[] ts = new double[4];
                for (int i = 0; i < 4; i++) ts[i] = i * 3.0 + 1.0 + rng.NextDouble() * 2.0;
                Array.Sort(ts);
                double[] px = new double[4], py = new double[4];
                for (int i = 0; i < 4; i++) { px[i] = ts[i]; py[i] = ts[i] * ts[i] + rng.NextDouble() * 0.5; }
                Fixed4(px, py, out double* xs, out double* ys);
                try
                {
                    Delaunay.Triangle* refTri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * 4);
                    Delaunay.Triangle* tri = (Delaunay.Triangle*)Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * 2);
                    int* adj = (int*)Marshal.AllocHGlobal(sizeof(int) * 6);
                    try
                    {
                        int refCnt = Delaunay.Build(xs, ys, 4, refTri);
                        if (refCnt != 2) continue;   // skip degenerate (collinear) quads

                        // triangulation A: diagonal (0,2) -> triangles (0,1,2) and (0,2,3)
                        // triangulation B: diagonal (1,3) -> triangles (1,2,3) and (0,1,3)
                        // try both; one is Delaunay, the other flips to it.
                        for (int variant = 0; variant < 2; variant++)
                        {
                            if (variant == 0)
                            {
                                tri[0] = new Delaunay.Triangle { A = 0, B = 1, C = 2 };
                                tri[1] = new Delaunay.Triangle { A = 0, B = 2, C = 3 };
                            }
                            else
                            {
                                tri[0] = new Delaunay.Triangle { A = 1, B = 2, C = 3 };
                                tri[1] = new Delaunay.Triangle { A = 0, B = 1, C = 3 };
                            }
                            BuildAdj(tri, 2, adj);
                            bool flipped = Delaunay.Flip(xs, ys, tri, adj, 0, 1);

                            // Either already Delaunay (flipped==false) or now Delaunay after one flip.
                            Assert.IsTrue(TriSetEqual(tri, 2, refTri, 2), $"t={t} variant={variant} flipped={flipped}");
                            Assert.IsTrue(AdjConsistent(tri, 2, adj), $"t={t} variant={variant} adj");
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal((nint)refTri); Marshal.FreeHGlobal((nint)tri); Marshal.FreeHGlobal((nint)adj);
                    }
                }
                finally { Marshal.FreeHGlobal((nint)xs); Marshal.FreeHGlobal((nint)ys); }
            }
        }

        private static void BuildAdj(Delaunay.Triangle* tri, int m, int* adj)
        {
            for (int t = 0; t < m; t++) for (int s = 0; s < 3; s++) adj[t * 3 + s] = -1;
            for (int t = 0; t < m; t++)
            {
                int a0 = tri[t].A, b0 = tri[t].B, c0 = tri[t].C;
                for (int u = t + 1; u < m; u++)
                {
                    int a1 = tri[u].A, b1 = tri[u].B, c1 = tri[u].C;
                    int side0 = SharedSide(a0, b0, c0, a1, b1, c1);
                    if (side0 < 0) continue;
                    int side1 = SharedSide(a1, b1, c1, a0, b0, c0);
                    adj[t * 3 + side0] = u;
                    adj[u * 3 + side1] = t;
                }
            }
        }

        private static int SharedSide(int a, int b, int c, int a2, int b2, int c2)
        {
            int[] va = { a, b, c };
            for (int s = 0; s < 3; s++)
            {
                int e1 = va[(s + 1) % 3], e2 = va[(s + 2) % 3];
                int cnt = 0;
                if (e1 == a2 || e1 == b2 || e1 == c2) cnt++;
                if (e2 == a2 || e2 == b2 || e2 == c2) cnt++;
                if (cnt == 2) return s;
            }
            return -1;
        }

        private static bool TriSetEqual(Delaunay.Triangle* x, int nx, Delaunay.Triangle* y, int ny)
        {
            if (nx != ny) return false;
            for (int i = 0; i < nx; i++)
            {
                bool found = false;
                for (int j = 0; j < ny; j++)
                    if (SameTri(x[i], y[j])) { found = true; break; }
                if (!found) return false;
            }
            return true;
        }

        private static bool SameTri(Delaunay.Triangle p, Delaunay.Triangle q)
        {
            int[] a = { p.A, p.B, p.C };
            int[] b = { q.A, q.B, q.C };
            Array.Sort(a); Array.Sort(b);
            return a[0] == b[0] && a[1] == b[1] && a[2] == b[2];
        }

        private static bool AdjConsistent(Delaunay.Triangle* tri, int m, int* adj)
        {
            for (int t = 0; t < m; t++)
                for (int s = 0; s < 3; s++)
                {
                    int nb = adj[t * 3 + s];
                    if (nb < 0) continue;
                    bool back = false;
                    for (int s2 = 0; s2 < 3; s2++) if (adj[nb * 3 + s2] == t) { back = true; break; }
                    if (!back) return false;
                    // shared edge check
                    int[] va = { tri[t].A, tri[t].B, tri[t].C };
                    int e1 = va[(s + 1) % 3], e2 = va[(s + 2) % 3];
                    int[] vb = { tri[nb].A, tri[nb].B, tri[nb].C };
                    int cnt = 0;
                    foreach (int vv in vb) if (vv == e1 || vv == e2) cnt++;
                    if (cnt != 2) return false;
                }
            return true;
        }

        private static void Fixed4(double[] px, double[] py, out double* xs, out double* ys)
        {
            xs = (double*)Marshal.AllocHGlobal(sizeof(double) * 4);
            ys = (double*)Marshal.AllocHGlobal(sizeof(double) * 4);
            for (int i = 0; i < 4; i++) { xs[i] = px[i]; ys[i] = py[i]; }
        }
    }
}
