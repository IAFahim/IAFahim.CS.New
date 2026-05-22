using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Basic
{
    public static unsafe class CeilDiv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int a, int b)
        {
            return a / b + ((a % b != 0 && ((a ^ b) >= 0)) ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b)
        {
            return a / b + ((a % b != 0 && ((a ^ b) >= 0)) ? 1 : 0);
        }
    }
}