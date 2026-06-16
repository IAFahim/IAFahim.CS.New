namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PowMod
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* poly, int lenPoly, long exponent, long* modPoly, int lenModPoly, long* result, out int lenResult, int MOD)
        {
            lenPoly = ComputeEffectiveLength(poly, lenPoly);
            lenModPoly = ComputeEffectiveLength(modPoly, lenModPoly);

            result[0] = 1L;
            lenResult = 1;

            if (exponent == 0L || lenModPoly <= 1)
            {
                if (lenModPoly == 1) lenResult = 0;
                return;
            }

            // After reducing the base modulo modPoly, the reduced base has degree
            // strictly less than deg(modPoly), i.e. length < lenModPoly. Every product
            // formed in the loop then has length <= 2*(lenModPoly-1)-1 = 2*lenModPoly-3,
            // so buffers of size lenModPoly*2+1 are always sufficient.
            // The quotient produced when reducing the (unreduced) initial base can be
            // longer than that, so the scratch buffers must accommodate lenPoly.
            int scratchLen = lenPoly > lenModPoly * 2 + 1 ? lenPoly : lenModPoly * 2 + 1;

            long* basePoly = stackalloc long[scratchLen];
            long* q = stackalloc long[scratchLen];
            long* r = stackalloc long[scratchLen];
            long* temp = stackalloc long[scratchLen];

            // Reduce the base modulo modPoly so its degree is below deg(modPoly).
            int lenBase;
            int lenQInit;
            DivMod.Run(poly, lenPoly, modPoly, lenModPoly, q, out lenQInit, basePoly, out lenBase, MOD);

            if (lenBase == 0)
            {
                // Base is congruent to 0 modulo modPoly; result is 0 for exponent >= 1.
                lenResult = 0;
                return;
            }

            while (exponent > 0L)
            {
                if ((exponent & 1L) == 1L)
                {
                    ToomCook.Multiply(result, lenResult, basePoly, lenBase, temp, MOD);
                    int tempLen = ComputeEffectiveLength(temp, lenResult + lenBase - 1);

                    int lenQ, lenR;
                    DivMod.Run(temp, tempLen, modPoly, lenModPoly, q, out lenQ, r, out lenR, MOD);

                    CopyArray(r, result, lenR);
                    lenResult = lenR;
                }

                exponent >>= 1;
                if (exponent == 0L) break;

                ToomCook.Multiply(basePoly, lenBase, basePoly, lenBase, temp, MOD);
                int tempLen2 = ComputeEffectiveLength(temp, lenBase + lenBase - 1);

                int lenQ2, lenR2;
                DivMod.Run(temp, tempLen2, modPoly, lenModPoly, q, out lenQ2, r, out lenR2, MOD);

                CopyArray(r, basePoly, lenR2);
                lenBase = lenR2;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyArray(long* src, long* dst, int len)
        {
            for (int i = 0; i < len; i++)
            {
                dst[i] = src[i];
            }
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
