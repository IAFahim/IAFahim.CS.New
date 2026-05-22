namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SquareFree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Kernel(long n)
        {
            return Radical.Run(n);
        }

        public static long Count(long n, int* mu)
        {
            if (n <= 0)
            {
                return 0;
            }
            int limit = (int)Math.Sqrt((double)n);
            SieveMobius(limit, mu);
            long ans = 0;
            for (int d = 1; d <= limit; d++)
            {
                if (mu[d] != 0)
                {
                    ans += (long)mu[d] * (n / ((long)d * d));
                }
            }
            return ans;
        }

        public static void Prefix(int n, int* result)
        {
            if (n < 0)
            {
                return;
            }
            result[0] = 0;
            for (int i = 1; i <= n; i++)
            {
                result[i] = 1;
            }
            for (int i = 2; i * i <= n; i++)
            {
                int i2 = i * i;
                for (int j = i2; j <= n; j += i2)
                {
                    result[j] = 0;
                }
            }
            for (int i = 1; i <= n; i++)
            {
                result[i] += result[i - 1];
            }
        }

        private static void SieveMobius(int limit, int* mu)
        {
            for (int i = 0; i <= limit; i++)
            {
                mu[i] = 1;
            }
            int* primes = stackalloc int[limit + 1];
            bool* isPrime = stackalloc bool[limit + 1];
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
        }
    }
}