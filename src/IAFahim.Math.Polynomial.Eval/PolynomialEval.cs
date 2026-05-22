namespace IAFahim.Math.Polynomial.Eval
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MultiPointEval
    {
        public static void Run(int n, long* poly, int m, long* x, long* res, long mod)
        {
            for (int i = 0; i < m; i++)
            {
                long val = 0;
                long cur = 1;
                long xi = x[i] % mod;
                if (xi < 0) xi += mod;
                for (int j = 0; j < n; j++)
                {
                    long p = poly[j] % mod;
                    if (p < 0) p += mod;
                    val = (val + p * cur) % mod;
                    cur = (cur * xi) % mod;
                }
                res[i] = val;
            }
        }
    }

    public static unsafe class ChirpZTransform
    {
        public static int Run(int n, long* a, long c, long d, long* res, long mod)
        {
            long* g = stackalloc long[n];
            long* h = stackalloc long[n];
            long pow = 1;
            for (int i = 0; i < n; i++)
            {
                g[i] = a[i] * pow % mod;
                pow = pow * c % mod;
            }
            pow = 1;
            for (int i = 0; i < n; i++)
            {
                h[i] = FastPow(pow, (long)i, mod);
                pow = pow * d % mod;
            }
            long* prod = stackalloc long[2 * n];
            int prodLen = PolynomialMulMod(n, g, n, h, prod, mod);
            for (int i = 0; i < prodLen; i++) res[i] = prod[i] % mod;
            return prodLen;
        }

        private static int PolynomialMulMod(int n, long* a, int m, long* b, long* res, long mod)
        {
            for (int i = 0; i < n + m - 1; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
            return n + m - 1;
        }

        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod;
            long b = a % mod;
            while (e > 0)
            {
                if ((e & 1) == 1) res = res * b % mod;
                b = b * b % mod;
                e >>= 1;
            }
            return res;
        }
    }
}