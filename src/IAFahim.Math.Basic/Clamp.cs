using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Basic
{
    public static unsafe class Clamp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int v, int lo, int hi)
        {
            if (v < lo) return lo;
            if (v > hi) return hi;
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long v, long lo, long hi)
        {
            if (v < lo) return lo;
            if (v > hi) return hi;
            return v;
        }
    }
}