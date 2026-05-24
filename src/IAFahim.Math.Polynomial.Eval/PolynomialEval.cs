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
            long* g = stackalloc long[n]; long* h = stackalloc long[n];
            long pow = 1; for (int i = 0; i < n; i++) { g[i] = a[i] * pow % mod; pow = pow * c % mod; }
            pow = 1; for (int i = 0; i < n; i++) { h[i] = FastPow(pow, (long)i, mod); pow = pow * d % mod; }
            long* prod = stackalloc long[2 * n];
            int prodLen = PolynomialMulMod(n, g, n, h, prod, mod);
            for (int i = 0; i < prodLen; i++) res[i] = prod[i];
            return prodLen;
        }

        private static int PolynomialMulMod(int n, long* a, int m, long* b, long* res, long mod)
        {
            for (int i = 0; i < n + m - 1; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++) res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
            return n + m - 1;
        }

        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod, b = a % mod;
            while (e > 0) { if ((e & 1) == 1) res = res * b % mod; b = b * b % mod; e >>= 1; }
            return res;
        }
    }
}
