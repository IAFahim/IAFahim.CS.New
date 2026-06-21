using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class Lcm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b)
        {
            if (a == 0 || b == 0) return 0;
            if (a < 0) a = -a;
            if (b < 0) b = -b;
            long g = Gcd.Run(a, b);
            return a / g * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int a, int b)
        {
            if (a == 0 || b == 0) return 0;
            if (a < 0) a = -a;
            if (b < 0) b = -b;
            int g = Gcd.Run(a, b);
            return a / g * b;
        }
    }
}