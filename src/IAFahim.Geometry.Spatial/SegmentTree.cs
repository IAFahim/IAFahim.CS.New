namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SegmentTree
    {
        private const int NullNode = -1;

        public struct Node
        {
            public double Lo, Hi;
            public int Left, Right;
            public double Min, Max;
            public int OriginalIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool OverlapsRange(double lo, double hi, double queryLow, double queryHigh)
            => lo <= queryHigh && hi >= queryLow;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PartitionByX(double* xs, int* idx, int start, int end, int k)
        {
            int l = start, r = end;
            while (l < r)
            {
                double pivot = xs[idx[k]];
                int i = l, j = r;
                while (i <= j)
                {
                    while (xs[idx[i]] < pivot) i++;
                    while (xs[idx[j]] > pivot) j--;
                    if (i <= j)
                    {
                        int t = idx[i]; idx[i] = idx[j]; idx[j] = t;
                        i++; j--;
                    }
                }
                if (j < k) l = i;
                if (k < i) r = j;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AllocNode(Node* nodes, ref int nextFree, double* xs, double* ys, int* idx, int mid)
        {
            int rootIdx = nextFree++;
            int originalId = idx[mid];
            nodes[rootIdx].Lo = xs[originalId];
            nodes[rootIdx].Hi = ys[originalId];
            nodes[rootIdx].OriginalIndex = originalId;
            nodes[rootIdx].Min = xs[originalId];
            nodes[rootIdx].Max = ys[originalId];
            return rootIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MergeChildBounds(Node* nodes, int rootIdx)
        {
            if (nodes[rootIdx].Left >= 0)
            {
                Node* left = &nodes[nodes[rootIdx].Left];
                if (left->Min < nodes[rootIdx].Min) nodes[rootIdx].Min = left->Min;
                if (left->Max > nodes[rootIdx].Max) nodes[rootIdx].Max = left->Max;
            }
            if (nodes[rootIdx].Right >= 0)
            {
                Node* right = &nodes[nodes[rootIdx].Right];
                if (right->Min < nodes[rootIdx].Min) nodes[rootIdx].Min = right->Min;
                if (right->Max > nodes[rootIdx].Max) nodes[rootIdx].Max = right->Max;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build2D(double* xs, double* ys, int n, Node* nodes)
        {
            if (n <= 0) return NullNode;
            int* idx = stackalloc int[n];
            for (int i = 0; i < n; i++) idx[i] = i;
            int nextFree = 0;
            return BuildRec(xs, ys, idx, 0, n - 1, nodes, ref nextFree);
        }

        private static int BuildRec(double* xs, double* ys, int* idx, int start, int end, Node* nodes, ref int nextFree)
        {
            if (start > end) return NullNode;
            int mid = start + (end - start) / 2;
            PartitionByX(xs, idx, start, end, mid);
            int rootIdx = AllocNode(nodes, ref nextFree, xs, ys, idx, mid);
            nodes[rootIdx].Left = BuildRec(xs, ys, idx, start, mid - 1, nodes, ref nextFree);
            nodes[rootIdx].Right = BuildRec(xs, ys, idx, mid + 1, end, nodes, ref nextFree);
            MergeChildBounds(nodes, rootIdx);
            return rootIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int QueryChild(Node* nodes, int child, double x1, double y1, double x2, double y2, int* outIdx)
        {
            if (child < 0) return 0;
            Node* c = &nodes[child];
            if (!OverlapsRange(c->Min, c->Max, x1, x2)) return 0;
            return Query2D(nodes, child, x1, y1, x2, y2, outIdx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query2D(Node* nodes, int root, double x1, double y1, double x2, double y2, int* outIdx)
        {
            int count = 0;
            if (root < 0) return count;
            Node* n = &nodes[root];
            if (OverlapsRange(n->Lo, n->Hi, x1, x2)) outIdx[count++] = n->OriginalIndex;
            count += QueryChild(nodes, n->Left, x1, y1, x2, y2, outIdx + count);
            count += QueryChild(nodes, n->Right, x1, y1, x2, y2, outIdx + count);
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build3D(double* xs, double* ys, double* zs, int n, Node* nodes)
        {
            return Build2D(xs, ys, n, nodes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query3D(Node* nodes, int root, double x1, double y1, double z1, double x2, double y2, double z2, int* outIdx)
        {
            return Query2D(nodes, root, x1, y1, x2, y2, outIdx);
        }
    }
}
