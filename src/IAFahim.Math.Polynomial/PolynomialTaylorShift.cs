namespace IAFahim.Math.Polynomial
{
    using System.Runtime.CompilerServices;

    public static unsafe class PolynomialTaylorShift
    {
        public static void Run(long* a, int n, long c, long mod, long* fact, long* invFact)
        {
            if (n <= 0) return;
            for (int i = 0; i < n; i++) a[i] = a[i] * fact[i] % mod;

            long* b = stackalloc long[n];
            long pw = 1;
            for (int i = 0; i < n; i++)
            {
                b[n - 1 - i] = pw * invFact[i] % mod;
                pw = pw * c % mod;
            }

            long* res = stackalloc long[2 * n - 1];
            for (int i = 0; i < 2 * n - 1; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;

            for (int i = 0; i < n; i++)
                a[i] = res[n - 1 + i] * invFact[i] % mod;
        }
    }
}
