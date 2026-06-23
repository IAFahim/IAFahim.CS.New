namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GeneratingFunction
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EgfMultiply(long* a, long* b, int n, int MOD, long* result)
        {
            for (int i = 0; i < n; i++) result[i] = 0L;
            if (n <= 0) return;

            long mod = (long)MOD;

            // Precompute factorials and inverse factorials over [0, n-1] once.
            long* fact = stackalloc long[n];
            long* invfact = stackalloc long[n];
            fact[0] = 1L;
            for (int i = 1; i < n; i++) fact[i] = (fact[i - 1] * (long)i) % mod;
            invfact[n - 1] = Combinatorial.ModPow(fact[n - 1], mod - 2L, mod);
            for (int i = n - 1; i > 0; i--) invfact[i - 1] = (invfact[i] * (long)i) % mod;

            for (int i = 0; i < n; i++)
            for (int j = 0; j < n - i; j++)
            {
                // Binom(i+j, i) = (i+j)! / (i! * j!)  (mod prime MOD)
                long bin = ((fact[i + j] * invfact[i]) % mod * invfact[j]) % mod;
                result[i + j] = (result[i + j] + ((a[i] * b[j]) % mod) * bin) % mod;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OgfMultiply(long* a, long* b, int n, int MOD, long* result)
        {
            for (int i = 0; i < n; i++) result[i] = 0L;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n - i; j++)
                result[i + j] = (result[i + j] + a[i] * b[j]) % (long)MOD;
        }

        // Returns the k-th coefficient g_k of the compositional inverse g of f,
        // where f(x) = f[1]*x + f[2]*x^2 + ... (f[0] = 0, f[1] invertible mod MOD).
        // Lagrange inversion: g_k = (1/k) * [x^{k-1}] (x/f(x))^k, computed modulo x^k.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildInverseSeries(long* f, int k, long mod, long* h)
        {
            long invPhi0 = Combinatorial.ModPow(f[1], mod - 2L, mod);
            h[0] = invPhi0;
            for (int i = 1; i < k; i++)
            {
                long acc = 0L;
                for (int j = 1; j <= i; j++)
                    acc = (acc + f[j + 1] * h[i - j]) % mod;
                h[i] = (mod - (invPhi0 * acc) % mod) % mod;
            }
        }

        public static long LagrangeInversion(long* f, int n, int k, int MOD)
        {
            if (k <= 0) return 0L;

            long mod = (long)MOD;

            long* h = stackalloc long[k];
            BuildInverseSeries(f, k, mod, h);

            // pw(x) = h(x)^k modulo x^k via binary exponentiation of truncated products.
            long* pw = stackalloc long[k];
            long* baseP = stackalloc long[k];
            long* tmp = stackalloc long[k];
            for (int i = 0; i < k; i++) { pw[i] = 0L; baseP[i] = h[i]; }
            pw[0] = 1L;

            int e = k;
            while (e > 0)
            {
                if ((e & 1) != 0)
                {
                    TruncMul(pw, baseP, tmp, k, mod);
                    for (int i = 0; i < k; i++) pw[i] = tmp[i];
                }
                e >>= 1;
                if (e > 0)
                {
                    TruncMul(baseP, baseP, tmp, k, mod);
                    for (int i = 0; i < k; i++) baseP[i] = tmp[i];
                }
            }

            long invK = Combinatorial.ModPow((long)k, mod - 2L, mod);
            return (pw[k - 1] * invK) % mod;
        }

        // dst = (a * b) modulo x^len, all coefficients reduced mod 'mod'. dst must differ from a and b.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TruncMul(long* a, long* b, long* dst, int len, long mod)
        {
            for (int i = 0; i < len; i++) dst[i] = 0L;
            for (int i = 0; i < len; i++)
            {
                long ai = a[i];
                if (ai == 0L) continue;
                for (int j = 0; j < len - i; j++)
                    dst[i + j] = (dst[i + j] + ai * b[j]) % mod;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TreeCount(int n, int MOD)
        {
            if (n <= 1) return 1L;
            long result = Combinatorial.ModPow((long)n, (long)(n - 2), (long)MOD);
            return result;
        }
    }
}