namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RootFind
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Find(long* poly, int n, int MOD, long* roots)
        {
            int count = 0;
            for (long x = 0; x < MOD && count < n; x++)
            {
                long val = 0;
                long xPow = 1;
                for (int i = 0; i < n; i++)
                {
                    val = (val + poly[i] * xPow) % MOD;
                    xPow = xPow * x % MOD;
                }
                if (val == 0) roots[count++] = x;
            }
            return count;
        }
    }
}
