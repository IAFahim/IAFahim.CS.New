namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TspMeetInMiddle
    {
        private const int MaxStackSolvableN = 12;

        private const int MinValidTourN = 3;

        private const int StartCity = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitTable(long* dp, int total, long inf)
        {
            for (int i = 0; i < total; i++) dp[i] = inf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExpandFrom(long* dp, long* w, int mask, int last, long cur, int n, long inf)
        {
            long* wLastRow = w + last * n;
            for (int u = 0; u < n; u++)
            {
                if ((mask & (1 << u)) != 0) continue;
                long edge = wLastRow[u];
                if (edge >= inf) continue;
                long cand = cur + edge;
                long* slot = dp + (mask | (1 << u)) * n + u;
                if (cand < *slot) *slot = cand;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RelaxTransitions(long* dp, long* w, int n, int size, long inf)
        {
            for (int mask = 1; mask < size; mask++)
            {
                long* maskRow = dp + mask * n;
                for (int last = 0; last < n; last++)
                {
                    if ((mask & (1 << last)) == 0) continue;
                    long cur = maskRow[last];
                    if (cur >= inf) continue;
                    ExpandFrom(dp, w, mask, last, cur, n, inf);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CloseTour(long* dp, long* w, int n, int size, long inf)
        {
            int fullMask = size - 1;
            long* fullRow = dp + fullMask * n;
            long best = inf;
            for (int last = StartCity + 1; last < n; last++)
            {
                long pathCost = fullRow[last];
                if (pathCost >= inf) continue;
                long back = w[last * n + StartCity];
                if (back >= inf) continue;
                long cand = pathCost + back;
                if (cand < best) best = cand;
            }
            return best;
        }

        public static long Run(int n, long* w, long inf)
        {
            if (n < MinValidTourN) return 0;
            if (n > MaxStackSolvableN) return inf;
            int size = 1 << n;
            int total = size * n;
            long* dp = stackalloc long[total];
            InitTable(dp, total, inf);
            dp[(1 << StartCity) * n + StartCity] = 0;
            RelaxTransitions(dp, w, n, size, inf);
            return CloseTour(dp, w, n, size, inf);
        }
    }
}
