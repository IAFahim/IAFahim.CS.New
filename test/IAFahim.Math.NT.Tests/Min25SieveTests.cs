namespace IAFahim.Math.NT.Tests
{
    using NUnit.Framework;

    public sealed unsafe class Min25SieveTests
    {
        // Brute-force prime count via simple sieve.
        private static long BrutePrimePi(long n)
        {
            if (n < 2) return 0;
            int nn = (int)n;
            bool* s = stackalloc bool[nn + 1];
            for (int i = 2; i <= nn; i++) s[i] = true;
            for (int i = 2; (long)i * i <= n; i++)
                if (s[i])
                    for (long j = (long)i * i; j <= n; j += i) s[(int)j] = false;
            long c = 0;
            for (int i = 2; i <= nn; i++) if (s[i]) c++;
            return c;
        }

        [Test]
        public void PrimePi_Small_MatchesBruteForce()
        {
            int maxV = 1000;
            int* primes = stackalloc int[maxV + 1];
            bool* isPrime = stackalloc bool[maxV + 1];
            long* w = stackalloc long[2 * maxV + 16];
            long* g = stackalloc long[2 * maxV + 16];
            int* map1 = stackalloc int[maxV + 1];
            int* map2 = stackalloc int[maxV + 1];
            for (int i = 0; i <= maxV; i++) { map1[i] = -1; map2[i] = -1; }

            // pi(10)=4, pi(100)=25, pi(1000)=168.
            long[] targets = { 10, 100, 1000 };
            long[] expected = { 4, 25, 168 };
            for (int t = 0; t < targets.Length; t++)
            {
                for (int i = 0; i <= maxV; i++) { map1[i] = -1; map2[i] = -1; }
                long got = Min25Sieve.PrimePi(targets[t], primes, isPrime, w, g, map1, map2);
                Assert.AreEqual(expected[t], got, $"PrimePi({targets[t]})");
                Assert.AreEqual(BrutePrimePi(targets[t]), got, $"PrimePi({targets[t]}) vs brute");
            }
        }
    }
}
