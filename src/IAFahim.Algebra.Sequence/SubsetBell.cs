namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubsetBell
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Transform(long* a, int n, int MOD)
        {
            long result = 0;
            long bell = 0;
            long* b = stackalloc long[n + 1];
            b[0] = 1;
            for (int i = 1; i <= n; i++)
            {
                b[i] = 0;
                long binom = 1;
                for (int k = 0; k < i; k++)
                {
                    b[i] = (b[i] + binom * b[k]) % MOD;
                    binom = binom * (i - 1 - k) % MOD * Combinatorial.ModPow(k + 1, MOD - 2, MOD) % MOD;
                }
            }
            for (int i = 0; i < n; i++)
                result = (result + a[i] * b[i]) % MOD;
            return result;
        }
    }
}
