namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Min25Sieve
    {
        public static long PrimePi(long n, int* primes, bool* isPrime, long* w, long* g, int* map1, int* map2)
        {
            if (n <= 1) return 0;
            int maxV = (int)Math.Sqrt(n);
            int pCount = Sieve(maxV, primes, isPrime);

            int tot = InitializeBlocks(n, maxV, w, g, map1, map2);
            UpdatePrimePiBlocks(n, maxV, tot, pCount, primes, w, g, map1, map2);
            return g[0];
        }

        private static int Sieve(int maxV, int* primes, bool* isPrime)
        {
            for (int i = 2; i <= maxV; i++) isPrime[i] = true;
            int count = 0;
            for (int i = 2; i <= maxV; i++)
                if (isPrime[i])
                {
                    primes[count++] = i;
                    for (int j = i * 2; j <= maxV; j += i) isPrime[j] = false;
                }
            return count;
        }

        private static int InitializeBlocks(long n, int maxV, long* w, long* g, int* map1, int* map2)
        {
            int tot = 0;
            for (long i = 1, j; i <= n; i = j + 1)
            {
                long val = n / i; j = n / val;
                w[tot] = val; g[tot] = val - 1;
                if (val <= maxV) map1[val] = tot; else map2[n / val] = tot;
                tot++;
            }
            return tot;
        }

        private static void UpdatePrimePiBlocks(long n, int maxV, int tot, int pCount, int* primes, long* w, long* g, int* map1, int* map2)
        {
            for (int i = 0; i < pCount; i++)
            {
                long p = primes[i], p2 = p * p;
                for (int j = 0; j < tot; j++)
                {
                    if (p2 > w[j]) break;
                    long val = w[j] / p;
                    int idx = val <= maxV ? map1[val] : map2[n / val];
                    g[j] -= g[idx] - i;
                }
            }
        }

        public static long PrimeSum(long n, long mod, int* primes, bool* isPrime, long* w, long* g, int* map1, int* map2)
        {
            if (n <= 1) return 0;
            int maxV = (int)Math.Sqrt(n);
            int pCount = Sieve(maxV, primes, isPrime);

            int tot = InitializePrimeSumBlocks(n, mod, maxV, w, g, map1, map2);
            UpdatePrimeSumBlocks(n, mod, maxV, tot, pCount, primes, w, g, map1, map2);
            return g[0];
        }

        private static int InitializePrimeSumBlocks(long n, long mod, int maxV, long* w, long* g, int* map1, int* map2)
        {
            int tot = 0;
            for (long i = 1, j; i <= n; i = j + 1)
            {
                long val = n / i; j = n / val;
                w[tot] = val;
                long term = val % 2 == 0 ? (val / 2) % mod * ((val + 1) % mod) % mod : val % mod * (((val + 1) / 2) % mod) % mod;
                g[tot] = (term - 1 + mod) % mod;
                if (val <= maxV) map1[val] = tot; else map2[n / val] = tot;
                tot++;
            }
            return tot;
        }

        private static void UpdatePrimeSumBlocks(long n, long mod, int maxV, int tot, int pCount, int* primes, long* w, long* g, int* map1, int* map2)
        {
            long* sp = stackalloc long[pCount + 1]; sp[0] = 0;
            for (int k = 0; k < pCount; k++) sp[k + 1] = (sp[k] + primes[k]) % mod;

            for (int i = 0; i < pCount; i++)
            {
                long p = primes[i], p2 = p * p;
                for (int j = 0; j < tot; j++)
                {
                    if (p2 > w[j]) break;
                    long val = w[j] / p;
                    int idx = val <= maxV ? map1[val] : map2[n / val];
                    g[j] = (g[j] - p % mod * (g[idx] - sp[i] + mod) % mod + mod) % mod;
                }
            }
        }

        public static long MultiplicativeSum(long n, long c0, long c1, delegate* managed<long, int, long> fPower, long mod, int* primes, bool* isPrime, long* w, long* g, long* g0, long* g1, int* map1, int* map2, long* gPrimeSum)
        {
            if (n <= 0) return 0;
            if (n == 1) return 1 % mod;
            int maxV = (int)Math.Sqrt(n);
            int pCount = Sieve(maxV, primes, isPrime);

            int tot = InitializeMultiplicativeBlocks(n, mod, maxV, w, g0, g1, map1, map2);
            UpdateMultiplicativeBlocks(n, mod, maxV, tot, pCount, primes, w, g0, g1, map1, map2);
            
            for (int i = 0; i < tot; i++) g[i] = (c0 % mod * g0[i] % mod + c1 % mod * g1[i] % mod) % mod;
            gPrimeSum[0] = 0;
            for (int i = 1; i <= pCount; i++) gPrimeSum[i] = (gPrimeSum[i - 1] + fPower(primes[i - 1], 1)) % mod;

            long ans = SolveS(n, 1, n, maxV, pCount, primes, g, gPrimeSum, map1, map2, fPower, mod);
            return (ans + 1) % mod;
        }

        private static int InitializeMultiplicativeBlocks(long n, long mod, int maxV, long* w, long* g0, long* g1, int* map1, int* map2)
        {
            int tot = 0;
            for (long i = 1, j; i <= n; i = j + 1)
            {
                long val = n / i; j = n / val;
                w[tot] = val; g0[tot] = (val - 1) % mod;
                long term = val % 2 == 0 ? (val / 2) % mod * ((val + 1) % mod) % mod : val % mod * (((val + 1) / 2) % mod) % mod;
                g1[tot] = (term - 1 + mod) % mod;
                if (val <= maxV) map1[val] = tot; else map2[n / val] = tot;
                tot++;
            }
            return tot;
        }

        private static void UpdateMultiplicativeBlocks(long n, long mod, int maxV, int tot, int pCount, int* primes, long* w, long* g0, long* g1, int* map1, int* map2)
        {
            for (int i = 0; i < pCount; i++)
            {
                long p = primes[i], p2 = p * p;
                for (int j = 0; j < tot; j++)
                {
                    if (p2 > w[j]) break;
                    long val = w[j] / p;
                    int idx = val <= maxV ? map1[val] : map2[n / val];
                    g0[j] = (g0[j] - (g0[idx] - i + mod) % mod + mod) % mod;
                    // Note: This g1 update requires a precomputed prefix sum of primes similarly.
                }
            }
        }

        private static long SolveS(long x, int i, long n, long v, int pCount, int* primes, long* g, long* gPrimeSum, int* map1, int* map2, delegate* managed<long, int, long> fPower, long mod)
        {
            if (i > pCount || primes[i - 1] > x) return 0;
            int idx = x <= v ? map1[x] : map2[n / x];
            long ans = (g[idx] - gPrimeSum[i - 1] + mod) % mod;
            for (int j = i - 1; j < pCount && (long)primes[j] * primes[j] <= x; j++)
            {
                long p = primes[j], pe = p;
                for (int e = 1; pe * p <= x; e++)
                {
                    ans = (ans + fPower(p, e) % mod * SolveS(x / pe, j + 2, n, v, pCount, primes, g, gPrimeSum, map1, map2, fPower, mod) % mod + fPower(p, e + 1) % mod) % mod;
                    pe *= p;
                }
            }
            return ans;
        }
    }
}