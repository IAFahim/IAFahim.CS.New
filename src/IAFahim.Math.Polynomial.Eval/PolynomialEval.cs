namespace IAFahim.Math.Polynomial.Eval
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MultiPointEval
    {
        public static void Run(int n, long* poly, int m, long* x, long* res, long mod)
        {
            for (int i = 0; i < m; i++) res[i] = EvaluateAt(n, poly, x[i], mod);
        }

        private static long EvaluateAt(int n, long* poly, long x, long mod)
        {
            long val = 0, xi = (x % mod + mod) % mod;
            for (int j = n - 1; j >= 0; j--)
            {
                long p = (poly[j] % mod + mod) % mod;
                val = (val * xi + p) % mod;
            }
            return val;
        }
    }

    public static unsafe class ChirpZTransform
    {
        public static int Run(int n, long* a, long c, long d, long* res, long mod)
        {
            int convLen = 2 * n - 1;
            long* g = stackalloc long[convLen];
            long* h = stackalloc long[convLen];
            for (int i = 0; i < convLen; i++) { g[i] = 0; h[i] = 0; }

            long invD = FastPow(d, mod - 2, mod);
            long cPow = 1;
            for (int i = 0; i < n; i++)
            {
                long binomial_i_2 = (long)i * (i - 1) / 2;
                long dPow = FastPow(d, binomial_i_2, mod);
                g[i] = a[i] * cPow % mod * dPow % mod;
                cPow = cPow * c % mod;

                long invDPow = FastPow(invD, binomial_i_2, mod);
                h[n - 1 + i] = invDPow;
                if (n - 1 - i >= 0)
                    h[n - 1 - i] = FastPow(invD, (long)i * (i - 1) / 2, mod);
            }

            long* prod = stackalloc long[convLen];
            for (int i = 0; i < convLen; i++) prod[i] = 0;
            NaiveConvolve(g, n, h, convLen, prod, convLen, mod);

            for (int k = 0; k < n; k++)
            {
                long binomial_k_2 = (long)k * (k - 1) / 2;
                long invDPow = FastPow(invD, binomial_k_2, mod);
                res[k] = prod[n - 1 + k] * invDPow % mod;
            }
            return n;
        }

        private static void NaiveConvolve(long* a, int an, long* b, int bn, long* res, int resLen, long mod)
        {
            for (int i = 0; i < resLen; i++) res[i] = 0;
            for (int i = 0; i < an; i++)
                for (int j = 0; j < bn; j++)
                    if (i + j < resLen)
                        res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
        }

        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod, b = a % mod;
            while (e > 0) { if ((e & 1) == 1) res = res * b % mod; b = b * b % mod; e >>= 1; }
            return res;
        }
    }
}
