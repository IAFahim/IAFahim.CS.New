namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PrimePi
    {
        public static long Run(long n, long* small, long* large)
        {
            if (n < 2) return 0;
            int sq = (int)Math.Sqrt((double)n);
            while ((long)(sq + 1) * (sq + 1) <= n) sq++;

            small[0] = 0;
            for (int i = 1; i <= sq; i++)
            {
                small[i] = i - 1;
                large[i] = n / i - 1;
            }

            for (int p = 2; p <= sq; p++)
            {
                if (small[p] == small[p - 1]) continue;
                long pcnt = small[p - 1];
                long p2 = (long)p * p;

                for (int i = 1; i <= sq; i++)
                {
                    long ip = (long)i * p;
                    long nip = n / ip;
                    if (nip < p) break;
                    long sub = (nip <= sq) ? small[nip] : large[ip];
                    large[i] -= sub - pcnt;
                }

                for (long i = sq; i >= p2; i--)
                    small[i] -= small[i / p] - pcnt;
            }

            return large[1];
        }
    }
}
