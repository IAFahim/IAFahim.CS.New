namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Quadtree
    {
        public struct Node
        {
            public double MinX, MinY, MaxX, MaxY;
            public int FirstChild;
            public int PointIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Node* nodes, int maxNodes)
        {
            if (n == 0 || maxNodes < 1) return 0;

            double minX = xs[0], minY = ys[0], maxX = xs[0], maxY = ys[0];
            for (int i = 1; i < n; i++)
            {
                if (xs[i] < minX) minX = xs[i];
                if (xs[i] > maxX) maxX = xs[i];
                if (ys[i] < minY) minY = ys[i];
                if (ys[i] > maxY) maxY = ys[i];
            }

            int nodeCount = 1;
            nodes[0].MinX = minX; nodes[0].MinY = minY;
            nodes[0].MaxX = maxX; nodes[0].MaxY = maxY;
            nodes[0].FirstChild = -1;
            nodes[0].PointIndex = -1;

            int* indices = stackalloc int[n];
            for (int i = 0; i < n; i++) indices[i] = i;

            BuildRecursive(xs, ys, nodes, 0, indices, n, ref nodeCount, maxNodes);
            return nodeCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildRecursive(double* xs, double* ys, Node* nodes, int u, int* indices, int count, ref int nodeCount, int maxNodes)
        {
            if (count == 0) return;
            if (count == 1)
            {
                nodes[u].PointIndex = indices[0];
                return;
            }

            if (nodeCount + 4 > maxNodes)
            {
                nodes[u].PointIndex = indices[0]; // Truncate
                return;
            }

            double midX = (nodes[u].MinX + nodes[u].MaxX) * 0.5;
            double midY = (nodes[u].MinY + nodes[u].MaxY) * 0.5;

            int first = nodeCount;
            nodes[u].FirstChild = first;
            nodeCount += 4;

            for (int i = 0; i < 4; i++)
            {
                nodes[first + i].FirstChild = -1;
                nodes[first + i].PointIndex = -1;
            }

            nodes[first].MinX = nodes[u].MinX; nodes[first].MaxX = midX;
            nodes[first].MinY = nodes[u].MinY; nodes[first].MaxY = midY;

            nodes[first + 1].MinX = midX; nodes[first + 1].MaxX = nodes[u].MaxX;
            nodes[first + 1].MinY = nodes[u].MinY; nodes[first + 1].MaxY = midY;

            nodes[first + 2].MinX = nodes[u].MinX; nodes[first + 2].MaxX = midX;
            nodes[first + 2].MinY = midY; nodes[first + 2].MaxY = nodes[u].MaxY;

            nodes[first + 3].MinX = midX; nodes[first + 3].MaxX = nodes[u].MaxX;
            nodes[first + 3].MinY = midY; nodes[first + 3].MaxY = nodes[u].MaxY;

            int* c0Start = indices;
            int n0 = 0;
            for (int i = 0; i < count; i++)
            {
                int p = indices[i];
                if (xs[p] <= midX && ys[p] <= midY) { int t = indices[n0]; indices[n0] = indices[i]; indices[i] = t; n0++; }
            }
            int* c1Start = indices + n0;
            int n1 = 0;
            for (int i = n0; i < count; i++)
            {
                int p = indices[i];
                if (xs[p] > midX && ys[p] <= midY) { int t = c1Start[n1]; c1Start[n1] = indices[i]; indices[i] = t; n1++; }
            }
            int* c2Start = c1Start + n1;
            int n2 = 0;
            for (int i = n0 + n1; i < count; i++)
            {
                int p = indices[i];
                if (xs[p] <= midX && ys[p] > midY) { int t = c2Start[n2]; c2Start[n2] = indices[i]; indices[i] = t; n2++; }
            }
            int* c3Start = c2Start + n2;
            int n3 = count - n0 - n1 - n2;

            BuildRecursive(xs, ys, nodes, first, c0Start, n0, ref nodeCount, maxNodes);
            BuildRecursive(xs, ys, nodes, first + 1, c1Start, n1, ref nodeCount, maxNodes);
            BuildRecursive(xs, ys, nodes, first + 2, c2Start, n2, ref nodeCount, maxNodes);
            BuildRecursive(xs, ys, nodes, first + 3, c3Start, n3, ref nodeCount, maxNodes);
        }
    }
}
