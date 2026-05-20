namespace IAFahim.Math.Transform.AnyMod
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ArbitraryModConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod)
        {
            const long MOD1 = 167772161;
            const long MOD2 = 469762049;
            const long MOD3 = 1224736769;
            long* t1 = stackalloc long[n];
            long* t2 = stackalloc long[n];
            long* t3 = stackalloc long[n];
            long* s1 = stackalloc long[m];
            long* s2 = stackalloc long[m];
            long* s3 = stackalloc long[m];
            long* r1 = stackalloc long[n + m];
            long* r2 = stackalloc long[n + m];
            long* r3 = stackalloc long[n + m];
            for (int i = 0; i < n; i++)
            {
                t1[i] = a[i] % MOD1;
                t2[i] = a[i] % MOD2;
                t3[i] = a[i] % MOD3;
            }
            for (int i = 0; i < m; i++)
            {
                s1[i] = b[i] % MOD1;
                s2[i] = b[i] % MOD2;
                s3[i] = b[i] % MOD3;
            }
            int len = n + m - 1;
            NttConvolutionSimple(t1, n, s1, m, r1, MOD1);
            NttConvolutionSimple(t2, n, s2, m, r2, MOD2);
            NttConvolutionSimple(t3, n, s3, m, r3, MOD3);
            long m12 = MOD1 * MOD2;
            long invM1Mod2 = ModInverse(MOD1 % MOD2, MOD2);
            long invM12Mod3 = ModInverse(m12 % MOD3, MOD3);
            for (int i = 0; i < len; i++)
            {
                long x1 = r1[i];
                long x2 = (r2[i] - x1) * invM1Mod2 % MOD2;
                if (x2 < 0) x2 += MOD2;
                long x3 = (r3[i] - x1 - m12 % MOD3 * x2 % MOD3) * invM12Mod3 % MOD3;
                if (x3 < 0) x3 += MOD3;
                long x = x1 + MOD1 * x2 + m12 * x3;
                res[i] = ((x % mod) + mod) % mod;
            }
            return len;
        }

        private static void NttConvolutionSimple(long* a, int n, long* b, int m, long* res, long mod)
        {
            int size = 1;
            while (size < n + m - 1) size <<= 1;
            long* fa = stackalloc long[size];
            long* fb = stackalloc long[size];
            for (int i = 0; i < n; i++) fa[i] = a[i];
            for (int i = n; i < size; i++) fa[i] = 0;
            for (int i = 0; i < m; i++) fb[i] = b[i];
            for (int i = m; i < size; i++) fb[i] = 0;
            long g = 3;
            long root = FastPow(g, (mod - 1) / size, mod);
            long* roots = stackalloc long[size];
            roots[0] = 1;
            for (int i = 1; i < size; i++) roots[i] = roots[i - 1] * root % mod;
            for (int i = 1, j = 0; i < size; i++)
            {
                int bit = size >> 1;
                while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
                j ^= bit;
                if (i < j) { long tmp = fa[i]; fa[i] = fa[j]; fa[j] = tmp; }
            }
            for (int len = 2; len <= size; len <<= 1)
            {
                int half = len >> 1;
                long w = roots[size / len];
                for (int i = 0; i < size; i += len)
                {
                    long wn = 1;
                    for (int j = 0; j < half; j++)
                    {
                        long u = fa[i + j];
                        long v = fa[i + j + half] * wn % mod;
                        fa[i + j] = (u + v) % mod;
                        fa[i + j + half] = (u - v + mod) % mod;
                        wn = wn * w % mod;
                    }
                }
            }
            for (int i = 1, j = 0; i < size; i++)
            {
                int bit = size >> 1;
                while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
                j ^= bit;
                if (i < j) { long tmp = fb[i]; fb[i] = fb[j]; fb[j] = tmp; }
            }
            for (int len = 2; len <= size; len <<= 1)
            {
                int half = len >> 1;
                long w = roots[size / len];
                for (int i = 0; i < size; i += len)
                {
                    long wn = 1;
                    for (int j = 0; j < half; j++)
                    {
                        long u = fb[i + j];
                        long v = fb[i + j + half] * wn % mod;
                        fb[i + j] = (u + v) % mod;
                        fb[i + j + half] = (u - v + mod) % mod;
                        wn = wn * w % mod;
                    }
                }
            }
            for (int i = 0; i < size; i++) fa[i] = fa[i] * fb[i] % mod;
            long invRoot = FastPow(root, mod - 2, mod);
            for (int i = 1, j = 0; i < size; i++)
            {
                int bit = size >> 1;
                while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
                j ^= bit;
                if (i < j) { long tmp = fa[i]; fa[i] = fa[j]; fa[j] = tmp; }
            }
            for (int len = 2; len <= size; len <<= 1)
            {
                int half = len >> 1;
                long w = FastPow(invRoot, (mod - 1) / len, mod);
                for (int i = 0; i < size; i += len)
                {
                    long wn = 1;
                    for (int j = 0; j < half; j++)
                    {
                        long u = fa[i + j];
                        long v = fa[i + j + half] * wn % mod;
                        fa[i + j] = (u + v) % mod;
                        fa[i + j + half] = (u - v + mod) % mod;
                        wn = wn * w % mod;
                    }
                }
            }
            long invN = FastPow(size, mod - 2, mod);
            for (int i = 0; i < n + m - 1; i++) res[i] = fa[i] * invN % mod;
        }

        private static long ModInverse(long a, long mod)
        {
            long b = mod, x = 0, y = 0;
            long g = ExtGcd(a, b, out x, out y);
            if (g != 1) return 1;
            return (x % b + b) % b;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
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