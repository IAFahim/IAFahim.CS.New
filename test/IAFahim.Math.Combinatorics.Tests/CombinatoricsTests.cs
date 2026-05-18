namespace IAFahim.Math.Combinatorics.Tests
{
    using IAFahim.Math.Combinatorics;
    using Xunit;

    public sealed unsafe class CombinatoricsTests
    {
        [Fact]
        public void Factorial_Basic()
        {
            Assert.Equal(1, Factorial.Run(0));
            Assert.Equal(1, Factorial.Run(1));
            Assert.Equal(2, Factorial.Run(2));
            Assert.Equal(6, Factorial.Run(3));
            Assert.Equal(24, Factorial.Run(4));
            Assert.Equal(120, Factorial.Run(5));
        }

        [Fact]
        public void Binom_Basic()
        {
            Assert.Equal(1, Binom.Run(0, 0));
            Assert.Equal(1, Binom.Run(5, 0));
            Assert.Equal(1, Binom.Run(5, 5));
            Assert.Equal(5, Binom.Run(5, 1));
            Assert.Equal(10, Binom.Run(5, 2));
            Assert.Equal(10, Binom.Run(5, 3));
            Assert.Equal(5, Binom.Run(5, 4));
        }

        [Fact]
        public void Catalan_Basic()
        {
            Assert.Equal(1, Catalan.Run(0));
            Assert.Equal(1, Catalan.Run(1));
            Assert.Equal(2, Catalan.Run(2));
            Assert.Equal(5, Catalan.Run(3));
            Assert.Equal(14, Catalan.Run(4));
        }

        [Fact]
        public void Derangements_Basic()
        {
            Assert.Equal(1, Derangements.Run(0));
            Assert.Equal(0, Derangements.Run(1));
            Assert.Equal(1, Derangements.Run(2));
            Assert.Equal(2, Derangements.Run(3));
            Assert.Equal(9, Derangements.Run(4));
        }

        [Fact]
        public void StirlingSecond_Basic()
        {
            long s10 = StirlingSecond.Run(5, 2);
            Assert.Equal(15, s10);
        }

        [Fact]
        public void BellNumbers_Basic()
        {
            Assert.Equal(1, BellNumbers.Run(0));
            Assert.Equal(1, BellNumbers.Run(1));
            Assert.Equal(2, BellNumbers.Run(2));
            Assert.Equal(5, BellNumbers.Run(3));
        }

        [Fact]
        public void SievePrimes_Basic()
        {
            const int n = 50;
            bool* isPrime = stackalloc bool[n];
            SievePrimes.Run(n, isPrime);
            Assert.False(isPrime[0]);
            Assert.False(isPrime[1]);
            Assert.True(isPrime[2]);
            Assert.True(isPrime[3]);
            Assert.False(isPrime[4]);
            Assert.True(isPrime[47]);
        }

        [Fact]
        public void LinearSieve_Basic()
        {
            const int n = 30;
            int* primes = stackalloc int[n];
            int* cnt = stackalloc int[1];
            int* lp = stackalloc int[n];
            LinearSieve.Run(n, primes, cnt, lp);
            Assert.True(*cnt >= 10);
        }
    }
}