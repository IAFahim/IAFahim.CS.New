namespace IAFahim.DS.SegmentTree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SegmentTreeBuild
    {
        public static void RunInt32(int* arr, int* tree, int node, int l, int r)
        {
            if (l == r)
            {
                tree[node] = arr[l];
                return;
            }
            int mid = (l + r) >> 1;
            RunInt32(arr, tree, node * 2, l, mid);
            RunInt32(arr, tree, node * 2 + 1, mid + 1, r);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        public static void RunInt64(long* arr, long* tree, int node, int l, int r)
        {
            if (l == r)
            {
                tree[node] = arr[l];
                return;
            }
            int mid = (l + r) >> 1;
            RunInt64(arr, tree, node * 2, l, mid);
            RunInt64(arr, tree, node * 2 + 1, mid + 1, r);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class SegmentTreeSet
    {
        public static void RunInt32(int* tree, int node, int l, int r, int idx, int val)
        {
            if (l == r)
            {
                tree[node] = val;
                return;
            }
            int mid = (l + r) >> 1;
            if (idx <= mid)
                RunInt32(tree, node * 2, l, mid, idx, val);
            else
                RunInt32(tree, node * 2 + 1, mid + 1, r, idx, val);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        public static void RunInt64(long* tree, int node, int l, int r, int idx, long val)
        {
            if (l == r)
            {
                tree[node] = val;
                return;
            }
            int mid = (l + r) >> 1;
            if (idx <= mid)
                RunInt64(tree, node * 2, l, mid, idx, val);
            else
                RunInt64(tree, node * 2 + 1, mid + 1, r, idx, val);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class SegmentTreeAdd
    {
        public static void RunInt32(int* tree, int node, int l, int r, int idx, int val)
        {
            if (l == r)
            {
                tree[node] += val;
                return;
            }
            int mid = (l + r) >> 1;
            if (idx <= mid)
                RunInt32(tree, node * 2, l, mid, idx, val);
            else
                RunInt32(tree, node * 2 + 1, mid + 1, r, idx, val);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class SegmentTreeQuery
    {
        public static int RunInt32(int* tree, int node, int l, int r, int ql, int qr)
        {
            if (qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = (l + r) >> 1;
            return RunInt32(tree, node * 2, l, mid, ql, qr) +
                   RunInt32(tree, node * 2 + 1, mid + 1, r, ql, qr);
        }

        public static long RunInt64(long* tree, int node, int l, int r, int ql, int qr)
        {
            if (qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = (l + r) >> 1;
            return RunInt64(tree, node * 2, l, mid, ql, qr) +
                   RunInt64(tree, node * 2 + 1, mid + 1, r, ql, qr);
        }
    }

    public static unsafe class SegmentTreeMaxRight
    {
        public static int Run(int* tree, int n, int l, long target)
        {
            int node = 1, lo = 0, hi = n - 1;
            if (tree[node] < target) return n;
            while (lo != hi)
            {
                int mid = (lo + hi) >> 1;
                if (tree[node * 2] >= target)
                {
                    node = node * 2;
                    hi = mid;
                }
                else
                {
                    target -= tree[node * 2];
                    node = node * 2 + 1;
                    lo = mid + 1;
                }
            }
            return lo;
        }
    }

    public static unsafe class SegmentTreeMinLeft
    {
        public static int Run(int* tree, int n, int r, long target)
        {
            int node = 1, lo = 0, hi = n - 1;
            if (tree[node] < target) return -1;
            while (lo != hi)
            {
                int mid = (lo + hi) >> 1;
                if (tree[node * 2 + 1] >= target)
                {
                    node = node * 2 + 1;
                    lo = mid + 1;
                }
                else
                {
                    target -= tree[node * 2 + 1];
                    node = node * 2;
                    hi = mid;
                }
            }
            return lo;
        }
    }
}