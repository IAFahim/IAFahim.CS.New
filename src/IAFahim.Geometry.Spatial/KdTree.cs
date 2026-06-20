namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KdTree
    {
        public struct Node
        {
            public double X, Y;
            public int PointIndex;
            public int Left, Right;
            public int Axis;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Node* nodes)
        {
            if (n == 0) return 0;

            int* indices = stackalloc int[n];
            for (int i = 0; i < n; i++) indices[i] = i;

            int nodeCount = 0;
            BuildRecursive(xs, ys, nodes, indices, n, 0, ref nodeCount);
            return nodeCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BuildRecursive(double* xs, double* ys, Node* nodes, int* indices, int count, int depth, ref int nodeCount)
        {
            if (count == 0) return -1;

            int axis = depth % 2;
            
            // Sort indices based on axis (heapsort, O(n log n))
            SortIndicesByAxis(xs, ys, indices, count, axis);

            int mid = count / 2;
            int u = nodeCount++;

            nodes[u].PointIndex = indices[mid];
            nodes[u].X = xs[indices[mid]];
            nodes[u].Y = ys[indices[mid]];
            nodes[u].Axis = axis;

            nodes[u].Left = BuildRecursive(xs, ys, nodes, indices, mid, depth + 1, ref nodeCount);

            int rightCount = count - mid - 1;
            nodes[u].Right = BuildRecursive(xs, ys, nodes, indices + mid + 1, rightCount, depth + 1, ref nodeCount);

            return u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double AxisKey(double* xs, double* ys, int idx, int axis) => axis == 0 ? xs[idx] : ys[idx];

        private static void SortIndicesByAxis(double* xs, double* ys, int* indices, int count, int axis)
        {
            for (int i = (count >> 1) - 1; i >= 0; i--) SiftDown(xs, ys, indices, i, count, axis);
            for (int end = count - 1; end > 0; end--)
            {
                int t = indices[0]; indices[0] = indices[end]; indices[end] = t;
                SiftDown(xs, ys, indices, 0, end, axis);
            }
        }

        private static void SiftDown(double* xs, double* ys, int* a, int i, int n, int axis)
        {
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = i;
                if (l < n && AxisKey(xs, ys, a[l], axis) > AxisKey(xs, ys, a[m], axis)) m = l;
                if (r < n && AxisKey(xs, ys, a[r], axis) > AxisKey(xs, ys, a[m], axis)) m = r;
                if (m == i) break;
                int t = a[i]; a[i] = a[m]; a[m] = t;
                i = m;
            }
        }
    }
}
