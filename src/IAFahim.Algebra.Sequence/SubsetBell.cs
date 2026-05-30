namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubsetBell
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* a, int n, int MOD)
        {
            long* b = stackalloc long[n + 1]; b[0] = 1L;
            for (int i = 1; i <= n; i++) ComputeBellRow(i, MOD, b);

            long res = 0L; for (int i = 0; i < n; i++) res = (res + a[i] * b[i]) % (long)MOD;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeBellRow(int i, int MOD, long* b)
        {
            b[i] = 0L; long binom = 1L;
            for (int k = 0; k < i; k++)
            {
                b[i] = (b[i] + binom * b[k]) % (long)MOD;
                binom = (binom * (long)(i - 1 - k)) % (long)MOD * Combinatorial.ModPow((long)(k + 1), (long)MOD - 2L, (long)MOD) % (long)MOD;
            }
        }
    }
}