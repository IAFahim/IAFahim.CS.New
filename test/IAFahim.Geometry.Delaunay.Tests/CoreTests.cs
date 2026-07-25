namespace IAFahim.Geometry.Delaunay.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class BowyerWatsonTests
    {
        [Test]
        public void FourPoints_Square_ExactlyTwoTrianglesCoveringHull()
        {
            double* xs = stackalloc double[4] { 0, 1, 1, 0 };
            double* ys = stackalloc double[4] { 0, 0, 1, 1 };
            BowyerWatson.Triangle* t = stackalloc BowyerWatson.Triangle[16];
            int k = BowyerWatson.Triangulate(xs, ys, 4, t, 16);
            Assert.AreEqual(2, k);
            // All triangle verts in 0..3 (no super-triangle leakage)
            for (int i = 0; i < k; i++)
            {
                Assert.IsTrue((uint)t[i].A < 4u);
                Assert.IsTrue((uint)t[i].B < 4u);
                Assert.IsTrue((uint)t[i].C < 4u);
            }
            // Union of faces uses each of 4 points at least once
            bool[] used = new bool[4];
            for (int i = 0; i < k; i++)
            {
                used[t[i].A] = true;
                used[t[i].B] = true;
                used[t[i].C] = true;
            }
            for (int i = 0; i < 4; i++) Assert.IsTrue(used[i], $"vertex {i} unused");
            // Positive area sum equals square area 1
            double area = 0;
            for (int i = 0; i < k; i++)
                area += Math.Abs(TriArea(xs, ys, t[i].A, t[i].B, t[i].C));
            Assert.IsTrue(Math.Abs(area - 1.0) < 1e-9, $"area={area}");
        }

        private static double TriArea(double* xs, double* ys, int a, int b, int c)
        {
            return 0.5 * ((xs[b] - xs[a]) * (ys[c] - ys[a]) - (ys[b] - ys[a]) * (xs[c] - xs[a]));
        }
    }
}
