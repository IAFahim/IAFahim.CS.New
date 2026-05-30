namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BostanMori
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* p, int pLen, long* q, int qLen, long k, int MOD)
        {
            while (k > 0L)
            {
                long* qneg = stackalloc long[qLen];
                for (int i = 0; i < qLen; i++)
                    qneg[i] = (i % 2 == 0) ? q[i] : ((long)MOD - q[i]) % (long)MOD;
                int sz = Math.Max(pLen, qLen);
                long* r = stackalloc long[sz * 2];
                long* s = stackalloc long[sz * 2];
                ToomCook.Multiply(p, pLen, qneg, qLen, r, MOD);
                ToomCook.Multiply(q, qLen, qneg, qLen, s, MOD);

                FilterPolynomial(p, pLen, r, (int)(k % 2L), sz);
                FilterPolynomial(q, qLen, s, 0, sz);

                k >>= 1;
            }
            return p[0] % (long)MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FilterPolynomial(long* dst, int len, long* src, int parity, int sz)
        {
            for (int i = 0; i < len; i++)
                dst[i] = (parity + 2 * i < len + sz) ? src[parity + 2 * i] : 0L;
        }
    }
}