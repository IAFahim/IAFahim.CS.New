namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ToomCook
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(long* a, long* b, long* result, int n, int MOD)
        {
            for (int i = 0; i < 2 * n; i++) result[i] = 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                result[i + j] = (result[i + j] + a[i] * b[j]) % MOD;
                if (result[i + j] < 0) result[i + j] += MOD;
            }
        }
    }
}
