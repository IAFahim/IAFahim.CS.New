namespace IAFahim.Linear.Matrix
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BerlekampMassey
    {
        public static int Run(long* s, int n, long* c)
        {
            int len = n + 1;
            long* C = stackalloc long[len], B = stackalloc long[len];
            int L = 0, m = 0; long b = 1;
            for (int i = 0; i < len; i++) { C[i] = 0; B[i] = 0; }
            C[0] = 1; B[0] = 1;
            for (int i = 0; i < n; i++)
            {
                long d = s[i];
                for (int j = 1; j <= L; j++) d += C[j] * s[i - j];
                if (d == 0) m++;
                else if (2 * L <= i) { L = i + 1 - L; UpdateBM(C, B, len, m, d, b, true); b = d; m = 1; }
                else { UpdateBM(C, B, len, m, d, b, false); m++; }
            }
            for (int i = 0; i <= L; i++) c[i] = C[i];
            return L;
        }

        private static void UpdateBM(long* C, long* B, int len, int m, long d, long b, bool replaceB)
        {
            int n = len - 1;
            long* T = stackalloc long[len]; if (replaceB) for (int j = 0; j < len; j++) T[j] = C[j];
            long coef = d * b;
            for (int j = 0; j <= n - m; j++) C[m + j] -= coef * B[j];
            if (replaceB) for (int j = 0; j < len; j++) B[j] = T[j];
        }
    }

    public static unsafe class Kitamasa
    {
        public static long Run(int k, long* init, long* trans, long n, long mod)
        {
            if (n < k) return init[n];

            // Order-1 recurrences (k == 1) reduce to a geometric sequence:
            // f(x) = x - trans[0], so x^n mod f == trans[0]^n, and a[n] == trans[0]^n * init[0].
            // The polynomial state below has degree < k coefficients, which cannot represent the
            // base x (degree 1) when k == 1, so this case is handled directly.
            if (k == 1) return ModMul(ModPow(trans[0], n, mod), init[0], mod);

            // Binary exponentiation of the base polynomial x (pol) into the accumulator 1 (res),
            // all modulo the characteristic polynomial f(x) = x^k - trans[k-1]*x^(k-1) - ... - trans[0].
            // After the loop res == x^n mod f, and a[n] == sum_j res[j] * init[j].
            long* pol = stackalloc long[k]; long* res = stackalloc long[k];
            long* newRes = stackalloc long[2 * k];
            for (int i = 0; i < k; i++) { pol[i] = 0; res[i] = 0; }
            res[0] = 1; pol[1] = 1;
            long exp = n;
            while (exp > 0)
            {
                if ((exp & 1) == 1) PerformKitamasaStep(k, res, pol, trans, mod, newRes);
                if (exp > 1) PerformKitamasaStep(k, pol, pol, trans, mod, newRes);
                exp >>= 1;
            }
            return ComputeResult(k, res, init, mod);
        }

        private static void PerformKitamasaStep(int k, long* a, long* b, long* trans, long mod, long* newRes)
        {
            int width = 2 * k;
            for (int i = 0; i < width; i++) newRes[i] = 0;
            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++) newRes[i + j] = (newRes[i + j] + (a[i] % mod) * (b[j] % mod)) % mod;

            for (int i = width - 1; i >= k; i--)
                for (int j = 1; j <= k; j++) newRes[i - j] = (newRes[i - j] + (newRes[i] % mod) * (trans[k - j] % mod)) % mod;

            for (int i = 0; i < k; i++) a[i] = newRes[i];
        }

        private static long ComputeResult(int k, long* res, long* init, long mod)
        {
            long ans = 0;
            for (int i = 0; i < k; i++) ans = (ans + (res[i] % mod) * (init[i] % mod)) % mod;
            return ans;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModMul(long a, long b, long mod)
        {
            return (a % mod) * (b % mod) % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1; b %= mod;
            while (e > 0)
            {
                if ((e & 1) == 1) r = r * b % mod;
                b = b * b % mod;
                e >>= 1;
            }
            return r;
        }
    }
}
