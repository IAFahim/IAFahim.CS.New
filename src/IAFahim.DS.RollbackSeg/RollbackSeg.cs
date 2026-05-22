namespace IAFahim.DS.RollbackSeg
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RollbackSegBuild
    {
        public static void RunInt32(int* arr, int* tree, int node, int l, int r)
        {
            if (l == r) { tree[node] = arr[l]; return; }
            int mid = (l + r) >> 1;
            RunInt32(arr, tree, node * 2, l, mid);
            RunInt32(arr, tree, node * 2 + 1, mid + 1, r);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        public static void RunInt64(long* arr, long* tree, int node, int l, int r)
        {
            if (l == r) { tree[node] = arr[l]; return; }
            int mid = (l + r) >> 1;
            RunInt64(arr, tree, node * 2, l, mid);
            RunInt64(arr, tree, node * 2 + 1, mid + 1, r);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class RollbackSegUpdate
    {
        public static void RangeAddInt64(long* tree, long* lazy, int* histNode, long* histVal, int* top, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                histNode[*top] = node;
                histVal[*top] = tree[node];
                (*top)++;
                tree[node] += val * (r - l + 1);
                if (l != r)
                {
                    histNode[*top] = node * 2;
                    histVal[*top] = lazy[node * 2];
                    (*top)++;
                    histNode[*top] = node * 2 + 1;
                    histVal[*top] = lazy[node * 2 + 1];
                    (*top)++;
                    lazy[node * 2] += val;
                    lazy[node * 2 + 1] += val;
                }
                return;
            }
            int mid = (l + r) >> 1;
            RangeAddInt64(tree, lazy, histNode, histVal, top, node * 2, l, mid, ql, qr, val);
            RangeAddInt64(tree, lazy, histNode, histVal, top, node * 2 + 1, mid + 1, r, ql, qr, val);
            histNode[*top] = node;
            histVal[*top] = tree[node];
            (*top)++;
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        public static void PointSetInt64(long* tree, int* histNode, long* histVal, int* top, int node, int l, int r, int idx, long val)
        {
            if (l == r)
            {
                histNode[*top] = node;
                histVal[*top] = tree[node];
                (*top)++;
                tree[node] = val;
                return;
            }
            int mid = (l + r) >> 1;
            if (idx <= mid) PointSetInt64(tree, histNode, histVal, top, node * 2, l, mid, idx, val);
            else PointSetInt64(tree, histNode, histVal, top, node * 2 + 1, mid + 1, r, idx, val);
            histNode[*top] = node;
            histVal[*top] = tree[node];
            (*top)++;
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class RollbackSegQuery
    {
        public static long RangeSumInt64(long* tree, long* lazy, int node, int l, int r, int ql, int qr)
        {
            if (qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = (l + r) >> 1;
            if (l != r)
            {
                long val = lazy[node];
                tree[node * 2] += val * (mid - l + 1);
                tree[node * 2 + 1] += val * (r - mid);
                lazy[node * 2] += val;
                lazy[node * 2 + 1] += val;
                lazy[node] = 0;
            }
            return RangeSumInt64(tree, lazy, node * 2, l, mid, ql, qr) +
                   RangeSumInt64(tree, lazy, node * 2 + 1, mid + 1, r, ql, qr);
        }
    }

    public static unsafe class RollbackSegRollback
    {
        public static void Run(long* tree, long* lazy, int* histNode, long* histVal, int* top, int checkpoint)
        {
            while (*top > checkpoint)
            {
                long val = histVal[--(*top)];
                int node = histNode[--(*top)];
                tree[node] = val;
            }
        }

        public static void UndoLast(long* tree, int* histNode, long* histVal, int* top)
        {
            if (*top < 1) return;
            long val = histVal[--(*top)];
            int node = histNode[--(*top)];
            tree[node] = val;
        }

        public static int GetCheckpoint(int* top) => *top;
    }

    public static unsafe class DynamicLiChaoAdd
    {
        public static void Run(long* seg, int* left, int* right, long m, long b, int node, long xl, long xr)
        {
            long mid = (xl + xr) >> 1;
            bool leftBetter = m * xl + b < m * xl + b;
            long curMid = seg[node * 2 + 0] * mid + seg[node * 2 + 1];
            long newMid = m * mid + b;
            if (newMid < curMid)
            {
                long oldM = seg[node * 2 + 0];
                long oldB = seg[node * 2 + 1];
                seg[node * 2 + 0] = m;
                seg[node * 2 + 1] = b;
                m = oldM;
                b = oldB;
            }
            if (xr - xl <= 1) return;
            if (m * xl + b < seg[node * 2 + 0] * xl + seg[node * 2 + 1])
            {
                if (left[node] == 0) { left[node] = (int)++seg[0]; }
                Run(seg, left, right, m, b, left[node], xl, mid);
            }
            else if (m * xr + b < seg[node * 2 + 0] * xr + seg[node * 2 + 1])
            {
                if (right[node] == 0) { int next = (int)++seg[0]; right[node] = next; }
                Run(seg, left, right, m, b, right[node], mid, xr);
            }
        }
    }

    public static unsafe class DynamicLiChaoQuery
    {
        public static long Run(long* seg, int* left, int* right, int node, long xl, long xr, long x)
        {
            long res = seg[node * 2 + 0] * x + seg[node * 2 + 1];
            if (xr - xl <= 1) return res;
            long mid = (xl + xr) >> 1;
            if (x < mid)
            {
                if (left[node] != 0)
                    return Math.Min(res, Run(seg, left, right, left[node], xl, mid, x));
            }
            else
            {
                if (right[node] != 0)
                    return Math.Min(res, Run(seg, left, right, right[node], mid, xr, x));
            }
            return res;
        }
    }

    public static unsafe class DivideConquerHull
    {
        public static void Run(long* dp, long* a, long* b, long* c, int n, int k, int* opt, int lo, int hi, int l, int r)
        {
            if (lo > hi) return;
            int mid = (lo + hi) >> 1;
            int bestK = l;
            long best = long.MaxValue;
            for (int i = l; i <= Math.Min(r, mid - 1); i++)
            {
                long val = dp[i] + a[i] * b[mid] + c[i];
                if (val < best) { best = val; bestK = i; }
            }
            dp[mid] = best;
            opt[mid] = bestK;
            Run(dp, a, b, c, n, k, opt, lo, mid - 1, l, bestK);
            Run(dp, a, b, c, n, k, opt, mid + 1, hi, bestK, r);
        }
    }

    public static unsafe class SegmentTreeDivideConquer
    {
        public static void Run(long* seg, long* dp, long* temp, int node, int l, int r, int ql, int qr, long val, int k)
        {
            if (qr < l || ql > r) return;
            int mid = (l + r) >> 1;
            if (ql <= l && r <= qr)
            {
                seg[node] = Math.Min(seg[node], val);
                return;
            }
            Run(seg, dp, temp, node * 2, l, mid, ql, qr, val, k);
            Run(seg, dp, temp, node * 2 + 1, mid + 1, r, ql, qr, val, k);
        }
    }

    public static unsafe class IntervalStabbing
    {
        public static int Run(int* starts, int* ends, int n, int point)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
                if (starts[i] <= point && point <= ends[i])
                    count++;
            return count;
        }
    }

    public static unsafe class RectangleStabbing
    {
        public static int Run(int* x1, int* y1, int* x2, int* y2, int n, int px, int py)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
                if (x1[i] <= px && px <= x2[i] && y1[i] <= py && py <= y2[i])
                    count++;
            return count;
        }
    }
}