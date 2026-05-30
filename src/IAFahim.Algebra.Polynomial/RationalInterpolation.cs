namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RationalInterpolation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long* xs, long* ys, int n, long MOD, long* num, long* den)
        {
            if (n == 0) return 0;
            long* c = stackalloc long[n];
            for (int i = 0; i < n; i++) c[i] = ys[i];
            long* r = stackalloc long[n];
            int degNum, degDen;
            ThieleInterpolation(xs, c, n, r, out degNum, out degDen, MOD);
            for (int i = 0; i <= degNum; i++) num[i] = r[i];
            for (int i = 0; i <= degDen; i++) den[i] = i == 0 ? 1 : r[n - 1 + i];
            return degNum + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThieleInterpolation(long* x, long* y, int n, long* res, out int degNum, out int degDen, long MOD)
        {
            degDen = n / 2;
            long* c = stackalloc long[n];
            for (int i = 0; i < n; i++) c[i] = y[i];
            int m = n;
            degNum = n - 1 - degDen;
            res[m - 1] = y[0];
            int nxt = 1;
            while (m > 1)
            {
                long* diffs = stackalloc long[m - 1];
                for (int i = 0; i < m - 1; i++)
                {
                    long num = x[nxt + i] - x[i];
                    long den = c[i + 1] - c[i];
                    long inv = ModInv(num, MOD);
                    diffs[i] = (den % MOD + MOD) * (inv % MOD + MOD) % MOD;
                }
                res[m - 2] = diffs[0];
                for (int i = 0; i < m - 2; i++) c[i] = diffs[i + 1];
                m--;
                nxt++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModInv(long a, long MOD)
        {
            long t = 0, newT = 1;
            long r = MOD, newR = a % MOD;
            if (newR < 0) newR += MOD;
            while (newR != 0)
            {
                long q = r / newR;
                long tmpT = t - q * newT;
                long tmpR = r - q * newR;
                t = newT; r = newR;
                newT = tmpT; newR = tmpR;
            }
            if (r > 1) return -1;
            if (t < 0) t += MOD;
            return t;
        }
    }
}