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

        public static void LandauVishkin(byte* text, int textLen, byte* pattern, int patLen, int k, int* results, int* count, int* curr, int* prev)
        {
            int n = textLen, m = patLen;
            *count = 0;
            for (int pos = 0; pos < n; pos++)
            {
                UpdateLandauVishkinDp(text, pattern, pos, m, k, curr, prev);
                if (curr[m - 1] <= k) results[(*count)++] = Math.Max(0, pos - m + 1);
                SwapBuffers(ref curr, ref prev);
            }
        }

        private static void UpdateLandauVishkinDp(byte* text, byte* pattern, int pos, int m, int k, int* curr, int* prev)
        {
            for (int d = 0; d < m; d++)
            {
                if (pos < d || m < d) curr[d] = Math.Min(pos, d);
                else
                {
                    if (text[pos - d] != pattern[d])
                        curr[d] = 1 + Math.Min(prev[d], Math.Min(d > 0 ? curr[d - 1] : int.MaxValue, d > 0 ? prev[d - 1] : int.MaxValue));
                    else
                        curr[d] = prev[d];
                }
            }
        }

        private static void SwapBuffers(ref int* a, ref int* b)
        {
            int* t = a; a = b; b = t;
        }
    }
}
