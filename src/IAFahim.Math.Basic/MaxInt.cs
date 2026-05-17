using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Basic
{
    public static unsafe class MaxInt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int a, int b)
        {
            return a > b ? a : b;
        }
    }
}