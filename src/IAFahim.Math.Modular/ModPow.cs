using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModPow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long b, long e, long mod)
        {
            long result = 1;
            b %= mod;
            if (b < 0) b += mod;
            while (e > 0)
            {
                if ((e & 1) == 1)
                    result = ModMul.Run(result, b, mod);
                e >>= 1;
                b = ModMul.Run(b, b, mod);
            }
            return result;
        }
    }
}