namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class LinearSieveDivisorCount
    {
        public static void Run(int* d, int* e, int* primes, int n, out int primeCount)
        {
            d[1] = 1;
            e[1] = 0;
            for (int i = 2; i <= n; i++) { d[i] = 0; e[i] = 0; }
            primeCount = 0;
            for (int i = 2; i <= n; i++)
            {
                if (d[i] == 0)
                {
                    d[i] = 2;
                    e[i] = 1;
                    primes[primeCount++] = i;
                }
                for (int j = 0; j < primeCount; j++)
                {
                    long prod = (long)i * primes[j];
                    if (prod > n) break;
                    int ip = (int)prod;
                    if (i % primes[j] == 0)
                    {
                        e[ip] = e[i] + 1;
                        d[ip] = d[i] / (e[i] + 1) * (e[i] + 2);
                        break;
                    }
                    e[ip] = 1;
                    d[ip] = d[i] * 2;
                }
            }
        }
    }
}
