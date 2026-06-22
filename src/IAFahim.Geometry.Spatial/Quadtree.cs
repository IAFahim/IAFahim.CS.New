namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Quadtree
    {
        private const int QuadrantCount = 4;

        private const int QuadrantLowerLeft = 0;

        private const int QuadrantLowerRight = 1;

        private const int QuadrantUpperLeft = 2;

        private const int QuadrantUpperRight = 3;

        private const double MidpointWeight = 0.5;

        private const int NoChild = -1;

        private const int NoPoint = -1;

        public struct Node
        {
            public double MinX, MinY, MaxX, MaxY;
            public int FirstChild;
            public int PointIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeBoundingBox(double* xs, double* ys, int n, out double minX, out double minY, out double maxX, out double maxY)
        {
            minX = xs[0]; minY = ys[0]; maxX = xs[0]; maxY = ys[0];
            for (int i = 1; i < n; i++)
            {
                if (xs[i] < minX) minX = xs[i];
                if (xs[i] > maxX) maxX = xs[i];
                if (ys[i] < minY) minY = ys[i];
                if (ys[i] > maxY) maxY = ys[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInQuadrant(double px, double py, double midX, double midY, int quadrant)
        {
            bool lowerY = py <= midY;
            bool leftX = px <= midX;
            switch (quadrant)
            {
                case QuadrantLowerLeft: return leftX && lowerY;
                case QuadrantLowerRight: return !leftX && lowerY;
                case QuadrantUpperLeft: return leftX && !lowerY;
                default: return !leftX && !lowerY;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PartitionQuadrant(double* xs, double* ys, int* indices, int lo, int count, double midX, double midY, int quadrant)
        {
            int matched = 0;
            for (int i = lo; i < count; i++)
            {
                int p = indices[i];
                if (IsInQuadrant(xs[p], ys[p], midX, midY, quadrant))
                {
                    int t = indices[lo + matched];
                    indices[lo + matched] = indices[i];
                    indices[i] = t;
                    matched++;
                }
            }
            return matched;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ConfigureQuadrantBounds(Node* nodes, int first, int u, double midX, double midY, int quadrant)
        {
            Node* child = &nodes[first + quadrant];
            switch (quadrant)
            {
                case QuadrantLowerLeft:
                    child->MinX = nodes[u].MinX; child->MaxX = midX;
                    child->MinY = nodes[u].MinY; child->MaxY = midY;
                    break;
                case QuadrantLowerRight:
                    child->MinX = midX; child->MaxX = nodes[u].MaxX;
                    child->MinY = nodes[u].MinY; child->MaxY = midY;
                    break;
                case QuadrantUpperLeft:
                    child->MinX = nodes[u].MinX; child->MaxX = midX;
                    child->MinY = midY; child->MaxY = nodes[u].MaxY;
                    break;
                default:
                    child->MinX = midX; child->MaxX = nodes[u].MaxX;
                    child->MinY = midY; child->MaxY = nodes[u].MaxY;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Node* nodes, int maxNodes)
        {
            if (n == 0 || maxNodes < 1) return 0;
            ComputeBoundingBox(xs, ys, n, out double minX, out double minY, out double maxX, out double maxY);
            int nodeCount = 1;
            nodes[0].MinX = minX; nodes[0].MinY = minY;
            nodes[0].MaxX = maxX; nodes[0].MaxY = maxY;
            nodes[0].FirstChild = NoChild;
            nodes[0].PointIndex = NoPoint;
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
            if (nodeCount + QuadrantCount > maxNodes)
            {
                nodes[u].PointIndex = indices[0];
                return;
            }

            double midX = (nodes[u].MinX + nodes[u].MaxX) * MidpointWeight;
            double midY = (nodes[u].MinY + nodes[u].MaxY) * MidpointWeight;

            int first = nodeCount;
            nodes[u].FirstChild = first;
            nodeCount += QuadrantCount;

            for (int i = 0; i < QuadrantCount; i++)
            {
                nodes[first + i].FirstChild = NoChild;
                nodes[first + i].PointIndex = NoPoint;
                ConfigureQuadrantBounds(nodes, first, u, midX, midY, i);
            }

            int n0 = PartitionQuadrant(xs, ys, indices, 0, count, midX, midY, QuadrantLowerLeft);
            int n1 = PartitionQuadrant(xs, ys, indices, n0, count, midX, midY, QuadrantLowerRight);
            int n2 = PartitionQuadrant(xs, ys, indices, n0 + n1, count, midX, midY, QuadrantUpperLeft);
            int n3 = count - n0 - n1 - n2;

            int* c0Start = indices;
            int* c1Start = indices + n0;
            int* c2Start = c1Start + n1;
            int* c3Start = c2Start + n2;

            BuildRecursive(xs, ys, nodes, first, c0Start, n0, ref nodeCount, maxNodes);
            BuildRecursive(xs, ys, nodes, first + 1, c1Start, n1, ref nodeCount, maxNodes);
            BuildRecursive(xs, ys, nodes, first + 2, c2Start, n2, ref nodeCount, maxNodes);
            BuildRecursive(xs, ys, nodes, first + 3, c3Start, n3, ref nodeCount, maxNodes);
        }
    }
}
