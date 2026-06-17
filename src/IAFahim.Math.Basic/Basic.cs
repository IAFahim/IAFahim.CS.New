namespace IAFahim.Math.Basic
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FastPow
    {
        public static long Run(long a, long e, long mod)
        {
            long res = 1 % mod; a = NormalizeModulo.Run(a, mod);
            while (e > 0) { if ((e & 1) == 1) res = SafeMulMod.Run(res, a, mod); a = SafeMulMod.Run(a, a, mod); e >>= 1; }
            return res;
        }
    }

    public static unsafe class IntegerSqrt
    {
        public static long Run(long x)
        {
            if (x < 0) return -1; if (x == 0) return 0;
            long lo = 0, hi = 3037000499L;
            while (lo < hi) { long mid = lo + (hi - lo + 1) / 2; if (mid * mid <= x) lo = mid; else hi = mid - 1; }
            return lo;
        }
    }

    public static unsafe class NthRoot
    {
        public static long Run(long x, int n)
        {
            if (n <= 0 || x < 0) return -1; if (n == 1) return x;
            long lo = 0, hi = 1L << (62 / n + 1);
            while (lo < hi) { long mid = lo + (hi - lo + 1) / 2; if (CanMultiply(mid, n, x)) lo = mid; else hi = mid - 1; }
            return lo;
        }
        private static bool CanMultiply(long mid, int n, long x) { long p = 1; for (int i = 0; i < n; i++) { if (p > x / mid) return false; p *= mid; } return p <= x; }
    }

    public static unsafe class IntegerCbrt
    {
        public static long Run(long x)
        {
            if (x < 0) return -1; if (x == 0) return 0;
            long lo = 0, hi = 2097151L;
            while (lo < hi) { long mid = lo + (hi - lo + 1) / 2; if (mid * mid * mid <= x) lo = mid; else hi = mid - 1; }
            return lo;
        }
    }

    public static unsafe class IsPerfectSquare { public static bool Run(long x) => x >= 0 && IntegerSqrt.Run(x) * IntegerSqrt.Run(x) == x; }
    public static unsafe class IsPowerOfTwo { public static bool Run(long x) => x > 0 && (x & (x - 1)) == 0; }

    public static unsafe class NextPowerOfTwo
    {
        public static long Run(long x) { if (x <= 0) return 1; x--; x |= x >> 1; x |= x >> 2; x |= x >> 4; x |= x >> 8; x |= x >> 16; x |= x >> 32; return x + 1; }
    }

    public static unsafe class PrevPowerOfTwo
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x) { if (x <= 0) return 0; x |= x >> 1; x |= x >> 2; x |= x >> 4; x |= x >> 8; x |= x >> 16; x |= x >> 32; return x - (x >>> 1); }
    }

    public static unsafe class FloorLog2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long x)
        {
            if (x <= 0) return 0; int len = 0;
            if ((x & unchecked((long)0xFFFFFFFF00000000UL)) != 0) { len += 32; x >>>= 32; }
            if ((x & 0xFFFF0000L) != 0) { len += 16; x >>>= 16; }
            if ((x & 0xFF00L) != 0) { len += 8; x >>>= 8; }
            if ((x & 0xF0L) != 0) { len += 4; x >>>= 4; }
            if ((x & 0xCL) != 0) { len += 2; x >>>= 2; }
            if ((x & 0x2L) != 0) { len += 1; x >>>= 1; }
            return len;
        }
    }

    public static unsafe class CeilLog2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long x) { if (x <= 0) return 0; int floor = FloorLog2.Run(x); return (x & (x - 1)) == 0 ? floor : floor + 1; }
    }

    public static unsafe class SafeMulMod
    {
        public static long Run(long a, long b, long mod)
        {
            a = NormalizeModulo.Run(a, mod); b = NormalizeModulo.Run(b, mod);
            if (mod <= int.MaxValue) return (a * b) % mod;
            long res = 0;
            while (b > 0) { if ((b & 1) == 1) res = SafeAdd(res, a, mod); a = SafeAdd(a, a, mod); b >>= 1; }
            return res;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long SafeAdd(long a, long b, long mod) { return a >= mod - b ? a - (mod - b) : a + b; }
    }

    public static unsafe class NormalizeModulo { [MethodImpl(MethodImplOptions.AggressiveInlining)] public static long Run(long x, long mod) { x %= mod; return x < 0 ? x + mod : x; } }
    public static unsafe class Minimize { [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Run(long* a, long b) { if (b < *a) *a = b; } }
    public static unsafe class Maximize { [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Run(long* a, long b) { if (b > *a) *a = b; } }
    public static unsafe class RelaxMin { [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Run(long* ptr, long val) { if (val < *ptr) { *ptr = val; return true; } return false; } }
    public static unsafe class RelaxMax { [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool Run(long* ptr, long val) { if (val > *ptr) { *ptr = val; return true; } return false; } }
    public static unsafe class SwapInts { [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Run(int* a, int* b) { int tmp = *a; *a = *b; *b = tmp; } }
    public static unsafe class SwapPairs { [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void Run(long* a, long* b) { long tmp = *a; *a = *b; *b = tmp; } }
}
