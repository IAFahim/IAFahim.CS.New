namespace IAFahim.DP
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BranchAndBound
    {
        public static long Run(int n, long* weight, long* value, long capacity, long* bestValue, int* bestSet)
        {
            *bestValue = 0;
            long curW = 0, curV = 0;
            int* curSet = stackalloc int[n];
            int* visited = stackalloc int[n];
            for (int i = 0; i < n; i++) visited[i] = 0;
            BranchRec(n, 0, weight, value, capacity, &curW, &curV, bestValue, curSet, visited);
            for (int i = 0; i < n; i++) bestSet[i] = visited[i];
            return *bestValue;
        }

        private static void BranchRec(int n, int idx, long* weight, long* value, long capacity, long* curW, long* curV, long* bestV, int* curSet, int* bestSet)
        {
            if (idx == n)
            {
                UpdateBestSolution(n, curV, bestV, curSet, bestSet);
                return;
            }
            if (*curW + weight[idx] <= capacity)
            {
                IncludeItem(n, idx, weight, value, capacity, curW, curV, bestV, curSet, bestSet);
            }
            if (CanPrune(n, idx, *curV, *bestV, value)) return;
            ExcludeItem(n, idx, weight, value, capacity, curW, curV, bestV, curSet, bestSet);
        }

        private static void UpdateBestSolution(int n, long* curV, long* bestV, int* curSet, int* bestSet)
        {
            if (*curV > *bestV)
            {
                *bestV = *curV;
                for (int i = 0; i < n; i++) bestSet[i] = curSet[i];
            }
        }

        private static void IncludeItem(int n, int idx, long* weight, long* value, long capacity, long* curW, long* curV, long* bestV, int* curSet, int* bestSet)
        {
            curSet[idx] = 1;
            *curW += weight[idx];
            *curV += value[idx];
            BranchRec(n, idx + 1, weight, value, capacity, curW, curV, bestV, curSet, bestSet);
            *curW -= weight[idx];
            *curV -= value[idx];
        }

        private static void ExcludeItem(int n, int idx, long* weight, long* value, long capacity, long* curW, long* curV, long* bestV, int* curSet, int* bestSet)
        {
            curSet[idx] = 0;
            BranchRec(n, idx + 1, weight, value, capacity, curW, curV, bestV, curSet, bestSet);
        }

        private static bool CanPrune(int n, int idx, long curV, long bestV, long* value)
        {
            long remaining = 0;
            for (int i = idx; i < n; i++) remaining += value[i];
            return curV + remaining <= bestV;
        }
    }

    public static unsafe class Knapsack01
    {
        public static long Run(int n, long capacity, long* weight, long* value, long* dp)
        {
            for (int i = 0; i <= n; i++)
                UpdateKnapsack01Row(i, n, capacity, weight, value, dp);
            return dp[n * (capacity + 1) + (int)capacity];
        }

        private static void UpdateKnapsack01Row(int i, int n, long capacity, long* weight, long* value, long* dp)
        {
            int cols = (int)capacity + 1;
            for (long w = 0; w <= capacity; w++)
            {
                if (i == 0 || w == 0) dp[i * cols + (int)w] = 0;
                else if (weight[i - 1] <= w)
                {
                    long include = value[i - 1] + dp[(i - 1) * cols + (int)(w - weight[i - 1])];
                    long exclude = dp[(i - 1) * cols + (int)w];
                    dp[i * cols + (int)w] = include > exclude ? include : exclude;
                }
                else dp[i * cols + (int)w] = dp[(i - 1) * cols + (int)w];
            }
        }

        public static long RunSpaceOptimized(int n, long capacity, long* weight, long* value, long* dp)
        {
            for (int i = 0; i <= capacity; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
                UpdateKnapsack01SpaceOptimized(capacity, weight[i], value[i], dp);
            return dp[capacity];
        }

        private static void UpdateKnapsack01SpaceOptimized(long capacity, long weight, long value, long* dp)
        {
            for (long w = capacity; w >= weight; w--)
            {
                long val = dp[w - (int)weight] + value;
                if (val > dp[w]) dp[w] = val;
            }
        }
    }

    public static unsafe class KnapsackUnbounded
    {
        public static long Run(int n, long capacity, long* weight, long* value, long* dp)
        {
            for (int i = 0; i <= capacity; i++) dp[i] = 0;
            for (long w = 0; w <= capacity; w++)
                UpdateKnapsackUnbounded(n, w, weight, value, dp);
            return dp[capacity];
        }

        private static void UpdateKnapsackUnbounded(int n, long w, long* weight, long* value, long* dp)
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
    }

    public static unsafe class KnapsackBounded
    {
        public static long Run(int n, long capacity, long* weight, long* value, int* count, long* dp)
        {
            for (int i = 0; i <= capacity; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
                UpdateKnapsackBoundedRow(capacity, weight[i], value[i], count[i], dp);
            return dp[capacity];
        }

        private static void UpdateKnapsackBoundedRow(long capacity, long weight, long value, int count, long* dp)
        {
            for (long w = capacity; w >= 0; w--)
                for (int k = 1; k <= count && k * weight <= w; k++)
                {
                    long val = dp[w - (int)(k * weight)] + k * value;
                    if (val > dp[w]) dp[w] = val;
                }
        }
    }

    public static unsafe class SubsetSum
    {
        public static bool Run(int n, long target, long* arr, bool* dp)
        {
            for (int i = 0; i <= target; i++) dp[i] = false;
            dp[0] = true;
            for (int i = 0; i < n; i++)
                UpdateSubsetSum(target, arr[i], dp);
            return dp[target];
        }

        private static void UpdateSubsetSum(long target, long val, bool* dp)
        {
            for (long s = target; s >= val; s--)
                if (dp[s - (int)val]) dp[s] = true;
        }
    }

    public static unsafe class BitsetSubsetSum
    {
        public static long Run(int n, long target, ulong* bitset, long* arr)
        {
            int bitLen = (int)(target >> 6) + 1;
            for (int i = 0; i < bitLen; i++) bitset[i] = 0;
            bitset[0] = 1UL;
            for (int i = 0; i < n; i++)
                ApplyBitsetShift(bitLen, target, bitset, arr[i]);
            return (bitset[(int)(target >> 6)] & (1UL << (int)(target & 63))) != 0 ? 1 : 0;
        }

        private static void ApplyBitsetShift(int bitLen, long target, ulong* bitset, long val)
        {
            int shift = (int)val;
            if (shift < 0 || shift > target) return;
            int wordShift = shift >> 6;
            int bitShift = shift & 63;
            for (int b = bitLen - 1; b >= 0; b--)
            {
                ulong lo = 0, hi = 0;
                if (b - wordShift >= 0) lo = bitset[b - wordShift] << bitShift;
                if (bitShift > 0 && b - wordShift - 1 >= 0) hi = bitset[b - wordShift - 1] >> (64 - bitShift);
                bitset[b] |= lo | hi;
            }
        }
    }

    public static unsafe class DivideConquerDp
    {
        public static void Optimize(int n, int k, long* dp, long* newDp, long* cost)
        {
            for (int i = 0; i <= n; i++) newDp[i] = long.MaxValue;
            int* opt = stackalloc int[n + 1];
            DcRec(1, n, 1, k, dp, newDp, opt, cost);
        }

        private static void DcRec(int l, int r, int optL, int optR, long* dp, long* newDp, int* opt, long* cost)
        {
            if (l > r) return;
            int mid = (l + r) >> 1;
            int bestOpt = -1;
            long bestVal = long.MaxValue;
            for (int k = optL; k <= Math.Min(mid, optR); k++)
            {
                long val = dp[k - 1] + cost[k + mid];
                if (val < bestVal) { bestVal = val; bestOpt = k; }
            }
            newDp[mid] = bestVal;
            opt[mid] = bestOpt;
            DcRec(l, mid - 1, optL, bestOpt, dp, newDp, opt, cost);
            DcRec(mid + 1, r, bestOpt, optR, dp, newDp, opt, cost);
        }
    }

    public static unsafe class KnuthOptimization
    {
        public static void Optimize(int n, long* dp, long* newDp, long* cost, int* opt)
        {
            for (int i = 1; i <= n; i++)
            {
                newDp[i] = long.MaxValue;
                for (int j = 1; j <= i; j++)
                {
                    long val = dp[j - 1] + cost[j + i];
                    if (val < newDp[i]) { newDp[i] = val; opt[i] = j; }
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
                if (Eval(ms, bs, mid, x) <= Eval(ms, bs, mid + 1, x)) hi = mid;
                else lo = mid + 1;
            }
            return Eval(ms, bs, lo, x);
        }

        private static long Eval(long* ms, long* bs, int idx, long x) => ms[idx] * x + bs[idx];
    }

    public static unsafe class LiChaoAddLine
    {
        public static void AddLine(long m, long b, int node, long l, long r, long* ms, long* bs, bool* has)
        {
            if (!has[node]) { ms[node] = m; bs[node] = b; has[node] = true; return; }
            long mid = (l + r) >> 1;
            if (ms[node] * mid + bs[node] > m * mid + b)
            {
                Swap(ref ms[node], ref m); Swap(ref bs[node], ref b);
            }
            if (l == r) return;
            if (ms[node] * l + bs[node] > m * l + b) AddLine(m, b, node * 2, l, mid, ms, bs, has);
            else AddLine(m, b, node * 2 + 1, mid + 1, r, ms, bs, has);
        }

        private static void Swap(ref long a, ref long b) { long t = a; a = b; b = t; }

        public static long Query(int node, long l, long r, long x, long* ms, long* bs, bool* has)
        {
            if (!has[node]) return long.MaxValue;
            long res = ms[node] * x + bs[node];
            if (l == r) return res;
            long mid = (l + r) >> 1;
            if (x <= mid) return Math.Min(res, Query(node * 2, l, mid, x, ms, bs, has));
            return Math.Min(res, Query(node * 2 + 1, mid + 1, r, x, ms, bs, has));
        }
    }

    public static unsafe class Smawk
    {
        public static long Run(int n, int m, long* mat, long* dp)
        {
            for (int j = 0; j < m; j++) dp[j] = mat[j];
            for (int i = 1; i < n; i++)
            {
                UpdateSmawkRow(i, n, m, mat, dp);
            }
            long ans = long.MaxValue;
            for (int j = 0; j < m; j++) if (dp[j] < ans) ans = dp[j];
            return ans;
        }

        private static void UpdateSmawkRow(int i, int n, int m, long* mat, long* dp)
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
    }

    public static unsafe class AlienDp
    {
        public static long Run(int n, long k, long* arr, Func<long, long, long> dist)
        {
            long lo = 0, hi = CalculateMaxDist(n, arr);
            while (lo < hi)
            {
                long mid = (lo + hi) >> 1;
                if (CountGroups(n, mid, arr, dist) <= k) hi = mid;
                else lo = mid + 1;
            }
            return lo;
        }

        private static long CalculateMaxDist(int n, long* arr)
        {
            long hi = 0;
            for (int i = 0; i < n; i++) hi += Math.Abs(arr[i]);
            return hi;
        }

        private static int CountGroups(int n, long mid, long* arr, Func<long, long, long> dist)
        {
            int groups = 1;
            long cur = 0;
            for (int i = 0; i < n; i++)
            {
                long d = dist(cur, arr[i]);
                if (cur + d > mid) { groups++; cur = d; }
                else cur += d;
            }
            return groups;
        }
    }

    public static unsafe class SubsetDp
    {
        public static void Run(int n, long* dp, long* newDp)
        {
            for (int i = 0; i < n; i++)
                UpdateSubsetDp(n, i, dp, newDp);
        }

        private static void UpdateSubsetDp(int n, int i, long* dp, long* newDp)
        {
            for (int mask = 0; mask < (1 << n); mask++)
                if ((mask & (1 << i)) != 0)
                    newDp[mask] += dp[mask ^ (1 << i)];
        }
    }

    public static unsafe class SosDp
    {
        public static void Run(int n, long* f)
        {
            for (int i = 0; i < n; i++)
                UpdateSosDp(n, i, f);
        }

        private static void UpdateSosDp(int n, int i, long* f)
        {
            for (int mask = 0; mask < (1 << n); mask++)
                if ((mask & (1 << i)) != 0)
                    f[mask] += f[mask ^ (1 << i)];
        }
    }

    public static unsafe class IntervalDp
    {
        public static long Run(int n, long* dp, long* cost)
        {
            for (int len = 1; len <= n; len++)
                for (int i = 0; i + len <= n; i++)
                {
                    int j = i + len - 1;
                    dp[i * n + j] = FindBestIntervalSplit(i, j, n, dp, cost);
                }
            return dp[0 * n + (n - 1)];
        }

        private static long FindBestIntervalSplit(int i, int j, int n, long* dp, long* cost)
        {
            long best = long.MaxValue;
            for (int k = i; k <= j; k++)
            {
                long val = (i < k ? dp[i * n + k - 1] : 0) + (k < j ? dp[(k + 1) * n + j] : 0) + cost[i * n + j];
                if (val < best) best = val;
            }
            return best;
        }
    }

    public static unsafe class MinPlusConvolution
    {
        public static void Run(int n, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                c[i * n + j] = FindMinPlusVal(i, j, n, a, b);
        }

        private static long FindMinPlusVal(int i, int j, int n, long* a, long* b)
        {
            long best = long.MaxValue;
            for (int k = 0; k < n; k++)
            {
                long val = a[i * n + k] + b[k * n + j];
                if (val < best) best = val;
            }
            return best;
        }
    }

    public static unsafe class MaxPlusConvolution
    {
        public static void Run(int n, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                c[i * n + j] = FindMaxPlusVal(i, j, n, a, b);
        }

        private static long FindMaxPlusVal(int i, int j, int n, long* a, long* b)
        {
            long best = long.MinValue;
            for (int k = 0; k < n; k++)
            {
                long val = a[i * n + k] + b[k * n + j];
                if (val > best) best = val;
            }
            return best;
        }
    }
}
