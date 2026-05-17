using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Basic
{
    public static unsafe class AbsInt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int v)
        {
            int mask = v >> 31;
            return (v + mask) ^ mask;
        }
    }
}