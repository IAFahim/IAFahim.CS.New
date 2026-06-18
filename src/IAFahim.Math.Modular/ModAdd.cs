using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, long mod)
        {
            a %= mod;
            if (a < 0) a += mod;
            b %= mod;
            if (b < 0) b += mod;
            long r = a + b;
            if (r >= mod) r -= mod;
            return r;
        }
    }
}