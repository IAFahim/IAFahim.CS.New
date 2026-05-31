using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Basic
{
    public static unsafe class FloorDiv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int a, int b)
        {
            // Bolt ⚡: Optimization - use Math.DivRem to compute quotient and remainder in a single hardware instruction
            int res = System.Math.DivRem(a, b, out int rem);
            if (rem != 0 && ((a ^ b) < 0))
            {
                res--;
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b)
        {
            // Bolt ⚡: Optimization - use Math.DivRem to compute quotient and remainder in a single hardware instruction
            long res = System.Math.DivRem(a, b, out long rem);
            if (rem != 0 && ((a ^ b) < 0))
            {
                res--;
            }
            return res;
        }
    }
}
