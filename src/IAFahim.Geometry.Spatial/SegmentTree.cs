namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SegmentTree
    {
        public struct Node
        {
            public double Lo, Hi;
            public int Left, Right;
            public double Min, Max;
            public int OriginalIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build2D(double* xs, double* ys, int n, Node* nodes)
        {
            if (n <= 0) return -1;
            int* idx = stackalloc int[n];
            for (int i = 0; i < n; i++) idx[i] = i;
            int nextFree = 0;
            return BuildRec(xs, ys, idx, 0, n - 1, nodes, ref nextFree);
        }

        private static int BuildRec(double* xs, double* ys, int* idx, int start, int end, Node* nodes, ref int nextFree)
        {
            if (start > end) return -1;
            int mid = start + (end - start) / 2;

            int k = mid;
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

            int rootIdx = nextFree++;
            int originalId = idx[mid];
            nodes[rootIdx].Lo = xs[originalId];
            nodes[rootIdx].Hi = ys[originalId];
            nodes[rootIdx].OriginalIndex = originalId;
            nodes[rootIdx].Min = xs[originalId];
            nodes[rootIdx].Max = ys[originalId];

            nodes[rootIdx].Left = BuildRec(xs, ys, idx, start, mid - 1, nodes, ref nextFree);
            nodes[rootIdx].Right = BuildRec(xs, ys, idx, mid + 1, end, nodes, ref nextFree);

            if (nodes[rootIdx].Left >= 0)
            {
                if (nodes[nodes[rootIdx].Left].Min < nodes[rootIdx].Min) nodes[rootIdx].Min = nodes[nodes[rootIdx].Left].Min;
                if (nodes[nodes[rootIdx].Left].Max > nodes[rootIdx].Max) nodes[rootIdx].Max = nodes[nodes[rootIdx].Left].Max;
            }
            if (nodes[rootIdx].Right >= 0)
            {
                if (nodes[nodes[rootIdx].Right].Min < nodes[rootIdx].Min) nodes[rootIdx].Min = nodes[nodes[rootIdx].Right].Min;
                if (nodes[nodes[rootIdx].Right].Max > nodes[rootIdx].Max) nodes[rootIdx].Max = nodes[nodes[rootIdx].Right].Max;
            }

            return rootIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query2D(Node* nodes, int root, double x1, double y1, double x2, double y2, int* outIdx)
        {
            int count = 0;
            if (root < 0) return count;
            Node* n = &nodes[root];

            if (n->Lo <= x2 && n->Hi >= x1)
            {
                outIdx[count++] = n->OriginalIndex;
            }

            if (n->Left >= 0 && nodes[n->Left].Min <= x2 && nodes[n->Left].Max >= x1)
            {
                count += Query2D(nodes, n->Left, x1, y1, x2, y2, outIdx + count);
            }
            if (n->Right >= 0 && nodes[n->Right].Min <= x2 && nodes[n->Right].Max >= x1)
            {
                count += Query2D(nodes, n->Right, x1, y1, x2, y2, outIdx + count);
            }

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
