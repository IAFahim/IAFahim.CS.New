namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FloorSum
    {
        public static long Run(long n, long m, long a, long b)
        {
            long ans = 0;
            if (a >= m)
            {
                ans += (n - 1) * n * (a / m) / 2;
                a %= m;
            }
            if (b >= m)
            {
                ans += n * (b / m);
                b %= m;
            }
            long yMax = (a * n + b) / m;
            long xMax = yMax * m - b;
            if (yMax == 0) return ans;
            ans += (n - (xMax + a - 1) / a) * yMax;
            ans += Run(yMax, a, m, (a - (xMax % a)) % a);
            return ans;
        }
    }
}
