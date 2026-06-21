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

        public static long Run(long a, long b, long mod, long* scratchKeys, long* scratchVals)
        {
            if (b == 1) return 0;
            a %= mod;
            if (a < 0) a += mod;
            b %= mod;
            if (b < 0) b += mod;

            long g = Gcd(a, mod);
            if (b % g != 0) return -1;

            long m = (long)Math.Ceiling(Math.Sqrt(mod));
            int tableSize = (int)m;
            for (int i = 0; i < tableSize; i++)
            {
                scratchKeys[i] = -1;
                scratchVals[i] = -1;
            }
            long am = 1;
            for (int i = 0; i < tableSize; i++)
            {
                long key = am;
                int pos = (int)(key % tableSize);
                int probed = 0;
                while (probed < tableSize && scratchKeys[pos] != -1 && scratchKeys[pos] != key)
                {
                    pos = (pos + 1) % tableSize;
                    probed++;
                }
                if (scratchKeys[pos] == -1)
                {
                    scratchKeys[pos] = key;
                    scratchVals[pos] = i;
                }
                am = ModMul(am, a, mod);
            }
            long factor = ModPow(am, mod - 2, mod);
            long cur = b;
            for (int i = 0; i < tableSize; i++)
            {
                int pos = (int)(cur % tableSize);
                int probed = 0;
                while (probed < tableSize && scratchKeys[pos] != -1 && scratchKeys[pos] != cur)
                {
                    pos = (pos + 1) % tableSize;
                    probed++;
                }
                if (scratchKeys[pos] == cur)
                {
                    long ans = (long)i * tableSize + scratchVals[pos];
                    if (ans < mod) return ans;
                }
                cur = ModMul(cur, factor, mod);
            }
            return -1;
        }
    }
}