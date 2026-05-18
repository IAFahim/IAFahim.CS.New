namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PrimitiveRoot
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

        private static long PowMod(long a, long e, long mod)
        {
            return IAFahim.Math.NT.ModPow.Run(a, e, mod);
        }

        public static long Run(long n)
        {
            if (n <= 1) return -1;
            if (n == 2) return 1;
            if (n == 4) return 3;

            long phi = Phi.Run(n);

            int* factors = stackalloc int[64];
            int fc = 0;
            long tmp = phi;
            for (long p = 2; p * p <= tmp; p++)
            {
                if (tmp % p == 0)
                {
                    factors[fc++] = (int)p;
                    while (tmp % p == 0) tmp /= p;
                }
            }
            if (tmp > 1) factors[fc++] = (int)tmp;

            for (long g = 2; g < n; g++)
            {
                if (Gcd(g, n) != 1) continue;
                bool ok = true;
                for (int i = 0; i < fc; i++)
                {
                    long exp = phi / factors[i];
                    if (PowMod(g, exp, n) == 1)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok) return g;
            }
            return -1;
        }
    }
}
