namespace IAFahim.Math.Polynomial
{
    using System.Runtime.CompilerServices;

    public static unsafe class OnlineNttConvolution
    {
        public static void Run(
            long* a, int n,
            long* res, int resLen,
            long mod, long g,
            long* work)
        {
            int size = 1;
            while (size < 2 * n) size <<= 1;

            long* fa = work;
            long* fb = work + size;
            long* roots = work + 2 * size;
            long* invRoots = work + 3 * size;

            long root = FastPow(g, (mod - 1) / size, mod);
            long cur = 1;
            for (int i = 0; i < size; i++) { roots[i] = cur; cur = cur * root % mod; }
            long invRoot = FastPow(root, mod - 2, mod);
            cur = 1;
            for (int i = 0; i < size; i++) { invRoots[i] = cur; cur = cur * invRoot % mod; }

            for (int len = 1; len <= resLen; len++)
            {
                int block = 1;
                while (block < len) block <<= 1;

                for (int i = 0; i < size; i++) { fa[i] = 0; fb[i] = 0; }

                for (int i = 0; i < len && i < n; i++) fa[i] = a[i];
                for (int i = 0; i < len && i < n; i++) fb[i] = a[i];

                NttForward(fa, size, mod, roots);
                NttForward(fb, size, mod, roots);
                for (int i = 0; i < size; i++) fa[i] = fa[i] * fb[i] % mod;
                NttInverse(fa, size, mod, invRoots);

                if (len - 1 < resLen)
                    res[len - 1] = len - 1 < size ? fa[len - 1] : 0;
            }
        }

        private static void NttForward(long* a, int n, long mod, long* roots)
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
                    PerformButterfly(a, i, half, w, mod);
                }
            }
        }

        private static void NttInverse(long* a, int n, long mod, long* invRoots)
        {
            NttForward(a, n, mod, invRoots);
            long invN = FastPow(n, mod - 2, mod);
            for (int i = 0; i < n; i++) a[i] = a[i] * invN % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PerformButterfly(long* a, int i, int half, long w, long mod)
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
