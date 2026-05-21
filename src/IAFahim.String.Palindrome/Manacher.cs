namespace IAFahim.String.Palindrome
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Manacher
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Odd(byte* s, int n, int* d)
        {
            int l = 0, r = -1;
            for (int i = 0; i < n; i++)
            {
                int k = l <= r && i <= r ? Math.Min(d[l + r - i], r - i + 1) : 1;
                while (i - k >= 0 && i + k < n && s[i - k] == s[i + k])
                    k++;
                d[i] = k--;
                if (i + k > r)
                {
                    l = i - k;
                    r = i + k;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Even(byte* s, int n, int* d)
        {
            int l = 0, r = -1;
            for (int i = 0; i < n; i++)
            {
                int k = l <= r && i <= r ? Math.Min(d[l + r - i + 1], r - i + 1) : 0;
                while (i - k - 1 >= 0 && i + k < n && s[i - k - 1] == s[i + k])
                    k++;
                d[i] = k--;
                if (i + k > r)
                {
                    l = i - k - 1;
                    r = i + k;
                }
            }
        }
    }
}
