namespace IAFahim.DP
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BranchAndBound
    {
        public static long Run(int n, long* weight, long* value, long capacity, long* bestValue, int* bestSet)
        {
            *bestValue = 0;
            long* curW = stackalloc long[1];
            long* curV = stackalloc long[1];
            *curW = 0;
            *curV = 0;
            int* curSet = stackalloc int[n];
            int* visited = stackalloc int[n];
            for (int i = 0; i < n; i++) visited[i] = 0;
            BranchRec(n, 0, weight, value, capacity, curW, curV, bestValue, curSet, visited);
            for (int i = 0; i < n; i++) bestSet[i] = visited[i];
            return *bestValue;
        }

        private static void BranchRec(int n, int idx, long* weight, long* value, long capacity, long* curW, long* curV, long* bestV, int* curSet, int* bestSet)
        {
            if (idx == n)
            {
                if (*curV > *bestV)
                {
                    *bestV = *curV;
                    for (int i = 0; i < n; i++) bestSet[i] = curSet[i];
                }
                return;
            }
            if (*curW + weight[idx] <= capacity)
            {
                curSet[idx] = 1;
                *curW += weight[idx];
                *curV += value[idx];
                BranchRec(n, idx + 1, weight, value, capacity, curW, curV, bestV, curSet, bestSet);
                *curW -= weight[idx];
                *curV -= value[idx];
            }
            long remaining = 0;
            for (int i = idx; i < n; i++) remaining += value[i];
            if (*curV + remaining <= *bestV) return;
            curSet[idx] = 0;
            BranchRec(n, idx + 1, weight, value, capacity, curW, curV, bestV, curSet, bestSet);
        }
    }

    public static unsafe class Knapsack01
    {
        public static long Run(int n, long capacity, long* weight, long* value, long* dp)
        {
            for (int i = 0; i <= n; i++)
            {
                for (long w = 0; w <= capacity; w++)
                {
                    if (i == 0 || w == 0)
                    {
                        dp[i * (capacity + 1) + (int)w] = 0;
                    }
                    else if (weight[i - 1] <= w)
                    {
                        long include = value[i - 1] + dp[(i - 1) * (capacity + 1) + (int)(w - weight[i - 1])];
                        long exclude = dp[(i - 1) * (capacity + 1) + (int)w];
                        dp[i * (capacity + 1) + (int)w] = include > exclude ? include : exclude;
                    }
                    else
                    {
                        dp[i * (capacity + 1) + (int)w] = dp[(i - 1) * (capacity + 1) + (int)w];
                    }
                }
            }
            return dp[n * (capacity + 1) + (int)capacity];
        }

        public static long RunSpaceOptimized(int n, long capacity, long* weight, long* value, long* dp)
        {
            for (int i = 0; i <= capacity; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                for (long w = capacity; w >= weight[i]; w--)
                {
                    long val = dp[w - (int)weight[i]] + value[i];
                    if (val > dp[w]) dp[w] = val;
                }
            }
            return dp[capacity];
        }
    }

    public static unsafe class KnapsackUnbounded
    {
        public static long Run(int n, long capacity, long* weight, long* value, long* dp)
        {
            for (int i = 0; i <= capacity; i++) dp[i] = 0;
            for (long w = 0; w <= capacity; w++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (weight[i] <= w)
                    {
                        long val = dp[w - (int)weight[i]] + value[i];
                        if (val > dp[w]) dp[w] = val;
                    }
                }
            }
            return dp[capacity];
        }
    }

    public static unsafe class KnapsackBounded
    {
        public static long Run(int n, long capacity, long* weight, long* value, int* count, long* dp)
        {
            for (int i = 0; i <= capacity; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                for (long w = capacity; w >= 0; w--)
                {
                    for (int k = 1; k <= count[i] && k * weight[i] <= w; k++)
                    {
                        long val = dp[w - (int)(k * weight[i])] + k * value[i];
                        if (val > dp[w]) dp[w] = val;
                    }
                }
            }
            return dp[capacity];
        }
    }

    public static unsafe class SubsetSum
    {
        public static bool Run(int n, long target, long* arr, bool* dp)
        {
            dp[0] = true;
            for (int i = 0; i < n; i++)
            {
                for (long s = target; s >= arr[i]; s--)
                {
                    if (dp[s - (int)arr[i]]) dp[s] = true;
                }
            }
            return dp[target];
        }
    }

    public static unsafe class BitsetSubsetSum
    {
        public static long Run(int n, long target, ulong* bitset, long* arr)
        {
            bitset[0] = 1UL;
            for (int i = 0; i < n; i++)
            {
                int idx = (int)(arr[i] >> 6);
                ulong mask = 1UL << (int)(arr[i] & 63);
                int bitLen = (int)(target >> 6) + 1;
                for (int b = bitLen; b >= 0; b--)
                {
                    if ((bitset[b] & mask) != 0 && b + idx < bitLen)
                    {
                        bitset[b + idx] |= bitset[b] << (int)(arr[i] & 63);
                    }
                }
            }
            return (bitset[(int)(target >> 6)] & (1UL << (int)(target & 63))) != 0 ? 1 : 0;
        }
    }

    public static unsafe class DivideConquerDp
    {
        public static void Optimize(int n, int k, long* dp, long* newDp, long* cost)
        {
            for (int i = 0; i <= n; i++) newDp[i] = long.MaxValue;
            int* opt = stackalloc int[n + 1];
            DcRec(1, n, 1, k, dp, newDp, opt);
        }

        private static void DcRec(int l, int r, int optL, int optR, long* dp, long* newDp, int* opt)
        {
            if (l > r) return;
            int mid = (l + r) >> 1;
            int bestOpt = -1;
            long bestVal = long.MaxValue;
            for (int k = optL; k <= Math.Min(mid, optR); k++)
            {
                long val = dp[k - 1] + cost[k * 1000 + mid];
                if (val < bestVal)
                {
                    bestVal = val;
                    bestOpt = k;
                }
            }
            newDp[mid] = bestVal;
            opt[mid] = bestOpt;
            DcRec(l, mid - 1, optL, bestOpt, dp, newDp, opt);
            DcRec(mid + 1, r, bestOpt, optR, dp, newDp, opt);
        }
    }

    public static unsafe class KnuthOptimization
    {
        public static void Optimize(int n, long* dp, long* newDp, long* cost, int* opt)
        {
            for (int i = 0; i <= n; i++) newDp[i] = long.MaxValue;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= i; j++)
                {
                    long val = dp[j - 1] + cost[j * 1000 + i];
                    if (val < newDp[i])
                    {
                        newDp[i] = val;
                        opt[i] = j;
                    }
                }
            }
        }
    }

    public static unsafe class ConvexHullTrickAdd
    {
        public static void AddLine(long m, long b, long* ms, long* bs, int* sz)
        {
            ms[*sz] = m;
            bs[*sz] = b;
            (*sz)++;
        }

        public static long Query(long x, long* ms, long* bs, int sz)
        {
            int lo = 0, hi = sz - 1;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                long y1 = ms[mid] * x + bs[mid];
                long y2 = ms[mid + 1] * x + bs[mid + 1];
                if (y1 <= y2) hi = mid;
                else lo = mid + 1;
            }
            return ms[lo] * x + bs[lo];
        }
    }

    public static unsafe class LiChaoAddLine
    {
        public static void AddLine(long m, long b, int node, long l, long r, long* ms, long* bs, bool* has)
        {
            if (!has[node])
            {
                ms[node] = m;
                bs[node] = b;
                has[node] = true;
                return;
            }
            long mid = (l + r) >> 1;
            if (ms[node] * mid + bs[node] > m * mid + b)
            {
                long tm = ms[node]; long tb = bs[node];
                ms[node] = m; bs[node] = b;
                m = tm; b = tb;
            }
            if (l == r) return;
            if (ms[node] * l + bs[node] > m * l + b)
                AddLine(m, b, node * 2, l, mid, ms, bs, has);
            else
                AddLine(m, b, node * 2 + 1, mid + 1, r, ms, bs, has);
        }

        public static long Query(int node, long l, long r, long x, long* ms, long* bs, bool* has)
        {
            if (!has[node]) return long.MaxValue;
            long res = ms[node] * x + bs[node];
            if (l == r) return res;
            long mid = (l + r) >> 1;
            if (x <= mid) return Math.Min(res, Query(node * 2, l, mid, x, ms, bs, has));
            else return Math.Min(res, Query(node * 2 + 1, mid + 1, r, x, ms, bs, has));
        }
    }

    public static unsafe class Smawk
    {
        public static long Run(int n, int m, long* mat, long* dp)
        {
            for (int j = 0; j < m; j++) dp[j] = mat[j];
            for (int i = 1; i < n; i++)
            {
                int* stack = stackalloc int[m];
                int sz = 0;
                for (int j = 0; j < m; j++)
                {
                    while (sz > 0 && mat[i * m + j] <= mat[i * m + stack[sz - 1]]) sz--;
                    stack[sz++] = j;
                }
                for (int k = 0; k < sz; k++)
                {
                    int j = stack[k];
                    long best = mat[i * m + j];
                    if (i > 1) best += dp[j];
                    dp[j] = best;
                }
            }
            long ans = long.MaxValue;
            for (int j = 0; j < m; j++) if (dp[j] < ans) ans = dp[j];
            return ans;
        }
    }

    public static unsafe class AlienDp
    {
        public static long Run(int n, long k, long* arr, Func<long, long, long> dist)
        {
            long lo = 0, hi = 0;
            for (int i = 0; i < n; i++) hi += Math.Abs(arr[i]);
            while (lo < hi)
            {
                long mid = (lo + hi) >> 1;
                int groups = 1;
                long cur = 0;
                for (int i = 0; i < n; i++)
                {
                    if (cur + dist(cur, arr[i]) > mid)
                    {
                        groups++;
                        cur = arr[i];
                    }
                    else cur += dist(cur, arr[i]);
                }
                if (groups <= k) hi = mid;
                else lo = mid + 1;
            }
            return lo;
        }
    }

    public static unsafe class SubsetDp
    {
        public static void Run(int n, long* dp, long* newDp)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                        newDp[mask] += dp[mask ^ (1 << i)];
                }
            }
        }
    }

    public static unsafe class SosDp
    {
        public static void Run(int n, long* f)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                        f[mask] += f[mask ^ (1 << i)];
                }
            }
        }
    }

    public static unsafe class IntervalDp
    {
        public static long Run(int n, long* dp, long* cost)
        {
            for (int len = 1; len <= n; len++)
            {
                for (int i = 0; i + len <= n; i++)
                {
                    int j = i + len - 1;
                    dp[i * n + j] = long.MaxValue;
                    for (int k = i; k <= j; k++)
                    {
                        long val = (i < k ? dp[i * n + k - 1] : 0) + (k < j ? dp[(k + 1) * n + j] : 0) + cost[i * n + j];
                        if (val < dp[i * n + j]) dp[i * n + j] = val;
                    }
                }
            }
            return dp[0 * n + (n - 1)];
        }
    }

    public static unsafe class MinPlusConvolution
    {
        public static void Run(int n, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    long best = long.MaxValue;
                    for (int k = 0; k < n; k++)
                    {
                        long val = a[i * n + k] + b[k * n + j];
                        if (val < best) best = val;
                    }
                    c[i * n + j] = best;
                }
            }
        }
    }

    public static unsafe class MaxPlusConvolution
    {
        public static void Run(int n, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    long best = long.MinValue;
                    for (int k = 0; k < n; k++)
                    {
                        long val = a[i * n + k] + b[k * n + j];
                        if (val > best) best = val;
                    }
                    c[i * n + j] = best;
                }
            }
        }
    }
}
