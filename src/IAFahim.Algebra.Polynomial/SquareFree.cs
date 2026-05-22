namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SquareFree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Factor(long* poly, int n, int MOD, long* outFact, int* outLens)
        {
            long* deriv = stackalloc long[n];
            for (int i = 1; i < n; i++)
                deriv[i - 1] = (long)((long)i % MOD) * poly[i] % MOD;
            deriv[n - 1] = 0;
            int dLen = n - 1;
            while (dLen > 0 && deriv[dLen - 1] == 0) dLen--;
            for (int i = 0; i < n; i++) outFact[i] = poly[i];
            outLens[0] = n;
            return 1;
        }
    }
}
