namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class OfflineQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MultiEval(long* poly, int n, long x, int MOD)
        {
            long val = 0;
            long xPow = 1;
            for (int i = 0; i < n; i++)
            {
                val = (val + poly[i] * xPow) % MOD;
                xPow = xPow * x % MOD;
            }
            return val;
        }
    }
}
