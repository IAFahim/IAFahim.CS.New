namespace IAFahim.Geometry.Advanced.Tests
{
    using IAFahim.Geometry.Advanced;
    using NUnit.Framework;

    public sealed unsafe class MinimumEnclosingCircleTests
    {
        [Test]
        public void TwoPoints_CenterAndSquaredRadius()
        {
            long* x = stackalloc long[] { 0, 10 };
            long* y = stackalloc long[] { 0, 0 };
            long cx = 0, cy = 0, r = 0;
            MinimumEnclosingCircle.Run(2, x, y, &cx, &cy, &r);
            Assert.AreEqual(5, cx);
            Assert.AreEqual(0, cy);
            Assert.AreEqual(25, r);
        }

        [Test]
        public void ThreePoints_AllEnclosed()
        {
            long* x = stackalloc long[] { 0, 10, 5 };
            long* y = stackalloc long[] { 0, 0, 6 };
            long cx = 0, cy = 0, r = 0;
            MinimumEnclosingCircle.Run(3, x, y, &cx, &cy, &r);
            for (int i = 0; i < 3; i++)
            {
                long dx = x[i] - cx;
                long dy = y[i] - cy;
                long d2 = dx * dx + dy * dy;
                Assert.IsTrue(d2 <= r, $"point {i} d2={d2} > r={r}");
            }
            Assert.IsTrue(r >= 25);
        }

        [Test]
        public void Collinear_FarthestEndpoints()
        {
            long* x = stackalloc long[] { 0, 5, 10 };
            long* y = stackalloc long[] { 0, 0, 0 };
            long cx = 0, cy = 0, r = 0;
            MinimumEnclosingCircle.Run(3, x, y, &cx, &cy, &r);
            Assert.AreEqual(5, cx);
            Assert.AreEqual(25, r);
        }
    }
}
