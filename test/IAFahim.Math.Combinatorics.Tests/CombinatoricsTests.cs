namespace IAFahim.Math.Combinatorics.Tests
{
    using IAFahim.Math.Combinatorics;
    using Xunit;

    public sealed unsafe class CombinatoricsTests
    {
        [Fact]
        public void Factorial_Basic()
        {
            const long mod = 1000000007;
            Assert.Equal(1, Factorial.Run(0, mod));
            Assert.Equal(1, Factorial.Run(1, mod));
            Assert.Equal(2, Factorial.Run(2, mod));
            Assert.Equal(6, Factorial.Run(3, mod));
            Assert.Equal(24, Factorial.Run(4, mod));
            Assert.Equal(120, Factorial.Run(5, mod));
        }

        [Fact]
        public void Binom_Basic()
        {
            const long mod = 1000000007;
            Assert.Equal(1, Binom.Run(0, 0, mod));
            Assert.Equal(1, Binom.Run(5, 0, mod));
            Assert.Equal(1, Binom.Run(5, 5, mod));
            Assert.Equal(5, Binom.Run(5, 1, mod));
            Assert.Equal(10, Binom.Run(5, 2, mod));
            Assert.Equal(10, Binom.Run(5, 3, mod));
            Assert.Equal(5, Binom.Run(5, 4, mod));
        }

        [Fact]
        public void Catalan_Basic()
        {
            const long mod = 1000000007;
            Assert.Equal(1, Catalan.Run(0, mod));
            Assert.Equal(1, Catalan.Run(1, mod));
            Assert.Equal(2, Catalan.Run(2, mod));
            Assert.Equal(5, Catalan.Run(3, mod));
            Assert.Equal(14, Catalan.Run(4, mod));
        }

        [Fact]
        public void Derangements_Basic()
        {
            const long mod = 1000000007;
            Assert.Equal(1, Derangements.Run(0, mod));
            Assert.Equal(0, Derangements.Run(1, mod));
            Assert.Equal(1, Derangements.Run(2, mod));
            Assert.Equal(2, Derangements.Run(3, mod));
            Assert.Equal(9, Derangements.Run(4, mod));
        }

        [Fact]
        public void StirlingSecond_Basic()
        {
            const long mod = 1000000007;
            long s10 = StirlingSecond.Run(5, 2, mod);
            Assert.Equal(15, s10);
        }

        [Fact]
        public void BellNumbers_Basic()
        {
            const long mod = 1000000007;
            Assert.Equal(1, BellNumbers.Run(0, mod));
            Assert.Equal(1, BellNumbers.Run(1, mod));
            Assert.Equal(2, BellNumbers.Run(2, mod));
            Assert.Equal(5, BellNumbers.Run(3, mod));
        }

        [Fact]
        public void SievePrimes_Basic()
        {
            const int n = 50;
            bool* isPrime = stackalloc bool[n + 1];
            int* primes = stackalloc int[n + 1];
            int count = SievePrimes.Run(primes, isPrime, n);
            Assert.False(isPrime[0]);
            Assert.False(isPrime[1]);
            Assert.True(isPrime[2]);
            Assert.True(isPrime[3]);
            Assert.False(isPrime[4]);
            Assert.True(isPrime[47]);
            Assert.True(count >= 15);
        }

        [Fact]
        public void LinearSieve_Basic()
        {
            const int n = 30;
            int* primes = stackalloc int[n + 1];
            int* lp = stackalloc int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                lp[i] = 0;
            }
            int count = LinearSieve.Run(primes, lp, n);
            Assert.True(count >= 10);
        }
    }
}