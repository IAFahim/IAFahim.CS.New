namespace IAFahim.DP.Optimization
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DivideConquerDp
    {
        public static long Run(int n, long* dp, long* a, long* b, long* c, long k)
        {
            for (int i = 0; i < n; i++)
            {
                dp[i] = long.MaxValue;
            }
            dp[0] = 0;
            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    long val = dp[j] + a[j] * b[i] + c[j];
                    if (val < dp[i]) dp[i] = val;
                }
            }
            return dp[n - 1];
        }
    }

    public static unsafe class KnuthOptimization
    {
        public static long Run(int n, long* dp, long* a, long* opt)
        {
            for (int i = 0; i < n; i++)
            {
                dp[i] = 0;
                opt[i] = i;
            }
            for (int len = 2; len <= n; len++)
            {
                for (int i = 0; i + len <= n; i++)
                {
                    int j = i + len - 1;
                    long best = long.MaxValue;
                    int bestK = i;
                    for (int k = (int)opt[i]; k <= (int)opt[i + 1] && k <= j; k++)
                    {
                        long val = dp[i * n + k] + dp[(k + 1) * n + j] + (long)len * a[j + 1];
                        if (val < best)
                        {
                            best = val;
                            bestK = k;
                        }
                    }
                    dp[i * n + j] = best;
                    opt[i * n + j] = bestK;
                }
            }
            return dp[0 * n + (n - 1)];
        }
    }

    public static unsafe class ConvexHullTrickAdd
    {
        public static void Run(long* m, long* b, int* size, long x, long y)
        {
            int sz = *size;
            m[sz] = x;
            b[sz] = y;
            *size = sz + 1;
        }
    }

    public static unsafe class ConvexHullTrickQuery
    {
        public static long Run(int* ptr, long* m, long* b, int size, long x)
        {
            while (*ptr + 1 < size)
            {
                long y1 = m[*ptr] * x + b[*ptr];
                long y2 = m[*ptr + 1] * x + b[*ptr + 1];
                if (y2 <= y1) (*ptr)++;
                else break;
            }
            return m[*ptr] * x + b[*ptr];
        }
    }

    public static unsafe class LiChaoAddLine
    {
        public static void Run(long* seg, long m, long b, int node, int l, int r, long x1, long x2)
        {
            int mid = (l + r) >> 1;
            long y1 = m * x1 + b;
            long y2 = m * x2 + b;
            long curMid = seg[node * 2 + 0];
            long curB = seg[node * 2 + 1];
            long curY = curMid * mid + curB;
            long newY = m * mid + b;
            if (newY < curY)
            {
                seg[node * 2 + 0] = m;
                seg[node * 2 + 1] = b;
                m = curMid;
                b = curB;
            }
            if (r - l == 1) return;
            long yL = m * x1 + b;
            long yR = m * x2 + b;
            if (yL < yR) Run(seg, m, b, node * 2 + 1, l, mid, x1, (x1 + x2) >> 1);
            else Run(seg, m, b, node * 2 + 2, mid, r, (x1 + x2) >> 1, x2);
        }
    }

    public static unsafe class LiChaoAddSegment
    {
        public static void Run(long* seg, long m, long b, int node, int l, int r, long x1, long x2, long s, long t)
        {
            if (t <= l || r <= s) return;
            if (s <= l && r <= t)
            {
                LiChaoAddLine.Run(seg, m, b, node, l, r, x1, x2);
                return;
            }
            int mid = (l + r) >> 1;
            long y1 = seg[node * 2 + 0] * x1 + seg[node * 2 + 1];
            long y2 = seg[node * 2 + 0] * x2 + seg[node * 2 + 1];
            long newY1 = m * x1 + b;
            long newY2 = m * x2 + b;
            if (newY1 < y1 || newY2 < y2)
            {
                Run(seg, m, b, node, l, r, x1, x2, s, t);
            }
            Run(seg, m, b, node * 2 + 1, l, mid, x1, (x1 + x2) >> 1, s, t);
            Run(seg, m, b, node * 2 + 2, mid, r, (x1 + x2) >> 1, x2, s, t);
        }
    }

    public static unsafe class LiChaoQuery
    {
        public static long Run(long* seg, int node, int l, int r, long x, long x1, long x2)
        {
            long res = seg[node * 2 + 0] * x + seg[node * 2 + 1];
            if (r - l == 1) return res;
            int mid = (l + r) >> 1;
            if (x < x1 + (x2 - x1 >> 1))
                return Math.Min(res, Run(seg, node * 2 + 1, l, mid, x, x1, (x1 + x2) >> 1));
            else
                return Math.Min(res, Run(seg, node * 2 + 2, mid, r, x, (x1 + x2) >> 1, x2));
        }
    }

    public static unsafe class MongeOptimize
    {
        public static bool Check(int i, int j, int k, long* a, long* b, long* c)
        {
            return (b[i] - b[j]) * (a[j] - a[k]) <= (b[j] - b[k]) * (a[i] - a[j]);
        }
    }

    public static unsafe class Smawk
    {
        public static long Run(int n, int m, long* a, int* row, int* col, long* res)
        {
            for (int i = 0; i < n; i++)
            {
                long best = long.MaxValue;
                int bestJ = -1;
                for (int j = 0; j < m; j++)
                {
                    if (row[i] == 1 || col[j] == 1)
                    {
                        if (a[i * m + j] < best)
                        {
                            best = a[i * m + j];
                            bestJ = j;
                        }
                    }
                }
                res[i] = best;
            }
            return 0;
        }
    }

    public static unsafe class LineContainerAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long IntersectX(long m1, long b1, long m2, long b2)
        {
            return (b2 - b1 + m1 - 1) / m1;
        }

        public static void Run(long* m, long* b, int* size, long newM, long newB)
        {
            int sz = *size;
            while (sz > 0)
            {
                long x = IntersectX(newM, newB, m[sz - 1], b[sz - 1]);
                if (sz == 1 || x > m[sz - 1] * (sz > 1 ? 1 : 0) + b[sz - 1])
                {
                    m[sz] = newM;
                    b[sz] = newB;
                    *size = sz + 1;
                    return;
                }
                sz--;
            }
            m[0] = newM;
            b[0] = newB;
            *size = 1;
        }
    }

    public static unsafe class LineContainerQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* ptr, long* m, long* b, int size, long x)
        {
            int pos = *ptr;
            int lo = 0, hi = size - 1;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (m[mid] * x + b[mid] <= m[mid + 1] * x + b[mid + 1])
                    hi = mid;
                else
                    lo = mid + 1;
            }
            *ptr = lo;
            return m[lo] * x + b[lo];
        }
    }
}