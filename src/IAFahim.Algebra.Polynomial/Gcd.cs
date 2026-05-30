namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Gcd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* a, int lenA, long* b, int lenB, long* gcd, out int lenGcd, int MOD)
        {
            long* u = stackalloc long[lenA];
            long* v = stackalloc long[lenB];
            
            CopyArray(a, u, lenA);
            CopyArray(b, v, lenB);
            
            int lenU = ComputeEffectiveLength(u, lenA);
            int lenV = ComputeEffectiveLength(v, lenB);

            long* q = stackalloc long[Math.Max(lenA, lenB) + 1];
            long* r = stackalloc long[Math.Max(lenA, lenB) + 1];

            while (lenV > 0)
            {
                int lenQ, lenR;
                DivMod.Run(u, lenU, v, lenV, q, out lenQ, r, out lenR, MOD);

                CopyArray(v, u, lenV);
                lenU = lenV;

                CopyArray(r, v, lenR);
                lenV = lenR;
            }

            if (lenU > 0)
            {
                long inv = ModInv(u[lenU - 1], (long)MOD);
                for (int i = 0; i < lenU; i++)
                {
                    gcd[i] = (u[i] * inv) % (long)MOD;
                }
            }
            lenGcd = lenU;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModInv(long a, long mod)
        {
            long b = mod, u = 1L, v = 0L;
            while (b > 0L)
            {
                long t = a / b;
                a -= t * b;
                long tmp = a;
                a = b;
                b = tmp;
                
                u -= t * v;
                tmp = u;
                u = v;
                v = tmp;
            }
            u %= mod;
            if (u < 0L) u += mod;
            return u;
        }
    }
}
