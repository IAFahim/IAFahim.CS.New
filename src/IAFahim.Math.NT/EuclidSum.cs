namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class EuclidSum
    {
        private static long Gcd(long a, long b)
        {
            if (a < 0) a = -a;
            if (b < 0) b = -b;
            while (b != 0)
            {
                long t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        public static long Run(long n, long m)
        {
            long g = Gcd(n, m);
            return SumCoPrime(n, m / g) * g;
        }

        private static long SumCoPrime(long nIn, long mIn)
        {
            long n = nIn, m = mIn;
            long ans = 0;
            while (m != 0)
            {
                ans += n * (m - 1) - m * (n - 1);
                long nn = m;
                long nm = n % m;
                n = nn; m = nm;
            }
            return ans;
        }
    }
}
