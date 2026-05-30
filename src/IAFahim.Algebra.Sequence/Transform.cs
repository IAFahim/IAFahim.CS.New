namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Transform
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Binomial(long* a, int n, int MOD, long* b)
        {
            for (int n2 = 0; n2 < n; n2++)
            {
                b[n2] = ComputeBinomialTerm(a, n2, MOD);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ComputeBinomialTerm(long* a, int n2, int MOD)
        {
            long sum = 0L;
            long comb = 1L;
            for (int k = 0; k <= n2; k++)
            {
                sum = (sum + comb * a[k]) % (long)MOD;
                if (k < n2)
                    comb = (comb * (long)(n2 - k)) % (long)MOD * Combinatorial.ModPow((long)(k + 1), (long)MOD - 2L, (long)MOD) % (long)MOD;
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InverseBinomial(long* a, int n, int MOD, long* b)
        {
            for (int n2 = 0; n2 < n; n2++)
            {
                b[n2] = ComputeInverseBinomialTerm(a, n2, MOD);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ComputeInverseBinomialTerm(long* a, int n2, int MOD)
        {
            long sum = 0L;
            long comb = 1L;
            for (int k = 0; k <= n2; k++)
            {
                long sign = (k % 2 == 0) ? 1L : (long)MOD - 1L;
                sum = (sum + (sign * comb) % (long)MOD * a[k]) % (long)MOD;
                if (k < n2)
                    comb = (comb * (long)(n2 - k)) % (long)MOD * Combinatorial.ModPow((long)(k + 1), (long)MOD - 2L, (long)MOD) % (long)MOD;
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SetPartition(int n, int MOD)
        {
            long* bell = stackalloc long[n + 1];
            bell[0] = 1L;
            for (int i = 1; i <= n; i++)
            {
                bell[i] = 0L;
                long binom = 1L;
                for (int k = 0; k < i; k++)
                {
                    bell[i] = (bell[i] + binom * bell[k]) % (long)MOD;
                    binom = (binom * (long)(i - 1 - k)) % (long)MOD * Combinatorial.ModPow((long)(k + 1), (long)MOD - 2L, (long)MOD) % (long)MOD;
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
    }
}