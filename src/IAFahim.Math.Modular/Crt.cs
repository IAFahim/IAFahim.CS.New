using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class Crt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long r1, long m1, long r2, long m2)
        {
            if (m1 <= 0 || m2 <= 0)
            {
                return -1;
            }
            r1 = (r1 % m1 + m1) % m1;
            r2 = (r2 % m2 + m2) % m2;
            long x;
            long y;
            long g = ExtendedGcd.Run(m1, m2, out x, out y);
            if ((r2 - r1) % g != 0)
            {
                return -1;
            }
            long m2_g = m2 / g;
            if (long.MaxValue / m1 < m2_g)
            {
                return -1;
            }
            long diff = (r2 - r1) / g;
            long t = ModMul.Run(x, diff, m2_g);
            long result = r1 + t * m1;
            return result;
        }
    }
}