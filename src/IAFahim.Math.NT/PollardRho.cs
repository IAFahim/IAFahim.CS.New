namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PollardRho
    {
        private static long AbsLong(long x)
        {
            return x >= 0 ? x : -x;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModMul(long a, long b, long mod)
        {
            return IAFahim.Math.NT.ModMul.Run(a, b, mod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            return IAFahim.Math.NT.ModPow.Run(b, e, mod);
        }

        private static long F(long x, long c, long mod)
        {
            return (ModMul(x, x, mod) + c) % mod;
        }

        public static long Run(long n)
        {
            if (n % 2 == 0) return 2;
            if (n % 3 == 0) return 3;
            if (MillerRabin.Run(n)) return n;

            long c = 1;
            long x = 2;
            long y = 2;
            long d = 1;
            while (d == 1)
            {
                x = F(x, c, n);
                y = F(F(y, c, n), c, n);
                d = AbsLong(x - y);
                d = Gcd(d, n);
            }
            if (d == n) return PollardRho.Run(n);
            return d;
        }
    }
}