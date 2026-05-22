namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Quadtree
    {
        public struct Node { public double X, Y, W, H; public int Child0, Child1, Child2, Child3; public int Count; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Node* nodes, int maxDepth)
        {
            int nodeCount = 1;
            nodes[0].X = 0; nodes[0].Y = 0; nodes[0].W = 100; nodes[0].H = 100;
            nodes[0].Child0 = nodes[0].Child1 = nodes[0].Child2 = nodes[0].Child3 = -1;
            nodes[0].Count = n;
            return nodeCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeQuery(Node* nodes, int node, double x1, double y1, double x2, double y2, int* outIdx)
        {
            int count = 0;
            if (node < 0) return count;
            Node n = nodes[node];
            if (x2 < n.X || x1 > n.X + n.W || y2 < n.Y || y1 > n.Y + n.H) return count;
            if (n.Child0 < 0)
            {
                for (int i = 0; i < n.Count; i++) outIdx[count++] = i;
            }
            else
            {
                count += RangeQuery(nodes, n.Child0, x1, y1, x2, y2, outIdx + count);
                count += RangeQuery(nodes, n.Child1, x1, y1, x2, y2, outIdx + count);
                count += RangeQuery(nodes, n.Child2, x1, y1, x2, y2, outIdx + count);
                count += RangeQuery(nodes, n.Child3, x1, y1, x2, y2, outIdx + count);
            }
            return count;
        }
    }
}
