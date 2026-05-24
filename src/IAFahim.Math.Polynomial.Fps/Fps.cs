namespace IAFahim.Math.Polynomial.Fps
{
    using System;
    using System.Runtime.CompilerServices;

    internal static unsafe class FpsShared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0) { long t = a / b; a -= t * b; long tmp = a; a = b; b = tmp; u -= t * v; tmp = u; u = v; v = tmp; }
            return (u % m + m) % m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod, b = a % mod;
            while (e > 0) { if ((e & 1) == 1) res = res * b % mod; b = b * b % mod; e >>= 1; }
            return res;
        }
    }

    public static unsafe class FormalPowerSeriesInverse
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n <= 0) return 0;
            long invA0 = FpsShared.ModInverse(a[0], mod); res[0] = invA0;
            for (int i = 1; i < n; i++) res[i] = (mod - ComputeInverseSum(i, a, res, mod)) * invA0 % mod;
            return n;
        }

        private static long ComputeInverseSum(int i, long* a, long* res, long mod)
        {
            long sum = 0;
            for (int j = 1; j <= i; j++) sum = (sum + a[j] * res[i - j]) % mod;
            return sum;
        }
    }

    public static unsafe class FormalPowerSeriesLog
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n <= 0) return 0;
            res[0] = 0; long invA0 = FpsShared.ModInverse(a[0], mod);
            for (int i = 1; i < n; i++)
            {
                long sum = ComputeLogSum(i, a, res, mod);
                long val = (i * a[i] % mod - sum + mod) % mod;
                res[i] = val * invA0 % mod * FpsShared.ModInverse(i, mod) % mod;
            }
            return n;
        }

        private static long ComputeLogSum(int i, long* a, long* res, long mod)
        {
            long sum = 0;
            for (int j = 1; j < i; j++) sum = (sum + (j * res[j] % mod) * a[i - j]) % mod;
            return sum;
        }
    }

    public static unsafe class FormalPowerSeriesExp
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n <= 0) return 0;
            res[0] = 1;
            for (int i = 1; i < n; i++)
            {
                long sum = ComputeExpSum(i, a, res, mod);
                res[i] = sum * FpsShared.ModInverse(i, mod) % mod;
            }
            return n;
        }

        private static long ComputeExpSum(int i, long* a, long* res, long mod)
        {
            long sum = 0;
            for (int j = 1; j <= i; j++) sum = (sum + (j * a[j] % mod) * res[i - j]) % mod;
            return sum;
        }
    }

    public static unsafe class FormalPowerSeriesPow
    {
        public static int Run(int n, long* a, long k, long* res, long mod)
        {
            if (k == 0) { if (n > 0) res[0] = 1; for (int i = 1; i < n; i++) res[i] = 0; return n; }
            int f = 0; while (f < n && a[f] == 0) f++;
            if (f >= n || (f > 0 && k >= n) || f * k >= n) { for (int i = 0; i < n; i++) res[i] = 0; return n; }
            
            int newN = n - (int)(f * k);
            long* s = stackalloc long[newN]; for (int i = 0; i < newN; i++) s[i] = a[i + f];
            long invF = FpsShared.ModInverse(s[0], mod), powF = FpsShared.FastPow(a[f], k, mod);
            for (int i = 0; i < newN; i++) s[i] = s[i] * invF % mod;
            
            long* ln = stackalloc long[newN]; FormalPowerSeriesLog.Run(newN, s, ln, mod);
            for (int i = 0; i < newN; i++) ln[i] = ln[i] * k % mod;
            
            long* ex = stackalloc long[newN]; FormalPowerSeriesExp.Run(newN, ln, ex, mod);
            for (int i = 0; i < (int)(f * k); i++) res[i] = 0;
            for (int i = 0; i < newN; i++) res[(int)(f * k) + i] = ex[i] * powF % mod;
            return n;
        }
    }
}
