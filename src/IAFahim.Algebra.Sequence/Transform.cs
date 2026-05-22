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
                long sum = 0;
                long comb = 1;
                for (int k = 0; k <= n2; k++)
                {
                    sum = (sum + comb * a[k]) % MOD;
                    if (k < n2)
                        comb = comb * (n2 - k) % MOD * Combinatorial.ModPow(k + 1, MOD - 2, MOD) % MOD;
                }
                b[n2] = sum;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InverseBinomial(long* a, int n, int MOD, long* b)
        {
            long* invFact = stackalloc long[n + 1];
            long* fact = stackalloc long[n + 1];
            fact[0] = 1;
            for (int i = 1; i <= n; i++) fact[i] = fact[i - 1] * i % MOD;
            invFact[n] = Combinatorial.ModPow(fact[n], MOD - 2, MOD);
            for (int i = n - 1; i >= 0; i--) invFact[i] = invFact[i + 1] * (i + 1) % MOD;
            for (int n2 = 0; n2 < n; n2++)
            {
                long sum = 0;
                long comb = 1;
                for (int k = 0; k <= n2; k++)
                {
                    long sign = (k % 2 == 0) ? 1 : MOD - 1;
                    sum = (sum + sign * comb % MOD * a[k]) % MOD;
                    if (k < n2)
                        comb = comb * (n2 - k) % MOD * Combinatorial.ModPow(k + 1, MOD - 2, MOD) % MOD;
                }
                b[n2] = sum;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SetPartition(int n, int MOD)
        {
            long* bell = stackalloc long[n + 1];
            bell[0] = 1;
            for (int i = 1; i <= n; i++)
            {
                bell[i] = 0;
                long binom = 1;
                for (int k = 0; k < i; k++)
                {
                    bell[i] = (bell[i] + binom * bell[k]) % MOD;
                    binom = binom * (i - 1 - k) % MOD * Combinatorial.ModPow(k + 1, MOD - 2, MOD) % MOD;
                }
            }
            return bell[n];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CayleyCount(int n, int MOD)
        {
            if (n <= 1) return 1;
            if (n == 2) return 1;
            return Combinatorial.ModPow(n, n - 2, MOD);
        }
    }
}
