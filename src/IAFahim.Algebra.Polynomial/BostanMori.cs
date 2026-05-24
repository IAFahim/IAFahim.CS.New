namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BostanMori
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long KthTerm(long* p, int pLen, long* q, int qLen, long k, int MOD)
        {
            while (k > 0)
            {
                long* qneg = stackalloc long[qLen];
                for (int i = 0; i < qLen; i++)
                    qneg[i] = (i % 2 == 0) ? q[i] : (MOD - q[i]) % MOD;
                int sz = Math.Max(pLen, qLen);
                long* r = stackalloc long[sz * 2];
                long* s = stackalloc long[sz * 2];
                ToomCook.Multiply(p, qneg, r, sz, MOD);
                ToomCook.Multiply(q, qneg, s, sz, MOD);
                
                FilterPolynomial(p, pLen, r, (int)(k % 2), sz);
                FilterPolynomial(q, qLen, s, 0, sz);
                
                k >>= 1;
            }
            return p[0] % MOD;
        }

        private static void FilterPolynomial(long* dst, int len, long* src, int parity, int sz)
        {
            for (int i = 0; i < len; i++)
                dst[i] = (parity + 2 * i < len + sz) ? src[parity + 2 * i] : 0;
        }
    }
}
