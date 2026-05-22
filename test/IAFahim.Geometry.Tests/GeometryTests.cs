namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.Geometry.Hull;
    using IAFahim.Geometry.Intersect;
    using IAFahim.Geometry.Voronoi;
    using IAFahim.Geometry.Spatial;
    using Xunit;

    public sealed unsafe class GeometryTests
    {
        [Fact]
        public void RotatingCalipers_MinAreaRect()
        {
            double* xs = stackalloc double[4];
            double* ys = stackalloc double[4];
            xs[0] = 0; ys[0] = 0;
            xs[1] = 4; ys[1] = 0;
            xs[2] = 4; ys[2] = 3;
            xs[3] = 0; ys[3] = 3;
            RotatingCalipers.Rect r = RotatingCalipers.MinArea(xs, ys, 4);
            Assert.True(r.W >= 0);
            Assert.True(r.H >= 0);
        }

        [Fact]
        public void RotatingCalipers_MinWidth()
        {
            double* xs = stackalloc double[4];
            double* ys = stackalloc double[4];
            xs[0] = 0; ys[0] = 0;
            xs[1] = 4; ys[1] = 0;
            xs[2] = 4; ys[2] = 3;
            xs[3] = 0; ys[3] = 3;
            double w = RotatingCalipers.MinWidth(xs, ys, 4);
            Assert.True(w > 0);
        }

        [Fact]
        public void MaximumInscribedCircle_Rectangle()
        {
            double* xs = stackalloc double[4];
            double* ys = stackalloc double[4];
            xs[0] = 0; ys[0] = 0;
            xs[1] = 4; ys[1] = 0;
            xs[2] = 4; ys[2] = 3;
            xs[3] = 0; ys[3] = 3;
            double r = MaximumInscribedCircle.Run(xs, ys, 4);
            Assert.True(r >= 0);
        }

        [Fact]
        public void ConvexHullTrick_AddQuery()
        {
            ConvexHullTrick.Line* hull = stackalloc ConvexHullTrick.Line[10];
            int size = 0;
            ConvexHullTrick.Line l0; l0.M = 2; l0.B = 0;
            ConvexHullTrick.Line l1; l1.M = 1; l1.B = 3;
            ConvexHullTrick.Line l2; l2.M = 0; l2.B = 6;
            ConvexHullTrick.Add(hull, &size, l0);
            ConvexHullTrick.Add(hull, &size, l1);
            ConvexHullTrick.Add(hull, &size, l2);
            Assert.True(size >= 2);
            long v = ConvexHullTrick.Query(hull, size, 0);
            Assert.True(v >= 0);
        }

        [Fact]
        public void MinkowskiSum_Convex()
        {
            double* ax = stackalloc double[3] { 0, 1, 0 };
            double* ay = stackalloc double[3] { 0, 0, 1 };
            double* bx = stackalloc double[3] { 0, 1, 0 };
            double* by = stackalloc double[3] { 0, 0, 1 };
            double* ox = stackalloc double[6];
            double* oy = stackalloc double[6];
            int n = MinkowskiSum.Convex(ax, ay, 3, bx, by, 3, ox, oy);
            Assert.Equal(6, n);
        }

        [Fact]
        public void Plane_PointPlaneDistance()
        {
            double d = Plane.PointPlaneDistance(1, 1, 1, 0, 0, 1, -5);
            Assert.Equal(-4, d, 5);
        }

        [Fact]
        public void Plane_LinePlaneIntersects()
        {
            double t;
            bool hit = Plane.LinePlaneIntersection(0, 0, 0, 0, 0, 1, 0, 0, 1, -5, &t);
            Assert.True(hit);
            Assert.Equal(5, t, 5);
        }

        [Fact]
        public void Plane_SegmentPlaneIntersects()
        {
            double t;
            bool hit = Plane.SegmentPlaneIntersection(0, 0, 0, 0, 0, 10, 0, 0, 1, -5, &t);
            Assert.True(hit);
            Assert.Equal(0.5, t, 5);
        }

        [Fact]
        public void Plane_SegmentPlaneMisses()
        {
            double t;
            bool hit = Plane.SegmentPlaneIntersection(0, 0, 0, 0, 0, 3, 0, 0, 1, -5, &t);
            Assert.False(hit);
        }

        [Fact]
        public void Sphere_LineIntersection()
        {
            double t1, t2;
            int n = Sphere.LineIntersection(0, 0, 0, 1, 2, 0, 0, 1, 0, 0, &t1, &t2);
            Assert.Equal(2, n);
        }

        [Fact]
        public void Sphere_SphereIntersection()
        {
            double cx, cy, cz, circleRadius, nx, ny, nz;
            bool hit = Sphere.SphereIntersection(0, 0, 0, 2, 3, 0, 0, 2, &cx, &cy, &cz, &circleRadius, &nx, &ny, &nz);
            Assert.True(hit);
            Assert.Equal(1.5, cx, 5);
        }

        [Fact]
        public void NearestNeighbor_FromPoints()
        {
            double* xs = stackalloc double[4] { 0, 1, 3, 5 };
            double* ys = stackalloc double[4] { 0, 0, 0, 0 };
            int idx = NearestNeighbor.FromPoints(xs, ys, 4, 3.1, 0);
            Assert.Equal(2, idx);
        }

        [Fact]
        public void NearestNeighbor_Range()
        {
            double* xs = stackalloc double[4] { 0, 1, 3, 5 };
            double* ys = stackalloc double[4] { 0, 0, 0, 0 };
            int* outIdx = stackalloc int[4];
            int c = NearestNeighbor.Range(2.5, 0, 1.5, xs, ys, 4, outIdx);
            Assert.Equal(2, c);
        }

        [Fact]
        public void Mst_Euclidean()
        {
            double* xs = stackalloc double[4] { 0, 1, 0, 1 };
            double* ys = stackalloc double[4] { 0, 0, 1, 1 };
            int* from = stackalloc int[6];
            int* to = stackalloc int[6];
            double* w = stackalloc double[6];
            double total = Mst.Euclidean(xs, ys, 4, from, to, w);
            Assert.Equal(3.0, total, 5);
        }

        [Fact]
        public void Mst_Manhattan()
        {
            double* xs = stackalloc double[4] { 0, 1, 0, 1 };
            double* ys = stackalloc double[4] { 0, 0, 1, 1 };
            int* from = stackalloc int[6];
            int* to = stackalloc int[6];
            double* w = stackalloc double[6];
            double total = Mst.Manhattan(xs, ys, 4, from, to, w);
            Assert.Equal(3.0, total, 5);
        }

        [Fact]
        public void Delaunay_Build()
        {
            double* xs = stackalloc double[4] { 0, 1, 0.5, 0 };
            double* ys = stackalloc double[4] { 0, 0, 0.5, 1 };
            Delaunay.Triangle* triangles = stackalloc Delaunay.Triangle[10];
            int count = Delaunay.Build(xs, ys, 4, triangles);
            Assert.True(count >= 1);
        }

        [Fact]
        public void KdTree_Build()
        {
            double* xs = stackalloc double[5] { 0, 1, 2, 3, 4 };
            double* ys = stackalloc double[5] { 0, 1, 2, 3, 4 };
            KdTree.Node* nodes = stackalloc KdTree.Node[5];
            int root = KdTree.Build(xs, ys, 5, nodes);
            Assert.Equal(5, root);
        }

        [Fact]
        public void BallTree_Nearest()
        {
            double* xs = stackalloc double[4] { 0, 1, 3, 5 };
            double* ys = stackalloc double[4] { 0, 0, 0, 0 };
            BallTree.Node* nodes = stackalloc BallTree.Node[4];
            BallTree.Build(xs, ys, 4, nodes);
            int best = BallTree.Nearest(nodes, 0, 3.1, 0);
            Assert.True(best >= 0);
        }

        [Fact]
        public void CoverTree_Nearest()
        {
            double* xs = stackalloc double[4] { 0, 1, 3, 5 };
            double* ys = stackalloc double[4] { 0, 0, 0, 0 };
            CoverTree.Node* nodes = stackalloc CoverTree.Node[4];
            CoverTree.Build(xs, ys, 4, nodes);
            int best = CoverTree.Nearest(nodes, 4, 3.1, 0);
            Assert.Equal(2, best);
        }

        [Fact]
        public void Wspd_Build()
        {
            double* xs = stackalloc double[4] { 0, 1, 2, 3 };
            double* ys = stackalloc double[4] { 0, 0, 0, 0 };
            Wspd.Pair* pairs = stackalloc Wspd.Pair[6];
            int c = Wspd.Build(xs, ys, 4, pairs, 2.0);
            Assert.True(c >= 2);
        }

        [Fact]
        public void ConvexHull3D_Tetrahedron()
        {
            double* xs = stackalloc double[4] { 0, 1, 0, 0 };
            double* ys = stackalloc double[4] { 0, 0, 1, 0 };
            double* zs = stackalloc double[4] { 0, 0, 0, 1 };
            ConvexHull3D.Face* faces = stackalloc ConvexHull3D.Face[4];
            int f = ConvexHull3D.Build(xs, ys, zs, 4, faces);
            Assert.Equal(4, f);
        }

        [Fact]
        public void StraightSkeleton_Build()
        {
            double* xs = stackalloc double[4] { 0, 4, 4, 0 };
            double* ys = stackalloc double[4] { 0, 0, 3, 3 };
            double* ox = stackalloc double[4];
            double* oy = stackalloc double[4];
            int n = StraightSkeleton.Build(xs, ys, 4, ox, oy);
            Assert.Equal(4, n);
        }

        [Fact]
        public void Bit3D_InitAddSum()
        {
            Bit3D.BIT3D bit;
            long* tree = stackalloc long[27];
            bit.Tree = tree;
            Bit3D.Init(&bit, 3, 3, 3);
            for (int i = 0; i < 27; i++) tree[i] = 0;
            Bit3D.Add(&bit, 1, 1, 1, 5);
            long s = Bit3D.Sum(&bit, 1, 1, 1);
            Assert.True(s >= 0);
        }

        [Fact]
        public void VisibilityGraph_Square()
        {
            double* ox = stackalloc double[4] { 0, 4, 4, 0 };
            double* oy = stackalloc double[4] { 0, 0, 3, 3 };
            int* from = stackalloc int[6];
            int* to = stackalloc int[6];
            double* w = stackalloc double[6];
            int e = VisibilityGraph.Build(ox, oy, 4, from, to, w);
            Assert.True(e >= 2);
        }
    }
}
