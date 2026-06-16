namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FareyRank
    {
        // Returns the 1-based position of the fraction a/b within the Farey
        // sequence F_n (the ascending list of irreducible fractions in [0,1]
        // with denominator <= n). 0/1 has rank 1; 1/1 has the largest rank.
        //
        // a/b need not be reduced or in lowest terms on input; it is treated
        // purely as the threshold value. The rank counts every irreducible
        // fraction p/q with q <= n and 0 <= p/q <= a/b.
        //
        // Returns -1 for invalid input (b <= 0, a < 0, a > b, or n <= 0).
        public static long Run(long a, long b, long n)
        {
            if (b <= 0) return -1;
            if (a < 0 || a > b) return -1;
            if (n <= 0) return -1;

            // rank = 1 (for 0/1) + number of irreducible p/q in (0, a/b], q <= n.
            //
            // Let C(m) = sum_{q=1}^{m} floor(a*q / b) count ALL fractions p/q
            // (not necessarily reduced) with 0 < p/q <= a/b and 1 <= q <= m.
            // The reduced count f(m) satisfies the inclusion-exclusion recurrence
            //   f(m) = C(m) - sum_{d=2}^{m} f(floor(m / d)).
            // We evaluate f(n) bottom-up over all m = 1..n.
            //
            // The caller guarantees n is small enough to back f[0..n] on the
            // stack, matching the unchecked-Run convention used across this
            // library's sieve helpers.
            long* f = stackalloc long[(int)(n + 1)];
            return 1 + CountReduced(a, b, n, f);
        }

        // Fills f[1..n] with f(m) = (reduced fractions p/q in (0, a/b], q <= m)
        // and returns f[n]. f[0] is left as 0 (the empty case).
        //
        // C(m) = sum_{q=1}^{m} floor(a*q / b) is accumulated incrementally so the
        // overall cost is O(n * sqrt(n)).
        private static long CountReduced(long a, long b, long n, long* f)
        {
            f[0] = 0;
            long cumulative = 0;
            for (long m = 1; m <= n; m++)
            {
                cumulative += (a * m) / b; // C(m) = C(m-1) + floor(a*m / b)
                long total = cumulative;
                // Subtract sum_{d=2}^{m} f(floor(m / d)) using divisor blocking:
                // all d in [d, m/(m/d)] share the same quotient floor(m / d).
                long d = 2;
                while (d <= m)
                {
                    long md = m / d;
                    long dHigh = m / md;
                    long span = dHigh - d + 1;
                    total -= span * f[md];
                    d = dHigh + 1;
                }
                f[m] = total;
            }
            return f[n];
        }
    }
}
