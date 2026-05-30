namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class OfflineQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MultiEval(long* poly, int n, long x, int MOD)
        {
            long val = 0L;
            long xPow = 1L;
            for (int i = 0; i < n; i++)
            {
                val = (val + poly[i] * xPow) % (long)MOD;
                xPow = (xPow * x) % (long)MOD;
            }
            return val;
        }
    }
}