namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class NearestNeighbor
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PointIdx
        {
            public double X, Y;
            public int Idx;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KDNode
        {
            public double X, Y;
            public int Idx;
            public int Left, Right;
            public double MinX, MinY, MaxX, MaxY;
        }

        private static void QuickSort(PointIdx* arr, int left, int right, int depth)
        {
            if (left >= right) return;
            int pivot = Partition(arr, left, right, depth);
            QuickSort(arr, left, pivot - 1, depth);
            QuickSort(arr, pivot + 1, right, depth);
        }

        private static int Partition(PointIdx* arr, int left, int right, int depth)
        {
            PointIdx pivotValue = arr[right];
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                bool less = (depth % 2 == 0) ? arr[j].X < pivotValue.X : arr[j].Y < pivotValue.Y;
                if (less)
                {
                    i++;
                    PointIdx temp = arr[i]; arr[i] = arr[j]; arr[j] = temp;
                }
            }
            PointIdx t1 = arr[i + 1]; arr[i + 1] = arr[right]; arr[right] = t1;
            return i + 1;
        }

        private static int BuildKD(PointIdx* pts, KDNode* nodes, int left, int right, int depth, ref int nodeCount)
        {
            if (left > right) return -1;
            QuickSort(pts, left, right, depth);
            int mid = left + (right - left) / 2;
            int node = nodeCount++;
            
            int leftChild = BuildKD(pts, nodes, left, mid - 1, depth + 1, ref nodeCount);
            int rightChild = BuildKD(pts, nodes, mid + 1, right, depth + 1, ref nodeCount);

            double minX = pts[mid].X, maxX = pts[mid].X;
            double minY = pts[mid].Y, maxY = pts[mid].Y;

            if (leftChild != -1)
            {
                minX = Math.Min(minX, nodes[leftChild].MinX);
                maxX = Math.Max(maxX, nodes[leftChild].MaxX);
                minY = Math.Min(minY, nodes[leftChild].MinY);
                maxY = Math.Max(maxY, nodes[leftChild].MaxY);
            }
            if (rightChild != -1)
            {
                minX = Math.Min(minX, nodes[rightChild].MinX);
                maxX = Math.Max(maxX, nodes[rightChild].MaxX);
                minY = Math.Min(minY, nodes[rightChild].MinY);
                maxY = Math.Max(maxY, nodes[rightChild].MaxY);
            }

            nodes[node] = new KDNode
            {
                X = pts[mid].X, Y = pts[mid].Y, Idx = pts[mid].Idx,
                Left = leftChild, Right = rightChild,
                MinX = minX, MinY = minY, MaxX = maxX, MaxY = maxY
            };
            return node;
        }

        private static void SearchNearest(KDNode* nodes, int node, double qx, double qy, ref double bestDist, ref int bestIdx)
        {
            if (node == -1) return;
            double dx = nodes[node].X - qx;
            double dy = nodes[node].Y - qy;
            double d = dx * dx + dy * dy;
            if (d < bestDist)
            {
                bestDist = d;
                bestIdx = nodes[node].Idx;
            }

            double dLeft = 0, dRight = 0;
            if (nodes[node].Left != -1)
            {
                double lx = Math.Max(nodes[nodes[node].Left].MinX, Math.Min(qx, nodes[nodes[node].Left].MaxX));
                double ly = Math.Max(nodes[nodes[node].Left].MinY, Math.Min(qy, nodes[nodes[node].Left].MaxY));
                dLeft = (lx - qx) * (lx - qx) + (ly - qy) * (ly - qy);
            }
            if (nodes[node].Right != -1)
            {
                double rx = Math.Max(nodes[nodes[node].Right].MinX, Math.Min(qx, nodes[nodes[node].Right].MaxX));
                double ry = Math.Max(nodes[nodes[node].Right].MinY, Math.Min(qy, nodes[nodes[node].Right].MaxY));
                dRight = (rx - qx) * (rx - qx) + (ry - qy) * (ry - qy);
            }

            if (dLeft < dRight)
            {
                if (dLeft < bestDist) SearchNearest(nodes, nodes[node].Left, qx, qy, ref bestDist, ref bestIdx);
                if (dRight < bestDist) SearchNearest(nodes, nodes[node].Right, qx, qy, ref bestDist, ref bestIdx);
            }
            else
            {
                if (dRight < bestDist) SearchNearest(nodes, nodes[node].Right, qx, qy, ref bestDist, ref bestIdx);
                if (dLeft < bestDist) SearchNearest(nodes, nodes[node].Left, qx, qy, ref bestDist, ref bestIdx);
            }
        }

        private static void SearchRange(KDNode* nodes, int node, double qx, double qy, double rSq, int* outIdx, ref int outCount)
        {
            if (node == -1) return;
            
            double dBoxX = Math.Max(nodes[node].MinX, Math.Min(qx, nodes[node].MaxX));
            double dBoxY = Math.Max(nodes[node].MinY, Math.Min(qy, nodes[node].MaxY));
            if ((dBoxX - qx) * (dBoxX - qx) + (dBoxY - qy) * (dBoxY - qy) > rSq) return;

            double dx = nodes[node].X - qx;
            double dy = nodes[node].Y - qy;
            if (dx * dx + dy * dy <= rSq)
            {
                outIdx[outCount++] = nodes[node].Idx;
            }

            SearchRange(nodes, nodes[node].Left, qx, qy, rSq, outIdx, ref outCount);
            SearchRange(nodes, nodes[node].Right, qx, qy, rSq, outIdx, ref outCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FromPoints(double* xs, double* ys, int n, double qx, double qy)
        {
            // Fallback for single query without building KD tree
            int best = 0;
            double bd = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                double dx = xs[i] - qx, dy = ys[i] - qy;
                double d = dx * dx + dy * dy;
                if (d < bd) { bd = d; best = i; }
            }
            return best;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FromVoronoi(double qx, double qy, double* xs, double* ys, int n, PointIdx* pts, KDNode* nodes)
        {
            // Instead of linear search, Build KD-Tree and query.
            if (n == 0) return -1;
            
            for (int i = 0; i < n; i++) { pts[i].X = xs[i]; pts[i].Y = ys[i]; pts[i].Idx = i; }
            int nodeCount = 0;
            int root = BuildKD(pts, nodes, 0, n - 1, 0, ref nodeCount);
            double bestDist = double.MaxValue;
            int bestIdx = -1;
            SearchNearest(nodes, root, qx, qy, ref bestDist, ref bestIdx);
            return bestIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Range(double qx, double qy, double r, double* xs, double* ys, int n, int* outIdx, PointIdx* pts, KDNode* nodes)
        {
            if (n == 0) return 0;
            
            for (int i = 0; i < n; i++) { pts[i].X = xs[i]; pts[i].Y = ys[i]; pts[i].Idx = i; }
            int nodeCount = 0;
            int root = BuildKD(pts, nodes, 0, n - 1, 0, ref nodeCount);
            int outCount = 0;
            SearchRange(nodes, root, qx, qy, r * r, outIdx, ref outCount);
            return outCount;
        }
    }
}
