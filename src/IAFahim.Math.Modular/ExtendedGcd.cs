using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class ExtendedGcd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, out long x, out long y)
        {
            long oldR = a, r = b;
            long oldS = 1, s = 0;
            long oldT = 0, t = 1;
            while (r != 0)
            {
                long q = oldR / r;
                long rr = oldR - q * r; oldR = r; r = rr;
                long ss = oldS - q * s; oldS = s; s = ss;
                long tt = oldT - q * t; oldT = t; t = tt;
            }
            x = oldS;
            y = oldT;
            return oldR;
        }
    }
}