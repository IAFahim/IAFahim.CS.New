namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Berlekamp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Factor(long* poly, int n, int MOD, long* outF, int* outL)
        {
            for (int i = 0; i < n; i++) outF[i] = poly[i];
            outL[0] = n;
            return 1;
        }
    }
}
