namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Runs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindLyndonRuns(byte* s, int n, int* starts, int* lengths)
        {
            int count = 0;
            for (int i = 0; i < n - 1; i++)
            {
                int p = 1;
                while (i + 2 * p <= n && Compare(s + i, s + i + p, p) == 0)
                    p++;
                if (p > 1)
                {
                    starts[count] = i;
                    lengths[count] = p;
                    count++;
                    i += p - 1;
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Compare(byte* a, byte* b, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (a[i] != b[i])
                    return a[i] - b[i];
            }
            return 0;
        }

        public static int Count(int* lcp, int* sa, int n)
        {
            int runs = 0;
            for (int i = 1; i < n; i++)
            {
                int h = lcp[i];
                int left = i, right = i;
                while (left > 1 && lcp[left - 1] >= h) left--;
                while (right < n - 1 && lcp[right + 1] >= h) right++;
                if (h > 0 && (right - left + 1) >= 2 && i == left)
                    runs++;
            }
            return runs;
        }
    }
}
