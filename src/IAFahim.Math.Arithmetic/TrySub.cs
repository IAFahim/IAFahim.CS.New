namespace IAFahim.Math.Arithmetic
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TrySub
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int a, int b, out int res)
        {
            res = a - b;
            return ((a ^ b) & (a ^ res)) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(long a, long b, out long res)
        {
            res = a - b;
            return ((a ^ b) & (a ^ res)) >= 0L;
        }
    }
}
