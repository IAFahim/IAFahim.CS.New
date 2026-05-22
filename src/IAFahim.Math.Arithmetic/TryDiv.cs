namespace IAFahim.Math.Arithmetic
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TryDiv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int a, int b, out int res)
        {
            if (b == 0 || (a == int.MinValue && b == -1))
            {
                res = default;
                return false;
            }
            res = a / b;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(long a, long b, out long res)
        {
            if (b == 0L || (a == long.MinValue && b == -1L))
            {
                res = default;
                return false;
            }
            res = a / b;
            return true;
        }
    }
}
