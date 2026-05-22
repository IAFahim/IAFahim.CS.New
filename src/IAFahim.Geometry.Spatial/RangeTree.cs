namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RangeTree
    {
        public struct Node
        {
            public double X, Y, Z;
            public int Left, Right;
            public int OriginalIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build2D(double* xs, double* ys, int n, Node* nodes)
        {
            if (n <= 0) return -1;
            int* idx = stackalloc int[n];
            for (int i = 0; i < n; i++) idx[i] = i;
            int nextFree = 0;
            return BuildRec(xs, ys, null, idx, 0, n - 1, nodes, ref nextFree, 0, false);
        }

        private static int BuildRec(double* xs, double* ys, double* zs, int* idx, int start, int end, Node* nodes, ref int nextFree, int depth, bool is3D)
        {
            if (start > end) return -1;
            int mid = start + (end - start) / 2;

            int axis = is3D ? (depth % 3) : (depth % 2);

            int k = mid;
            int l = start, r = end;
            while (l < r)
            {
                double pivot = axis == 0 ? xs[idx[k]] : (axis == 1 ? ys[idx[k]] : zs[idx[k]]);
                int i = l, j = r;
                while (i <= j)
                {
                    while ((axis == 0 ? xs[idx[i]] : (axis == 1 ? ys[idx[i]] : zs[idx[i]])) < pivot) i++;
                    while ((axis == 0 ? xs[idx[j]] : (axis == 1 ? ys[idx[j]] : zs[idx[j]])) > pivot) j--;
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
            nodes[rootIdx].X = xs[originalId];
            nodes[rootIdx].Y = ys[originalId];
            if (is3D) nodes[rootIdx].Z = zs[originalId];
            nodes[rootIdx].OriginalIndex = originalId;

            nodes[rootIdx].Left = BuildRec(xs, ys, zs, idx, start, mid - 1, nodes, ref nextFree, depth + 1, is3D);
            nodes[rootIdx].Right = BuildRec(xs, ys, zs, idx, mid + 1, end, nodes, ref nextFree, depth + 1, is3D);

            return rootIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query2D(Node* nodes, int root, double x1, double y1, double x2, double y2, int* outIdx)
        {
            int count = 0;
            QueryRec(nodes, root, x1, y1, x2, y2, outIdx, ref count, 0);
            return count;
        }

        private static void QueryRec(Node* nodes, int node, double x1, double y1, double x2, double y2, int* outIdx, ref int count, int depth)
        {
            if (node < 0) return;
            Node* n = &nodes[node];

            bool inRange = n->X >= x1 && n->X <= x2 && n->Y >= y1 && n->Y <= y2;
            if (inRange) outIdx[count++] = n->OriginalIndex;

            int axis = depth % 2;
            double val = axis == 0 ? n->X : n->Y;
            double minV = axis == 0 ? x1 : y1;
            double maxV = axis == 0 ? x2 : y2;

            if (minV <= val) QueryRec(nodes, n->Left, x1, y1, x2, y2, outIdx, ref count, depth + 1);
            if (maxV >= val) QueryRec(nodes, n->Right, x1, y1, x2, y2, outIdx, ref count, depth + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build3D(double* xs, double* ys, double* zs, int n, Node* nodes)
        {
            if (n <= 0) return -1;
            int* idx = stackalloc int[n];
            for (int i = 0; i < n; i++) idx[i] = i;
            int nextFree = 0;
            return BuildRec(xs, ys, zs, idx, 0, n - 1, nodes, ref nextFree, 0, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query3D(Node* nodes, int root, double x1, double y1, double z1, double x2, double y2, double z2, int* outIdx)
        {
            int count = 0;
            QueryRec3D(nodes, root, x1, y1, z1, x2, y2, z2, outIdx, ref count, 0);
            return count;
        }

        private static void QueryRec3D(Node* nodes, int node, double x1, double y1, double z1, double x2, double y2, double z2, int* outIdx, ref int count, int depth)
        {
            if (node < 0) return;
            Node* n = &nodes[node];

            bool inRange = n->X >= x1 && n->X <= x2 && n->Y >= y1 && n->Y <= y2 && n->Z >= z1 && n->Z <= z2;
            if (inRange) outIdx[count++] = n->OriginalIndex;

            int axis = depth % 3;
            double val = axis == 0 ? n->X : (axis == 1 ? n->Y : n->Z);
            double minV = axis == 0 ? x1 : (axis == 1 ? y1 : z1);
            double maxV = axis == 0 ? x2 : (axis == 1 ? y2 : z2);

            if (minV <= val) QueryRec3D(nodes, n->Left, x1, y1, z1, x2, y2, z2, outIdx, ref count, depth + 1);
            if (maxV >= val) QueryRec3D(nodes, n->Right, x1, y1, z1, x2, y2, z2, outIdx, ref count, depth + 1);
        }
    }
}
