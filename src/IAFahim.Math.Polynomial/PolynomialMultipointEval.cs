namespace IAFahim.Math.Polynomial
{
    using System.Runtime.CompilerServices;

    public static unsafe class PolynomialMultipointEval
    {
        public static void Run(long* poly, int n, long* points, long* values, int m, long mod)
        {
            for (int i = 0; i < m; i++)
            {
                long x = points[i] % mod;
                long val = 0;
                for (int j = n - 1; j >= 0; j--)
                    val = (val * x + poly[j]) % mod;
                values[i] = (val + mod) % mod;
            }
        }

        public static void RunInterpolate(long* xs, long* ys, int n, long* poly, long mod)
        {
            for (int i = 0; i < n; i++) poly[i] = 0;

            long* tmp = stackalloc long[n];

            for (int i = 0; i < n; i++)
            {
                long num = ys[i] % mod;
                long den = 1;
                for (int j = 0; j < n; j++)
                {
                    if (j == i) continue;
                    den = den * ((xs[i] - xs[j] + mod) % mod) % mod;
                }
                long coef = num * FastPow(den, mod - 2, mod) % mod;

                tmp[0] = coef;
                for (int k = 1; k < n; k++) tmp[k] = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == i) continue;
                    for (int k = n - 1; k >= 1; k--)
                        tmp[k] = (tmp[k] * ((mod - xs[j]) % mod) + tmp[k - 1]) % mod;
                    tmp[0] = tmp[0] * ((mod - xs[j]) % mod) % mod;
                }

                for (int k = 0; k < n; k++)
                    poly[k] = (poly[k] + tmp[k]) % mod;
            }
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
