namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Transform
    {
        public static void Binomial(long* a, int n, int MOD, long* b)
        {
            long mod = (long)MOD;
            long* inv = stackalloc long[n < 1 ? 1 : n];
            FillModInverses(inv, n - 1, mod);
            for (int n2 = 0; n2 < n; n2++)
            {
                b[n2] = ComputeBinomialTerm(a, n2, mod, inv);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ComputeBinomialTerm(long* a, int n2, long mod, long* inv)
        {
            long sum = 0L;
            long comb = 1L;
            for (int k = 0; k <= n2; k++)
            {
                sum = (sum + comb * a[k]) % mod;
                if (k < n2)
                    comb = (comb * (long)(n2 - k)) % mod * inv[k + 1] % mod;
            }
            return sum;
        }

        public static void InverseBinomial(long* a, int n, int MOD, long* b)
        {
            long mod = (long)MOD;
            long* inv = stackalloc long[n < 1 ? 1 : n];
            FillModInverses(inv, n - 1, mod);
            for (int n2 = 0; n2 < n; n2++)
            {
                b[n2] = ComputeInverseBinomialTerm(a, n2, mod, inv);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ComputeInverseBinomialTerm(long* a, int n2, long mod, long* inv)
        {
            long sum = 0L;
            long comb = 1L;
            // sign = (-1)^(n2 - k). Starting at k = 0 it is (-1)^n2; it toggles each step.
            long sign = ((n2 & 1) == 0) ? 1L : mod - 1L;
            for (int k = 0; k <= n2; k++)
            {
                sum = (sum + (sign * comb) % mod * a[k]) % mod;
                if (k < n2)
                    comb = (comb * (long)(n2 - k)) % mod * inv[k + 1] % mod;
                sign = mod - sign;
            }
            return sum;
        }

        public static long SetPartition(int n, int MOD)
        {
            long mod = (long)MOD;
            long* bell = stackalloc long[n + 1];
            long* inv = stackalloc long[n + 1];
            FillModInverses(inv, n, mod);
            bell[0] = 1L;
            for (int i = 1; i <= n; i++)
            {
                bell[i] = 0L;
                long binom = 1L;
                for (int k = 0; k < i; k++)
                {
                    bell[i] = (bell[i] + binom * bell[k]) % mod;
                    binom = (binom * (long)(i - 1 - k)) % mod * inv[k + 1] % mod;
                }
            }
            return bell[n];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CayleyCount(int n, int MOD)
        {
            if (n <= 1) return 1L;
            if (n == 2) return 1L;
            return Combinatorial.ModPow((long)n, (long)(n - 2), (long)MOD);
        }

        // Linear sieve of modular inverses: fills inv[1..maxIndex] with the modular
        // inverse of each integer. Valid when mod is prime and mod > maxIndex (the same
        // primality precondition the Fermat-based ModPow already assumes), which guarantees
        // mod % i != 0 so every i in 1..maxIndex is invertible. O(maxIndex) total.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FillModInverses(long* inv, int maxIndex, long mod)
        {
            if (maxIndex < 1) return;
            inv[1] = 1L;
            for (int i = 2; i <= maxIndex; i++)
            {
                inv[i] = (mod - (mod / i) * inv[mod % i] % mod) % mod;
            }
        }
    }
}
