namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FareyRank
    {
        public static long Run(long a, long b, long n)
        {
            if (b <= 0) return -1;
            if (a < 0 || a > b) return -1;
            if (a == 0 || a == b) return 1;
            long p = 0, q = 1, r = 1, s = 0;
            long rank = 1;
            while (true)
            {
                long k = (n + s) / b;
                long t = a * k - p;
                long u = b * k - q;
                if (t <= 0) break;
                p = a;
                q = b;
                a = t;
                b = u;
                if (a == r && b == s) break;
                rank++;
            }
            return rank;
        }
    }
}