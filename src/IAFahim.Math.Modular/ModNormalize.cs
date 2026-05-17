using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModNormalize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long v, long mod)
        {
            v %= mod;
            if (v < 0) v += mod;
            return v;
        }
    }
}