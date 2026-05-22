namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RationalInterpolation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long* xs, long* ys, int n, int MOD, long* num, long* den)
        {
            for (int i = 0; i < n; i++) num[i] = ys[i] % MOD;
            den[0] = 1;
            for (int i = 1; i < n; i++) den[i] = 0;
            return n;
        }
    }
}
