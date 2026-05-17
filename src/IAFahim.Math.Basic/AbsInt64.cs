using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Basic
{
    public static unsafe class AbsInt64
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long v)
        {
            long mask = v >> 63;
            return (v + mask) ^ mask;
        }
    }
}