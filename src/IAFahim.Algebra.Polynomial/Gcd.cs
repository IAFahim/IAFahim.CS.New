namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Gcd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* a, int lenA, long* b, int lenB, long* gcd, out int lenGcd, int MOD)
        {
            int maxLen = Math.Max(lenA, lenB);

            long* u = stackalloc long[maxLen];
            long* v = stackalloc long[maxLen];

            CopyArray(a, u, lenA);
            CopyArray(b, v, lenB);

            int lenU = ComputeEffectiveLength(u, lenA);
            int lenV = ComputeEffectiveLength(v, lenB);

            long* q = stackalloc long[maxLen + 1];
            long* r = stackalloc long[maxLen + 1];

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
                long mod = (long)MOD;
                long inv = ModInv(u[lenU - 1], mod);
                for (int i = 0; i < lenU; i++)
                {
                    gcd[i] = (u[i] * inv) % mod;
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
                long na = a - t * b;
                a = b;
                b = na;

                long nu = u - t * v;
                u = v;
                v = nu;
            }
            u %= mod;
            if (u < 0L) u += mod;
            return u;
        }
    }
}
