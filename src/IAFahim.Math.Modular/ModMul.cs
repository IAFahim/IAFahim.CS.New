using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModMul
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, long mod)
        {
            if (mod <= 1) return 0;
            long result = 0;
            a %= mod;
            if (a < 0) a += mod;
            b %= mod;
            if (b < 0) b += mod;

            while (b > 0)
            {
                if ((b & 1) == 1)
                {
                    result = (mod - result <= a) ? result - (mod - a) : result + a;
                }
                a = (mod - a <= a) ? a - (mod - a) : a + a;
                b >>= 1;
            }
            return result;
        }
    }
}