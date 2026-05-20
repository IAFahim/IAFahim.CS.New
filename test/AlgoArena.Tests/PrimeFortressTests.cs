namespace AlgoArena.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Math.NT;
    using IAFahim.Math.Modular;

    public sealed unsafe class PrimeFortressTests
    {
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(5, true)]
        [TestCase(7, true)]
        [TestCase(11, true)]
        [TestCase(17, true)]
        [TestCase(19, true)]
        [TestCase(23, true)]
        [TestCase(4, false)]
        [TestCase(6, false)]
        [TestCase(9, false)]
        [TestCase(15, false)]
        [TestCase(21, false)]
        [TestCase(100, false)]
        public void MillerRabin_PrimeDetection(long n, bool expected)
        {
            Assert.AreEqual(expected, MillerRabin.Run(n));
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 2)]
        [TestCase(5, 4)]
        [TestCase(6, 2)]
        [TestCase(7, 6)]
        [TestCase(8, 4)]
        [TestCase(9, 6)]
        [TestCase(10, 4)]
        [TestCase(12, 4)]
        [TestCase(15, 8)]
        [TestCase(20, 8)]
        [TestCase(30, 8)]
        [TestCase(97, 96)]
        public void Phi_EulerTotient(long n, long expected)
        {
            Assert.AreEqual(expected, Phi.Run(n));
        }

        [TestCase(1, 1)]
        [TestCase(2, -1)]
        [TestCase(3, -1)]
        [TestCase(4, 0)]
        [TestCase(5, -1)]
        [TestCase(6, 1)]
        [TestCase(7, -1)]
        [TestCase(8, 0)]
        [TestCase(9, 0)]
        [TestCase(10, 1)]
        [TestCase(12, 0)]
        [TestCase(15, 1)]
        [TestCase(18, 0)]
        [TestCase(30, -1)]
        [TestCase(42, -1)]
        public void Mobius_Classification(long n, int expected)
        {
            Assert.AreEqual(expected, Mobius.Run(n));
        }

        [TestCase(12, new long[] { 2, 2, 3 })]
        [TestCase(60, new long[] { 2, 2, 3, 5 })]
        [TestCase(100, new long[] { 2, 2, 5, 5 })]
        [TestCase(7, new long[] { 7 })]
        [TestCase(16, new long[] { 2, 2, 2, 2 })]
        [TestCase(360, new long[] { 2, 2, 2, 3, 3, 5 })]
        public void Factorize_PrimeFactors(long n, long[] expected)
        {
            long* factors = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            try
            {
                int count = Factorize.Run(n, factors);
                Assert.AreEqual(expected.Length, count);
                for (int i = 0; i < count; i++)
                    Assert.AreEqual(expected[i], factors[i]);
            }
            finally { Marshal.FreeHGlobal((nint)factors); }
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(4, 3)]
        [TestCase(6, 4)]
        [TestCase(12, 6)]
        [TestCase(28, 6)]
        [TestCase(30, 8)]
        [TestCase(60, 12)]
        [TestCase(100, 9)]
        public void Divisors_Count(long n, int expected)
        {
            long* divisors = (long*)Marshal.AllocHGlobal(10000 * sizeof(long));
            try
            {
                int count = Divisors.Run(n, divisors);
                Assert.AreEqual(expected, count);
                var sortedDivs = new long[count];
                for (int i = 0; i < count; i++) sortedDivs[i] = divisors[i];
                Array.Sort(sortedDivs);
                for (int i = 1; i < count; i++)
                    Assert.IsTrue(sortedDivs[i] >= sortedDivs[i - 1]);
            }
            finally { Marshal.FreeHGlobal((nint)divisors); }
        }

        [TestCase(12, 42, 6)]
        [TestCase(18, 24, 6)]
        [TestCase(7, 14, 7)]
        [TestCase(25, 50, 25)]
        [TestCase(100, 35, 5)]
        [TestCase(17, 42, 1)]
        public void Gcd_EuclideanAlgorithm(long a, long b, long expected)
        {
            Assert.AreEqual(expected, Gcd.Run(a, b));
        }

        [Test]
        public void Factorize_LargeSemiprime_ProductMatches()
        {
            long n = 997 * 991;
            long* factors = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            try
            {
                int count = Factorize.Run(n, factors);
                long prod = 1;
                for (int i = 0; i < count; i++) prod *= factors[i];
                Assert.AreEqual(n, prod);
                Assert.AreEqual(2, count);
            }
            finally { Marshal.FreeHGlobal((nint)factors); }
        }

        [Test]
        public void Factorize_VeryLargeSemiprime_ProductMatches()
        {
            long n = 999999937L * 999999929L;
            long* factors = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            try
            {
                int count = Factorize.Run(n, factors);
                long prod = 1;
                for (int i = 0; i < count; i++) prod *= factors[i];
                Assert.AreEqual(n, prod);
                Assert.AreEqual(2, count);
            }
            finally { Marshal.FreeHGlobal((nint)factors); }
        }

        [Test]
        public void PrimitiveRoot_SmallPrimes()
        {
            Assert.AreEqual(1, PrimitiveRoot.Run(2));
            Assert.AreEqual(2, PrimitiveRoot.Run(3));
            Assert.AreEqual(2, PrimitiveRoot.Run(5));
            Assert.AreEqual(3, PrimitiveRoot.Run(7));
        }

        [TestCase(6, 12)]
        [TestCase(28, 56)]
        [TestCase(496, 992)]
        [TestCase(12, 28)]
        public void DivisorSum_ReturnsSigma(long n, long expectedSigma)
        {
            long sum = DivisorSum.Run(n);
            Assert.AreEqual(expectedSigma, sum);
        }

        [TestCase(1)]
        [TestCase(4)]
        [TestCase(27)]
        [TestCase(45)]
        public void DivisorSum_DeficientNumber(long n)
        {
            long sum = DivisorSum.Run(n);
            Assert.IsTrue(sum < 2 * n);
        }
    }
}