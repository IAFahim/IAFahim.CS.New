namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MobiusSieve
    {
        public static void Run(int* mu, int* primes, int n, out int primeCount)
        {
            int* isComposite = stackalloc int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                mu[i] = 0;
                isComposite[i] = 0;
            }
            mu[1] = 1;
            primeCount = 0;
            for (int i = 2; i <= n; i++)
            {
                if (isComposite[i] == 0)
                {
                    primes[primeCount++] = i;
                    mu[i] = -1;
                }
                for (int j = 0; j < primeCount; j++)
                {
                    long product = (long)i * primes[j];
                    if (product > n) break;
                    isComposite[product] = 1;
                    if (i % primes[j] == 0)
                    {
                        mu[product] = 0;
                        break;
                    }
                    mu[product] = -mu[i];
                }
            }
        }
    }
}
