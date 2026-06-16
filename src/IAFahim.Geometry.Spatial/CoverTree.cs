namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class CoverTree
    {
        public struct Node { public double X, Y; public int Level; public int Next; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Node* nodes)
        {
            for (int i = 0; i < n; i++) { nodes[i].X = xs[i]; nodes[i].Y = ys[i]; nodes[i].Level = 0; nodes[i].Next = -1; }
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Nearest(Node* nodes, int n, double qx, double qy)
        {
            if (n <= 0) return -1;
            int best = -1;
            double bd = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                double dx = nodes[i].X - qx, dy = nodes[i].Y - qy;
                double d = dx * dx + dy * dy;
                if (d < bd) { bd = d; best = i; }
            }
            return best;
        }
    }
}
