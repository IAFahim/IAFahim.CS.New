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
                CopyDp(m, dp, tmp);
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
            for (int mask = 0; mask < maskCount; mask++) tmp[mask] = dp[mask];
            for (int mask = 0; mask < maskCount; mask++)
            {
                if ((mask & bit) != 0)
                {
                    int nmask = mask ^ bit;
                    if (dp[mask] != long.MinValue)
                    {
                        long cand = dp[mask] + rowA[j];
                        if (cand > tmp[nmask]) tmp[nmask] = cand;
                    }
                }
            }
        }

        private static void CopyDp(int m, long* dp, long* tmp)
        {
            int maskCount = 1 << m;
            for (int mask = 0; mask < maskCount; mask++) dp[mask] = tmp[mask];
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
                CopyDp(m, dp, tmp);
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
            for (int mask = 0; mask < maskCount; mask++) tmp[mask] = dp[mask];
            for (int mask = 0; mask < maskCount; mask++)
            {
                if ((mask & bit) != 0)
                {
                    int nmask = mask ^ bit;
                    if (dp[mask] != long.MinValue)
                    {
                        long cand = dp[mask] + rowA[j];
                        if (cand > tmp[nmask]) tmp[nmask] = cand;
                    }
                }
            }
        }

        private static void CopyDp(int m, long* dp, long* tmp)
        {
            int maskCount = 1 << m;
            for (int mask = 0; mask < maskCount; mask++) dp[mask] = tmp[mask];
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
            for (int i = Math.Min(size, cap) - 1; i >= 0; i--)
            {
                long val2 = dp[vNode * cap + subSize] + valV;
                if (i + weightV < cap) dp[u * cap + i + weightV] = Math.Max(dp[u * cap + i + weightV], val2);
            }
        }
    }

    public static unsafe class IntervalDp
    {
        public static long Run(int n, int* a, long* dp, long* tmp)
        {
            for (int i = 0; i < n; i++) dp[i] = 0;
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
            for (int k = i; k < j; k++)
            {
                long d1 = dp[i * n + k];
                long d2 = dp[(k + 1) * n + j];
                if (d1 != long.MaxValue && d2 != long.MaxValue)
                {
                    long cand = d1 + d2 + 1;
                    if (cand < best) best = cand;
                }
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
            for (int j = 0; j < m; j++)
            {
                if (a[i] != INF && b[j] != INF)
                {
                    long val = a[i] + b[j];
                    if (val < c[i + j]) c[i + j] = val;
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
            int bestK = -1;
            int start = opt[i], end = (i + 1 < n) ? opt[i + 1] : j;
            for (int k = start; k <= end; k++)
            {
                long d1 = dp[i * n + k], d2 = dp[k * n + j];
                if (d1 != long.MaxValue && d2 != long.MaxValue)
                {
                    long val = d1 + d2 + 1;
                    if (val < best) { best = val; bestK = k; }
                }
            }
            opt[i] = bestK;
            return best;
        }
    }
}