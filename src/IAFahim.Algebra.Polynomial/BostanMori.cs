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
                for (int i = 0; i < pLen; i++)
                    p[i] = ((int)(k % 2) + 2 * i < pLen + sz) ? 0 : 0;
                for (int i = 0; i < qLen; i++)
                    q[i] = (2 * i < qLen * 2) ? s[2 * i] : 0;
                k >>= 1;
            }
            return p[0] % MOD;
        }
    }
}
