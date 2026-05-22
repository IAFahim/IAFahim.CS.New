namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BallTree
    {
        public struct Node { public double X, Y; public double R; public int Left, Right; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Node* nodes)
        {
            for (int i = 0; i < n; i++) { nodes[i].X = xs[i]; nodes[i].Y = ys[i]; nodes[i].R = 0; nodes[i].Left = nodes[i].Right = -1; }
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Nearest(Node* nodes, int root, double qx, double qy)
        {
            if (root < 0) return -1;
            int best = root;
            double bd = double.MaxValue;
            double dx = nodes[root].X - qx, dy = nodes[root].Y - qy;
            double d = dx * dx + dy * dy;
            if (d < bd) { bd = d; best = root; }
            NearestRec(nodes, nodes[root].Left, qx, qy, ref best, ref bd);
            NearestRec(nodes, nodes[root].Right, qx, qy, ref best, ref bd);
            return best;
        }

        private static void NearestRec(Node* nodes, int idx, double qx, double qy, ref int best, ref double bd)
        {
            if (idx < 0) return;
            double dx = nodes[idx].X - qx, dy = nodes[idx].Y - qy;
            double d = dx * dx + dy * dy;
            if (d < bd) { bd = d; best = idx; }
            NearestRec(nodes, nodes[idx].Left, qx, qy, ref best, ref bd);
            NearestRec(nodes, nodes[idx].Right, qx, qy, ref best, ref bd);
        }
    }
}
