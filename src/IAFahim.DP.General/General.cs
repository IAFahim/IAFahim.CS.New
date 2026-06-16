namespace IAFahim.DP.General
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ProfileDp
    {
        public static long Run(int m, int n, int* a, long* dp, long* tmp)
        {
            InitializeDp(m, dp);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                UpdateProfileDpMasks(m, j, a + i * m, dp, tmp);
                long* swap = dp; dp = tmp; tmp = swap;
            }
            return dp[0];
        }

        private static void InitializeDp(int m, long* dp)
        {
            int maskCount = 1 << m;
            for (int i = 0; i < maskCount; i++) dp[i] = long.MinValue;
            dp[0] = 0;
        }

        private static void UpdateProfileDpMasks(int m, int j, int* rowA, long* dp, long* tmp)
        {
            int bit = 1 << j;
            int maskCount = 1 << m;
            long addV = rowA[j];
            for (int mask = 0; mask < maskCount; mask++) tmp[mask] = dp[mask];
            for (int mask = bit; mask < maskCount; mask = (mask + 1) | bit)
            {
                long cur = dp[mask];
                if (cur != long.MinValue)
                {
                    int nmask = mask ^ bit;
                    long cand = cur + addV;
                    if (cand > tmp[nmask]) tmp[nmask] = cand;
                }
            }
        }
    }

    public static unsafe class BrokenProfileDp
    {
        public static long Run(int m, int n, int* a, long* dp, long* tmp, int* state)
        {
            InitializeDp(m, dp);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                UpdateBrokenProfileDpMasks(m, j, a + i * m, dp, tmp);
                long* swap = dp; dp = tmp; tmp = swap;
            }
            return dp[0];
        }

        private static void InitializeDp(int m, long* dp)
        {
            int maskCount = 1 << m;
            for (int i = 0; i < maskCount; i++) dp[i] = long.MinValue;
            dp[0] = 0;
        }

        private static void UpdateBrokenProfileDpMasks(int m, int j, int* rowA, long* dp, long* tmp)
        {
            int bit = 1 << j;
            int maskCount = 1 << m;
            long addV = rowA[j];
            for (int mask = 0; mask < maskCount; mask++) tmp[mask] = dp[mask];
            for (int mask = bit; mask < maskCount; mask = (mask + 1) | bit)
            {
                long cur = dp[mask];
                if (cur != long.MinValue)
                {
                    int nmask = mask ^ bit;
                    long cand = cur + addV;
                    if (cand > tmp[nmask]) tmp[nmask] = cand;
                }
            }
        }
    }

    public static unsafe class TreeKnapsack
    {
        public static void Run(int u, int p, int* head, int* to, int* next, int* w, long* v, long* dp, long* tmp, int cap)
        {
            dp[u * cap] = 0;
            int size = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int vNode = to[e];
                if (vNode == p) continue;
                int subSize = 0;
                Run(vNode, u, head, to, next, w, v, dp, tmp, cap);
                ProcessTreeKnapsackChild(u, vNode, v[vNode], w[vNode], size, subSize, dp, cap);
                size += subSize;
            }
        }

        private static void ProcessTreeKnapsackChild(int u, int vNode, long valV, int weightV, int size, int subSize, long* dp, int cap)
        {
            long val2 = dp[vNode * cap + subSize] + valV;
            long* dpU = dp + u * cap;
            int start = Math.Min(size, cap) - 1;
            int limit = cap - weightV - 1;
            if (limit < start) start = limit;
            for (int i = start; i >= 0; i--)
            {
                long* slot = dpU + i + weightV;
                if (val2 > *slot) *slot = val2;
            }
        }
    }

    public static unsafe class IntervalDp
    {
        public static long Run(int n, int* a, long* dp, long* tmp)
        {
            for (int i = 0; i < n; i++) dp[i * n + i] = 0;
            for (int len = 2; len <= n; len++)
            {
                for (int i = 0; i + len <= n; i++)
                {
                    int j = i + len - 1;
                    dp[i * n + j] = FindBestIntervalSplit(i, j, n, dp);
                }
            }
            return dp[0 * n + (n - 1)];
        }

        private static long FindBestIntervalSplit(int i, int j, int n, long* dp)
        {
            long best = long.MaxValue;
            long* rowI = dp + i * n;
            long* rowK = dp + (i + 1) * n + j;
            for (int k = i; k < j; k++)
            {
                long d1 = rowI[k];
                long d2 = *rowK;
                if (d1 != long.MaxValue && d2 != long.MaxValue)
                {
                    long cand = d1 + d2 + 1;
                    if (cand < best) best = cand;
                }
                rowK += n;
            }
            return best;
        }
    }

    public static unsafe class MinPlusConvolution
    {
        public static void Run(int n, int m, long* a, long* b, long* c, long INF)
        {
            for (int i = 0; i < n + m; i++) c[i] = INF;
            for (int i = 0; i < n; i++)
            {
                long ai = a[i];
                if (ai == INF) continue;
                long* cBase = c + i;
                for (int j = 0; j < m; j++)
                {
                    long bj = b[j];
                    if (bj != INF)
                    {
                        long val = ai + bj;
                        if (val < cBase[j]) cBase[j] = val;
                    }
                }
            }
        }
    }

    public static unsafe class QuadrangleInequalityDp
    {
        public static long Run(int n, int m, long* dp, long* tmp, int* opt)
        {
            for (int i = 0; i < n; i++) { dp[i * n + i] = 0; opt[i] = i; }
            for (int len = 2; len <= m; len++)
            {
                for (int i = 0; i + len <= n; i++)
                {
                    int j = i + len - 1;
                    dp[i * n + j] = FindBestQuadrangleSplit(i, j, n, dp, opt);
                }
            }
            return dp[0 * n + (m - 1)];
        }

        private static long FindBestQuadrangleSplit(int i, int j, int n, long* dp, int* opt)
        {
            long best = long.MaxValue;
            int bestK = i;
            int start = opt[i], end = (i + 1 < n) ? opt[i + 1] : j - 1;
            if (end > j - 1) end = j - 1;
            long* rowI = dp + i * n;
            long* rowK = dp + (start + 1) * n + j;
            for (int k = start; k <= end; k++)
            {
                long d1 = rowI[k], d2 = *rowK;
                if (d1 != long.MaxValue && d2 != long.MaxValue)
                {
                    long val = d1 + d2 + 1;
                    if (val < best) { best = val; bestK = k; }
                }
                rowK += n;
            }
            opt[i] = bestK;
            return best;
        }
    }
}