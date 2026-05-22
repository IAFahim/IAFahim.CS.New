namespace IAFahim.Math.Arithmetic
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TryMul
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int a, int b, out int res)
        {
            long w = (long)a * (long)b;
            res = (int)w;
            return w >= (long)int.MinValue && w <= (long)int.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(long a, long b, out long res)
        {
            if (a == 0L || b == 0L)
            {
                res = 0L;
                return true;
            }
            res = a * b;
            if (a == -1L && b == long.MinValue)
            {
                return false;
            }
            if (b == -1L && a == long.MinValue)
            {
                return false;
            }
            return b == res / a;
        }
    }
}
