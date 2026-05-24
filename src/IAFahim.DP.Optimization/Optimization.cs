namespace IAFahim.DP.Optimization
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KnuthOptimization
    {
        public static long Run(int n, long* dp, long* a, long* opt)
        {
            InitializeDp(n, dp, opt);
            for (int len = 2; len <= n; len++)
            {
                ComputeRow(n, len, dp, a, opt);
            }
            return dp[n - 1];
        }

        private static void InitializeDp(int n, long* dp, long* opt)
        {
            for (int i = 0; i < n * n; i++) { dp[i] = 0; opt[i] = -1; }
            for (int i = 0; i < n; i++) opt[i * n + i] = i;
        }

        private static void ComputeRow(int n, int len, long* dp, long* a, long* opt)
        {
            for (int i = 0; i + len <= n; i++)
            {
                int j = i + len - 1;
                dp[i * n + j] = FindBestSplit(i, j, n, len, dp, a, opt);
            }
        }

        private static long FindBestSplit(int i, int j, int n, int len, long* dp, long* a, long* opt)
        {
            long best = long.MaxValue;
            int bestK = -1;
            int start = (int)opt[i * n + j - 1];
            int end = (int)opt[(i + 1) * n + j];
            if (start == -1) start = i;
            if (end == -1) end = j - 1;

            for (int k = start; k <= end && k < j; k++)
            {
                long val = dp[i * n + k] + dp[(k + 1) * n + j] + (a[j + 1] - a[i]); // Cost(i, j) = a[j+1]-a[i]
                if (val < best) { best = val; bestK = k; }
            }
            opt[i * n + j] = bestK;
            return best;
        }
    }

    public static unsafe class LiChaoAddLine
    {
        public static void Run(long* seg, long m, long b, int node, int l, int r, long x1, long x2)
        {
            int mid = (l + r) >> 1;
            long midX = x1 + (x2 - x1) / 2;
            long curMidM = seg[node * 2 + 0], curMidB = seg[node * 2 + 1];
            
            bool betterAtMid = (m * midX + b < curMidM * midX + curMidB);
            if (betterAtMid) { Swap(ref m, ref curMidM); Swap(ref b, ref curMidB); seg[node * 2 + 0] = curMidM; seg[node * 2 + 1] = curMidB; }
            
            if (r - l == 1) return;
            if (m * x1 + b < curMidM * x1 + curMidB) Run(seg, m, b, node * 2 + 1, l, mid, x1, midX);
            else Run(seg, m, b, node * 2 + 2, mid, r, midX, x2);
        }

        private static void Swap(ref long x, ref long y) { long t = x; x = y; y = t; }
    }
}
