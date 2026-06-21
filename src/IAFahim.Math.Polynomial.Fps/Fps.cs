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
        private const long IdentityExponent = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountLeadingZeros(long* a, int n)
        {
            int f = 0;
            while (f < n && a[f] == 0) f++;
            return f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShiftExceedsLength(int f, int n, long k)
        {
            return f >= n || (f > 0 && k >= n) || f * k >= n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FillZeros(long* res, int n)
        {
            for (int i = 0; i < n; i++) res[i] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteUnitSeries(long* res, int n)
        {
            if (n > 0) res[0] = 1;
            for (int i = 1; i < n; i++) res[i] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void NormalizeShiftedSeries(long* s, long* a, int f, int newN, long invF, long mod)
        {
            for (int i = 0; i < newN; i++) s[i] = a[i + f] * invF % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ScaleInPlace(long* s, long k, int newN, long mod)
        {
            for (int i = 0; i < newN; i++) s[i] = s[i] * k % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteShiftedResult(long* res, long* ex, int shift, int newN, long powF, long mod)
        {
            for (int i = 0; i < shift; i++) res[i] = 0;
            for (int i = 0; i < newN; i++) res[shift + i] = ex[i] * powF % mod;
        }

        public static int Run(int n, long* a, long k, long* res, long mod)
        {
            if (k == IdentityExponent) { WriteUnitSeries(res, n); return n; }
            int f = CountLeadingZeros(a, n);
            if (ShiftExceedsLength(f, n, k)) { FillZeros(res, n); return n; }
            int newN = n - (int)(f * k);
            long invF = FpsShared.ModInverse(a[f], mod);
            long powF = FpsShared.FastPow(a[f], k, mod);
            long* s = stackalloc long[newN];
            NormalizeShiftedSeries(s, a, f, newN, invF, mod);
            long* ln = stackalloc long[newN];
            FormalPowerSeriesLog.Run(newN, s, ln, mod);
            ScaleInPlace(ln, k, newN, mod);
            long* ex = stackalloc long[newN];
            FormalPowerSeriesExp.Run(newN, ln, ex, mod);
            WriteShiftedResult(res, ex, (int)(f * k), newN, powF, mod);
            return n;
        }
    }
}
