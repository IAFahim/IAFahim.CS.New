namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Octree
    {
        public struct Node { public double X, Y, Z, Size; public int* Children; public int Count; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, double* zs, int n, Node* nodes)
        {
            nodes[0].X = 0; nodes[0].Y = 0; nodes[0].Z = 0; nodes[0].Size = 100;
            nodes[0].Count = n;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeQuery(Node* nodes, int node, double x1, double y1, double z1, double x2, double y2, double z2, int* outIdx)
        {
            int count = 0;
            if (node < 0) return count;
            Node n = nodes[node];
            if (x2 < n.X || x1 > n.X + n.Size || y2 < n.Y || y1 > n.Y + n.Size || z2 < n.Z || z1 > n.Z + n.Size)
                return count;
            for (int i = 0; i < n.Count; i++) outIdx[count++] = i;
            return count;
        }
    }
}
