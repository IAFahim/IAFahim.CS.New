namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class LinearSievePhi
    {
        public static void Run(int* phi, int* primes, int n, out int primeCount)
        {
            phi[1] = 1;
            for (int i = 2; i <= n; i++) phi[i] = 0;
            primeCount = 0;
            for (int i = 2; i <= n; i++)
            {
                if (phi[i] == 0)
                {
                    phi[i] = i - 1;
                    primes[primeCount++] = i;
                }
                for (int j = 0; j < primeCount; j++)
                {
                    long prod = (long)i * primes[j];
                    if (prod > n) break;
                    int ip = (int)prod;
                    if (i % primes[j] == 0)
                    {
                        phi[ip] = phi[i] * primes[j];
                        break;
                    }
                    phi[ip] = phi[i] * (primes[j] - 1);
                }
            }
        }
    }
}
