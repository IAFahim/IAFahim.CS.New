namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Min25Sieve
    {
        public static long PrimePi(long n, int* primes, bool* isPrime, long* w, long* g, int* map1, int* map2)
        {
            if (n <= 1)
            {
                return 0;
            }
            long v = (long)Math.Sqrt((double)n);
            int maxV = (int)v;

            for (int i = 2; i <= maxV; i++)
            {
                isPrime[i] = true;
            }
            int pCount = 0;
            for (int i = 2; i <= maxV; i++)
            {
                if (isPrime[i])
                {
                    primes[pCount++] = i;
                    for (int j = i * 2; j <= maxV; j += i)
                    {
                        isPrime[j] = false;
                    }
                }
            }

            int tot = 0;
            for (long i = 1, j; i <= n; i = j + 1)
            {
                long val = n / i;
                j = n / val;
                w[tot] = val;
                g[tot] = val - 1;
                tot++;
            }

            for (int i = 0; i <= maxV; i++)
            {
                map1[i] = 0;
                map2[i] = 0;
            }

            for (int i = 0; i < tot; i++)
            {
                if (w[i] <= v)
                {
                    map1[w[i]] = i;
                }
                else
                {
                    map2[n / w[i]] = i;
                }
            }

            for (int i = 0; i < pCount; i++)
            {
                long p = primes[i];
                long p2 = p * p;
                for (int j = 0; j < tot; j++)
                {
                    if (p2 > w[j])
                    {
                        break;
                    }
                    long val = w[j] / p;
                    int idx = val <= v ? map1[val] : map2[n / val];
                    g[j] -= g[idx] - (long)i;
                }
            }

            return g[0];
        }

        public static long PrimeSum(long n, long mod, int* primes, bool* isPrime, long* w, long* g, int* map1, int* map2)
        {
            if (n <= 1)
            {
                return 0;
            }
            long v = (long)Math.Sqrt((double)n);
            int maxV = (int)v;

            for (int i = 2; i <= maxV; i++)
            {
                isPrime[i] = true;
            }
            int pCount = 0;
            for (int i = 2; i <= maxV; i++)
            {
                if (isPrime[i])
                {
                    primes[pCount++] = i;
                    for (int j = i * 2; j <= maxV; j += i)
                    {
                        isPrime[j] = false;
                    }
                }
            }

            int tot = 0;
            for (long i = 1, j; i <= n; i = j + 1)
            {
                long val = n / i;
                j = n / val;
                w[tot] = val;
                long term = val % 2 == 0 ? (val / 2) % mod * ((val + 1) % mod) % mod : val % mod * (((val + 1) / 2) % mod) % mod;
                g[tot] = (term - 1 + mod) % mod;
                tot++;
            }

            for (int i = 0; i <= maxV; i++)
            {
                map1[i] = 0;
                map2[i] = 0;
            }

            for (int i = 0; i < tot; i++)
            {
                if (w[i] <= v)
                {
                    map1[w[i]] = i;
                }
                else
                {
                    map2[n / w[i]] = i;
                }
            }

            for (int i = 0; i < pCount; i++)
            {
                long p = primes[i];
                long p2 = p * p;
                long sp = 0;
                for (int k = 0; k < i; k++)
                {
                    sp = (sp + primes[k]) % mod;
                }

                for (int j = 0; j < tot; j++)
                {
                    if (p2 > w[j])
                    {
                        break;
                    }
                    long val = w[j] / p;
                    int idx = val <= v ? map1[val] : map2[n / val];
                    long term = (g[idx] - sp + mod) % mod;
                    g[j] = (g[j] - p % mod * term % mod + mod) % mod;
                }
            }

            return g[0];
        }

        public static long MultiplicativeSum(
            long n,
            long c0,
            long c1,
            delegate* managed<long, int, long> fPower,
            long mod,
            int* primes,
            bool* isPrime,
            long* w,
            long* g,
            long* g0,
            long* g1,
            int* map1,
            int* map2,
            long* gPrimeSum)
        {
            if (n <= 0)
            {
                return 0;
            }
            if (n == 1)
            {
                return 1 % mod;
            }

            long v = (long)Math.Sqrt((double)n);
            int maxV = (int)v;

            for (int i = 2; i <= maxV; i++)
            {
                isPrime[i] = true;
            }
            int pCount = 0;
            for (int i = 2; i <= maxV; i++)
            {
                if (isPrime[i])
                {
                    primes[pCount++] = i;
                    for (int j = i * 2; j <= maxV; j += i)
                    {
                        isPrime[j] = false;
                    }
                }
            }

            int tot = 0;
            for (long i = 1, j; i <= n; i = j + 1)
            {
                long val = n / i;
                j = n / val;
                w[tot] = val;
                g0[tot] = (val - 1) % mod;
                long term = val % 2 == 0 ? (val / 2) % mod * ((val + 1) % mod) % mod : val % mod * (((val + 1) / 2) % mod) % mod;
                g1[tot] = (term - 1 + mod) % mod;
                tot++;
            }

            for (int i = 0; i <= maxV; i++)
            {
                map1[i] = 0;
                map2[i] = 0;
            }

            for (int i = 0; i < tot; i++)
            {
                if (w[i] <= v)
                {
                    map1[w[i]] = i;
                }
                else
                {
                    map2[n / w[i]] = i;
                }
            }

            for (int i = 0; i < pCount; i++)
            {
                long p = primes[i];
                long p2 = p * p;
                long sp0 = (long)i;
                long sp1 = 0;
                for (int k = 0; k < i; k++)
                {
                    sp1 = (sp1 + primes[k]) % mod;
                }

                for (int j = 0; j < tot; j++)
                {
                    if (p2 > w[j])
                    {
                        break;
                    }
                    long val = w[j] / p;
                    int idx = val <= v ? map1[val] : map2[n / val];
                    long term0 = (g0[idx] - sp0 + mod) % mod;
                    g0[j] = (g0[j] - term0 + mod) % mod;

                    long term1 = (g1[idx] - sp1 + mod) % mod;
                    g1[j] = (g1[j] - p % mod * term1 % mod + mod) % mod;
                }
            }

            for (int i = 0; i < tot; i++)
            {
                g[i] = (c0 % mod * g0[i] % mod + c1 % mod * g1[i] % mod) % mod;
            }

            gPrimeSum[0] = 0;
            for (int i = 1; i <= pCount; i++)
            {
                gPrimeSum[i] = (gPrimeSum[i - 1] + fPower(primes[i - 1], 1)) % mod;
            }

            long ans = SolveS(n, 1, n, v, pCount, primes, g, gPrimeSum, map1, map2, fPower, mod);
            return (ans + 1) % mod;
        }

        private static long SolveS(
            long x,
            int i,
            long n,
            long v,
            int pCount,
            int* primes,
            long* g,
            long* gPrimeSum,
            int* map1,
            int* map2,
            delegate* managed<long, int, long> fPower,
            long mod)
        {
            if (i - 1 >= pCount)
            {
                int idx = x <= v ? map1[x] : map2[n / x];
                return (g[idx] - gPrimeSum[pCount] + mod) % mod;
            }
            if (primes[i - 1] > x)
            {
                return 0;
            }
            int idxVar = x <= v ? map1[x] : map2[n / x];
            long ans = (g[idxVar] - gPrimeSum[i - 1] + mod) % mod;
            for (int j = i - 1; j < pCount && (long)primes[j] * primes[j] <= x; j++)
            {
                long p = primes[j];
                long pe = p;
                for (int e = 1; pe * p <= x; e++)
                {
                    long term1 = fPower(p, e);
                    long term2 = SolveS(x / pe, j + 2, n, v, pCount, primes, g, gPrimeSum, map1, map2, fPower, mod);
                    long term3 = fPower(p, e + 1);
                    ans = (ans + term1 % mod * term2 % mod + term3 % mod) % mod;
                    pe *= p;
                }
            }
            return ans;
        }
    }
}