namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PrimePiLehmer
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

            int a = (int)Run((long)Math.Pow(n, 1.0 / 4.0), primes, primeCount, phiCache);
            int b = (int)Run((long)Math.Pow(n, 1.0 / 2.0), primes, primeCount, phiCache);
            int c = (int)Run((long)Math.Pow(n, 1.0 / 3.0), primes, primeCount, phiCache);

            long ans = Phi(n, a, primes, phiCache) + (long)(b + a - 2) * (b - a + 1) / 2;

            for (int i = a + 1; i <= b; i++)
            {
                long w = n / primes[i - 1];
                ans -= Run(w, primes, primeCount, phiCache);
                if (i <= c)
                {
                    int bi = (int)Run((long)Math.Pow(w, 0.5), primes, primeCount, phiCache);
                    for (int j = i; j <= bi; j++)
                    {
                        ans -= Run(w / primes[j - 1], primes, primeCount, phiCache) - (j - 1);
                    }
                }
            }

            return ans;
        }

        public static long Phi(long m, int a, int* primes, int* phiCache)
        {
            if (a == 0) return m;
            if (m <= primes[a - 1]) return 1;
            if (a <= PhiCachePrimes && m < PhiCacheSize)
            {
                return phiCache[m * (PhiCachePrimes + 1) + a];
            }
            return Phi(m, a - 1, primes, phiCache) - Phi(m / primes[a - 1], a - 1, primes, phiCache);
        }

        public static void InitPhiCache(int* phiCache, int* primes, int primeCount)
        {
            for (int i = 0; i < PhiCacheSize; i++)
            {
                phiCache[i * (PhiCachePrimes + 1)] = i;
            }

            for (int j = 1; j <= PhiCachePrimes; j++)
            {
                int p = primes[j - 1];
                for (int i = 0; i < PhiCacheSize; i++)
                {
                    phiCache[i * (PhiCachePrimes + 1) + j] = phiCache[i * (PhiCachePrimes + 1) + j - 1] -
                                                             phiCache[(i / p) * (PhiCachePrimes + 1) + j - 1];
                }
            }
        }
    }
}
