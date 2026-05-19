namespace AlgoArena.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.Math.NT;
    using IAFahim.Math.Modular;

    public sealed unsafe class PrimeFortressTests
    {
        [Theory]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(5, true)]
        [InlineData(7, true)]
        [InlineData(11, true)]
        [InlineData(17, true)]
        [InlineData(19, true)]
        [InlineData(23, true)]
        [InlineData(4, false)]
        [InlineData(6, false)]
        [InlineData(9, false)]
        [InlineData(15, false)]
        [InlineData(21, false)]
        [InlineData(100, false)]
        public void MillerRabin_PrimeDetection(long n, bool expected)
        {
            Assert.Equal(expected, MillerRabin.Run(n));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        [InlineData(4, 2)]
        [InlineData(5, 4)]
        [InlineData(6, 2)]
        [InlineData(7, 6)]
        [InlineData(8, 4)]
        [InlineData(9, 6)]
        [InlineData(10, 4)]
        [InlineData(12, 4)]
        [InlineData(15, 8)]
        [InlineData(20, 8)]
        [InlineData(30, 8)]
        [InlineData(97, 96)]
        public void Phi_EulerTotient(long n, long expected)
        {
            Assert.Equal(expected, Phi.Run(n));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, -1)]
        [InlineData(3, -1)]
        [InlineData(4, 0)]
        [InlineData(5, -1)]
        [InlineData(6, 1)]
        [InlineData(7, -1)]
        [InlineData(8, 0)]
        [InlineData(9, 0)]
        [InlineData(10, 1)]
        [InlineData(12, 0)]
        [InlineData(15, 1)]
        [InlineData(18, 0)]
        [InlineData(30, -1)]
        [InlineData(42, -1)]
        public void Mobius_Classification(long n, int expected)
        {
            Assert.Equal(expected, Mobius.Run(n));
        }

        [Theory]
        [InlineData(12, new long[] { 2, 2, 3 })]
        [InlineData(60, new long[] { 2, 2, 3, 5 })]
        [InlineData(100, new long[] { 2, 2, 5, 5 })]
        [InlineData(7, new long[] { 7 })]
        [InlineData(16, new long[] { 2, 2, 2, 2 })]
        [InlineData(360, new long[] { 2, 2, 2, 3, 3, 5 })]
        public void Factorize_PrimeFactors(long n, long[] expected)
        {
            long* factors = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            try
            {
                int count = Factorize.Run(n, factors);
                Assert.Equal(expected.Length, count);
                for (int i = 0; i < count; i++)
                    Assert.Equal(expected[i], factors[i]);
            }
            finally { Marshal.FreeHGlobal((nint)factors); }
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(4, 3)]
        [InlineData(6, 4)]
        [InlineData(12, 6)]
        [InlineData(28, 6)]
        [InlineData(30, 8)]
        [InlineData(60, 12)]
        [InlineData(100, 9)]
        public void Divisors_Count(long n, int expected)
        {
            long* divisors = (long*)Marshal.AllocHGlobal(10000 * sizeof(long));
            try
            {
                int count = Divisors.Run(n, divisors);
                Assert.Equal(expected, count);
                var sortedDivs = new long[count];
                for (int i = 0; i < count; i++) sortedDivs[i] = divisors[i];
                Array.Sort(sortedDivs);
                for (int i = 1; i < count; i++)
                    Assert.True(sortedDivs[i] >= sortedDivs[i - 1]);
            }
            finally { Marshal.FreeHGlobal((nint)divisors); }
        }

        [Theory]
        [InlineData(12, 42, 6)]
        [InlineData(18, 24, 6)]
        [InlineData(7, 14, 7)]
        [InlineData(25, 50, 25)]
        [InlineData(100, 35, 5)]
        [InlineData(17, 42, 1)]
        public void Gcd_EuclideanAlgorithm(long a, long b, long expected)
        {
            Assert.Equal(expected, Gcd.Run(a, b));
        }

        [Fact]
        public void Factorize_LargeSemiprime_ProductMatches()
        {
            long n = 997 * 991;
            long* factors = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            try
            {
                int count = Factorize.Run(n, factors);
                long prod = 1;
                for (int i = 0; i < count; i++) prod *= factors[i];
                Assert.Equal(n, prod);
                Assert.Equal(2, count);
            }
            finally { Marshal.FreeHGlobal((nint)factors); }
        }

        [Fact]
        public void Factorize_VeryLargeSemiprime_ProductMatches()
        {
            long n = 999999937L * 999999929L;
            long* factors = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            try
            {
                int count = Factorize.Run(n, factors);
                long prod = 1;
                for (int i = 0; i < count; i++) prod *= factors[i];
                Assert.Equal(n, prod);
                Assert.Equal(2, count);
            }
            finally { Marshal.FreeHGlobal((nint)factors); }
        }

        [Fact]
        public void PrimitiveRoot_SmallPrimes()
        {
            Assert.Equal(1, PrimitiveRoot.Run(2));
            Assert.Equal(2, PrimitiveRoot.Run(3));
            Assert.Equal(2, PrimitiveRoot.Run(5));
            Assert.Equal(3, PrimitiveRoot.Run(7));
        }

        [Theory]
        [InlineData(6, 12)]
        [InlineData(28, 56)]
        [InlineData(496, 992)]
        [InlineData(12, 28)]
        public void DivisorSum_ReturnsSigma(long n, long expectedSigma)
        {
            long sum = DivisorSum.Run(n);
            Assert.Equal(expectedSigma, sum);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(27)]
        [InlineData(45)]
        public void DivisorSum_DeficientNumber(long n)
        {
            long sum = DivisorSum.Run(n);
            Assert.True(sum < 2 * n);
        }
    }
}