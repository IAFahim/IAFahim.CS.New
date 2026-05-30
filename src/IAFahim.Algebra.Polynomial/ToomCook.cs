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
            
            for (int i = 0; i < resultLen; i++)
            {
                result[i] = 0L;
            }
            
            for (int i = 0; i < lenA; i++)
            {
                if (a[i] == 0L) continue;
                for (int j = 0; j < lenB; j++)
                {
                    result[i + j] = (result[i + j] + a[i] * b[j]) % (long)MOD;
                }
            }
        }
    }
}
