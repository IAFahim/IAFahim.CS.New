using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ModMul
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, long mod)
        {
            long result = 0;
            a %= mod;
            if (a < 0) a += mod;
            if (a <= long.MaxValue / 2 && mod <= long.MaxValue / 2)
            {
                while (b > 0)
                {
                    if ((b & 1) == 1)
                    {
                        result = (result + a) % mod;
                    }
                    a = (a + a) % mod;
                    b >>= 1;
                }
            }
            else
            {
                while (b > 0)
                {
                    if ((b & 1) == 1)
                    {
                        result = ((result % mod) + (a % mod)) % mod;
                    }
                    a = ((a % mod) + (a % mod)) % mod;
                    b >>= 1;
                }
            }
            return result;
        }
    }
}