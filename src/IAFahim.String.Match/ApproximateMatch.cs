namespace IAFahim.String.Match
{
using System.Runtime.InteropServices;
    using System;

    public static unsafe class ApproximateMatch
    {
        public static void Find(byte* text, int textLen, byte* pattern, int patLen, int k, int* results, int* count)
        {
            *count = 0;
            for (int i = 0; i <= textLen - patLen; i++)
            {
                if (EditDistance.Levenshtein(text + i, patLen, pattern, patLen, k) <= k)
                    results[(*count)++] = i;
            }
        }

        public static void LandauVishkin(byte* text, int textLen, byte* pattern, int patLen, int k, int* results, int* count)
        {
            int n = textLen;
            int m = patLen;
            int* D = (int*)Marshal.AllocHGlobal(sizeof(int) * (k + 1));
            int* curr = (int*)Marshal.AllocHGlobal(sizeof(int) * (k + 1));
            int* prev = (int*)Marshal.AllocHGlobal(sizeof(int) * (k + 1));
            *count = 0;
            for (int pos = 0; pos < n; pos++)
            {
                for (int d = 0; d <= k; d++)
                {
                    if (pos < d || m < d)
                        curr[d] = d > pos ? pos : d;
                    else
                    {
                        if (text[pos - d] != pattern[d])
                            curr[d] = 1 + Math.Min(prev[d], Math.Min(d > 0 ? curr[d - 1] : int.MaxValue, d > 0 ? prev[d - 1] : int.MaxValue));
                        else
                            curr[d] = prev[d];
                    }
                }
                if (curr[k] <= k)
                    results[(*count)++] = pos - k;
                int* tmp = prev; prev = curr; curr = tmp;
            }
            Marshal.FreeHGlobal((nint)D);
            Marshal.FreeHGlobal((nint)curr);
            Marshal.FreeHGlobal((nint)prev);
        }
    }
}
