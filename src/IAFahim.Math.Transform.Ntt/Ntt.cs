namespace IAFahim.Math.Transform.Ntt
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class NttInit
    {
        public static void Run(int logN, long mod, long g, long* roots, long* invRoots)
        {
            int n = 1 << logN; long gPow = FastPow(g, (mod - 1) / n, mod);
            long root = 1; for (int i = 0; i < n; i++) { roots[i] = root; root = root * gPow % mod; }
            for (int i = 0; i < n; i++) invRoots[i] = FastPow(roots[i], mod - 2, mod);
        }

        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod, b = a % mod;
            while (e > 0) { if ((e & 1) == 1) res = res * b % mod; b = b * b % mod; e >>= 1; }
            return res;
        }
    }

    public static unsafe class NttTransform
    {
        public static void Forward(long* a, int n, long mod, long* roots)
        {
            BitReverse(a, n);
            for (int len = 2; len <= n; len <<= 1)
            {
                int half = len >> 1; long w = roots[n / len];
                for (int i = 0; i < n; i += len) PerformButterflyStep(a, i, half, w, mod);
            }
        }

        private static void PerformButterflyStep(long* a, int i, int half, long w, long mod)
        {
            long wn = 1;
            for (int j = 0; j < half; j++)
            {
                long u = a[i + j], v = a[i + j + half] * wn % mod;
                a[i + j] = (u + v) % mod; a[i + j + half] = (u - v + mod) % mod;
                wn = wn * w % mod;
            }
        }

        public static void Inverse(long* a, int n, long mod, long* invRoots)
        {
            Forward(a, n, mod, invRoots);
            long invN = ModInverse(n, mod);
            for (int i = 0; i < n; i++) a[i] = a[i] * invN % mod;
        }

        private static void BitReverse(long* a, int n)
        {
            for (int i = 1, j = 0; i < n; i++)
            {
                int bit = n >> 1;
                while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
                j ^= bit;
                if (i < j) { long tmp = a[i]; a[i] = a[j]; a[j] = tmp; }
            }
        }

        private static long ModInverse(long a, long mod)
        {
            long b = mod, u = 1, v = 0;
            while (b != 0) { long t = a / b; a -= t * b; long tmp = a; a = b; b = tmp; u -= t * v; tmp = u; u = v; v = tmp; }
            return (u % mod + mod) % mod;
        }
    }

    public static unsafe class NttConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod, long g)
        {
            int size = 1; while (size < n + m - 1) size <<= 1;
            long* fa = stackalloc long[size], fb = stackalloc long[size];
            for (int i = 0; i < size; i++) { fa[i] = i < n ? a[i] : 0; fb[i] = i < m ? b[i] : 0; }
            
            long root = FastPow(g, (mod - 1) / size, mod);
            long* roots = stackalloc long[size]; long cur = 1;
            for (int i = 0; i < size; i++) { roots[i] = cur; cur = cur * root % mod; }
            
            NttTransform.Forward(fa, size, mod, roots);
            NttTransform.Forward(fb, size, mod, roots);
            for (int i = 0; i < size; i++) fa[i] = fa[i] * fb[i] % mod;
            
            long invRoot = FastPow(root, mod - 2, mod);
            long* invRoots = stackalloc long[size]; cur = 1;
            for (int i = 0; i < size; i++) { invRoots[i] = cur; cur = cur * invRoot % mod; }
            
            NttTransform.Inverse(fa, size, mod, invRoots);
            for (int i = 0; i < n + m - 1; i++) res[i] = fa[i];
            return n + m - 1;
        }

        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod, b = a % mod;
            while (e > 0) { if ((e & 1) == 1) res = res * b % mod; b = b * b % mod; e >>= 1; }
            return res;
        }
    }
}
