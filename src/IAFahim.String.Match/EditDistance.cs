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
            for (int i = 0; i < len; i++)
                if (a[i] != b[i]) dist++;
            return dist;
        }

        public static int Levenshtein(byte* a, int lenA, byte* b, int lenB, int maxDist)
        {
            if (Math.Abs(lenA - lenB) > maxDist) return maxDist + 1;
            int* prev = (int*)Marshal.AllocHGlobal(sizeof(int) * (lenB + 1));
            int* curr = (int*)Marshal.AllocHGlobal(sizeof(int) * (lenB + 1));
            for (int j = 0; j <= lenB; j++) prev[j] = j;
            for (int i = 1; i <= lenA; i++)
            {
                curr[0] = i;
                int min = i;
                for (int j = 1; j <= lenB; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    curr[j] = Math.Min(Math.Min(prev[j] + 1, curr[j - 1] + 1), prev[j - 1] + cost);
                    if (curr[j] < min) min = curr[j];
                }
                if (min > maxDist) break;
                var tmp = prev; prev = curr; curr = tmp;
            }
            int result = prev[lenB];
            Marshal.FreeHGlobal((nint)prev);
            Marshal.FreeHGlobal((nint)curr);
            return result;
        }

        public static bool Ukkonen(byte* a, int lenA, byte* b, int lenB, int k, bool* trace)
        {
            int m = lenB;
            int* v = (int*)Marshal.AllocHGlobal(sizeof(int) * (m + 1));
            for (int d = 0; d <= k; d++)
            {
                for (int i = d; i <= m; i++)
                {
                    if (i == d)
                        v[i] = 0;
                    else
                    {
                        int min = v[i - 1] + 1;
                        if (v[i] + 1 < min) min = v[i] + 1;
                        int j = i - 1 - v[i];
                        while (j >= 0 && d > 0 && a[j] != b[j])
                        {
                            if (++min > d) break;
                            j--;
                        }
                        v[i] = min;
                    }
                    if (trace != null) trace[d * (m + 1) + i] = v[i] <= k;
                }
                if (v[m] <= k)
                {
                    Marshal.FreeHGlobal((nint)v);
                    return true;
                }
            }
            Marshal.FreeHGlobal((nint)v);
            return false;
        }
    }
}
