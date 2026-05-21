namespace IAFahim.String.Palindrome
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LyndonFactorization
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Factorize(byte* s, int n, int* starts, int* lengths)
        {
            int count = 0;
            int i = 0;
            while (i < n)
            {
                int j = i + 1;
                int k = i;
                while (j < n && s[k] <= s[j])
                {
                    if (s[k] < s[j])
                        k = i;
                    else
                        k++;
                    j++;
                }
                while (i <= k)
                {
                    starts[count] = i;
                    lengths[count] = j - k;
                    count++;
                    i += j - k;
                }
            }
            return count;
        }
    }
}
