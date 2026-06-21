namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubsetConvolutionRanked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopCount(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ZeroBuffers(long* f, long* g, long* h, long totalSize)
        {
            for (long i = 0; i < totalSize; i++) { f[i] = 0; g[i] = 0; h[i] = 0; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ScatterRanked(long* a, long* b, long* f, long* g, int n, long mod)
        {
            for (int mask = 0; mask < n; mask++)
            {
                int pc = PopCount(mask);
                f[(long)pc * n + mask] = a[mask] % mod;
                g[(long)pc * n + mask] = b[mask] % mod;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyForwardSos(long* f, long* g, int n, int logN, long mod)
        {
            int numRanks = logN + 1;
            for (int r = 0; r < numRanks; r++)
            {
                long* fRow = f + (long)r * n;
                long* gRow = g + (long)r * n;
                for (int i = 0; i < logN; i++)
                    for (int mask = 0; mask < n; mask++)
                        if ((mask & (1 << i)) != 0)
                        {
                            fRow[mask] = (fRow[mask] + fRow[mask ^ (1 << i)]) % mod;
                            gRow[mask] = (gRow[mask] + gRow[mask ^ (1 << i)]) % mod;
                        }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ConvolveRanks(long* f, long* g, long* h, int n, int logN, long mod)
        {
            int numRanks = logN + 1;
            for (int mask = 0; mask < n; mask++)
                for (int i = 0; i < numRanks; i++)
                {
                    long sum = 0;
                    for (int j = 0; j <= i; j++)
                    {
                        long prod = MulMod(f[(long)j * n + mask] % mod, g[(long)(i - j) * n + mask] % mod, mod);
                        sum = (sum + prod) % mod;
                    }
                    h[(long)i * n + mask] = sum;
                }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyInverseSos(long* h, int n, int logN, long mod)
        {
            int numRanks = logN + 1;
            for (int r = 0; r < numRanks; r++)
            {
                long* hRow = h + (long)r * n;
                for (int i = 0; i < logN; i++)
                    for (int mask = 0; mask < n; mask++)
                        if ((mask & (1 << i)) != 0)
                            hRow[mask] = (hRow[mask] - hRow[mask ^ (1 << i)] + mod) % mod;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GatherRanked(long* c, long* h, int n)
        {
            for (int mask = 0; mask < n; mask++)
            {
                int pc = PopCount(mask);
                c[mask] = h[(long)pc * n + mask];
            }
        }

        public static void Run(long* a, long* b, long* c, int logN, long mod, long* f, long* g, long* h)
        {
            int n = 1 << logN;
            int numRanks = logN + 1;
            long totalSize = (long)numRanks * n;
            ZeroBuffers(f, g, h, totalSize);
            ScatterRanked(a, b, f, g, n, mod);
            ApplyForwardSos(f, g, n, logN, mod);
            ConvolveRanks(f, g, h, n, logN, mod);
            ApplyInverseSos(h, n, logN, mod);
            GatherRanked(c, h, n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long MulMod(long a, long b, long mod)
        {
            long result = 0;
            a %= mod;
            if (a < 0) a += mod;
            while (b > 0)
            {
                if ((b & 1) != 0)
                {
                    result += a;
                    if (result >= mod) result -= mod;
                }
                a <<= 1;
                if (a >= mod) a -= mod;
                b >>= 1;
            }
            return result;
        }
    }
}
