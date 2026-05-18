namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Bsgs
    {
        private static long ModMul(long a, long b, long mod)
        {
            return IAFahim.Math.NT.ModMul.Run(a, b, mod);
        }

        private static long ModPow(long a, long e, long mod)
        {
            return IAFahim.Math.NT.ModPow.Run(a, e, mod);
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

        public static long Run(long a, long b, long mod)
        {
            if (b == 1) return 0;
            a %= mod;
            if (a < 0) a += mod;
            b %= mod;
            if (b < 0) b += mod;

            long g = Gcd(a, mod);
            if (b % g != 0) return -1;

            long m = (long)Math.Ceiling(Math.Sqrt(mod));
            long* vals = stackalloc long[(int)m];
            long* keys = stackalloc long[(int)m];
            for (int i = 0; i < m; i++)
            {
                keys[i] = -1;
                vals[i] = -1;
            }
            long am = 1;
            for (int i = 0; i < m; i++)
            {
                long key = am;
                int pos = (int)(key % m);
                while (pos < m && keys[pos] != -1 && keys[pos] != key) pos++;
                if (pos < m && keys[pos] == -1)
                {
                    keys[pos] = key;
                    vals[pos] = i;
                }
                am = ModMul(am, a, mod);
            }
            long factor = ModPow(am, mod - 2, mod);
            long cur = b;
            for (int i = 0; i < m; i++)
            {
                int pos = (int)(cur % m);
                while (pos < m && keys[pos] != -1 && keys[pos] != cur) pos++;
                if (pos < m && keys[pos] == cur)
                {
                    long ans = i * m + vals[pos];
                    if (ans < mod) return ans;
                }
                cur = ModMul(cur, factor, mod);
            }
            return -1;
        }
    }
}