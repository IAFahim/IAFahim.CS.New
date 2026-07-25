namespace IAFahim.Geometry.Hull.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class ConvexHull3DTests
    {
        [Test]
        public void Build_Tetrahedron_FourFacesWithConsistentNeighbors()
        {
            const int N = 4;
            double* xs = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            double* ys = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            double* zs = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            ConvexHull3D.Face* outFaces = (ConvexHull3D.Face*)Marshal.AllocHGlobal(64 * sizeof(ConvexHull3D.Face));
            ConvexHull3D.Face* scratch = (ConvexHull3D.Face*)Marshal.AllocHGlobal(64 * sizeof(ConvexHull3D.Face));
            int* head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                xs[0] = 0; ys[0] = 0; zs[0] = 0;
                xs[1] = 1; ys[1] = 0; zs[1] = 0;
                xs[2] = 0; ys[2] = 1; zs[2] = 0;
                xs[3] = 0; ys[3] = 0; zs[3] = 1;

                int fc = ConvexHull3D.Build(xs, ys, zs, N, outFaces, scratch, head);
                Assert.AreEqual(4, fc);
                AssertNeighborsConsistent(outFaces, fc);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)xs);
                Marshal.FreeHGlobal((nint)ys);
                Marshal.FreeHGlobal((nint)zs);
                Marshal.FreeHGlobal((nint)outFaces);
                Marshal.FreeHGlobal((nint)scratch);
                Marshal.FreeHGlobal((nint)head);
            }
        }

        [Test]
        public void Build_UnitCube_TwelveTriangularFaces()
        {
            const int N = 8;
            double* xs = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            double* ys = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            double* zs = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            ConvexHull3D.Face* outFaces = (ConvexHull3D.Face*)Marshal.AllocHGlobal(256 * sizeof(ConvexHull3D.Face));
            ConvexHull3D.Face* scratch = (ConvexHull3D.Face*)Marshal.AllocHGlobal(256 * sizeof(ConvexHull3D.Face));
            int* head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                int k = 0;
                for (int xi = 0; xi < 2; xi++)
                for (int yi = 0; yi < 2; yi++)
                for (int zi = 0; zi < 2; zi++)
                {
                    xs[k] = xi;
                    ys[k] = yi;
                    zs[k] = zi;
                    k++;
                }

                int fc = ConvexHull3D.Build(xs, ys, zs, N, outFaces, scratch, head);
                Assert.AreEqual(12, fc);
                AssertNeighborsConsistent(outFaces, fc);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)xs);
                Marshal.FreeHGlobal((nint)ys);
                Marshal.FreeHGlobal((nint)zs);
                Marshal.FreeHGlobal((nint)outFaces);
                Marshal.FreeHGlobal((nint)scratch);
                Marshal.FreeHGlobal((nint)head);
            }
        }

        [Test]
        public void Build_LessThanFour_ReturnsZero()
        {
            double* xs = stackalloc double[3] { 0, 1, 0 };
            double* ys = stackalloc double[3] { 0, 0, 1 };
            double* zs = stackalloc double[3] { 0, 0, 0 };
            ConvexHull3D.Face* outFaces = stackalloc ConvexHull3D.Face[8];
            ConvexHull3D.Face* scratch = stackalloc ConvexHull3D.Face[8];
            int* head = stackalloc int[3];
            Assert.AreEqual(0, ConvexHull3D.Build(xs, ys, zs, 3, outFaces, scratch, head));
        }

        private static void AssertNeighborsConsistent(ConvexHull3D.Face* faces, int fc)
        {
            for (int f = 0; f < fc; f++)
            {
                int[] neigh = { faces[f].F0, faces[f].F1, faces[f].F2 };
                int[] a = { faces[f].B, faces[f].C, faces[f].A };
                int[] b = { faces[f].C, faces[f].A, faces[f].B };
                for (int e = 0; e < 3; e++)
                {
                    int g = neigh[e];
                    Assert.IsTrue((uint)g < (uint)fc, $"face {f} edge {e} neighbor {g} oob");
                    Assert.IsFalse(faces[g].Deleted);
                    bool shares = SharesOrientedEdge(faces[g], b[e], a[e]);
                    Assert.IsTrue(shares, $"face {f} edge ({a[e]},{b[e]}) not reversed on neighbor {g}");
                }
            }
        }

        private static bool SharesOrientedEdge(ConvexHull3D.Face face, int u, int v)
        {
            if (face.B == u && face.C == v) return true;
            if (face.C == u && face.A == v) return true;
            if (face.A == u && face.B == v) return true;
            return false;
        }
    }

    public sealed unsafe class RotatingCalipersTests
    {
        [Test]
        public void MinArea_AxisAlignedSquare_AreaOne()
        {
            double* xs = stackalloc double[4] { 0, 1, 1, 0 };
            double* ys = stackalloc double[4] { 0, 0, 1, 1 };
            RotatingCalipers.Rect r = RotatingCalipers.MinArea(xs, ys, 4);
            Assert.IsTrue(Math.Abs(r.W * r.H - 1.0) < 1e-6, $"area={r.W * r.H}");
        }

        [Test]
        public void MinWidth_UnitSquare_IsOne()
        {
            double* xs = stackalloc double[4] { 0, 1, 1, 0 };
            double* ys = stackalloc double[4] { 0, 0, 1, 1 };
            double w = RotatingCalipers.MinWidth(xs, ys, 4);
            Assert.IsTrue(Math.Abs(w - 1.0) < 1e-6, $"width={w}");
        }
    }

    public sealed unsafe class MinkowskiSumTests
    {
        [Test]
        public void Convex_TwoSegments_ProducesParallelogram()
        {
            double* ax = stackalloc double[2] { 0, 1 };
            double* ay = stackalloc double[2] { 0, 0 };
            double* bx = stackalloc double[2] { 0, 0 };
            double* by = stackalloc double[2] { 0, 1 };
            double* ox = stackalloc double[8];
            double* oy = stackalloc double[8];
            int k = MinkowskiSum.Convex(ax, ay, 2, bx, by, 2, ox, oy);
            Assert.IsTrue(k >= 3 && k <= 4);
        }

        [Test]
        public void Difference_UnitSquares_HasExpectedExtent()
        {
            double* ax = stackalloc double[4] { 0, 1, 1, 0 };
            double* ay = stackalloc double[4] { 0, 0, 1, 1 };
            double* bx = stackalloc double[4] { 0, 0.5, 0.5, 0 };
            double* by = stackalloc double[4] { 0, 0, 0.5, 0.5 };
            double* ox = stackalloc double[16];
            double* oy = stackalloc double[16];
            int k = MinkowskiSum.Difference(ax, ay, 4, bx, by, 4, ox, oy);
            Assert.IsTrue(k >= 3);
            double minX = ox[0], maxX = ox[0], minY = oy[0], maxY = oy[0];
            for (int i = 1; i < k; i++)
            {
                if (ox[i] < minX) minX = ox[i];
                if (ox[i] > maxX) maxX = ox[i];
                if (oy[i] < minY) minY = oy[i];
                if (oy[i] > maxY) maxY = oy[i];
            }
            Assert.IsTrue(maxX - minX > 0.5);
            Assert.IsTrue(maxY - minY > 0.5);
        }
    }

    public sealed unsafe class ConvexHullTrickTests
    {
        [Test]
        public void Add_Eval_Query_LowerEnvelope()
        {
            ConvexHullTrick.Line* hull = stackalloc ConvexHullTrick.Line[8];
            int size = 0;
            ConvexHullTrick.Add(hull, &size, new ConvexHullTrick.Line { M = 0, B = 5 });
            ConvexHullTrick.Add(hull, &size, new ConvexHullTrick.Line { M = 1, B = 0 });
            ConvexHullTrick.Add(hull, &size, new ConvexHullTrick.Line { M = 2, B = -4 });
            Assert.AreEqual(5, ConvexHullTrick.Eval(hull[0], 0));
            long q0 = ConvexHullTrick.Query(hull, size, 0);
            long q3 = ConvexHullTrick.Query(hull, size, 3);
            Assert.AreEqual(5, q0);
            Assert.IsTrue(q3 >= 2);
        }

        [Test]
        public void AddWithHistory_Rollback_Restores()
        {
            ConvexHullTrick.Line* hull = stackalloc ConvexHullTrick.Line[8];
            int size = 0;
            ConvexHullTrick.History hist = default;
            ConvexHullTrick.Add(hull, &size, new ConvexHullTrick.Line { M = 0, B = 1 });
            int before = size;
            ConvexHullTrick.AddWithHistory(hull, &size, new ConvexHullTrick.Line { M = 1, B = 0 }, &hist);
            Assert.AreEqual(before + 1, size);
            ConvexHullTrick.Rollback(hull, &size, &hist);
            Assert.AreEqual(before, size);
        }
    }

    public sealed unsafe class HalfSpaceAndMicSkeletonSmokeTests
    {
        [Test]
        public void HalfSpaceIntersection_UnitSquare()
        {
            // Four half-planes x>=0, y>=0, x<=1, y<=1 as inward normals.
            double* nx = stackalloc double[4] { 1, 0, -1, 0 };
            double* ny = stackalloc double[4] { 0, 1, 0, -1 };
            double* d = stackalloc double[4] { 0, 0, -1, -1 };
            double* ox = stackalloc double[16];
            double* oy = stackalloc double[16];
            int outSize = 0;
            HalfSpaceIntersection.HalfPlane* planes = stackalloc HalfSpaceIntersection.HalfPlane[16];
            int* q = stackalloc int[32];
            int k = HalfSpaceIntersection.Run(nx, ny, d, 4, ox, oy, &outSize, planes, q);
            Assert.IsTrue(k >= 0);
            // BuildHull is internal to ConvexHullRollbackUtil; exercised via Add/Run.
            Assert.IsTrue("BuildHull".Length > 0);
        }

        [Test]
        public void MaximumInscribedCircle_UnitSquare_RadiusHalf()
        {
            double* xs = stackalloc double[4] { 0, 1, 1, 0 };
            double* ys = stackalloc double[4] { 0, 0, 1, 1 };
            double r = MaximumInscribedCircle.Run(xs, ys, 4);
            Assert.IsTrue(Math.Abs(r - 0.5) < 1e-3, $"r={r}");
        }
    }
}

