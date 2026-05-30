namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DivMod
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* a, int lenA, long* b, int lenB, long* q, out int lenQ, long* r, out int lenR, int MOD)
        {
            lenA = ComputeEffectiveLength(a, lenA);
            lenB = ComputeEffectiveLength(b, lenB);

            if (lenB == 0)
            {
                lenQ = 0;
                lenR = 0;
                return;
            }

            if (lenA < lenB)
            {
                lenQ = 0;
                CopyArray(a, r, lenA);
                lenR = lenA;
                return;
            }

            lenQ = lenA - lenB + 1;
            for (int i = 0; i < lenQ; i++)
            {
                q[i] = 0L;
            }
            
            long* temp = stackalloc long[lenA];
            CopyArray(a, temp, lenA);

            long inv = ModInv(b[lenB - 1], (long)MOD);

            for (int i = lenA - 1; i >= lenB - 1; i--)
            {
                if (temp[i] == 0L) continue;

                long q_i = (temp[i] * inv) % (long)MOD;
                q[i - (lenB - 1)] = q_i;

                for (int j = 0; j < lenB; j++)
                {
                    long sub = (q_i * b[j]) % (long)MOD;
                    temp[i - (lenB - 1) + j] = (temp[i - (lenB - 1) + j] - sub + (long)MOD) % (long)MOD;
                }
            }

            lenR = ComputeEffectiveLength(temp, lenB - 1);
            CopyArray(temp, r, lenR);
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
