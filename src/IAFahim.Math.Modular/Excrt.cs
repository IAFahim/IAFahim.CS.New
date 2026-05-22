using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class Excrt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* remainders, long* moduli, int len)
        {
            if (len == 0)
            {
                return 0;
            }
            long r = remainders[0];
            long m = moduli[0];
            if (m <= 0)
            {
                return -1;
            }
            r = (r % m + m) % m;
            for (int i = 1; i < len; i++)
            {
                long m2 = moduli[i];
                if (m2 <= 0)
                {
                    return -1;
                }
                long r2 = (remainders[i] % m2 + m2) % m2;
                long x;
                long y;
                long g = ExtendedGcd.Run(m, m2, out x, out y);
                if ((r2 - r) % g != 0)
                {
                    return -1;
                }
                long m2_g = m2 / g;
                if (long.MaxValue / m < m2_g)
                {
                    return -1;
                }
                long lcm = m * m2_g;
                long diff = (r2 - r) / g;
                long t = ModMul.Run(x, diff, m2_g);
                r = r + t * m;
                m = lcm;
            }
            return r;
        }
    }
}