namespace IAFahim.Math.Combinatorics.Tests
{
    using IAFahim.Math.Combinatorics;
    using NUnit.Framework;

    public sealed unsafe class CombinatoricsTests
    {
        [Test]
        public void Factorial_Basic()
        {
            const long mod = 1000000007;
            Assert.AreEqual(1, Factorial.Run(0, mod));
            Assert.AreEqual(1, Factorial.Run(1, mod));
            Assert.AreEqual(2, Factorial.Run(2, mod));
            Assert.AreEqual(6, Factorial.Run(3, mod));
            Assert.AreEqual(24, Factorial.Run(4, mod));
            Assert.AreEqual(120, Factorial.Run(5, mod));
        }

        [Test]
        public void Binom_Basic()
        {
            const long mod = 1000000007;
            Assert.AreEqual(1, Binom.Run(0, 0, mod));
            Assert.AreEqual(1, Binom.Run(5, 0, mod));
            Assert.AreEqual(1, Binom.Run(5, 5, mod));
            Assert.AreEqual(5, Binom.Run(5, 1, mod));
            Assert.AreEqual(10, Binom.Run(5, 2, mod));
            Assert.AreEqual(10, Binom.Run(5, 3, mod));
            Assert.AreEqual(5, Binom.Run(5, 4, mod));
        }

        [Test]
        public void Catalan_Basic()
        {
            const long mod = 1000000007;
            Assert.AreEqual(1, Catalan.Run(0, mod));
            Assert.AreEqual(1, Catalan.Run(1, mod));
            Assert.AreEqual(2, Catalan.Run(2, mod));
            Assert.AreEqual(5, Catalan.Run(3, mod));
            Assert.AreEqual(14, Catalan.Run(4, mod));
        }

        [Test]
        public void Derangements_Basic()
        {
            const long mod = 1000000007;
            Assert.AreEqual(1, Derangements.Run(0, mod));
            Assert.AreEqual(0, Derangements.Run(1, mod));
            Assert.AreEqual(1, Derangements.Run(2, mod));
            Assert.AreEqual(2, Derangements.Run(3, mod));
            Assert.AreEqual(9, Derangements.Run(4, mod));
        }

        [Test]
        public void StirlingSecond_Basic()
        {
            const long mod = 1000000007;
            long s10 = StirlingSecond.Run(5, 2, mod);
            Assert.AreEqual(15, s10);
        }

        [Test]
        public void BellNumbers_Basic()
        {
            const long mod = 1000000007;
            Assert.AreEqual(1, BellNumbers.Run(0, mod));
            Assert.AreEqual(1, BellNumbers.Run(1, mod));
            Assert.AreEqual(2, BellNumbers.Run(2, mod));
            Assert.AreEqual(5, BellNumbers.Run(3, mod));
        }

        [Test]
        public void SievePrimes_Basic()
        {
            const int n = 50;
            bool* isPrime = stackalloc bool[n + 1];
            int* primes = stackalloc int[n + 1];
            int count = SievePrimes.Run(primes, isPrime, n);
            Assert.IsFalse(isPrime[0]);
            Assert.IsFalse(isPrime[1]);
            Assert.IsTrue(isPrime[2]);
            Assert.IsTrue(isPrime[3]);
            Assert.IsFalse(isPrime[4]);
            Assert.IsTrue(isPrime[47]);
            Assert.IsTrue(count >= 15);
        }

        [Test]
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
            Assert.IsTrue(count >= 10);
        }

        [Test]
        public void MultisetPermutations_AABB_Is6()
        {
            // AABB: 4!/(2!*2!) = 6.
            const long mod = 1000000007;
            int* counts = stackalloc int[2] { 2, 2 };
            Assert.AreEqual(6, MultisetPermutations.Run(4, counts, 2, mod));
        }

        [Test]
        public void MultisetPermutations_AAAB_Is4()
        {
            // AAAB: 4!/(3!*1!) = 4.
            const long mod = 1000000007;
            int* counts = stackalloc int[2] { 3, 1 };
            Assert.AreEqual(4, MultisetPermutations.Run(4, counts, 2, mod));
        }

        [Test]
        public void PartitionNumbers_KnownValues()
        {
            // p(0)=1, p(1)=1, p(2)=2, p(3)=3, p(4)=5, p(5)=7.
            const long mod = 1000000007;
            Assert.AreEqual(1, PartitionNumbers.Run(0, mod));
            Assert.AreEqual(1, PartitionNumbers.Run(1, mod));
            Assert.AreEqual(2, PartitionNumbers.Run(2, mod));
            Assert.AreEqual(3, PartitionNumbers.Run(3, mod));
            Assert.AreEqual(5, PartitionNumbers.Run(4, mod));
            Assert.AreEqual(7, PartitionNumbers.Run(5, mod));
        }

        [Test]
        public void StarsBars_KnownValues()
        {
            // C(n+k-1, k): stars(3,2)=C(4,2)=6.
            const long mod = 1000000007;
            Assert.AreEqual(6, StarsBars.Run(3, 2, mod));
            Assert.AreEqual(10, StarsBars.Run(4, 2, mod));
            Assert.AreEqual(15, StarsBars.Run(5, 2, mod));
        }

        [Test]
        public void StirlingFirst_Unsigned_n4()
        {
            // Unsigned |s(n,k)|: |s(4,1)|=6, |s(4,2)|=11, |s(4,3)|=6, |s(4,4)|=1.
            const long mod = 1000000007;
            Assert.AreEqual(6, StirlingFirst.Run(4, 1, mod));
            Assert.AreEqual(11, StirlingFirst.Run(4, 2, mod));
            Assert.AreEqual(6, StirlingFirst.Run(4, 3, mod));
            Assert.AreEqual(1, StirlingFirst.Run(4, 4, mod));
        }

        [Test]
        public void PermuteCount_IsFactorial()
        {
            const long mod = 1000000007;
            Assert.AreEqual(1, PermuteCount.Run(0, mod));
            Assert.AreEqual(1, PermuteCount.Run(1, mod));
            Assert.AreEqual(2, PermuteCount.Run(2, mod));
            Assert.AreEqual(120, PermuteCount.Run(5, mod));
        }
    }
}