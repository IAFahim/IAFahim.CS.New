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
            int total = n * n;
            for (int i = 0; i < total; i++) { dp[i] = 0; opt[i] = -1; }
            for (int i = 0; i < n; i++) opt[i * n + i] = i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeRow(int n, int len, long* dp, long* a, long* opt)
        {
            for (int i = 0; i + len <= n; i++)
            {
                int j = i + len - 1;
                dp[i * n + j] = FindBestSplit(i, j, n, len, dp, a, opt);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long FindBestSplit(int i, int j, int n, int len, long* dp, long* a, long* opt)
        {
            long best = long.MaxValue;
            int bestK = -1;
            int rowI = i * n;
            int start = (int)opt[rowI + j - 1];
            int end = (int)opt[(i + 1) * n + j];
            if (start == -1) start = i;
            if (end == -1) end = j - 1;

            long cost = a[j + 1] - a[i]; // Cost(i, j) = a[j+1]-a[i], invariant over k
            int kp1Row = (start + 1) * n;
            for (int k = start; k <= end && k < j; k++, kp1Row += n)
            {
                long val = dp[rowI + k] + dp[kp1Row + j];
                if (val < best) { best = val; bestK = k; }
            }
            opt[rowI + j] = bestK;
            return bestK == -1 ? best : best + cost;
        }
    }

    public static unsafe class LiChaoAddLine
    {
        // Inserts the line y = m*x + b over the integer coordinate domain [x1, x2].
        // Precondition (unchecked): x1 <= x2. The recursion is driven solely by the
        // coordinate domain (a single, consistent subdivision); the index bounds l, r
        // are accepted for API compatibility but are not used for routing.
        // Storage for node N lives in seg[N*2] (slope) and seg[N*2+1] (intercept);
        // children are nodes N*2+1 (left, [x1, midX]) and N*2+2 (right, [midX+1, x2]).
        public static void Run(long* seg, long m, long b, int node, int l, int r, long x1, long x2)
        {
            long midX = x1 + ((x2 - x1) >> 1); // x2 >= x1 invariant => >>1 == /2
            long curMidM = seg[node * 2 + 0], curMidB = seg[node * 2 + 1];

            // Keep the line that is better at the midpoint stored at this node;
            // continue descending with the displaced line. After this block,
            // (curMidM, curMidB) is the line now stored at the node and
            // (m, b) is the displaced line that descends.
            if (m * midX + b < curMidM * midX + curMidB)
            {
                long displacedM = curMidM, displacedB = curMidB;
                curMidM = m;
                curMidB = b;
                seg[node * 2 + 0] = curMidM;
                seg[node * 2 + 1] = curMidB;
                m = displacedM;
                b = displacedB;
            }

            if (x1 == x2) return;
            if (m * x1 + b < curMidM * x1 + curMidB) Run(seg, m, b, node * 2 + 1, l, r, x1, midX);
            else Run(seg, m, b, node * 2 + 2, l, r, midX + 1, x2);
        }
    }
}
