namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class LinearSieveDivisorSum
    {
        public static void Run(long* sigma, long* sp, int* primes, int n, out int primeCount)
        {
            sigma[1] = 1;
            sp[1] = 1;
            for (int i = 2; i <= n; i++) { sigma[i] = 0; sp[i] = 0; }
            primeCount = 0;
            for (int i = 2; i <= n; i++)
            {
                if (sigma[i] == 0)
                {
                    sigma[i] = 1L + i;
                    sp[i] = 1L + i;
                    primes[primeCount++] = i;
                }
                for (int j = 0; j < primeCount; j++)
                {
                    long prod = (long)i * primes[j];
                    if (prod > n) break;
                    int ip = (int)prod;
                    if (i % primes[j] == 0)
                    {
                        sp[ip] = sp[i] * primes[j] + 1;
                        sigma[ip] = sigma[i] / sp[i] * sp[ip];
                        break;
                    }
                    sp[ip] = 1L + primes[j];
                    sigma[ip] = sigma[i] * (1L + primes[j]);
                }
            }
        }
    }
}
