namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ToomCook
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(long* a, int lenA, long* b, int lenB, long* result, int MOD)
        {
            int resultLen = lenA + lenB - 1;
            if (resultLen <= 0) return;

            long mod = MOD;

            for (int i = 0; i < resultLen; i++)
            {
                result[i] = 0L;
            }

            for (int i = 0; i < lenA; i++)
            {
                long ai = a[i];
                if (ai == 0L) continue;
                for (int j = 0; j < lenB; j++)
                {
                    long v = (result[i + j] + ai * b[j]) % mod;
                    result[i + j] = v < 0L ? v + mod : v;
                }
            }
        }
    }
}
