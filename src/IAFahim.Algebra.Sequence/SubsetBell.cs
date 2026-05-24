namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubsetBell
    {
        public static long Transform(long* a, int n, int MOD)
        {
            long* b = stackalloc long[n + 1]; b[0] = 1;
            for (int i = 1; i <= n; i++) ComputeBellRow(i, MOD, b);
            
            long res = 0; for (int i = 0; i < n; i++) res = (res + a[i] * b[i]) % MOD;
            return res;
        }

        private static void ComputeBellRow(int i, int MOD, long* b)
        {
            b[i] = 0; long binom = 1;
            for (int k = 0; k < i; k++)
            {
                b[i] = (b[i] + binom * b[k]) % MOD;
                binom = binom * (i - 1 - k) % MOD * Combinatorial.ModPow(k + 1, MOD - 2, MOD) % MOD;
            }
        }
    }
}
