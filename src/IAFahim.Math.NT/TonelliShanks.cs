namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TonelliShanks
    {
        private static long ModPow(long a, long e, long mod)
        {
            return IAFahim.Math.NT.ModPow.Run(a, e, mod);
        }

        private static long ModSqrtOne(long a, long p)
        {
            if (a == 0) return 0;
            if (p == 2) return a;
            if ((p & 3) == 3) return ModPow(a, (p + 1) >> 2, p);
            long q = p - 1;
            int s = 0;
            while ((q & 1) == 0)
            {
                q >>= 1;
                s++;
            }
            long z = 2;
            while (ModPow(z, (p - 1) >> 1, p) != p - 1) z++;
            long c = ModPow(z, q, p);
            long x = ModPow(a, (q + 1) >> 1, p);
            long t = ModPow(a, q, p);
            long m = s;
            while (t != 1)
            {
                long t2 = t;
                int i = 1;
                while (i < m)
                {
                    t2 = ModMul(t2, t2, p);
                    if (t2 == 1) break;
                    i++;
                }
                if (i == m) return -1;
                long b = ModPow(c, 1L << (int)(m - i - 1), p);
                x = ModMul(x, b, p);
                t = ModMul(t, ModMul(b, b, p), p);
                c = ModMul(b, b, p);
                m = i;
            }
            return x;
        }

        private static long ModMul(long a, long b, long mod)
        {
            return IAFahim.Math.NT.ModMul.Run(a, b, mod);
        }

        public static long Run(long a, long p)
        {
            if (a < 0) a = ((a % p) + p) % p;
            if (a == 0 || p == 2) return a;
            long res = ModSqrtOne(a, p);
            if (res == -1) return -1;
            long alt = p - res;
            return res < alt ? res : alt;
        }
    }
}