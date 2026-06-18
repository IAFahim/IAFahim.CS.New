namespace IAFahim.DS.SegmentTree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LazySegmentBuild
    {
        public static void RunInt32(int* arr, int* tree, int node, int l, int r)
        {
            if (l == r)
            {
                tree[node] = arr[l];
                return;
            }
            int mid = l + ((r - l) >> 1);
            RunInt32(arr, tree, node * 2, l, mid);
            RunInt32(arr, tree, node * 2 + 1, mid + 1, r);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class LazySegmentApply
    {
        public static void RangeAddInt32(int* tree, int* lazy, int node, int l, int r, int ql, int qr, int val)
        {
            PushInt32(tree, lazy, node, l, r);
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                tree[node] += val * (r - l + 1);
                if (l != r)
                {
                    lazy[node * 2] += val;
                    lazy[node * 2 + 1] += val;
                }
                return;
            }
            int mid = l + ((r - l) >> 1);
            RangeAddInt32(tree, lazy, node * 2, l, mid, ql, qr, val);
            RangeAddInt32(tree, lazy, node * 2 + 1, mid + 1, r, ql, qr, val);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        public static void RangeSetInt32(int* tree, int* lazy, bool* hasLazy, int node, int l, int r, int ql, int qr, int val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                tree[node] = val * (r - l + 1);
                if (l != r)
                {
                    lazy[node * 2] = val;
                    lazy[node * 2 + 1] = val;
                    hasLazy[node * 2] = true;
                    hasLazy[node * 2 + 1] = true;
                }
                hasLazy[node] = false;
                return;
            }
            PushSetInt32(tree, lazy, hasLazy, node, l, r);
            int mid = l + ((r - l) >> 1);
            RangeSetInt32(tree, lazy, hasLazy, node * 2, l, mid, ql, qr, val);
            RangeSetInt32(tree, lazy, hasLazy, node * 2 + 1, mid + 1, r, ql, qr, val);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        private static void PushInt32(int* tree, int* lazy, int node, int l, int r)
        {
            if (lazy[node] != 0 && l != r)
            {
                int mid = l + ((r - l) >> 1);
                tree[node * 2] += lazy[node] * (mid - l + 1);
                tree[node * 2 + 1] += lazy[node] * (r - mid);
                lazy[node * 2] += lazy[node];
                lazy[node * 2 + 1] += lazy[node];
                lazy[node] = 0;
            }
        }

        private static void PushSetInt32(int* tree, int* lazy, bool* hasLazy, int node, int l, int r)
        {
            if (hasLazy[node] && l != r)
            {
                int mid = l + ((r - l) >> 1);
                tree[node * 2] = lazy[node] * (mid - l + 1);
                tree[node * 2 + 1] = lazy[node] * (r - mid);
                lazy[node * 2] = lazy[node];
                lazy[node * 2 + 1] = lazy[node];
                hasLazy[node * 2] = true;
                hasLazy[node * 2 + 1] = true;
                hasLazy[node] = false;
            }
        }
    }

    public static unsafe class LazySegmentPush
    {
        public static void Run(int* tree, int* lazy, int node, int l, int r)
        {
            if (lazy[node] != 0 && l != r)
            {
                int mid = l + ((r - l) >> 1);
                tree[node * 2] += lazy[node] * (mid - l + 1);
                tree[node * 2 + 1] += lazy[node] * (r - mid);
                lazy[node * 2] += lazy[node];
                lazy[node * 2 + 1] += lazy[node];
                lazy[node] = 0;
            }
        }
    }

    public static unsafe class LazySegmentPull
    {
        public static void Run(int* tree, int node)
        {
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class LazySegmentQuery
    {
        public static int RangeSumInt32(int* tree, int* lazy, int node, int l, int r, int ql, int qr)
        {
            if (qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = l + ((r - l) >> 1);
            LazySegmentPush.Run(tree, lazy, node, l, r);
            return RangeSumInt32(tree, lazy, node * 2, l, mid, ql, qr) +
                   RangeSumInt32(tree, lazy, node * 2 + 1, mid + 1, r, ql, qr);
        }
    }

    public static unsafe class LazySegmentUpdate
    {
        public static void RangeAddInt32(int* tree, int* lazy, int node, int l, int r, int ql, int qr, int val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                tree[node] += val * (r - l + 1);
                if (l != r)
                {
                    lazy[node * 2] += val;
                    lazy[node * 2 + 1] += val;
                }
                return;
            }
            int mid = l + ((r - l) >> 1);
            LazySegmentPush.Run(tree, lazy, node, l, r);
            RangeAddInt32(tree, lazy, node * 2, l, mid, ql, qr, val);
            RangeAddInt32(tree, lazy, node * 2 + 1, mid + 1, r, ql, qr, val);
            LazySegmentPull.Run(tree, node);
        }
    }
}