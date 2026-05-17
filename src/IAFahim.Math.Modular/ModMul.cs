using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModMul
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, long mod)
        {
            long result = 0;
            a %= mod;
            if (a < 0) a += mod;
            while (b > 0)
            {
                if ((b & 1) == 1)
                {
                    result = (result + a) % mod;
                }
                a = (a * 2) % mod;
                b >>= 1;
            }
            return result;
        }
    }
}