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
            int mid = l + ((r - l) >> 1);
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
        public static int RunInt32(int* tree, int* left, int* right, int prev, int lIn, int rIn, int idx, int val)
        {
            int first = ++tree[0];
            int node = first;
            int prevNode = prev;
            int l = lIn, r = rIn;

            // Path of allocated ancestors for the bottom-up sum recompute.
            // Depth is bounded by log2 of the int range (<= 31), so 64 is safe.
            int* path = stackalloc int[64];
            int depth = 0;

            while (true)
            {
                if (prevNode != 0)
                {
                    left[node] = left[prevNode];
                    right[node] = right[prevNode];
                    tree[node] = tree[prevNode];
                }
                else
                {
                    left[node] = 0;
                    right[node] = 0;
                    tree[node] = 0;
                }

                if (l == r)
                {
                    tree[node] = val;
                    break;
                }

                int mid = l + ((r - l) >> 1);
                int child = ++tree[0];
                int prevChild;
                if (idx <= mid)
                {
                    left[node] = child;
                    prevChild = prevNode != 0 ? left[prevNode] : 0;
                    r = mid;
                }
                else
                {
                    right[node] = child;
                    prevChild = prevNode != 0 ? right[prevNode] : 0;
                    l = mid + 1;
                }
                path[depth++] = node;
                prevNode = prevChild;
                node = child;
            }

            while (depth > 0)
            {
                int pn = path[--depth];
                tree[pn] = tree[left[pn]] + tree[right[pn]];
            }
            return first;
        }
    }

    public static unsafe class PersistentSegmentQuery
    {
        public static int RunInt32(int* tree, int* left, int* right, int node, int l, int r, int ql, int qr)
        {
            if (node == 0 || qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = l + ((r - l) >> 1);
            return RunInt32(tree, left, right, left[node], l, mid, ql, qr) +
                   RunInt32(tree, left, right, right[node], mid + 1, r, ql, qr);
        }
    }

    public static unsafe class DynamicSegmentUpdate
    {
        public static void Run(int* tree, int* left, int* right, int* alloc, int node, int lIn, int rIn, int idx, int val)
        {
            int* path = stackalloc int[64];
            int depth = 0;
            int l = lIn, r = rIn;
            while (l != r)
            {
                int mid = l + ((r - l) >> 1);
                if (idx <= mid)
                {
                    if (left[node] == 0) left[node] = ++(*alloc);
                    path[depth++] = node;
                    node = left[node];
                    r = mid;
                }
                else
                {
                    if (right[node] == 0) right[node] = ++(*alloc);
                    path[depth++] = node;
                    node = right[node];
                    l = mid + 1;
                }
            }
            tree[node] = val;
            while (depth > 0)
            {
                int pn = path[--depth];
                tree[pn] = tree[left[pn]] + tree[right[pn]];
            }
        }
    }

    public static unsafe class DynamicSegmentQuery
    {
        public static int Run(int* tree, int* left, int* right, int node, int l, int r, int ql, int qr)
        {
            if (node == 0 || qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = l + ((r - l) >> 1);
            return Run(tree, left, right, left[node], l, mid, ql, qr) +
                   Run(tree, left, right, right[node], mid + 1, r, ql, qr);
        }
    }
}