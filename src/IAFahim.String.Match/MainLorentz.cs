namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MainLorentz
    {
        public struct Run
        {
            public int Start;
            public int Period;
            public int Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PeriodMatchesAt(byte* s, int start, int period)
        {
            for (int k = 0; k < period; k++)
                if (s[start + k] != s[start + period + k]) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ExtendRun(byte* s, int n, int start, int period)
        {
            int len = 2 * period;
            while (start + len < n && s[start + len] == s[start + len % period]) len++;
            return len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSubsumed(Run* runs, int count, int period, int start, int len)
        {
            for (int r = 0; r < count; r++)
            {
                if (runs[r].Period == period &&
                    runs[r].Start <= start &&
                    runs[r].Start + runs[r].Length >= start + len)
                    return true;
            }
            return false;
        }

        public static int Find(byte* s, int n, Run* runs)
        {
            int count = 0;
            for (int period = 1; period * 2 <= n; period++)
            {
                for (int start = 0; start + 2 * period <= n; start++)
                {
                    if (!PeriodMatchesAt(s, start, period)) continue;
                    int len = ExtendRun(s, n, start, period);
                    if (IsSubsumed(runs, count, period, start, len)) continue;
                    runs[count].Start = start;
                    runs[count].Period = period;
                    runs[count].Length = len;
                    count++;
                }
            }
            return count;
        }
    }
}
