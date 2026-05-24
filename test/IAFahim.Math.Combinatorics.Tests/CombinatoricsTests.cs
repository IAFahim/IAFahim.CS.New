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
    }
}