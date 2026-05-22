namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RemainderTree
    {
        public static int Build(long* poly, int polyLen, long* values, int n, int MOD, long* remainders, int* remLens)
        {
            for (int i = 0; i < n; i++)
            {
                long v = values[i] % MOD;
                if (v < 0) v += MOD;
                long r = 0;
                long xPow = 1;
                for (int j = 0; j < polyLen; j++)
                {
                    r = (r + poly[j] * xPow) % MOD;
                    xPow = xPow * v % MOD;
                }
                remainders[i] = r;
                remLens[i] = 1;
            }
            return n;
        }
    }
}
