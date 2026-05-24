namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class NearestNeighbor
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PointIdx { public double X, Y; public int Idx; }

        [StructLayout(LayoutKind.Sequential)]
        public struct KDNode
        {
            public double X, Y; public int Idx, Left, Right;
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
                if (less) { i++; PointIdx temp = arr[i]; arr[i] = arr[j]; arr[j] = temp; }
            }
            PointIdx t1 = arr[i + 1]; arr[i + 1] = arr[right]; arr[right] = t1;
            return i + 1;
        }

        public static int BuildKD(PointIdx* pts, KDNode* nodes, int left, int right, int depth, ref int nodeCount)
        {
            if (left > right) return -1;
            QuickSort(pts, left, right, depth);
            int mid = left + (right - left) / 2;
            int node = nodeCount++;
            int lc = BuildKD(pts, nodes, left, mid - 1, depth + 1, ref nodeCount);
            int rc = BuildKD(pts, nodes, mid + 1, right, depth + 1, ref nodeCount);
            UpdateNodeBounds(nodes, node, pts[mid], lc, rc);
            return node;
        }

        private static void UpdateNodeBounds(KDNode* nodes, int node, PointIdx p, int lc, int rc)
        {
            double minX = p.X, maxX = p.X, minY = p.Y, maxY = p.Y;
            if (lc != -1) { minX = Math.Min(minX, nodes[lc].MinX); maxX = Math.Max(maxX, nodes[lc].MaxX); minY = Math.Min(minY, nodes[lc].MinY); maxY = Math.Max(maxY, nodes[lc].MaxY); }
            if (rc != -1) { minX = Math.Min(minX, nodes[rc].MinX); maxX = Math.Max(maxX, nodes[rc].MaxX); minY = Math.Min(minY, nodes[rc].MinY); maxY = Math.Max(maxY, nodes[rc].MaxY); }
            nodes[node] = new KDNode { X = p.X, Y = p.Y, Idx = p.Idx, Left = lc, Right = rc, MinX = minX, MinY = minY, MaxX = maxX, MaxY = maxY };
        }

        public static void SearchNearest(KDNode* nodes, int node, double qx, double qy, ref double bestDist, ref int bestIdx)
        {
            if (node == -1) return;
            UpdateBest(nodes, node, qx, qy, ref bestDist, ref bestIdx);

            double dL = DistToBox(nodes, nodes[node].Left, qx, qy);
            double dR = DistToBox(nodes, nodes[node].Right, qx, qy);

            if (dL < dR) { SearchIfBetter(nodes, nodes[node].Left, qx, qy, dL, ref bestDist, ref bestIdx); SearchIfBetter(nodes, nodes[node].Right, qx, qy, dR, ref bestDist, ref bestIdx); }
            else { SearchIfBetter(nodes, nodes[node].Right, qx, qy, dR, ref bestDist, ref bestIdx); SearchIfBetter(nodes, nodes[node].Left, qx, qy, dL, ref bestDist, ref bestIdx); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateBest(KDNode* nodes, int node, double qx, double qy, ref double bestDist, ref int bestIdx)
        {
            double dx = nodes[node].X - qx, dy = nodes[node].Y - qy;
            double d = dx * dx + dy * dy;
            if (d < bestDist) { bestDist = d; bestIdx = nodes[node].Idx; }
        }

        private static double DistToBox(KDNode* nodes, int node, double qx, double qy)
        {
            if (node == -1) return double.MaxValue;
            double dx = Math.Max(nodes[node].MinX - qx, Math.Max(0, qx - nodes[node].MaxX));
            double dy = Math.Max(nodes[node].MinY - qy, Math.Max(0, qy - nodes[node].MaxY));
            return dx * dx + dy * dy;
        }

        private static void SearchIfBetter(KDNode* nodes, int node, double qx, double qy, double dBox, ref double bestDist, ref int bestIdx)
        {
            if (node != -1 && dBox < bestDist) SearchNearest(nodes, node, qx, qy, ref bestDist, ref bestIdx);
        }

        public static void SearchRange(KDNode* nodes, int node, double qx, double qy, double rSq, int* outIdx, ref int outCount)
        {
            if (node == -1 || DistToBox(nodes, node, qx, qy) > rSq) return;
            double dx = nodes[node].X - qx, dy = nodes[node].Y - qy;
            if (dx * dx + dy * dy <= rSq) outIdx[outCount++] = nodes[node].Idx;
            SearchRange(nodes, nodes[node].Left, qx, qy, rSq, outIdx, ref outCount);
            SearchRange(nodes, nodes[node].Right, qx, qy, rSq, outIdx, ref outCount);
        }
    }
}
