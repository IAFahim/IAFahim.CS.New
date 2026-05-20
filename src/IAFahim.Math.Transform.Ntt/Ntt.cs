namespace IAFahim.Math.Transform.Ntt
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class NttInit
    {
        public static void Run(int logN, long mod, long g, long* roots, long* invRoots)
        {
            int n = 1 << logN;
            long root = 1;
            long gPow = FastPow(g, (mod - 1) / n, mod);
            for (int i = 0; i < n; i++)
            {
                roots[i] = root;
                root = root * gPow % mod;
            }
            for (int i = 0; i < n; i++)
                invRoots[i] = FastPow(roots[i], mod - 2, mod);
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

    public static unsafe class NttTransform
    {
        public static void Forward(long* a, int n, long mod, long* roots)
        {
            for (int i = 1, j = 0; i < n; i++)
            {
                int bit = n >> 1;
                while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
                j ^= bit;
                if (i < j) { long tmp = a[i]; a[i] = a[j]; a[j] = tmp; }
            }
            for (int len = 2; len <= n; len <<= 1)
            {
                int half = len >> 1;
                long w = roots[n / len];
                for (int i = 0; i < n; i += len)
                {
                    long wn = 1;
                    for (int j = 0; j < half; j++)
                    {
                        long u = a[i + j];
                        long v = a[i + j + half] * wn % mod;
                        a[i + j] = (u + v) % mod;
                        a[i + j + half] = (u - v + mod) % mod;
                        wn = wn * w % mod;
                    }
                }
            }
        }

        public static void Inverse(long* a, int n, long mod, long* invRoots)
        {
            Forward(a, n, mod, invRoots);
            long invN = ModInverse(n, mod);
            for (int i = 0; i < n; i++)
                a[i] = a[i] * invN % mod;
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
    }

    public static unsafe class NttConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod, long g)
        {
            int size = 1;
            while (size < n + m - 1) size <<= 1;
            long* roots = stackalloc long[size];
            long* invRoots = stackalloc long[size];
            NttInit.Run(0, mod, g, roots, invRoots);
            long* fa = stackalloc long[size];
            long* fb = stackalloc long[size];
            for (int i = 0; i < n; i++) fa[i] = a[i];
            for (int i = n; i < size; i++) fa[i] = 0;
            for (int i = 0; i < m; i++) fb[i] = b[i];
            for (int i = m; i < size; i++) fb[i] = 0;
            long root = FastPow(g, (mod - 1) / size, mod);
            long cur = 1;
            for (int i = 0; i < size; i++) { roots[i] = cur; cur = cur * root % mod; }
            NttTransform.Forward(fa, size, mod, roots);
            NttTransform.Forward(fb, size, mod, roots);
            for (int i = 0; i < size; i++)
                fa[i] = fa[i] * fb[i] % mod;
            long invRoot = FastPow(root, mod - 2, mod);
            cur = 1;
            for (int i = 0; i < size; i++) { invRoots[i] = cur; cur = cur * invRoot % mod; }
            NttTransform.Inverse(fa, size, mod, invRoots);
            for (int i = 0; i < n + m - 1; i++) res[i] = fa[i];
            return n + m - 1;
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