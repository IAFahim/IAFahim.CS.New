namespace IAFahim.Math.Arithmetic
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TryAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int a, int b, out int res)
        {
            res = a + b;
            return ((a ^ res) & (b ^ res)) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(long a, long b, out long res)
        {
            res = a + b;
            return ((a ^ res) & (b ^ res)) >= 0L;
        }
    }
}
