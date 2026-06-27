namespace IAFahim.Geometry.Basic
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class OrientationExact
    {
        private const long FastBound = 2_000_000_000L;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long ax, long ay, long bx, long by, long cx, long cy)
        {
            long abx = bx - ax, aby = by - ay;
            long acx = cx - ax, acy = cy - ay;
            if (InFastRange(abx) && InFastRange(aby) && InFastRange(acx) && InFastRange(acy))
            {
                long cross = abx * acy - aby * acx;
                return (cross > 0 ? 1 : 0) - (cross < 0 ? 1 : 0);
            }
            Mul128(abx, acy, out long h1, out ulong l1);
            Mul128(aby, acx, out long h2, out ulong l2);
            return SubSign128(h1, l1, h2, l2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InFastRange(long v) => v >= -FastBound && v <= FastBound;

        internal static void Mul128(long a, long b, out long hi, out ulong lo)
        {
            ulong ua = a >= 0 ? (ulong)a : (ulong)(~(ulong)a) + 1UL;
            ulong ub = b >= 0 ? (ulong)b : (ulong)(~(ulong)b) + 1UL;
            ulong a0 = ua & 0xFFFFFFFFUL, a1 = ua >> 32;
            ulong b0 = ub & 0xFFFFFFFFUL, b1 = ub >> 32;
            ulong p00 = a0 * b0, p01 = a0 * b1, p10 = a1 * b0, p11 = a1 * b1;
            ulong mid = (p00 >> 32) + (p01 & 0xFFFFFFFFUL) + (p10 & 0xFFFFFFFFUL);
            lo = (p00 & 0xFFFFFFFFUL) | (mid << 32);
            ulong uhi = p11 + (p01 >> 32) + (p10 >> 32) + (mid >> 32);
            bool neg = (a < 0) ^ (b < 0);
            if (neg)
            {
                ulong nlo = 0UL - lo;
                uhi = 0UL - uhi - (lo != 0UL ? 1UL : 0UL);
                lo = nlo;
            }
            hi = (long)uhi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int SubSign128(long h1, ulong l1, long h2, ulong l2)
        {
            ulong dlo = l1 - l2;
            ulong borrow = l2 > l1 ? 1UL : 0UL;
            long dhi = h1 - h2 - (long)borrow;
            if (dhi < 0) return -1;
            if (dhi > 0) return 1;
            return dlo == 0UL ? 0 : 1;
        }
    }
}
