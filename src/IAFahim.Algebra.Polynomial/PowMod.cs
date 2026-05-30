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

            long* basePoly = stackalloc long[lenModPoly * 2 + 1];
            int lenBase = lenPoly;
            CopyArray(poly, basePoly, lenBase);

            long* q = stackalloc long[lenModPoly * 2 + 1];
            long* r = stackalloc long[lenModPoly * 2 + 1];
            long* temp = stackalloc long[lenModPoly * 2 + 1];

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

                ToomCook.Multiply(basePoly, lenBase, basePoly, lenBase, temp, MOD);
                int tempLen2 = ComputeEffectiveLength(temp, lenBase + lenBase - 1);
                
                int lenQ2, lenR2;
                DivMod.Run(temp, tempLen2, modPoly, lenModPoly, q, out lenQ2, r, out lenR2, MOD);
                
                CopyArray(r, basePoly, lenR2);
                lenBase = lenR2;

                exponent >>= 1;
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
