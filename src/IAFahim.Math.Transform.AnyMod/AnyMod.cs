namespace IAFahim.Math.Transform.AnyMod
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ArbitraryModConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod)
        {
            const long MOD1 = 167772161, MOD2 = 469762049, MOD3 = 1224736769;
            long* r1 = stackalloc long[n + m - 1], r2 = stackalloc long[n + m - 1], r3 = stackalloc long[n + m - 1];
            
            ConvolveWithMod(a, n, b, m, r1, MOD1);
            ConvolveWithMod(a, n, b, m, r2, MOD2);
            ConvolveWithMod(a, n, b, m, r3, MOD3);

            return CombineCrt(n + m - 1, r1, r2, r3, res, mod);
        }

        private static void ConvolveWithMod(long* a, int n, long* b, int m, long* r, long mod)
        {
            // The per-input modular reduction is fused into InitializeNttArrays' zero-padded copy,
            // so no separate ta/tb scratch is needed.
            NttConvolutionSimple(a, n, b, m, r, mod);
        }

        private static int CombineCrt(int len, long* r1, long* r2, long* r3, long* res, long mod)
        {
            const long MOD1 = 167772161, MOD2 = 469762049, MOD3 = 1224736769;
            long m12 = MOD1 * MOD2, invM1Mod2 = ModInverse(MOD1 % MOD2, MOD2), invM12Mod3 = ModInverse(m12 % MOD3, MOD3);
            // Garner mixed-radix digit for the M3 term: a3 = (r3 - a1 - MOD1*a2) * inv(MOD1*MOD2, MOD3).
            // The subtracted coefficient is MOD1 (the radix already accumulated into a2), NOT m12.
            long mod1ModM3 = MOD1 % MOD3;
            // Reduce the constants modulo `mod` once; the per-element products are formed with a
            // 64-bit-safe MulMod so the full CRT value (which spans up to ~85 bits) never has to be
            // materialized in a single Int64 (which would overflow for true coefficients > ~9.2e18).
            long mod1ModM = MOD1 % mod; if (mod1ModM < 0) mod1ModM += mod;
            long m12ModM = m12 % mod; if (m12ModM < 0) m12ModM += mod;
            for (int i = 0; i < len; i++)
            {
                long x1 = r1[i];
                long x2 = (r2[i] - x1) * invM1Mod2 % MOD2; if (x2 < 0) x2 += MOD2;
                // Reduce the inner difference mod MOD3 before the inverse multiply so neither the
                // mod1ModM3*x2 product nor the *invM12Mod3 product can overflow Int64.
                long t3 = (r3[i] - x1 - mod1ModM3 * (x2 % MOD3)) % MOD3;
                long x3 = t3 * invM12Mod3 % MOD3; if (x3 < 0) x3 += MOD3;
                // x = x1 + MOD1*x2 + m12*x3  (mod `mod`), accumulated without 64-bit overflow.
                long x = x1 % mod; if (x < 0) x += mod;
                x = AddMod(x, MulMod(mod1ModM, x2 % mod, mod), mod);
                x = AddMod(x, MulMod(m12ModM, x3 % mod, mod), mod);
                res[i] = x;
            }
            return len;
        }

        private static void NttConvolutionSimple(long* a, int n, long* b, int m, long* res, long mod)
        {
            int size = 1; while (size < n + m - 1) size <<= 1;
            long* fa = stackalloc long[size], fb = stackalloc long[size];
            InitializeNttArrays(size, n, a, fa, m, b, fb, mod);

            long g = 3, root = FastPow(g, (mod - 1) / size, mod);
            long* roots = stackalloc long[size]; roots[0] = 1;
            for (int i = 1; i < size; i++) roots[i] = roots[i - 1] * root % mod;

            PerformNtt(fa, size, mod, roots);
            PerformNtt(fb, size, mod, roots);
            for (int i = 0; i < size; i++) fa[i] = fa[i] * fb[i] % mod;
            PerformInverseNtt(fa, size, mod, roots);

            for (int i = 0; i < n + m - 1; i++) res[i] = fa[i];
        }

        private static void InitializeNttArrays(int size, int n, long* a, long* fa, int m, long* b, long* fb, long mod)
        {
            // Fuse the per-input modular reduction with the zero-padded copy (single pass each).
            // The NTT moduli are < 2^31; a[i] % mod for long inputs yields the same residue the NTT
            // butterflies operate on. Inputs into the NTT are assumed non-negative by the library
            // contract, matching the prior ta/tb behaviour (which also used a plain %).
            for (int i = 0; i < n; i++) fa[i] = a[i] % mod; for (int i = n; i < size; i++) fa[i] = 0;
            for (int i = 0; i < m; i++) fb[i] = b[i] % mod; for (int i = m; i < size; i++) fb[i] = 0;
        }

        private static void PerformNtt(long* a, int n, long mod, long* roots)
        {
            BitReverse(a, n);
            for (int len = 2; len <= n; len <<= 1)
            {
                int half = len >> 1, step = n / len;
                for (int i = 0; i < n; i += len)
                {
                    for (int j = 0; j < half; j++)
                    {
                        long wn = roots[j * step];
                        long u = a[i + j], v = a[i + j + half] * wn % mod;
                        a[i + j] = (u + v) % mod; a[i + j + half] = (u - v + mod) % mod;
                    }
                }
            }
        }

        private static void PerformInverseNtt(long* a, int n, long mod, long* roots)
        {
            // Inverse twiddles are the forward roots in reverse: invRoot^i == root^(n-i) == roots[n-i].
            // This avoids a modular exponentiation (FastPow(root, mod-2)) plus an n-length power chain.
            long* invRoots = stackalloc long[n]; invRoots[0] = roots[0];
            for (int i = 1; i < n; i++) invRoots[i] = roots[n - i];
            PerformNtt(a, n, mod, invRoots);
            long invN = FastPow(n, mod - 2, mod);
            for (int i = 0; i < n; i++) a[i] = a[i] * invN % mod;
        }

        private static void BitReverse(long* a, int n)
        {
            for (int i = 1, j = 0; i < n; i++)
            {
                int bit = n >> 1;
                while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
                j ^= bit;
                if (i < j) { long t = a[i]; a[i] = a[j]; a[j] = t; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModInverse(long a, long mod)
        {
            long b = mod, u = 1, v = 0;
            while (b > 0) { long t = a / b; a -= t * b; long tmp = a; a = b; b = tmp; u -= t * v; tmp = u; u = v; v = tmp; }
            u %= mod; if (u < 0) u += mod;
            return u;
        }

        // Adds two non-negative residues a,b in [0,mod) without overflow, assuming mod < 2^62.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long AddMod(long a, long b, long mod)
        {
            long s = a + b;
            if (s >= mod) s -= mod;
            return s;
        }

        // Computes (a*b) % mod for non-negative a,b in [0,mod) without 64-bit overflow,
        // using binary (shift-add) multiplication. Valid for mod < 2^62.
        private static long MulMod(long a, long b, long mod)
        {
            long result = 0;
            while (b > 0)
            {
                if ((b & 1) == 1) result = AddMod(result, a, mod);
                a = AddMod(a, a, mod);
                b >>= 1;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod, b = a % mod;
            while (e > 0) { if ((e & 1) == 1) res = res * b % mod; b = b * b % mod; e >>= 1; }
            return res;
        }
    }
}