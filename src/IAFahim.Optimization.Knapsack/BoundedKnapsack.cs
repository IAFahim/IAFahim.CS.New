namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BoundedKnapsack
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long BinarySplit(long* w, long* v, int* cnt, int n, int cap, long* dp)
        {
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++) ProcessItemBinary(i, w, v, cnt, cap, dp);
            return dp[cap];
        }

        private static void ProcessItemBinary(int i, long* w, long* v, int* cnt, int cap, long* dp)
        {
            int k = 1, c = cnt[i];
            while (c > 0)
            {
                int take = Math.Min(k, c); long tw = take * w[i], tv = take * v[i];
                for (int j = cap; j >= tw; j--) { long cand = dp[j - (int)tw] + tv; if (cand > dp[j]) dp[j] = cand; }
                c -= take; k <<= 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MonotoneQueue(long* w, long* v, int* cnt, int n, int cap, long* dp, int* q)
        {
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                int wi = (int)w[i]; if (wi == 0) continue;
                for (int r = 0; r < wi && r <= cap; r++) ProcessRemainderGroup(r, wi, v[i], cnt[i], cap, dp, q);
            }
            return dp[cap];
        }

        private static void ProcessRemainderGroup(int r, int wi, long vi, int ci, int cap, long* dp, int* q)
        {
            int head = 0, tail = 0, maxJ = (cap - r) / wi;
            for (int j = 0; j <= maxJ; j++)
            {
                long val = dp[r + j * wi] - j * vi;
                while (tail > head && val >= dp[r + q[tail - 1] * wi] - q[tail - 1] * vi) tail--;
                q[tail++] = j;
                while (q[head] < j - ci) head++;
                long best = dp[r + q[head] * wi] + (j - q[head]) * vi;
                if (best > dp[r + j * wi]) dp[r + j * wi] = best;
            }
        }
    }
}
