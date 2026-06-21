namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class MoebiusPrefix
    {
        public static void Run(int n, int* result, int* mu, int* primes, bool* isPrime)
        {
            if (n < 0)
            {
                return;
            }
            result[0] = 0;
            if (n == 0)
            {
                return;
            }

            for (int i = 0; i <= n; i++)
            {
                mu[i] = 1;
            }
            for (int i = 2; i <= n; i++)
            {
                isPrime[i] = true;
            }
            int pCount = 0;
            mu[1] = 1;
            for (int i = 2; i <= n; i++)
            {
                if (isPrime[i])
                {
                    primes[pCount++] = i;
                    mu[i] = -1;
                }
                for (int j = 0; j < pCount && (long)i * primes[j] <= n; j++)
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
            result[0] = 0;
            for (int i = 1; i <= n; i++)
            {
                result[i] = result[i - 1] + mu[i];
            }
        }
    }
}
