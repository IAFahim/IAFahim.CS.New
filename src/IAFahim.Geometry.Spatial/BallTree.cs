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
            if (n <= 0) return 0;
            int* idx = stackalloc int[n];
            for (int i = 0; i < n; i++) idx[i] = i;
            int nextSlot = 0;
            BuildRec(xs, ys, idx, 0, n - 1, nodes, ref nextSlot);
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeBounds(double* xs, double* ys, int* idx, int lo, int hi, out double minX, out double maxX, out double minY, out double maxY)
        {
            minX = double.MaxValue; maxX = double.MinValue;
            minY = double.MaxValue; maxY = double.MinValue;
            for (int i = lo; i <= hi; i++)
            {
                double px = xs[idx[i]], py = ys[idx[i]];
                if (px < minX) minX = px; if (px > maxX) maxX = px;
                if (py < minY) minY = py; if (py > maxY) maxY = py;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ComputeRadius(double* xs, double* ys, int* idx, int lo, int hi, int p)
        {
            double r2 = 0;
            for (int i = lo; i <= hi; i++)
            {
                double ddx = xs[idx[i]] - xs[p], ddy = ys[idx[i]] - ys[p];
                double dd = ddx * ddx + ddy * ddy;
                if (dd > r2) r2 = dd;
            }
            return Math.Sqrt(r2);
        }

        private static int BuildRec(double* xs, double* ys, int* idx, int lo, int hi, Node* nodes, ref int nextSlot)
        {
            int slot = nextSlot++;
            ComputeBounds(xs, ys, idx, lo, hi, out double minX, out double maxX, out double minY, out double maxY);
            bool splitX = (maxX - minX) >= (maxY - minY);
            int mid = (lo + hi) >> 1;
            SortRangeByAxis(xs, ys, idx, lo, hi, splitX);

            int p = idx[mid];
            nodes[slot].X = xs[p];
            nodes[slot].Y = ys[p];
            nodes[slot].R = ComputeRadius(xs, ys, idx, lo, hi, p);
            nodes[slot].Left = lo <= mid - 1 ? BuildRec(xs, ys, idx, lo, mid - 1, nodes, ref nextSlot) : -1;
            nodes[slot].Right = mid + 1 <= hi ? BuildRec(xs, ys, idx, mid + 1, hi, nodes, ref nextSlot) : -1;
            return slot;
        }

        private static void SortRangeByAxis(double* xs, double* ys, int* idx, int lo, int hi, bool splitX)
        {
            int n = hi - lo + 1;
            for (int i = (n >> 1) - 1; i >= 0; i--) SiftDownAxis(xs, ys, idx, lo, i, n, splitX);
            for (int end = n - 1; end > 0; end--)
            {
                int t = idx[lo]; idx[lo] = idx[lo + end]; idx[lo + end] = t;
                SiftDownAxis(xs, ys, idx, lo, 0, end, splitX);
            }
        }

        private static void SiftDownAxis(double* xs, double* ys, int* idx, int lo, int i, int n, bool splitX)
        {
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = i;
                double mv = splitX ? xs[idx[lo + m]] : ys[idx[lo + m]];
                if (l < n)
                {
                    double lv = splitX ? xs[idx[lo + l]] : ys[idx[lo + l]];
                    if (lv > mv) { m = l; mv = lv; }
                }
                if (r < n)
                {
                    double rv = splitX ? xs[idx[lo + r]] : ys[idx[lo + r]];
                    if (rv > mv) { m = r; mv = rv; }
                }
                if (m == i) break;
                int t = idx[lo + i]; idx[lo + i] = idx[lo + m]; idx[lo + m] = t;
                i = m;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Nearest(Node* nodes, int root, double qx, double qy)
        {
            if (root < 0) return -1;
            int best = root;
            double bd = double.MaxValue;
            NearestRec(nodes, root, qx, qy, ref best, ref bd);
            return best;
        }

        private static void NearestRec(Node* nodes, int idx, double qx, double qy, ref int best, ref double bd)
        {
            Node node = nodes[idx];
            double dx = node.X - qx, dy = node.Y - qy;
            double d2 = dx * dx + dy * dy;
            if (d2 < bd) { bd = d2; best = idx; }

            int l = node.Left, r = node.Right;
            double dl2 = l >= 0 ? CenterSqDist(nodes[l], qx, qy) : double.MaxValue;
            double dr2 = r >= 0 ? CenterSqDist(nodes[r], qx, qy) : double.MaxValue;
            int first = dl2 <= dr2 ? l : r;
            int second = first == l ? r : l;
            double firstD2 = first == l ? dl2 : dr2;
            double secondD2 = second == l ? dl2 : dr2;

            if (first >= 0 && BallMinSq(firstD2, nodes[first].R) < bd)
                NearestRec(nodes, first, qx, qy, ref best, ref bd);
            if (second >= 0 && BallMinSq(secondD2, nodes[second].R) < bd)
                NearestRec(nodes, second, qx, qy, ref best, ref bd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double CenterSqDist(Node node, double qx, double qy)
        {
            double dx = node.X - qx, dy = node.Y - qy;
            return dx * dx + dy * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double BallMinSq(double centerSqDist, double radius)
        {
            double gap = Math.Sqrt(centerSqDist) - radius;
            return gap > 0 ? gap * gap : 0;
        }
    }
}
