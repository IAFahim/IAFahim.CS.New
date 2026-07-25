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
            int c = node * 2;
            int c1 = c + 1;
            RunInt32(arr, tree, c, l, mid);
            RunInt32(arr, tree, c1, mid + 1, r);
            tree[node] = tree[c] + tree[c1];
        }

        public static void RunInt64(long* arr, long* tree, int node, int l, int r)
        {
            if (l == r) { tree[node] = arr[l]; return; }
            int mid = (l + r) >> 1;
            int c = node * 2;
            int c1 = c + 1;
            RunInt64(arr, tree, c, l, mid);
            RunInt64(arr, tree, c1, mid + 1, r);
            tree[node] = tree[c] + tree[c1];
        }
    }

    public static unsafe class RollbackSegUpdate
    {
        // histType 0 = tree[node], 1 = lazy[node]. One logical push = one slot; *top advances by 1.
        // Lazy lives on the node itself (standard): full cover updates tree[node] and lazy[node].
        // Query is non-mutating and threads ancestor lazy as a carry so rollback history stays pure.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HistPush(int* histNode, long* histVal, byte* histType, int* top, int node, long old, byte type)
        {
            int t = *top;
            histNode[t] = node;
            histVal[t] = old;
            histType[t] = type;
            *top = t + 1;
        }

        public static void RangeAddInt64(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                HistPush(histNode, histVal, histType, top, node, tree[node], 0);
                tree[node] += val * (r - l + 1);
                if (l != r)
                {
                    HistPush(histNode, histVal, histType, top, node, lazy[node], 1);
                    lazy[node] += val;
                }
                return;
            }
            int mid = (l + r) >> 1;
            int c = node * 2;
            int c1 = c + 1;
            RangeAddInt64(tree, lazy, histNode, histVal, histType, top, c, l, mid, ql, qr, val);
            RangeAddInt64(tree, lazy, histNode, histVal, histType, top, c1, mid + 1, r, ql, qr, val);
            HistPush(histNode, histVal, histType, top, node, tree[node], 0);
            // children tree[] already include their own lazy; add this node's pending lazy once.
            tree[node] = tree[c] + tree[c1] + lazy[node] * (r - l + 1);
        }

        public static void PointSetInt64(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top, int node, int l, int r, int idx, long val)
        {
            if (l == r)
            {
                HistPush(histNode, histVal, histType, top, node, tree[node], 0);
                tree[node] = val;
                return;
            }
            // Point set under a pending range-add: force the add into children first so the
            // leaf write does not discard a still-pending parent lazy.
            if (lazy[node] != 0)
            {
                long add = lazy[node];
                int c = node * 2;
                int c1 = c + 1;
                int mid = (l + r) >> 1;
                HistPush(histNode, histVal, histType, top, c, tree[c], 0);
                HistPush(histNode, histVal, histType, top, c, lazy[c], 1);
                tree[c] += add * (mid - l + 1);
                lazy[c] += add;
                HistPush(histNode, histVal, histType, top, c1, tree[c1], 0);
                HistPush(histNode, histVal, histType, top, c1, lazy[c1], 1);
                tree[c1] += add * (r - mid);
                lazy[c1] += add;
                HistPush(histNode, histVal, histType, top, node, lazy[node], 1);
                lazy[node] = 0;
            }
            int mid2 = (l + r) >> 1;
            int ch = node * 2;
            int ch1 = ch + 1;
            if (idx <= mid2) PointSetInt64(tree, lazy, histNode, histVal, histType, top, ch, l, mid2, idx, val);
            else PointSetInt64(tree, lazy, histNode, histVal, histType, top, ch1, mid2 + 1, r, idx, val);
            HistPush(histNode, histVal, histType, top, node, tree[node], 0);
            tree[node] = tree[ch] + tree[ch1] + lazy[node] * (r - l + 1);
        }

        // Back-compat: point set without lazy (safe only when RangeAdd was never used).
        public static void PointSetInt64(long* tree, int* histNode, long* histVal, byte* histType, int* top, int node, int l, int r, int idx, long val)
        {
            if (l == r)
            {
                HistPush(histNode, histVal, histType, top, node, tree[node], 0);
                tree[node] = val;
                return;
            }
            int mid = (l + r) >> 1;
            int c = node * 2;
            int c1 = c + 1;
            if (idx <= mid) PointSetInt64(tree, histNode, histVal, histType, top, c, l, mid, idx, val);
            else PointSetInt64(tree, histNode, histVal, histType, top, c1, mid + 1, r, idx, val);
            HistPush(histNode, histVal, histType, top, node, tree[node], 0);
            tree[node] = tree[c] + tree[c1];
        }
    }

    public static unsafe class RollbackSegQuery
    {
        // Non-mutating: tree[node] already includes lazy[node]; ancestor pending is `add`.
        public static long RangeSumInt64(long* tree, long* lazy, int node, int l, int r, int ql, int qr)
        {
            return RangeSumRec(tree, lazy, node, l, r, ql, qr, 0);
        }

        private static long RangeSumRec(long* tree, long* lazy, int node, int l, int r, int ql, int qr, long add)
        {
            if (qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node] + add * (r - l + 1);
            int mid = (l + r) >> 1;
            long childAdd = add + lazy[node];
            return RangeSumRec(tree, lazy, node * 2, l, mid, ql, qr, childAdd) +
                   RangeSumRec(tree, lazy, node * 2 + 1, mid + 1, r, ql, qr, childAdd);
        }
    }

    public static unsafe class RollbackSegRollback
    {
        // Each history slot is independent (tree OR lazy for one node). Undo one slot at a time.
        public static void Run(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top, int checkpoint)
        {
            while (*top > checkpoint)
            {
                UndoLast(tree, lazy, histNode, histVal, histType, top);
            }
        }

        public static void UndoLast(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top)
        {
            if (*top < 1) return;
            int t = --(*top);
            long val = histVal[t];
            int node = histNode[t];
            byte type = histType[t];
            if (type == 0) tree[node] = val;
            else lazy[node] = val;
        }

        public static int GetCheckpoint(int* top) => *top;
    }

    public static unsafe class DynamicLiChaoAdd
    {
        public static void Run(long* seg, int* left, int* right, long m, long b, int node, long xl, long xr)
        {
            long mid = (xl + xr) >> 1;
            int idx2 = node * 2;
            int idx2b = idx2 + 1;
            long curMid = seg[idx2] * mid + seg[idx2b];
            long newMid = m * mid + b;
            if (newMid < curMid)
            {
                long oldM = seg[idx2];
                long oldB = seg[idx2b];
                seg[idx2] = m;
                seg[idx2b] = b;
                m = oldM;
                b = oldB;
            }
            if (xr - xl <= 1) return;
            long sm = seg[idx2];
            long sb = seg[idx2b];
            if (m * xl + b < sm * xl + sb)
            {
                if (left[node] == 0) { left[node] = (int)++seg[0]; }
                Run(seg, left, right, m, b, left[node], xl, mid);
            }
            else if (m * xr + b < sm * xr + sb)
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
            int b2 = node * 2;
            long res = seg[b2] * x + seg[b2 + 1];
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
            int hiBound = r < mid - 1 ? r : mid - 1;
            long bm = b[mid];
            for (int i = l; i <= hiBound; i++)
            {
                long val = dp[i] + a[i] * bm + c[i];
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
                count += (starts[i] <= point & point <= ends[i]) ? 1 : 0;
            return count;
        }
    }

    public static unsafe class RectangleStabbing
    {
        public static int Run(int* x1, int* y1, int* x2, int* y2, int n, int px, int py)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
                count += (x1[i] <= px & px <= x2[i] & y1[i] <= py & py <= y2[i]) ? 1 : 0;
            return count;
        }
    }
}