namespace IAFahim.Math.NT.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class MultiplicativeGenTests
    {
        // Powerful numbers: n where every prime factor appears with exponent >= 2.
        // First few (OEIS A001694): 1, 4, 8, 9, 16, 25, 27, 32, 36, 49, 64, 72, ...
        [Test]
        public void PowerfulNumbers_Below100_MatchesOEIS()
        {
            long* res = stackalloc long[256];
            int count = PowerfulNumbers.Generate(100, res);
            // Expected powerful numbers <= 100.
            long[] expected = { 1, 4, 8, 9, 16, 25, 27, 32, 36, 49, 64, 72, 81, 100 };
            Assert.AreEqual(expected.Length, count, $"count={count}");
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], res[i], $"idx={i}");
        }

        [Test]
        public void PowerfulNumbers_LargeLimit_NoBoundaryMiss()
        {
            // Stress the sqrt-rounding fix. Powerful numbers up to N ~ 2.17*sqrt(N);
            // for 1e6 that's ~2200, raw pairs maybe ~5000.
            long* res = stackalloc long[10000];
            int count = PowerfulNumbers.Generate(1000000, res);
            Assert.IsTrue(count > 200, $"count={count}");
            for (int i = 1; i < count; i++)
                Assert.Less(res[i - 1], res[i], $"not strictly increasing at {i}");
        }

        // 5-smooth (Hamming) numbers below 50: 1,2,3,4,5,6,8,9,10,12,15,16,18,20,24,25,27,30,32,36,40,45,48,50? no <= limit
        [Test]
        public void SmoothNumbers_5Smooth_Below50()
        {
            int* primes = stackalloc int[3] { 2, 3, 5 };
            long* res = stackalloc long[256];
            int count = SmoothNumbers.Generate(3, 50, res, primes);
            // Expected 5-smooth numbers <= 50 (Hamming numbers).
            long[] expected = { 1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 15, 16, 18, 20, 24, 25, 27, 30, 32, 36, 40, 45, 48, 50 };
            // Verify all generated are 5-smooth and <= 50.
            for (int i = 0; i < count; i++)
            {
                Assert.LessOrEqual(res[i], 50);
                long v = res[i];
                while (v % 2 == 0) v /= 2;
                while (v % 3 == 0) v /= 3;
                while (v % 5 == 0) v /= 5;
                Assert.AreEqual(1, v, $"res[{i}]={res[i]} not 5-smooth");
            }
            Assert.AreEqual(expected.Length, count, $"count={count}");
            for (int i = 0; i < count; i++)
                Assert.AreEqual(expected[i], res[i], $"idx={i}");
        }
    }
}
