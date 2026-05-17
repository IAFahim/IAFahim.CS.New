using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class Excrt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* remainders, long* moduli, int len)
        {
            if (len == 0) return 0;
            long r = remainders[0];
            long m = moduli[0];
            for (int i = 1; i < len; i++)
            {
                long g = Gcd.Run(m, moduli[i]);
                if ((remainders[i] - r) % g != 0) return -1;
                long x, y;
                ExtendedGcd.Run(m, moduli[i], out x, out y);
                long lcm = m / g * moduli[i];
                long diff = (remainders[i] - r) / g;
                r = ModNormalize.Run(r + ModMul.Run(diff, m, lcm) * x, lcm);
                m = lcm;
            }
            return r;
        }
    }
}