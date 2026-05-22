namespace IAFahim.DP.General
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ProfileDp
    {
        public static long Run(int m, int n, int* a, long* dp, long* tmp)
        {
            for (int i = 0; i < (1 << m); i++) dp[i] = long.MinValue;
            dp[0] = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    int bit = 1 << j;
                    int maskCount = 1 << m;
                    for (int mask = 0; mask < maskCount; mask++)
                    {
                        tmp[mask] = dp[mask];
                    }
                    for (int mask = 0; mask < maskCount; mask++)
                    {
                        if ((mask & bit) != 0)
                        {
                            int nmask = mask ^ bit;
                            long current = dp[mask];
                            if (current != long.MinValue)
                            {
                                long cand = current + (long)a[i * m + j];
                                if (cand > tmp[nmask]) tmp[nmask] = cand;
                            }
                        }
                    }
                    for (int mask = 0; mask < maskCount; mask++)
                    {
                        dp[mask] = tmp[mask];
                    }
                }
            }
            return dp[0];
        }
    }

    public static unsafe class BrokenProfileDp
    {
        public static long Run(int m, int n, int* a, long* dp, long* tmp, int* state)
        {
            for (int i = 0; i < (1 << m); i++) dp[i] = long.MinValue;
            dp[0] = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    int bit = 1 << j;
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        tmp[mask] = dp[mask];
                    }
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        if ((mask & bit) != 0)
                        {
                            int nmask = mask ^ bit;
                            long current = dp[mask];
                            if (current != long.MinValue)
                            {
                                long val = a[i * m + j];
                                long cand = current + val;
                                if (cand > tmp[nmask]) tmp[nmask] = cand;
                            }
                        }
                    }
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        dp[mask] = tmp[mask];
                    }
                }
            }
            return dp[0];
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
                for (int i = size - 1; i >= 0; i--)
                {
                    long val = dp[u * cap + i];
                    long val2 = dp[vNode * cap + subSize] + v[vNode];
                    int w2 = w[vNode];
                    if (i + w2 < cap) dp[u * cap + i + w2] = Math.Max(dp[u * cap + i + w2], val2);
                }
                size += subSize;
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
                    dp[i * n + j] = best;
                }
            }
            return dp[0 * n + (n - 1)];
        }
    }

    public static unsafe class MinPlusConvolution
    {
        public static void Run(int n, int m, long* a, long* b, long* c, long INF)
        {
            for (int i = 0; i < n + m; i++) c[i] = INF;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    long va = a[i], vb = b[j];
                    if (va != INF && vb != INF)
                    {
                        long val = va + vb;
                        if (val < c[i + j]) c[i + j] = val;
                    }
                }
            }
        }
    }

    public static unsafe class QuadrangleInequalityDp
    {
        public static long Run(int n, int m, long* dp, long* tmp, int* opt)
        {
            for (int i = 0; i < n; i++)
            {
                dp[i * n + i] = 0;
                opt[i] = i;
            }
            for (int len = 2; len <= m; len++)
            {
                for (int i = 0; i + len <= n; i++)
                {
                    int j = i + len - 1;
                    long best = long.MaxValue;
                    int bestK = -1;
                    int start = opt[i];
                    int end = (i + 1 < n) ? opt[i + 1] : j;
                    for (int k = start; k <= end; k++)
                    {
                        long d1 = dp[i * n + k];
                        long d2 = dp[k * n + j];
                        if (d1 != long.MaxValue && d2 != long.MaxValue)
                        {
                            long val = d1 + d2 + 1;
                            if (val < best)
                            {
                                best = val;
                                bestK = k;
                            }
                        }
                    }
                    dp[i * n + j] = best;
                    tmp[i * n + j] = best;
                    opt[i] = bestK;
                }
            }
            return dp[0 * n + (m - 1)];
        }
    }
}