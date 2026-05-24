namespace IAFahim.Math.Transform.AnyMod
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ArbitraryModConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod)
        {
            const long MOD1 = 167772161, MOD2 = 469762049, MOD3 = 1224736769;
            long* r1 = stackalloc long[n + m], r2 = stackalloc long[n + m], r3 = stackalloc long[n + m];
            
            ConvolveWithMod(a, n, b, m, r1, MOD1);
            ConvolveWithMod(a, n, b, m, r2, MOD2);
            ConvolveWithMod(a, n, b, m, r3, MOD3);

            return CombineCrt(n + m - 1, r1, r2, r3, res, mod);
        }

        private static void ConvolveWithMod(long* a, int n, long* b, int m, long* r, long mod)
        {
            long* ta = stackalloc long[n], tb = stackalloc long[m];
            for (int i = 0; i < n; i++) ta[i] = a[i] % mod;
            for (int i = 0; i < m; i++) tb[i] = b[i] % mod;
            NttConvolutionSimple(ta, n, tb, m, r, mod);
        }

        private static int CombineCrt(int len, long* r1, long* r2, long* r3, long* res, long mod)
        {
            const long MOD1 = 167772161, MOD2 = 469762049, MOD3 = 1224736769;
            long m12 = MOD1 * MOD2, invM1Mod2 = ModInverse(MOD1 % MOD2, MOD2), invM12Mod3 = ModInverse(m12 % MOD3, MOD3);
            for (int i = 0; i < len; i++)
            {
                long x1 = r1[i];
                long x2 = (r2[i] - x1) * invM1Mod2 % MOD2; if (x2 < 0) x2 += MOD2;
                long x3 = (r3[i] - x1 - (m12 % MOD3) * (x2 % MOD3)) * invM12Mod3 % MOD3; if (x3 < 0) x3 += MOD3;
                long x = x1 + MOD1 * x2 + m12 * x3;
                res[i] = ((x % mod) + mod) % mod;
            }
            return len;
        }

        private static void NttConvolutionSimple(long* a, int n, long* b, int m, long* res, long mod)
        {
            int size = 1; while (size < n + m - 1) size <<= 1;
            long* fa = stackalloc long[size], fb = stackalloc long[size];
            InitializeNttArrays(size, n, a, fa, m, b, fb);

            long g = 3, root = FastPow(g, (mod - 1) / size, mod);
            long* roots = stackalloc long[size]; roots[0] = 1;
            for (int i = 1; i < size; i++) roots[i] = roots[i - 1] * root % mod;

            PerformNtt(fa, size, mod, roots);
            PerformNtt(fb, size, mod, roots);
            for (int i = 0; i < size; i++) fa[i] = fa[i] * fb[i] % mod;
            PerformInverseNtt(fa, size, mod, root);

            for (int i = 0; i < n + m - 1; i++) res[i] = fa[i];
        }

        private static void InitializeNttArrays(int size, int n, long* a, long* fa, int m, long* b, long* fb)
        {
            for (int i = 0; i < n; i++) fa[i] = a[i]; for (int i = n; i < size; i++) fa[i] = 0;
            for (int i = 0; i < m; i++) fb[i] = b[i]; for (int i = m; i < size; i++) fb[i] = 0;
        }

        private static void PerformNtt(long* a, int n, long mod, long* roots)
        {
            BitReverse(a, n);
            for (int len = 2; len <= n; len <<= 1)
            {
                int half = len >> 1; long w = roots[n / len];
                for (int i = 0; i < n; i += len)
                {
                    long wn = 1;
                    for (int j = 0; j < half; j++)
                    {
                        long u = a[i + j], v = a[i + j + half] * wn % mod;
                        a[i + j] = (u + v) % mod; a[i + j + half] = (u - v + mod) % mod;
                        wn = wn * w % mod;
                    }
                }
            }
        }

        private static void PerformInverseNtt(long* a, int n, long mod, long root)
        {
            long invRoot = FastPow(root, mod - 2, mod);
            long* invRoots = stackalloc long[n]; invRoots[0] = 1;
            for (int i = 1; i < n; i++) invRoots[i] = invRoots[i - 1] * invRoot % mod;
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

        private static long ModInverse(long a, long mod)
        {
            long b = mod, u = 1, v = 0;
            while (b > 0) { long t = a / b; a -= t * b; long tmp = a; a = b; b = tmp; u -= t * v; tmp = u; u = v; v = tmp; }
            u %= mod; if (u < 0) u += mod;
            return u;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1; long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1; y = x1 - (a / b) * y1; return g;
        }

        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod, b = a % mod;
            while (e > 0) { if ((e & 1) == 1) res = res * b % mod; b = b * b % mod; e >>= 1; }
            return res;
        }
    }
}