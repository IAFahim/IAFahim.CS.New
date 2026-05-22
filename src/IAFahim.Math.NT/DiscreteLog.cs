namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DiscreteLog
    {
        private static long ModMul(long a, long b, long mod)
        {
            return IAFahim.Math.NT.ModMul.Run(a, b, mod);
        }

        private static long ModPow(long a, long e, long mod)
        {
            return IAFahim.Math.NT.ModPow.Run(a, e, mod);
        }

        public static long Run(long a, long b, long mod, long* scratchKeys, long* scratchVals)
        {
            return Bsgs.Run(a, b, mod, scratchKeys, scratchVals);
        }
    }
}