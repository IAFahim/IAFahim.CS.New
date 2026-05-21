namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class LinearSieveMaxPrime
    {
        public static void Run(int* maxPrime, int* primes, int n, out int primeCount)
        {
            for (int i = 0; i <= n; i++) maxPrime[i] = 0;
            primeCount = 0;
            for (int i = 2; i <= n; i++)
            {
                if (maxPrime[i] == 0)
                {
                    maxPrime[i] = i;
                    primes[primeCount++] = i;
                }
                for (int j = 0; j < primeCount; j++)
                {
                    long prod = (long)i * primes[j];
                    if (prod > n) break;
                    maxPrime[(int)prod] = maxPrime[i];
                    if (i % primes[j] == 0) break;
                }
            }
        }
    }
}
