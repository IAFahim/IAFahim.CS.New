namespace IAFahim.String.Compress
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ZivLempel
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Factorize(byte* input, int len, LzFactorization.Factor* output)
        {
            return LzFactorization.Factorize(input, len, output);
        }
    }
}
