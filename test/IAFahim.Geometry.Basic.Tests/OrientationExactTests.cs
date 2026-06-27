namespace IAFahim.Geometry.Basic.Tests
{
    using IAFahim.Geometry.Basic;
    using System;
    using System.Numerics;
    using NUnit.Framework;

    public sealed class OrientationExactTests
    {
        [Test]
        public void MatchesBigInteger_ForLargeAndSmallCoords()
        {
            Random rng = new Random(424242);
            long bound = (long)1 << 40;
            for (int t = 0; t < 2000; t++)
            {
                long ax = Rnd(rng, bound), ay = Rnd(rng, bound);
                long bx = Rnd(rng, bound), by = Rnd(rng, bound);
                long cx = Rnd(rng, bound), cy = Rnd(rng, bound);
                int expected = Sign(BigInteger.Multiply(bx - ax, cy - ay) - BigInteger.Multiply(by - ay, cx - ax));
                int actual = OrientationExact.Run(ax, ay, bx, by, cx, cy);
                Assert.AreEqual(expected, actual, $"t={t} ({ax},{ay})({bx},{by})({cx},{cy})");
            }
        }

        [Test]
        public void FastPath_SmallCoords_MatchesNaive()
        {
            Random rng = new Random(7);
            for (int t = 0; t < 500; t++)
            {
                long ax = rng.Next(-1000, 1000), ay = rng.Next(-1000, 1000);
                long bx = rng.Next(-1000, 1000), by = rng.Next(-1000, 1000);
                long cx = rng.Next(-1000, 1000), cy = rng.Next(-1000, 1000);
                long cross = (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
                int expected = (cross > 0 ? 1 : 0) - (cross < 0 ? 1 : 0);
                Assert.AreEqual(expected, OrientationExact.Run(ax, ay, bx, by, cx, cy));
            }
        }

        [Test]
        public void Collinear_ReturnsZero_NearOverflowFrontier()
        {
            long k = 3_000_000_000L;
            int s = OrientationExact.Run(0, 0, k, k, 2 * k, 2 * k);
            Assert.AreEqual(0, s, "exactly collinear large points");
            int left = OrientationExact.Run(0, 0, k, k, 2 * k, 2 * k + 1);
            int right = OrientationExact.Run(0, 0, k, k, 2 * k + 1, 2 * k);
            Assert.AreEqual(1, left, "left of diagonal");
            Assert.AreEqual(-1, right, "right of diagonal");
        }

        private static long Rnd(Random rng, long bound)
        {
            long u = (long)rng.Next(int.MinValue, int.MaxValue);
            long v = rng.Next(int.MinValue, int.MaxValue);
            return (u * v) % bound;
        }

        private static int Sign(BigInteger x)
        {
            if (x > 0) return 1;
            if (x < 0) return -1;
            return 0;
        }
    }
}
