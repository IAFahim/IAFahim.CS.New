namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BostanMori
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* p, int pLen, long* q, int qLen, long k, int MOD)
        {
            long mod = (long)MOD;

            // The numerator length may grow up to qLen after the first halving step
            // (Bostan-Mori keeps a proper fraction: deg(P) < deg(Q)), while the
            // denominator length stays fixed at qLen across every iteration.
            // Size the scratch buffers once to the worst-case product length:
            //   U = P * Q(-x)  -> max(pLen, qLen) + qLen - 1 coefficients
            //   V = Q * Q(-x)  -> 2*qLen - 1 coefficients
            // both of which fit in 2*sz entries with sz = max(pLen, qLen).
            int sz = Math.Max(pLen, qLen);
            long* qneg = stackalloc long[qLen];
            long* r = stackalloc long[sz * 2];
            long* s = stackalloc long[sz * 2];
            // Dedicated numerator scratch. The caller's p buffer only has pLen
            // capacity, but the rebuilt proper-fraction numerator has up to qLen
            // coefficients, so it would overflow p when pLen < qLen. Write the new
            // numerator here instead and read it back on the next iteration.
            long* num = stackalloc long[qLen];

            // Current numerator: starts as the caller's p (length pLen), then lives
            // in num (length qLen) after the first halving step.
            long* curNum = p;
            int nLen = pLen;

            while (k > 0L)
            {
                for (int i = 0; i < qLen; i++)
                    qneg[i] = ((i & 1) == 0) ? q[i] : (mod - q[i]) % mod;

                // U(x) = P(x) * Q(-x), V(x) = Q(x) * Q(-x)
                ToomCook.Multiply(curNum, nLen, qneg, qLen, r, MOD);
                ToomCook.Multiply(q, qLen, qneg, qLen, s, MOD);

                int uLen = nLen + qLen - 1; // valid coefficients written into r
                int vLen = qLen + qLen - 1; // valid coefficients written into s

                // New numerator = even/odd part of U (parity = k & 1); the proper
                // fraction guarantees it has at most qLen coefficients. Multiply has
                // already fully consumed curNum into r, so writing num is safe even
                // when curNum == num.
                FilterPolynomial(num, qLen, r, (int)(k & 1L), uLen);
                // New denominator = even part of V; exactly qLen coefficients. q has
                // qLen capacity, so writing qLen coefficients back into q is in bounds.
                FilterPolynomial(q, qLen, s, 0, vLen);

                curNum = num;
                nLen = qLen;
                k >>= 1;
            }

            // [x^0] P/Q = P(0) / Q(0) = curNum[0] * inv(q[0]) (mod MOD).
            long numerator = curNum[0] % mod;
            if (numerator < 0L) numerator += mod;
            long result = (numerator * ModInv(q[0] % mod, mod)) % mod;
            if (result < 0L) result += mod;
            return result;
        }

        // Writes dstLen coefficients of the (parity)-shifted even/odd part of src:
        // dst[i] = src[parity + 2*i] when that index lies inside the srcLen valid
        // coefficients produced by ToomCook.Multiply, otherwise 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FilterPolynomial(long* dst, int dstLen, long* src, int parity, int srcLen)
        {
            int idx = parity;
            for (int i = 0; i < dstLen; i++)
            {
                dst[i] = (idx < srcLen) ? src[idx] : 0L;
                idx += 2;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModInv(long a, long mod)
        {
            long b = mod, u = 1L, v = 0L;
            while (b > 0L)
            {
                long t = a / b;
                a -= t * b;
                long tmp = a;
                a = b;
                b = tmp;

                u -= t * v;
                tmp = u;
                u = v;
                v = tmp;
            }
            u %= mod;
            if (u < 0L) u += mod;
            return u;
        }
    }
}
