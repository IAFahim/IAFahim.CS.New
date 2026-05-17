using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModDiv
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, long mod)
        {
            long inv = ModInv.Run(b, mod);
            if (inv < 0) return -1;
            return ModMul.Run(a, inv, mod);
        }
    }
}