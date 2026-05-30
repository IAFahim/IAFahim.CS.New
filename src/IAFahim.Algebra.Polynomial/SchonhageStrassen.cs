namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SchonhageStrassen
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(long* a, long* b, long* result, int n, int MOD)
        {
            ToomCook.Multiply(a, n, b, n, result, MOD);
        }
    }
}