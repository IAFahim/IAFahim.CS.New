namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class LcmConvolution
    {
        // Computes the divisor-sum (zeta) transform in place:
        //   F[j] = sum_{d | j} f[d].
        // Uses the prime-chain "sum over subsets of prime exponents" method so that
        // each source value is read BEFORE it is mutated, giving the exact zeta
        // transform (a plain "for d: for j=2d step d: f[j]+=f[d]" double-counts
        // because it reads already-accumulated values of f[d]).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forward(long* f, int n)
        {
            int* spf = stackalloc int[n + 1];
            int* primes = stackalloc int[n + 1];
            int primeCount = LinearSieve(spf, primes, n);

            for (int pi = 0; pi < primeCount; pi++)
            {
                int p = primes[pi];
                int limit = n / p;
                for (int j = 1; j <= limit; j++)
                    f[j * p] += f[j];
            }
        }

        // Inverse (divisor Mobius) transform in place; exact inverse of Forward:
        //   f[j] = sum_{d | j} mu[j/d] * F[d].
        // Implemented as the prime-chain inverse: for each prime p, walk j in
        // descending order and subtract f[j] from f[j*p], reading the finalized
        // value at each step. mu is retained for API/source compatibility; the
        // prime-chain inverse needs no explicit Mobius weighting.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Inverse(long* f, int n, int* mu)
        {
            int* spf = stackalloc int[n + 1];
            int* primes = stackalloc int[n + 1];
            int primeCount = LinearSieve(spf, primes, n);

            for (int pi = 0; pi < primeCount; pi++)
            {
                int p = primes[pi];
                for (int j = n / p; j >= 1; j--)
                    f[j * p] -= f[j];
            }
        }

        public static void Run(long* a, long* b, long* result, int n, int* mu)
        {
            Forward(a, n);
            Forward(b, n);
            for (int i = 1; i <= n; i++) result[i] = a[i] * b[i];
            Inverse(result, n, mu);
        }

        // Linear (Euler) sieve filling primes[0..return-1] ascending and the
        // smallest-prime-factor table spf[2..n]. Returns the prime count.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LinearSieve(int* spf, int* primes, int n)
        {
            for (int i = 0; i <= n; i++) spf[i] = 0;
            int primeCount = 0;
            for (int i = 2; i <= n; i++)
            {
                if (spf[i] == 0)
                {
                    spf[i] = i;
                    primes[primeCount++] = i;
                }
                for (int j = 0; j < primeCount; j++)
                {
                    int p = primes[j];
                    long product = (long)i * p;
                    if (p > spf[i] || product > n) break;
                    spf[(int)product] = p;
                }
            }
            return primeCount;
        }
    }
}
