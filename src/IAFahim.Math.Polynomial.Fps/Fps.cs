namespace IAFahim.Math.Polynomial.Fps
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FormalPowerSeriesInverse
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n <= 0) return 0;
            long invA0 = ModInverse(a[0], mod);
            res[0] = invA0;
            for (int i = 1; i < n; i++)
            {
                long sum = 0;
                for (int j = 1; j <= i; j++)
                {
                    sum = (sum + a[j] * res[i - j]) % mod;
                }
                res[i] = (mod - sum) * invA0 % mod;
            }
            return n;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
        }
    }

    public static unsafe class FormalPowerSeriesLog
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n <= 0) return 0;
            res[0] = 0;
            long invA0 = ModInverse(a[0], mod);
            for (int i = 1; i < n; i++)
            {
                long sum = 0;
                for (int j = 1; j < i; j++)
                {
                    sum = (sum + (j * res[j] % mod) * a[i - j]) % mod;
                }
                long val = (i * a[i] % mod - sum + mod) % mod;
                val = val * invA0 % mod;
                res[i] = val * ModInverse(i, mod) % mod;
            }
            return n;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
        }
    }

    public static unsafe class FormalPowerSeriesExp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n <= 0) return 0;
            res[0] = 1;
            for (int i = 1; i < n; i++)
            {
                long sum = 0;
                for (int j = 1; j <= i; j++)
                {
                    sum = (sum + (j * a[j] % mod) * res[i - j]) % mod;
                }
                res[i] = sum * ModInverse(i, mod) % mod;
            }
            return n;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
        }
    }

    public static unsafe class FormalPowerSeriesPow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, long k, long* res, long mod)
        {
            if (k == 0)
            {
                if (n > 0) res[0] = 1;
                for (int i = 1; i < n; i++) res[i] = 0;
                return n;
            }
            int firstNonZero = 0;
            while (firstNonZero < n && a[firstNonZero] == 0) firstNonZero++;
            if (firstNonZero >= n)
            {
                for (int i = 0; i < n; i++) res[i] = 0;
                return n;
            }
            if (firstNonZero * k >= n)
            {
                for (int i = 0; i < n; i++) res[i] = 0;
                return n;
            }
            int newN = n - (int)(firstNonZero * k);
            long* shifted = stackalloc long[newN];
            for (int i = 0; i < newN; i++) shifted[i] = a[i + firstNonZero];
            long invFirst = ModInverse(shifted[0], mod);
            long powFirst = FastPow(a[firstNonZero], k, mod);
            for (int i = 0; i < newN; i++) shifted[i] = shifted[i] * invFirst % mod;
            long* lnShifted = stackalloc long[newN];
            FormalPowerSeriesLog.Run(newN, shifted, lnShifted, mod);
            for (int i = 0; i < newN; i++) lnShifted[i] = lnShifted[i] * k % mod;
            long* expLn = stackalloc long[newN];
            FormalPowerSeriesExp.Run(newN, lnShifted, expLn, mod);
            for (int i = 0; i < newN; i++) expLn[i] = expLn[i] * powFirst % mod;
            for (int i = 0; i < (int)(firstNonZero * k); i++) res[i] = 0;
            for (int i = 0; i < newN; i++) res[(int)(firstNonZero * k) + i] = expLn[i];
            return n;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
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

    public static unsafe class FormalPowerSeriesSqrt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n <= 0) return 0;
            long r0 = TonelliShanks(a[0], mod);
            if (r0 < 0) return -1;
            res[0] = r0;
            long inv2Res0 = ModInverse(2 * r0 % mod, mod);
            for (int i = 1; i < n; i++)
            {
                long sum = 0;
                for (int j = 1; j < i; j++)
                {
                    sum = (sum + res[j] * res[i - j]) % mod;
                }
                res[i] = (a[i] - sum + mod) % mod * inv2Res0 % mod;
            }
            return n;
        }

        private static long TonelliShanks(long n, long p)
        {
            if (n == 0) return 0;
            if (p == 2) return n & 1;
            if (LegendresSymbol(n, p) != 1) return -1;
            if (p % 4 == 3) return FastPow(n, (p + 1) / 4, p);
            long s = p - 1;
            int q = 0;
            while ((s & 1) == 0) { s >>= 1; q++; }
            long z = 2;
            while (LegendresSymbol(z, p) != -1) z++;
            long c = FastPow(z, s, p);
            long r = FastPow(n, (s + 1) / 2, p);
            long t = FastPow(n, s, p);
            int m = q;
            while (t != 1)
            {
                long t2 = t;
                int i = 0;
                for (; i < m; i++)
                {
                    if (t2 == 1) break;
                    t2 = t2 * t2 % p;
                }
                if (i >= m) return -1;
                long b = FastPow(c, 1L << (m - i - 1), p);
                r = r * b % p;
                c = b * b % p;
                t = t * c % p;
                m = i;
            }
            return r;
        }

        private static long LegendresSymbol(long a, long p)
        {
            long r = FastPow(a, (p - 1) / 2, p);
            return r == p - 1 ? -1 : r;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
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
