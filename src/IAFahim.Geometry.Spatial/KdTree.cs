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
            
            // Sort indices based on axis
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    bool swap = false;
                    if (axis == 0 && xs[indices[j]] < xs[indices[i]]) swap = true;
                    if (axis == 1 && ys[indices[j]] < ys[indices[i]]) swap = true;

                    if (swap)
                    {
                        int t = indices[i];
                        indices[i] = indices[j];
                        indices[j] = t;
                    }
                }
            }

            int mid = count / 2;
            int u = nodeCount++;
            
            nodes[u].PointIndex = indices[mid];
            nodes[u].X = xs[indices[mid]];
            nodes[u].Y = ys[indices[mid]];
            nodes[u].Axis = axis;

            int* leftIndices = stackalloc int[mid];
            for (int i = 0; i < mid; i++) leftIndices[i] = indices[i];
            nodes[u].Left = BuildRecursive(xs, ys, nodes, leftIndices, mid, depth + 1, ref nodeCount);

            int rightCount = count - mid - 1;
            int* rightIndices = stackalloc int[rightCount];
            for (int i = 0; i < rightCount; i++) rightIndices[i] = indices[mid + 1 + i];
            nodes[u].Right = BuildRecursive(xs, ys, nodes, rightIndices, rightCount, depth + 1, ref nodeCount);

            return u;
        }
    }
}
