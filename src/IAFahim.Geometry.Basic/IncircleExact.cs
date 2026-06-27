namespace IAFahim.Geometry.Basic
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class IncircleExact
    {
        private const long FastBound = 46340L;

        public static int Run(long ax, long ay, long bx, long by, long cx, long cy, long dx, long dy)
        {
            long adx = ax - dx, ady = ay - dy;
            long bdx = bx - dx, bdy = by - dy;
            long cdx = cx - dx, cdy = cy - dy;
            if (InFastRange(adx) && InFastRange(ady) && InFastRange(bdx) && InFastRange(bdy) &&
                InFastRange(cdx) && InFastRange(cdy))
            {
                long aLift = adx * adx + ady * ady;
                long bLift = bdx * bdx + bdy * bdy;
                long cLift = cdx * cdx + cdy * cdy;
                long crossA = bdx * cdy - bdy * cdx;
                long crossB = cdx * ady - cdy * adx;
                long crossC = adx * bdy - ady * bdx;
                long det = aLift * crossA + bLift * crossB + cLift * crossC;
                if (det > 0) return 1;
                if (det < 0) return -1;
                return 0;
            }
            return Exact256(adx, ady, bdx, bdy, cdx, cdy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InFastRange(long v) => v >= -FastBound && v <= FastBound;

        private static int Exact256(long adx, long ady, long bdx, long bdy, long cdx, long cdy)
        {
            Int256 det = Int256.Zero;
            AddTerm(ref det, adx, ady, bdx, cdy, bdy, cdx);
            AddTerm(ref det, bdx, bdy, cdx, ady, cdy, adx);
            AddTerm(ref det, cdx, cdy, adx, bdy, ady, bdx);
            return det.Sign();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddTerm(ref Int256 det, long lx, long ly, long cx0, long cy0, long cx1, long cy1)
        {
            Int256 lift = Int256.FromSquareSum(lx, ly);
            Int256 cross = Int256.FromCross(cx0, cy0, cx1, cy1);
            Int256 term = Int256.MulUnsigned(ref lift, ref cross);
            det.AddAssign(ref term);
        }

        internal struct Int256
        {
            public ulong M0, M1, M2, M3;
            public bool Neg;

            public static readonly Int256 Zero = default;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Sign()
            {
                if (M0 == 0 && M1 == 0 && M2 == 0 && M3 == 0) return 0;
                return Neg ? -1 : 1;
            }

            public static Int256 FromSquareSum(long x, long y)
            {
                ulong ux = AbsLong(x), uy = AbsLong(y);
                U128 xsq = MulU64(ux, ux);
                U128 ysq = MulU64(uy, uy);
                Int256 r;
                r.M0 = xsq.Lo; r.M1 = xsq.Hi; r.M2 = 0; r.M3 = 0; r.Neg = false;
                AddU128ToUnsigned(ref r, ysq.Lo, ysq.Hi);
                return r;
            }

            public static Int256 FromCross(long a, long b, long c, long d)
            {
                bool aneg, bneg;
                U128 p1 = MulSigned64(a, b, out aneg);
                U128 p2 = MulSigned64(c, d, out bneg);
                Int256 r = default;
                r.M0 = p1.Lo; r.M1 = p1.Hi; r.M2 = 0; r.M3 = 0; r.Neg = aneg;
                Int256 q = default;
                q.M0 = p2.Lo; q.M1 = p2.Hi; q.M2 = 0; q.M3 = 0; q.Neg = !bneg;
                r.AddAssign(ref q);
                return r;
            }

            public static Int256 MulUnsigned(ref Int256 a, ref Int256 b)
            {
                bool sign = a.Neg ^ b.Neg;
                Int256 r = MultiplyMagnitude(a.M0, a.M1, a.M2, a.M3, b.M0, b.M1, b.M2, b.M3);
                r.Neg = sign && r.Sign() != 0;
                return r;
            }

            public void AddAssign(ref Int256 b)
            {
                if (Neg == b.Neg) AddMagnitude(ref b);
                else
                {
                    int c = CompareMagnitude(ref b);
                    if (c >= 0) SubMagnitude(ref b);
                    else { Int256 tmp = b; tmp.SubMagnitude(ref this); this = tmp; }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static ulong AbsLong(long x) => x >= 0 ? (ulong)x : (ulong)(-(x + 1)) + 1UL;

            private static U128 MulU64(ulong a, ulong b)
            {
                ulong a0 = a & 0xFFFFFFFFUL, a1 = a >> 32;
                ulong b0 = b & 0xFFFFFFFFUL, b1 = b >> 32;
                ulong p00 = a0 * b0, p01 = a0 * b1, p10 = a1 * b0, p11 = a1 * b1;
                ulong mid = (p00 >> 32) + (p01 & 0xFFFFFFFFUL) + (p10 & 0xFFFFFFFFUL);
                U128 r;
                r.Lo = (p00 & 0xFFFFFFFFUL) | (mid << 32);
                r.Hi = p11 + (p01 >> 32) + (p10 >> 32) + (mid >> 32);
                return r;
            }

            private static U128 MulSigned64(long a, long b, out bool neg)
            {
                neg = (a < 0) ^ (b < 0);
                ulong ua = AbsLong(a), ub = AbsLong(b);
                return MulU64(ua, ub);
            }

            private static void AddU128ToUnsigned(ref Int256 r, ulong lo, ulong hi)
            {
                int carry = 0;
                ulong d0 = AddC(r.M0, lo, ref carry);
                ulong d1 = AddC(r.M1, hi, ref carry);
                ulong d2 = AddC(r.M2, 0, ref carry);
                ulong d3 = AddC(r.M3, 0, ref carry);
                r.M0 = d0; r.M1 = d1; r.M2 = d2; r.M3 = d3;
            }

            private static void SubU128FromUnsigned(ref Int256 r, ulong lo, ulong hi)
            {
                int borrow = 0;
                ulong d0 = SubC(r.M0, lo, ref borrow);
                ulong d1 = SubC(r.M1, hi, ref borrow);
                ulong d2 = SubC(r.M2, 0, ref borrow);
                ulong d3 = SubC(r.M3, 0, ref borrow);
                r.M0 = d0; r.M1 = d1; r.M2 = d2; r.M3 = d3;
            }

            private void AddMagnitude(ref Int256 b)
            {
                int carry = 0;
                M0 = AddC(M0, b.M0, ref carry);
                M1 = AddC(M1, b.M1, ref carry);
                M2 = AddC(M2, b.M2, ref carry);
                M3 = AddC(M3, b.M3, ref carry);
            }

            private void SubMagnitude(ref Int256 b)
            {
                int borrow = 0;
                M0 = SubC(M0, b.M0, ref borrow);
                M1 = SubC(M1, b.M1, ref borrow);
                M2 = SubC(M2, b.M2, ref borrow);
                M3 = SubC(M3, b.M3, ref borrow);
            }

            private int CompareMagnitude(ref Int256 b)
            {
                if (M3 != b.M3) return M3 < b.M3 ? -1 : 1;
                if (M2 != b.M2) return M2 < b.M2 ? -1 : 1;
                if (M1 != b.M1) return M1 < b.M1 ? -1 : 1;
                if (M0 != b.M0) return M0 < b.M0 ? -1 : 1;
                return 0;
            }

            private static Int256 MultiplyMagnitude(ulong a0, ulong a1, ulong a2, ulong a3,
                                                    ulong b0, ulong b1, ulong b2, ulong b3)
            {
                Int256 r = default;
                AccMul(ref r, a0, b0, 0);
                AccMul(ref r, a0, b1, 1); AccMul(ref r, a1, b0, 1);
                AccMul(ref r, a0, b2, 2); AccMul(ref r, a1, b1, 2); AccMul(ref r, a2, b0, 2);
                AccMul(ref r, a0, b3, 3); AccMul(ref r, a1, b2, 3); AccMul(ref r, a2, b1, 3); AccMul(ref r, a3, b0, 3);
                return r;
            }

            private static void AccMul(ref Int256 r, ulong a, ulong b, int shift)
            {
                U128 p = MulU64(a, b);
                int carry = 0;
                if (shift == 0)
                {
                    r.M0 = AddC(r.M0, p.Lo, ref carry);
                    r.M1 = AddC(r.M1, p.Hi, ref carry);
                    r.M2 = AddC(r.M2, 0, ref carry);
                    r.M3 = AddC(r.M3, 0, ref carry);
                }
                else if (shift == 1)
                {
                    r.M1 = AddC(r.M1, p.Lo, ref carry);
                    r.M2 = AddC(r.M2, p.Hi, ref carry);
                    r.M3 = AddC(r.M3, 0, ref carry);
                }
                else if (shift == 2)
                {
                    r.M2 = AddC(r.M2, p.Lo, ref carry);
                    r.M3 = AddC(r.M3, p.Hi, ref carry);
                }
                else
                {
                    r.M3 = AddC(r.M3, p.Lo, ref carry);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static ulong AddC(ulong a, ulong b, ref int carry)
            {
                ulong s = a + b;
                int c1 = s < a ? 1 : 0;
                ulong s2 = s + (ulong)carry;
                int c2 = s2 < s ? 1 : 0;
                carry = c1 + c2;
                return s2;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static ulong SubC(ulong a, ulong b, ref int borrow)
            {
                ulong d = a - (ulong)borrow;
                int b1 = d > a ? 1 : 0;
                ulong d2 = d - b;
                int b2 = d2 > d ? 1 : 0;
                borrow = b1 + b2;
                return d2;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static ulong AddWithCarry(ulong a, ulong b, out ulong carry)
            {
                ulong s = a + b;
                carry = s < a ? 1UL : 0UL;
                return s;
            }
        }

        internal struct U128
        {
            public ulong Lo;
            public ulong Hi;
        }
    }
}
