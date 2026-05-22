namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PrimePiMeissel
    {
        private const int PhiCacheSize = 20000;
        private const int PhiCachePrimes = 100;

        public static long Run(long n, int* primes, int primeCount, int* phiCache)
        {
            if (n < 2) return 0;
            if (n < 1000000)
            {
                int lo = 0, hi = primeCount;
                while (lo < hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (primes[mid] <= n) lo = mid + 1;
                    else hi = mid;
                }
                return lo;
            }

            int a = (int)Run((long)Math.Pow(n, 1.0 / 3.0), primes, primeCount, phiCache);
            int b = (int)Run((long)Math.Pow(n, 1.0 / 2.0), primes, primeCount, phiCache);

            long ans = Phi(n, a, primes, phiCache) + a - 1;

            for (int i = a + 1; i <= b; i++)
            {
                ans -= Run(n / primes[i - 1], primes, primeCount, phiCache) - (i - 1);
            }

            return ans;
        }

        private static long Phi(long m, int a, int* primes, int* phiCache)
        {
            if (a == 0) return m;
            if (m <= primes[a - 1]) return 1;
            if (a <= PhiCachePrimes && m < PhiCacheSize)
            {
                return phiCache[m * (PhiCachePrimes + 1) + a];
            }
            return Phi(m, a - 1, primes, phiCache) - Phi(m / primes[a - 1], a - 1, primes, phiCache);
        }
    }
}
