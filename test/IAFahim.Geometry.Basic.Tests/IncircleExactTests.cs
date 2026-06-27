namespace IAFahim.Geometry.Basic.Tests
{
    using IAFahim.Geometry.Basic;
    using System;
    using System.Numerics;
    using NUnit.Framework;

    public sealed class IncircleExactTests
    {
        [Test]
        public void MatchesBigInteger_LargeCoords()
        {
            Random rng = new Random(31337);
            long bound = (long)1 << 31;
            int mismatches = 0;
            for (int t = 0; t < 3000; t++)
            {
                long ax = Rnd(rng, bound), ay = Rnd(rng, bound);
                long bx = Rnd(rng, bound), by = Rnd(rng, bound);
                long cx = Rnd(rng, bound), cy = Rnd(rng, bound);
                long dx = Rnd(rng, bound), dy = Rnd(rng, bound);
                int expected = Sign(BiIncircle(ax, ay, bx, by, cx, cy, dx, dy));
                int actual = IncircleExact.Run(ax, ay, bx, by, cx, cy, dx, dy);
                if (expected != actual)
                {
                    mismatches++;
                    if (mismatches <= 5)
                        Assert.Fail($"t={t}: expected {expected} got {actual} for ({ax},{ay})({bx},{by})({cx},{cy})({dx},{dy})");
                }
            }
            Assert.AreEqual(0, mismatches, "IncircleExact must match BigInteger for all 3000 large-coord cases");
        }

        [Test]
        public void Cocircular_ReturnsZero()
        {
            long k = 3_000_000_000L;
            int s = IncircleExact.Run(-k, 0, 0, -k, k, 0, 0, k);
            Assert.AreEqual(0, s, "four points on a circle through (±k,0),(0,±k)");
        }

        [Test]
        public void InsideVsOutside_CorrectSign()
        {
            int inside = IncircleExact.Run(0, 0, 100, 0, 0, 100, 1, 1);
            int outside = IncircleExact.Run(0, 0, 100, 0, 0, 100, 1000, 1000);
            Assert.AreNotEqual(0, inside, "point clearly inside circumcircle");
            Assert.AreNotEqual(0, outside, "point clearly outside");
            Assert.AreNotEqual(inside, outside, "inside/outside must have opposite sign");
        }

        private static BigInteger BiIncircle(long ax, long ay, long bx, long by, long cx, long cy, long dx, long dy)
        {
            BigInteger adx = ax - dx, ady = ay - dy;
            BigInteger bdx = bx - dx, bdy = by - dy;
            BigInteger cdx = cx - dx, cdy = cy - dy;
            BigInteger aLift = adx * adx + ady * ady;
            BigInteger bLift = bdx * bdx + bdy * bdy;
            BigInteger cLift = cdx * cdx + cdy * cdy;
            BigInteger crossA = bdx * cdy - bdy * cdx;
            BigInteger crossB = cdx * ady - cdy * adx;
            BigInteger crossC = adx * bdy - ady * bdx;
            return aLift * crossA + bLift * crossB + cLift * crossC;
        }

        private static int Sign(BigInteger x) => x > 0 ? 1 : x < 0 ? -1 : 0;

        private static long Rnd(Random rng, long bound)
        {
            long u = (long)rng.Next(int.MinValue, int.MaxValue);
            long v = rng.Next(int.MinValue, int.MaxValue);
            long r = (u * v) % bound;
            return r;
        }
    }
}
