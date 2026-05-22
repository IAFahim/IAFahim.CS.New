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
            for (int i = 0; i <= n; i++) { c[i] = 0; b[i] = 0; tmp[i] = 0; }
            c[0] = 1; b[0] = 1;
            int cl = 1, bl = 1;
            long ld = 1;
            int shift = 0;
            for (int i = 0; i < n; i++)
            {
                long d = s[i];
                for (int j = 1; j < cl; j++)
                    d = (d + c[j] * s[i - j]) % MOD;
                if (d < 0) d += MOD;
                if (d == 0) { shift++; continue; }
                for (int j = 0; j < cl; j++) tmp[j] = c[j];
                long inv = ModPow(ld, MOD - 2, MOD);
                long factor = d * inv % MOD;
                int newLen = Math.Max(cl, bl + i - shift + 1);
                for (int j = 0; j < bl; j++)
                {
                    int idx = j + (i - shift) + 1;
                    if (idx < newLen && idx >= 0)
                        c[idx] = (c[idx] - factor * b[j] % MOD + MOD) % MOD;
                }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1; b %= mod; if (b < 0) b += mod;
            while (e > 0) { if ((e & 1) != 0) r = r * b % mod; b = b * b % mod; e >>= 1; }
            return r;
        }
    }
}
