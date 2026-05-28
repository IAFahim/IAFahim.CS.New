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
            for (int d = 0; d < m; d++)
            {
                prev[d] = d + 1;
            }
            for (int pos = 0; pos < n; pos++)
            {
                UpdateLandauVishkinDp(text, pattern, pos, m, k, curr, prev);
                if (curr[m - 1] <= k) results[(*count)++] = Math.Max(0, pos - m + 1);
                SwapBuffers(ref curr, ref prev);
            }
        }

        private static void UpdateLandauVishkinDp(byte* text, byte* pattern, int pos, int m, int k, int* curr, int* prev)
        {
            curr[0] = (text[pos] == pattern[0]) ? 0 : 1;
            for (int d = 1; d < m; d++)
            {
                int cost = (text[pos] == pattern[d]) ? 0 : 1;
                int replace = prev[d - 1] + cost;
                int delete = prev[d] + 1;
                int insert = curr[d - 1] + 1;
                curr[d] = Math.Min(replace, Math.Min(delete, insert));
            }
        }

        private static void SwapBuffers(ref int* a, ref int* b)
        {
            int* t = a; a = b; b = t;
        }
    }
}
