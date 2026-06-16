namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SternBrocot
    {
        public static void Run(long n, long d, long* num, long* den, int* depth)
        {
            long a = 0, b = 1, c = 1, dd = 0;
            *depth = 0;
            while (true)
            {
                // leftDist = signed cross distance of the left boundary a/b from n/d.
                // rightDist = signed cross distance of the right boundary c/dd from n/d.
                // mediant cmp = (a+c)*d - n*(b+dd) = rightDist - leftDist (identity), so the
                // mediant comparison and both step formulas reuse these two products.
                long leftDist = n * b - a * d;
                long rightDist = c * d - n * dd;
                long cmp = rightDist - leftDist;
                if (cmp == 0) break;
                if (cmp < 0)
                {
                    // Mediant < target: advance the left boundary toward n/d.
                    // rightDist == 0 means the right boundary already equals n/d (target sits
                    // on an endpoint and is never produced as a mediant) -> stop, avoid /0.
                    if (rightDist == 0) break;
                    long steps = leftDist / rightDist;
                    if (steps <= 0) steps = 1;
                    a += steps * c;
                    b += steps * dd;
                }
                else
                {
                    // Mediant > target: advance the right boundary toward n/d.
                    // leftDist == 0 means the left boundary already equals n/d -> stop, avoid /0.
                    if (leftDist == 0) break;
                    long steps = rightDist / leftDist;
                    if (steps <= 0) steps = 1;
                    c += steps * a;
                    dd += steps * b;
                }
                (*depth)++;
            }
            *num = n;
            *den = d;
        }
    }
}
