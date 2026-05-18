namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LegendreSymbol
    {
        private static long ModPow(long a, long e, long mod)
        {
            return IAFahim.Math.NT.ModPow.Run(a, e, mod);
        }

        public static int Run(long a, long p)
        {
            a = ((a % p) + p) % p;
            if (a == 0) return 0;
            long val = ModPow(a, (p - 1) >> 1, p);
            if (val == p - 1) return -1;
            return 1;
        }
    }
}
