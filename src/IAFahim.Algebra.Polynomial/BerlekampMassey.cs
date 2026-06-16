namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BerlekampMassey
{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long* s, int n, int MOD, long* c)
        {
            if (n <= 0)
            {
                c[0] = 1L;
                return 1;
            }

            long* b = stackalloc long[n + 1];
            long* tmp = stackalloc long[n + 1];

            InitArrays(n, c, b, tmp);

            int L = 0;
            int m = 1;
            long b_val = 1L;

            for (int i = 0; i < n; i++)
            {
                long d = CalculateDiscrepancy(s, c, i, L, MOD);

                if (d == 0L)
                {
                    m++;
                    continue;
                }

                CopyArray(c, tmp, n);

                long factor = (d * ModPow(b_val, (long)MOD - 2L, (long)MOD)) % (long)MOD;

                UpdateCoefficients(c, b, factor, L, m, n, MOD);

                if (2 * L <= i)
                {
                    L = i + 1 - L;
                    CopyArray(tmp, b, n);
                    b_val = d;
                    m = 1;
                }
                else
                {
                    m++;
                }
            }

            return L + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitArrays(int n, long* c, long* b, long* tmp)
        {
            for (int i = 0; i <= n; i++)
            {
                c[i] = 0L;
                b[i] = 0L;
                tmp[i] = 0L;
            }
            c[0] = 1L;
            b[0] = 1L;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateDiscrepancy(long* s, long* c, int i, int L, int MOD)
        {
            long d = s[i];
            for (int j = 1; j <= L; j++)
            {
                d = (d + c[j] * s[i - j]) % (long)MOD;
            }
            if (d < 0L)
            {
                d += (long)MOD;
            }
            return d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyArray(long* src, long* dst, int n)
        {
            for (int j = 0; j <= n; j++)
            {
                dst[j] = src[j];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateCoefficients(long* c, long* b, long factor, int L, int m, int n, int MOD)
        {
            for (int j = 0; j <= L; j++)
            {
                int idx = j + m;
                if (idx <= n)
                {
                    long t = c[idx] - (factor * b[j]) % (long)MOD;
                    c[idx] = t + ((t >> 63) & (long)MOD);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1L;
            b %= mod;
            if (b < 0L)
            {
                b += mod;
            }
            while (e > 0L)
            {
                if ((e & 1L) != 0L)
                {
                    r = (r * b) % mod;
                }
                b = (b * b) % mod;
                e >>= 1;
            }
            return r;
        }
    }
}