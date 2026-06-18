namespace IAFahim.String.Palindrome
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class OccurrenceCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Count(byte* s, int n)
        {
            long count = 0;
            int* odd = stackalloc int[n];
            Manacher.Odd(s, n, odd);
            for (int i = 0; i < n; i++)
                count += odd[i];
            int* even = stackalloc int[n];
            Manacher.Even(s, n, even);
            for (int i = 0; i < n; i++)
                count += even[i];
            return count;
        }
    }
}
