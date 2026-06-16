namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RationalInterpolation
    {
        private const long NonInvertible = -1L;

        // Computes a rational interpolant R(x) = num(x) / den(x) passing through
        // the n points (xs[i], ys[i]) over the prime field Z/MOD via Thiele's
        // continued fraction. The continued-fraction coefficients are converted
        // into explicit numerator/denominator polynomials using the convergent
        // recurrence P_k = a_k*P_{k-1} + (x - x_{k-1})*P_{k-2}.
        //
        // Coefficients are written little-endian (index = power of x), reduced
        // into [0, MOD). Numerator degree is n/2, denominator degree is (n-1)/2,
        // so caller buffers num/den of length n are always large enough.
        // Returns the numerator length (degNum + 1), or 0 if n == 0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long* xs, long* ys, int n, long MOD, long* num, long* den)
        {
            if (n == 0) return 0;

            // Thiele continued-fraction coefficients a[0..n-1].
            long* a = stackalloc long[n];
            ThieleInterpolation(xs, ys, n, a, MOD);

            // Reduce node abscissae once into [0, MOD) for the polynomial recurrence.
            long* xr = stackalloc long[n];
            for (int i = 0; i < n; i++)
            {
                long v = xs[i] % MOD;
                if (v < 0) v += MOD;
                xr[i] = v;
            }

            // Convergent recurrence. P holds numerator polynomials, Q denominators.
            // pPrev/qPrev = convergent k-1, pPrev2/qPrev2 = convergent k-2.
            // Initial: P_{-1} = 1, Q_{-1} = 0, P_0 = a[0], Q_0 = 1.
            long* pPrev2 = stackalloc long[n];
            long* qPrev2 = stackalloc long[n];
            long* pPrev = stackalloc long[n];
            long* qPrev = stackalloc long[n];
            long* pCur = stackalloc long[n];
            long* qCur = stackalloc long[n];

            for (int i = 0; i < n; i++)
            {
                pPrev2[i] = 0; qPrev2[i] = 0;
                pPrev[i] = 0; qPrev[i] = 0;
            }
            pPrev2[0] = 1L;              // P_{-1} = 1
            // Q_{-1} = 0 (already zeroed)
            long a0 = a[0] % MOD;
            if (a0 < 0) a0 += MOD;
            pPrev[0] = a0;              // P_0 = a[0]
            qPrev[0] = 1L;             // Q_0 = 1

            int degPPrev2 = 0;          // degree of P_{-1}
            int degQPrev2 = -1;         // Q_{-1} = 0 has no nonzero term
            int degPPrev = 0;           // degree of P_0
            int degQPrev = 0;           // degree of Q_0

            for (int k = 1; k < n; k++)
            {
                long ak = a[k] % MOD;
                if (ak < 0) ak += MOD;
                long xk1 = xr[k - 1];   // x_{k-1}, already in [0, MOD)

                int degPCur = ConvergentStep(pPrev, degPPrev, pPrev2, degPPrev2, ak, xk1, pCur, MOD);
                int degQCur = ConvergentStep(qPrev, degQPrev, qPrev2, degQPrev2, ak, xk1, qCur, MOD);

                // Shift: (k-1) <- (k-2), k <- (k-1) by rotating buffers.
                long* tp = pPrev2; pPrev2 = pPrev; pPrev = pCur; pCur = tp;
                long* tq = qPrev2; qPrev2 = qPrev; qPrev = qCur; qCur = tq;
                degPPrev2 = degPPrev; degPPrev = degPCur;
                degQPrev2 = degQPrev; degQPrev = degQCur;
            }

            // After the loop pPrev/qPrev hold the final convergents P_{n-1}/Q_{n-1}.
            int degNum = n / 2;
            int degDen = (n - 1) / 2;
            for (int i = 0; i <= degNum; i++) num[i] = pPrev[i];
            for (int i = 0; i <= degDen; i++) den[i] = qPrev[i];

            return degNum + 1;
        }

        // dst = ak * prev + (x - xk1) * prev2, all coefficients in [0, MOD).
        // prev has degree degPrev, prev2 has degree degPrev2 (-1 means zero poly).
        // Returns the degree of dst (0 if dst is the zero polynomial).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ConvergentStep(long* prev, int degPrev, long* prev2, int degPrev2, long ak, long xk1, long* dst, long MOD)
        {
            // Multiplying prev2 (degree degPrev2) by (x - xk1) raises its degree by one.
            int shiftedDeg = degPrev2 < 0 ? -1 : degPrev2 + 1;
            int top = degPrev > shiftedDeg ? degPrev : shiftedDeg;
            if (top < 0) top = 0;

            long negX = MOD - xk1;      // -x_{k-1} mod MOD, in [1, MOD] (==MOD only if xk1==0)
            if (negX == MOD) negX = 0;

            for (int i = 0; i <= top; i++)
            {
                long acc = 0L;

                // ak * prev[i]
                if (i <= degPrev)
                {
                    acc = ak * prev[i] % MOD;
                }

                // (x - xk1) * prev2 contributes prev2[i-1] (from the x factor)
                // and (-xk1) * prev2[i] (from the constant factor).
                if (i >= 1 && (i - 1) <= degPrev2 && degPrev2 >= 0)
                {
                    acc += prev2[i - 1];
                    if (acc >= MOD) acc -= MOD;
                }
                if (i <= degPrev2 && degPrev2 >= 0 && negX != 0L)
                {
                    acc += negX * prev2[i] % MOD;
                    if (acc >= MOD) acc -= MOD;
                }

                dst[i] = acc;
            }

            int deg = top;
            while (deg > 0 && dst[deg] == 0L) deg--;
            return deg;
        }

        // Fills a[0..n-1] with the Thiele continued-fraction (reciprocal-difference)
        // coefficients for the points (x[i], y[i]) over Z/MOD. y is treated as
        // read-only; the working columns are kept in private scratch buffers.
        //
        // Uses the reciprocal (inverse) difference recurrence
        //   rho_{-1}        = 0
        //   rho_0(x_i)      = y_i
        //   rho_k(x_i..x_{i+k}) = rho_{k-2}(x_{i+1}..x_{i+k-1})
        //                         + (x_i - x_{i+k}) / (rho_{k-1}(x_i..) - rho_{k-1}(x_{i+1}..))
        // and sets the continued-fraction coefficient
        //   a[k] = rho_k(x_0..x_k) - rho_{k-2}(x_0..x_{k-2})
        // (with rho_{-1} = rho_{-2} = 0).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThieleInterpolation(long* x, long* y, int n, long* a, long MOD)
        {
            // Reduce abscissae once into [0, MOD).
            long* xr = stackalloc long[n];
            for (int i = 0; i < n; i++)
            {
                long v = x[i] % MOD;
                if (v < 0) v += MOD;
                xr[i] = v;
            }

            // prevPrev = column k-2 (rho_{-1} = 0 initially),
            // prev     = column k-1 (rho_0 = y initially).
            long* prevPrev = stackalloc long[n];
            long* prev = stackalloc long[n];
            long* cur = stackalloc long[n];
            for (int i = 0; i < n; i++)
            {
                prevPrev[i] = 0L;
                long v = y[i] % MOD;
                if (v < 0) v += MOD;
                prev[i] = v;
            }

            a[0] = prev[0];
            for (int k = 1; k < n; k++)
            {
                int count = n - k;          // number of entries in column k
                for (int i = 0; i < count; i++)
                {
                    long denom = (prev[i] - prev[i + 1]) % MOD;
                    if (denom < 0) denom += MOD;
                    long inv = ModInv(denom, MOD);

                    long xdiff = (xr[i] - xr[i + k]) % MOD;
                    if (xdiff < 0) xdiff += MOD;

                    long term = xdiff * inv % MOD;
                    long val = prevPrev[i + 1] + term;
                    if (val >= MOD) val -= MOD;
                    cur[i] = val;
                }

                // Continued-fraction coefficient: subtract rho_{k-2}(x_0..x_{k-2}),
                // which is the head of the prevPrev column.
                long coeff = cur[0] - prevPrev[0];
                if (coeff < 0) coeff += MOD;
                a[k] = coeff;

                // Advance the rolling columns: prevPrev <- prev <- cur.
                long* tmp = prevPrev;
                prevPrev = prev;
                prev = cur;
                cur = tmp;
            }
        }

        // Modular inverse of a mod MOD via the extended Euclidean algorithm.
        // a may be any (possibly negative) value; it is reduced into [0, MOD)
        // first. Returns the inverse in [0, MOD), or NonInvertible (-1) when a
        // is not coprime to MOD (e.g. a == 0).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModInv(long a, long MOD)
        {
            long t = 0, newT = 1;
            long r = MOD, newR = a % MOD;
            if (newR < 0) newR += MOD;
            while (newR != 0)
            {
                long q = r / newR;
                long tmpT = t - q * newT;
                long tmpR = r - q * newR;
                t = newT; r = newR;
                newT = tmpT; newR = tmpR;
            }
            if (r > 1) return NonInvertible;
            if (t < 0) t += MOD;
            return t;
        }
    }
}
