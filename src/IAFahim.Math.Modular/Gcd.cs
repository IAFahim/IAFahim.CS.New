using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class Gcd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b)
        {
            ulong ua = (ulong)(a < 0 ? -a : a);
            ulong ub = (ulong)(b < 0 ? -b : b);
            while (ub != 0)
            {
                ulong t = ub;
                ub = ua % ub;
                ua = t;
            }
            return (long)ua;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int a, int b)
        {
            uint ua = (uint)(a < 0 ? -a : a);
            uint ub = (uint)(b < 0 ? -b : b);
            while (ub != 0)
            {
                uint t = ub;
                ub = ua % ub;
                ua = t;
            }
            return (int)ua;
        }
    }
}