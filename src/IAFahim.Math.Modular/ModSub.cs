using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModSub
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, long mod)
        {
            long r = a - b;
            if (r < 0) r += mod;
            return r;
        }
    }
}