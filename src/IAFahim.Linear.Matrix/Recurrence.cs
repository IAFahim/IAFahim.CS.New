namespace IAFahim.Linear.Matrix
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BerlekampMassey
    {
        public static int Run(long* s, int n, long* c)
        {
            long* C = stackalloc long[n], B = stackalloc long[n];
            int L = 0, m = 0; long b = 1;
            for (int i = 0; i < n; i++) { C[i] = 0; B[i] = 0; }
            C[0] = 1; B[0] = 1;
            for (int i = 0; i < n; i++)
            {
                long d = s[i];
                for (int j = 1; j <= L; j++) d += C[j] * s[i - j];
                if (d == 0) m++;
                else if (2 * L <= i) { L = i + 1 - L; UpdateBM(C, B, n, m, d, b, true); b = d; m = 1; }
                else { UpdateBM(C, B, n, m, d, b, false); m++; }
            }
            for (int i = 0; i <= L; i++) c[i] = C[i];
            return L;
        }

        private static void UpdateBM(long* C, long* B, int n, int m, long d, long b, bool replaceB)
        {
            long* T = stackalloc long[n]; if (replaceB) for (int j = 0; j < n; j++) T[j] = C[j];
            long coef = d * b;
            for (int j = 0; j <= n - m; j++) C[m + j] -= coef * B[j];
            if (replaceB) for (int j = 0; j < n; j++) B[j] = T[j];
        }
    }

    public static unsafe class Kitamasa
    {
        public static long Run(int k, long* init, long* trans, long n, long mod)
        {
            if (n < k) return init[n];
            long* pol = stackalloc long[k]; long* res = stackalloc long[k];
            for (int i = 0; i < k; i++) { pol[i] = 0; res[i] = 0; }
            pol[0] = 1; res[1] = 1;
            long exp = n - k + 1;
            while (exp > 0)
            {
                if ((exp & 1) == 1) PerformKitamasaStep(k, res, pol, trans, mod);
                if (exp > 1) PerformKitamasaStep(k, pol, pol, trans, mod);
                exp >>= 1;
            }
            return ComputeResult(k, res, init, mod);
        }

        private static void PerformKitamasaStep(int k, long* a, long* b, long* trans, long mod)
        {
            long* newRes = stackalloc long[2 * k];
            for (int i = 0; i < 2 * k; i++) newRes[i] = 0;
            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++) newRes[i + j] = (newRes[i + j] + (a[i] % mod) * (b[j] % mod)) % mod;
            
            for (int i = 2 * k - 1; i >= k; i--)
                for (int j = 1; j <= k; j++) newRes[i - j] = (newRes[i - j] + (newRes[i] % mod) * (trans[k - j] % mod)) % mod;
            
            for (int i = 0; i < k; i++) a[i] = newRes[i];
        }

        private static long ComputeResult(int k, long* res, long* init, long mod)
        {
            long ans = 0;
            for (int i = 0; i < k; i++) ans = (ans + (res[i] % mod) * (init[k - 1 - i] % mod)) % mod;
            return ans;
        }
    }
}
