namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BerlekampMassey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long* s, int n, int MOD, long* c)
        {
            long* b = stackalloc long[n + 1];
            long* tmp = stackalloc long[n + 1];
            InitArrays(n, c, b, tmp);
            
            c[0] = 1; b[0] = 1;
            int cl = 1, bl = 1;
            long ld = 1;
            int shift = 0;

            for (int i = 0; i < n; i++)
            {
                long d = CalculateDiscrepancy(s, i, c, cl, MOD);
                if (d == 0) { shift++; continue; }
                
                for (int j = 0; j < cl; j++) tmp[j] = c[j];
                
                long factor = d * ModPow(ld, MOD - 2, MOD) % MOD;
                int newLen = Math.Max(cl, bl + i - shift + 1);
                
                UpdateCoefficients(c, b, bl, newLen, factor, i - shift, MOD);
                
                if (2 * cl <= i)
                {
                    for (int j = 0; j <= cl; j++) b[j] = tmp[j];
                    bl = cl;
                    ld = d;
                    shift = i;
                }
                cl = newLen;
            }
            return cl;
        }

        private static void InitArrays(int n, long* c, long* b, long* tmp)
        {
            for (int i = 0; i <= n; i++) { c[i] = 0; b[i] = 0; tmp[i] = 0; }
        }

        private static long CalculateDiscrepancy(long* s, int i, long* c, int cl, int MOD)
        {
            long d = s[i];
            for (int j = 1; j < cl; j++)
                d = (d + c[j] * s[i - j]) % MOD;
            if (d < 0) d += MOD;
            return d;
        }

        private static void UpdateCoefficients(long* c, long* b, int bl, int newLen, long factor, int offset, int MOD)
        {
            for (int j = 0; j < bl; j++)
            {
                int idx = j + offset + 1;
                if (idx < newLen && idx >= 0)
                    c[idx] = (c[idx] - factor * b[j] % MOD + MOD) % MOD;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1; b %= mod; if (b < 0) b += mod;
            while (e > 0) { if ((e & 1) != 0) r = r * b % mod; b = b * b % mod; e >>= 1; }
            return r;
        }
    }
}
