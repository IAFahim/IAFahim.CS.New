namespace IAFahim.String.Match
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class EditDistance
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hamming(byte* a, byte* b, int len)
        {
            int dist = 0;
            for (int i = 0; i < len; i++) if (a[i] != b[i]) dist++;
            return dist;
        }

        public static int Levenshtein(byte* a, int lenA, byte* b, int lenB, int maxDist)
        {
            if (Math.Abs(lenA - lenB) > maxDist) return maxDist + 1;
            int* prev = (int*)Marshal.AllocHGlobal(sizeof(int) * (lenB + 1)), curr = (int*)Marshal.AllocHGlobal(sizeof(int) * (lenB + 1));
            try
            {
                InitializeLevenshtein(lenB, prev);
                for (int i = 1; i <= lenA; i++)
                {
                    if (UpdateLevenshteinRow(i, lenB, a, b, prev, curr) > maxDist) break;
                    SwapBuffers(ref prev, ref curr);
                }
                return prev[lenB];
            }
            finally { Marshal.FreeHGlobal((nint)prev); Marshal.FreeHGlobal((nint)curr); }
        }

        private static void InitializeLevenshtein(int lenB, int* prev)
        {
            for (int j = 0; j <= lenB; j++) prev[j] = j;
        }

        private static int UpdateLevenshteinRow(int i, int lenB, byte* a, byte* b, int* prev, int* curr)
        {
            curr[0] = i; int min = i;
            for (int j = 1; j <= lenB; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                curr[j] = Math.Min(Math.Min(prev[j] + 1, curr[j - 1] + 1), prev[j - 1] + cost);
                if (curr[j] < min) min = curr[j];
            }
            return min;
        }

        private static void SwapBuffers(ref int* a, ref int* b) { int* t = a; a = b; b = t; }

        public static bool Ukkonen(byte* a, int lenA, byte* b, int lenB, int k, bool* trace)
        {
            int m = lenB; int* v = (int*)Marshal.AllocHGlobal(sizeof(int) * (m + 1));
            try
            {
                for (int d = 0; d <= k; d++)
                {
                    if (PerformUkkonenIteration(a, b, m, d, k, v, trace)) return true;
                }
                return false;
            }
            finally { Marshal.FreeHGlobal((nint)v); }
        }

        private static bool PerformUkkonenIteration(byte* a, byte* b, int m, int d, int k, int* v, bool* trace)
        {
            for (int i = d; i <= m; i++)
            {
                if (i == d) v[i] = 0;
                else
                {
                    int min = Math.Min(v[i - 1] + 1, v[i] + 1), j = i - 1 - v[i];
                    while (j >= 0 && d > 0 && a[j] != b[j]) { if (++min > d) break; j--; }
                    v[i] = min;
                }
                if (trace != null) trace[d * (m + 1) + i] = v[i] <= k;
            }
            return v[m] <= k;
        }
    }
}
