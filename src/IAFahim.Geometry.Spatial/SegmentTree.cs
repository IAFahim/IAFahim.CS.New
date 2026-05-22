namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SegmentTree
    {
        public struct Node { public double Lo, Hi; public int Left, Right; public double Min, Max; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build2D(double* xs, double* ys, int n, Node* nodes)
        {
            for (int i = 0; i < n; i++) { nodes[i].Lo = xs[i]; nodes[i].Hi = ys[i]; nodes[i].Left = nodes[i].Right = -1; nodes[i].Min = xs[i]; nodes[i].Max = ys[i]; }
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query2D(Node* nodes, int root, double x1, double y1, double x2, double y2, int* outIdx)
        {
            int count = 0;
            if (root < 0) return count;
            Node n = nodes[root];
            if (n.Min >= x1 && n.Max <= x2) { outIdx[count++] = root; return count; }
            if (n.Min > x2 || n.Max < x1) return count;
            if (n.Left >= 0) count += Query2D(nodes, n.Left, x1, y1, x2, y2, outIdx + count);
            if (n.Right >= 0) count += Query2D(nodes, n.Right, x1, y1, x2, y2, outIdx + count);
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
            return Query2D(nodes, root, x1, y2, x2, y2, outIdx);
        }
    }
}
