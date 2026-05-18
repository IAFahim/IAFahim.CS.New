namespace IAFahim.DS.SegmentTree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PersistentSegmentBuild
    {
        public static int RunInt32(int* arr, int* roots, int* left, int* right, int* tree, int prev, int l, int r)
        {
            int node = ++tree[0];
            if (prev != 0)
            {
                left[node] = left[prev];
                right[node] = right[prev];
                tree[node] = tree[prev];
            }
            if (l == r)
            {
                tree[node] = arr[l];
                return node;
            }
            int mid = (l + r) >> 1;
            int leftChild = RunInt32(arr, roots, left, right, tree, prev == 0 ? 0 : left[prev], l, mid);
            int rightChild = RunInt32(arr, roots, left, right, tree, prev == 0 ? 0 : right[prev], mid + 1, r);
            left[node] = leftChild;
            right[node] = rightChild;
            tree[node] = tree[leftChild] + tree[rightChild];
            return node;
        }
    }

    public static unsafe class PersistentSegmentUpdate
    {
        public static int RunInt32(int* tree, int* left, int* right, int prev, int l, int r, int idx, int val)
        {
            int node = ++tree[0];
            if (prev != 0)
            {
                left[node] = left[prev];
                right[node] = right[prev];
                tree[node] = tree[prev];
            }
            if (l == r)
            {
                tree[node] = val;
                return node;
            }
            int mid = (l + r) >> 1;
            if (idx <= mid)
                left[node] = RunInt32(tree, left, right, prev == 0 ? 0 : left[prev], l, mid, idx, val);
            else
                right[node] = RunInt32(tree, left, right, prev == 0 ? 0 : right[prev], mid + 1, r, idx, val);
            tree[node] = tree[left[node]] + tree[right[node]];
            return node;
        }
    }

    public static unsafe class PersistentSegmentQuery
    {
        public static int RunInt32(int* tree, int* left, int* right, int node, int l, int r, int ql, int qr)
        {
            if (node == 0 || qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = (l + r) >> 1;
            return RunInt32(tree, left, right, left[node], l, mid, ql, qr) +
                   RunInt32(tree, left, right, right[node], mid + 1, r, ql, qr);
        }
    }

    public static unsafe class DynamicSegmentUpdate
    {
        public static void Run(int* tree, int* left, int* right, int* alloc, int node, int l, int r, int idx, int val)
        {
            if (l == r)
            {
                tree[node] = val;
                return;
            }
            int mid = (l + r) >> 1;
            if (idx <= mid)
            {
                if (left[node] == 0) left[node] = ++(*alloc);
                Run(tree, left, right, alloc, left[node], l, mid, idx, val);
            }
            else
            {
                if (right[node] == 0) right[node] = ++(*alloc);
                Run(tree, left, right, alloc, right[node], mid + 1, r, idx, val);
            }
            tree[node] = tree[left[node]] + tree[right[node]];
        }
    }

    public static unsafe class DynamicSegmentQuery
    {
        public static int Run(int* tree, int* left, int* right, int node, int l, int r, int ql, int qr)
        {
            if (node == 0 || qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = (l + r) >> 1;
            return Run(tree, left, right, left[node], l, mid, ql, qr) +
                   Run(tree, left, right, right[node], mid + 1, r, ql, qr);
        }
    }
}