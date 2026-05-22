namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KdTree
    {
        public struct Node { public double X, Y; public int Left, Right; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Node* nodes)
        {
            for (int i = 0; i < n; i++) { nodes[i].X = xs[i]; nodes[i].Y = ys[i]; nodes[i].Left = nodes[i].Right = -1; }
            return n;
        }

        public static int KNearest(Node* nodes, int root, double qx, double qy, int k, int* outIdx, double* dists)
        {
            int count = 0;
            for (int i = 0; i < k; i++) dists[i] = double.MaxValue;
            KnearestRec(nodes, root, qx, qy, k, dists, outIdx, ref count, 0);
            return count;
        }

        private static void KnearestRec(Node* nodes, int idx, double qx, double qy, int k, double* dists, int* outs, ref int c, int depth)
        {
            if (idx < 0) return;
            Node n = nodes[idx];
            double dx = n.X - qx, dy = n.Y - qy;
            double d = dx * dx + dy * dy;
            if (c < k) { outs[c] = idx; dists[c] = d; c++; }
            else
            {
                double maxD = 0; int maxI = 0;
                for (int i = 0; i < c; i++) { if (dists[i] > maxD) { maxD = dists[i]; maxI = i; } }
                if (d < maxD) { dists[maxI] = d; outs[maxI] = idx; }
            }
            int next = (depth % 2 == 0) ? n.Left : n.Right;
            int other = (depth % 2 == 0) ? n.Right : n.Left;
            KnearestRec(nodes, next, qx, qy, k, dists, outs, ref c, depth + 1);
            KnearestRec(nodes, other, qx, qy, k, dists, outs, ref c, depth + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Insert(Node* nodes, int root, double x, double y) { return root; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Delete(Node* nodes, int root, double x, double y) { return root; }
    }
}
