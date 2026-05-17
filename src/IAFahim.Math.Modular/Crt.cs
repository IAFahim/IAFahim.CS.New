using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class Crt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long r1, long m1, long r2, long m2)
        {
            long g = Gcd.Run(m1, m2);
            if ((r2 - r1) % g != 0) return -1;
            long lcm = m1 / g * m2;
            long x, y;
            ExtendedGcd.Run(m1, m2, out x, out y);
            long diff = (r2 - r1) / g;
            long result = r1 + ModMul.Run(diff, m1, lcm) * x;
            return ModNormalize.Run(result, lcm);
        }
    }
}