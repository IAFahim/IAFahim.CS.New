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

            InitializePiDists(n, sq, small, large);
            for (int p = 2; p <= sq; p++)
            {
                if (small[p] != small[p - 1])
                {
                    PerformPiSieveStep(n, sq, p, small, large);
                }
            }
            return large[1];
        }

        private static void InitializePiDists(long n, int sq, long* small, long* large)
        {
            small[0] = 0;
            for (int i = 1; i <= sq; i++) { small[i] = i - 1; large[i] = n / i - 1; }
        }

        private static void PerformPiSieveStep(long n, int sq, int p, long* small, long* large)
        {
            long pcnt = small[p - 1];
            long p2 = (long)p * p;
            for (int i = 1; i <= sq; i++)
            {
                long nip = n / ((long)i * p);
                if (nip < p) break;
                large[i] -= ((nip <= sq) ? small[nip] : large[(long)i * p]) - pcnt;
            }
            for (long i = sq; i >= p2; i--)
                small[i] -= small[i / p] - pcnt;
        }
    }
}
