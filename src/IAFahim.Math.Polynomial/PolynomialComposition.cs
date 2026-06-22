namespace IAFahim.Math.Polynomial
{
    using System.Runtime.CompilerServices;

    public static unsafe class PolynomialComposition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AccumulateCoefficient(long* res, long fk, long* gpow, int n, long mod)
        {
            for (int j = 0; j < n; j++)
                res[j] = (res[j] + fk * gpow[j]) % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AdvanceGpow(long* gpow, long* g, int m, int n, long mod, long* next)
        {
            for (int j = 0; j < n; j++) next[j] = 0;
            for (int j = 0; j < n; j++)
            {
                if (gpow[j] == 0) continue;
                for (int l = 0; l < m && j + l < n; l++)
                    next[j + l] = (next[j + l] + gpow[j] * g[l]) % mod;
            }
            for (int j = 0; j < n; j++) gpow[j] = next[j];
        }

        public static void Run(int n, long* f, int m, long* g, long* res, long mod, long* tmp)
        {
            for (int i = 0; i < n; i++) res[i] = 0;
            if (n == 0) return;

            long* gpow = stackalloc long[n];
            for (int i = 0; i < n; i++) gpow[i] = 0;
            gpow[0] = 1;

            long* next = stackalloc long[n];
            for (int k = 0; k < n; k++)
            {
                if (f[k] != 0) AccumulateCoefficient(res, f[k] % mod, gpow, n, mod);
                if (k < n - 1) AdvanceGpow(gpow, g, m, n, mod, next);
            }
        }

        public static void RunNaive(int n, long* f, int m, long* g, long* res, long mod)
        {
            long* tmp = stackalloc long[1];
            Run(n, f, m, g, res, mod, tmp);
        }
    }
}
