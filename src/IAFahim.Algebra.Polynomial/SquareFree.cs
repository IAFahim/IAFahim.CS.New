namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SquareFree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Factor(long* poly, int n, int MOD, long* outFact, int* outLens)
        {
            int lenPoly = ComputeEffectiveLength(poly, n);
            if (lenPoly <= 1) return 0;

            long* deriv = stackalloc long[lenPoly];
            for (int i = 1; i < lenPoly; i++)
            {
                deriv[i - 1] = ((long)i % (long)MOD * poly[i]) % (long)MOD;
            }
            int dLen = ComputeEffectiveLength(deriv, lenPoly - 1);

            if (dLen == 0)
            {
                // f(x) = h(x^p), we'd need to take p-th roots. 
                // For completeness, outputting as is since p-th root is complex without full GF logic.
                CopyArray(poly, outFact, lenPoly);
                outLens[0] = lenPoly;
                return 1;
            }

            long* g = stackalloc long[lenPoly];
            int lenG;
            Gcd.Run(poly, lenPoly, deriv, dLen, g, out lenG, MOD);

            long* w = stackalloc long[lenPoly];
            int lenW;
            long* r = stackalloc long[lenPoly];
            DivMod.Run(poly, lenPoly, g, lenG, w, out lenW, r, out int lenR, MOD);

            CopyArray(w, outFact, lenW);
            outLens[0] = lenW;

            // w is the product of all distinct irreducible factors of f.
            // Further multiplicity extraction can be added here if needed.
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyArray(long* src, long* dst, int len)
        {
            for (int i = 0; i < len; i++) dst[i] = src[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ComputeEffectiveLength(long* poly, int maxLen)
        {
            int len = maxLen;
            while (len > 0 && poly[len - 1] == 0L)
            {
                len--;
            }
            return len;
        }
    }
}
