namespace IAFahim.Optimization.Geometric.Tests
{
    using NUnit.Framework;

    public sealed unsafe class MinEnclosingBallTests
    {
        [Test]
        public void Empty_ZeroRadius()
        {
            int* p = stackalloc int[1];
            var c = MinEnclosingBall.Welzl(null, null, 0, p);
            Assert.AreEqual(0.0, c.R, 1e-12);
        }

        [Test]
        public void TwoPoints_CoversBoth()
        {
            double* xs = stackalloc double[2];
            double* ys = stackalloc double[2];
            int* p = stackalloc int[2];
            xs[0] = 0; ys[0] = 0;
            xs[1] = 2; ys[1] = 0;
            var c = MinEnclosingBall.Welzl(xs, ys, 2, p);
            Assert.AreEqual(1.0, c.R, 1e-9);
            Assert.AreEqual(1.0, c.X, 1e-9);
            Assert.AreEqual(0.0, c.Y, 1e-9);
        }

        [Test]
        public void ThreePoints_Equilateral()
        {
            double* xs = stackalloc double[3];
            double* ys = stackalloc double[3];
            int* p = stackalloc int[3];
            xs[0] = 0; ys[0] = 0;
            xs[1] = 1; ys[1] = 0;
            xs[2] = 0.5; ys[2] = 0.86602540378;
            var c = MinEnclosingBall.Welzl(xs, ys, 3, p);
            Assert.IsTrue(c.R > 0.5 && c.R < 0.6);
            for (int i = 0; i < 3; i++)
            {
                double dx = xs[i] - c.X, dy = ys[i] - c.Y;
                Assert.IsTrue(dx * dx + dy * dy <= c.R * c.R + 1e-6);
            }
        }
    }
}
