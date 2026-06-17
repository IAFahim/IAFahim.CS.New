using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModInv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long mod)
        {
            long x, y;
            long g = ExtendedGcd.Run(a, mod, out x, out y);
            if (g < 0) { g = -g; x = -x; }
            if (g != 1) return -1;
            return ModNormalize.Run(x, mod);
        }
    }
}