namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RangeTree
    {
        public struct Node2D { public double Key; public int Left, Right; public int SubTree; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build2D(double* xs, double* ys, int n, Node2D* nodes)
        {
            for (int i = 0; i < n; i++) { nodes[i].Key = xs[i]; nodes[i].Left = nodes[i].Right = -1; nodes[i].SubTree = i; }
            for (int i = 1; i < n; i++)
            {
                int p = i - 1;
                while (p >= 0 && nodes[p].Key > nodes[i].Key)
                {
                    nodes[p + 1] = nodes[p];
                    p--;
                }
                nodes[p + 1].Key = xs[i];
                nodes[p + 1].SubTree = i;
            }
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query2D(Node2D* nodes, int root, double x1, double x2, int* outIdx)
        {
            int count = 0;
            if (root < 0) return count;
            Node2D n = nodes[root];
            if (n.Key >= x1 && n.Key <= x2) outIdx[count++] = n.SubTree;
            if (n.Key > x1 && n.Left >= 0) count += Query2D(nodes, n.Left, x1, x2, outIdx + count);
            if (n.Key < x2 && n.Right >= 0) count += Query2D(nodes, n.Right, x1, x2, outIdx + count);
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build3D(double* xs, double* ys, double* zs, int n, Node2D* nodes)
        {
            return Build2D(xs, ys, n, nodes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query3D(Node2D* nodes, int root, double x1, double y1, double z1, double x2, double y2, double z2, int* outIdx)
        {
            return Query2D(nodes, root, x1, x2, outIdx);
        }
    }
}
