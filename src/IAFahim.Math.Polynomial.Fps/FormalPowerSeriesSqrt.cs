namespace IAFahim.Math.Polynomial.Fps
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FormalPowerSeriesSqrt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n <= 0) return 0;
            long r0 = TonelliShanks(a[0], mod);
            if (r0 < 0) return -1;
            if (r0 == 0)
            {
                for (int i = 0; i < n; i++)
                {
                    if (a[i] != 0) return -1;
                }
                for (int i = 0; i < n; i++) res[i] = 0;
                return n;
            }
            res[0] = r0;
            long inv2Res0 = FpsShared.ModInverse(2 * r0 % mod, mod);
            for (int i = 1; i < n; i++)
            {
                long sum = ComputeSqrtSum(i, res, mod);
                res[i] = (a[i] - sum + mod) % mod * inv2Res0 % mod;
            }
            return n;
        }

        private static long ComputeSqrtSum(int i, long* res, long mod)
        {
            long sum = 0;
            for (int j = 1; j < i; j++) sum = (sum + res[j] * res[i - j]) % mod;
            return sum;
        }

        private static long TonelliShanks(long n, long p)
        {
            if (n == 0) return 0;
            if (p == 2) return n & 1;
            if (LegendresSymbol(n, p) != 1) return -1;
            if (p % 4 == 3) return FpsShared.FastPow(n, (p + 1) / 4, p);

            InitializeTonelliShanks(p, out long q, out int s, out long z);
            return SolveTonelliShanks(n, p, q, s, z);
        }

        private static void InitializeTonelliShanks(long p, out long q, out int s, out long z)
        {
            q = p - 1; s = 0; while ((q & 1) == 0) { q >>= 1; s++; }
            z = 2; while (LegendresSymbol(z, p) != -1) z++;
        }

        private static long SolveTonelliShanks(long n, long p, long q, int s, long z)
        {
            long c = FpsShared.FastPow(z, q, p), r = FpsShared.FastPow(n, (q + 1) / 2, p), t = FpsShared.FastPow(n, q, p);
            int m = s;
            while (t != 1)
            {
                int i = FindFirstOne(t, p, m);
                if (i == -1) return -1;
                long b = FpsShared.FastPow(c, 1L << (m - i - 1), p);
                r = r * b % p; c = b * b % p; t = t * c % p; m = i;
            }
            return r;
        }

        private static int FindFirstOne(long t, long p, int m)
        {
            long t2 = t;
            for (int i = 0; i < m; i++) { if (t2 == 1) return i; t2 = t2 * t2 % p; }
            return -1;
        }

        private static long LegendresSymbol(long a, long p)
        {
            long r = FpsShared.FastPow(a, (p - 1) / 2, p);
            return r == p - 1 ? -1 : r;
        }
    }
}
