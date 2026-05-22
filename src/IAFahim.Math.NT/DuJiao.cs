namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DuJiao
    {
        public static long Phi(long n)
        {
            if (n <= 0)
            {
                return 0;
            }
            if (n <= 10000)
            {
                long* preSum = stackalloc long[(int)n + 1];
                SievePhi((int)n, preSum);
                return preSum[n];
            }

            long b = (long)Math.Pow((double)n, 2.0 / 3.0);
            if (b < 10000)
            {
                b = 10000;
            }
            if (b > n)
            {
                b = n;
            }

            long* preSumLarge = stackalloc long[(int)b + 1];
            long memoSize = n / b + 2;
            long* memo = stackalloc long[(int)memoSize];
            bool* memoized = stackalloc bool[(int)memoSize];

            for (int i = 0; i < memoSize; i++)
            {
                memoized[i] = false;
            }
            return GetPhi(n, n, b, preSumLarge, memo, memoized);
        }

        public static long Mobius(long n)
        {
            if (n <= 0)
            {
                return 0;
            }
            if (n <= 10000)
            {
                long* preSum = stackalloc long[(int)n + 1];
                SieveMobius((int)n, preSum);
                return preSum[n];
            }

            long b = (long)Math.Pow((double)n, 2.0 / 3.0);
            if (b < 10000)
            {
                b = 10000;
            }
            if (b > n)
            {
                b = n;
            }

            long* preSumLarge = stackalloc long[(int)b + 1];
            long memoSize = n / b + 2;
            long* memo = stackalloc long[(int)memoSize];
            bool* memoized = stackalloc bool[(int)memoSize];

            for (int i = 0; i < memoSize; i++)
            {
                memoized[i] = false;
            }
            return GetMobius(n, n, b, preSumLarge, memo, memoized);
        }

        private static void SievePhi(int limit, long* preSum)
        {
            int* phi = stackalloc int[limit + 1];
            int* primes = stackalloc int[limit + 1];
            bool* isPrime = stackalloc bool[limit + 1];

            for (int i = 0; i <= limit; i++)
            {
                phi[i] = i;
            }
            for (int i = 2; i <= limit; i++)
            {
                isPrime[i] = true;
            }
            int pCount = 0;
            phi[1] = 1;
            for (int i = 2; i <= limit; i++)
            {
                if (isPrime[i])
                {
                    primes[pCount++] = i;
                    phi[i] = i - 1;
                }
                for (int j = 0; j < pCount && i * primes[j] <= limit; j++)
                {
                    int p = primes[j];
                    isPrime[i * p] = false;
                    if (i % p == 0)
                    {
                        phi[i * p] = phi[i] * p;
                        break;
                    }
                    else
                    {
                        phi[i * p] = phi[i] * (p - 1);
                    }
                }
            }
            preSum[0] = 0;
            for (int i = 1; i <= limit; i++)
            {
                preSum[i] = preSum[i - 1] + (long)phi[i];
            }
        }

        private static void SieveMobius(int limit, long* preSum)
        {
            int* mu = stackalloc int[limit + 1];
            int* primes = stackalloc int[limit + 1];
            bool* isPrime = stackalloc bool[limit + 1];

            for (int i = 0; i <= limit; i++)
            {
                mu[i] = 1;
            }
            for (int i = 2; i <= limit; i++)
            {
                isPrime[i] = true;
            }
            int pCount = 0;
            mu[1] = 1;
            for (int i = 2; i <= limit; i++)
            {
                if (isPrime[i])
                {
                    primes[pCount++] = i;
                    mu[i] = -1;
                }
                for (int j = 0; j < pCount && i * primes[j] <= limit; j++)
                {
                    int p = primes[j];
                    isPrime[i * p] = false;
                    if (i % p == 0)
                    {
                        mu[i * p] = 0;
                        break;
                    }
                    else
                    {
                        mu[i * p] = -mu[i];
                    }
                }
            }
            preSum[0] = 0;
            for (int i = 1; i <= limit; i++)
            {
                preSum[i] = preSum[i - 1] + (long)mu[i];
            }
        }

        private static long GetPhi(
            long x,
            long n,
            long b,
            long* preSum,
            long* memo,
            bool* memoized)
        {
            if (x <= b)
            {
                return preSum[x];
            }
            long idx = n / x;
            if (memoized[idx])
            {
                return memo[idx];
            }

            long ans = x % 2 == 0 ? (x / 2) * (x + 1) : x * ((x + 1) / 2);
            for (long l = 2, r; l <= x; l = r + 1)
            {
                long val = x / l;
                r = x / val;
                ans -= (r - l + 1) * GetPhi(val, n, b, preSum, memo, memoized);
            }

            memo[idx] = ans;
            memoized[idx] = true;
            return ans;
        }

        private static long GetMobius(
            long x,
            long n,
            long b,
            long* preSum,
            long* memo,
            bool* memoized)
        {
            if (x <= b)
            {
                return preSum[x];
            }
            long idx = n / x;
            if (memoized[idx])
            {
                return memo[idx];
            }

            long ans = 1;
            for (long l = 2, r; l <= x; l = r + 1)
            {
                long val = x / l;
                r = x / val;
                ans -= (r - l + 1) * GetMobius(val, n, b, preSum, memo, memoized);
            }

            memo[idx] = ans;
            memoized[idx] = true;
            return ans;
        }
    }
}
