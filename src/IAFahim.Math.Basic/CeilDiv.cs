using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Basic
{
    public static unsafe class CeilDiv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int a, int b)
        {
            // Bolt: Math.DivRem calculates quotient and remainder simultaneously, replacing two division instructions with one.
            int q = System.Math.DivRem(a, b, out int rem);
            return q + ((rem != 0 && ((a ^ b) >= 0)) ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b)
        {
            // Bolt: Math.DivRem calculates quotient and remainder simultaneously, replacing two division instructions with one.
            long q = System.Math.DivRem(a, b, out long rem);
            return q + ((rem != 0 && ((a ^ b) >= 0)) ? 1 : 0);
        }
    }
}