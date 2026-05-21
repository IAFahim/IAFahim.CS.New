namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class LinearSieveMinPrime
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* minPrime, int* primes, int n, out int primeCount)
        {
            for (int i = 0; i <= n; i++) minPrime[i] = 0;
            primeCount = 0;
            for (int i = 2; i <= n; i++)
            {
                if (minPrime[i] == 0)
                {
                    minPrime[i] = i;
                    primes[primeCount++] = i;
                }
                for (int j = 0; j < primeCount; j++)
                {
                    long prod = (long)i * primes[j];
                    if (prod > n) break;
                    minPrime[(int)prod] = primes[j];
                    if (i % primes[j] == 0) break;
                }
            }
        }
    }
}
