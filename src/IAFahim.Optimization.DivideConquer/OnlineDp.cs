namespace IAFahim.Optimization.DivideConquer
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class OnlineDp
    {
        public static void Recompute(long* dp, int l, int r, int optL, int optR, int k, delegate*<int,int,long> calc)
        {
            if (l > r) return;
            int mid = (l + r) >> 1;
            long best = long.MaxValue;
            int bestPos = -1;
            int start = Math.Max(optL, mid - k);
            int end = Math.Min(optR, mid);
            for (int j = start; j <= end; j++)
            {
                long cand = calc(mid, j) + (j > 0 ? dp[j - 1] : 0);
                if (cand < best) { best = cand; bestPos = j; }
            }
            dp[mid] = best;
            Recompute(dp, l, mid - 1, optL, bestPos, k, calc);
            Recompute(dp, mid + 1, r, bestPos, optR, k, calc);
        }
    }
}
