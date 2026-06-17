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

        private static int BuildRec(double* xs, double* ys, int* idx, int lo, int hi, Node* nodes, ref int nextSlot)
        {
            int slot = nextSlot++;
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;
            for (int i = lo; i <= hi; i++)
            {
                double px = xs[idx[i]], py = ys[idx[i]];
                if (px < minX) minX = px; if (px > maxX) maxX = px;
                if (py < minY) minY = py; if (py > maxY) maxY = py;
            }
            bool splitX = (maxX - minX) >= (maxY - minY);
            int mid = (lo + hi) >> 1;
            SortRangeByAxis(xs, ys, idx, lo, hi, splitX);

            int p = idx[mid];
            nodes[slot].X = xs[p];
            nodes[slot].Y = ys[p];
            double r2 = 0;
            for (int i = lo; i <= hi; i++)
            {
                double ddx = xs[idx[i]] - xs[p], ddy = ys[idx[i]] - ys[p];
                double dd = ddx * ddx + ddy * ddy;
                if (dd > r2) r2 = dd;
            }
            nodes[slot].R = Math.Sqrt(r2);
            nodes[slot].Left = lo <= mid - 1 ? BuildRec(xs, ys, idx, lo, mid - 1, nodes, ref nextSlot) : -1;
            nodes[slot].Right = mid + 1 <= hi ? BuildRec(xs, ys, idx, mid + 1, hi, nodes, ref nextSlot) : -1;
            return slot;
        }

        private static void SortRangeByAxis(double* xs, double* ys, int* idx, int lo, int hi, bool splitX)
        {
            for (int i = lo + 1; i <= hi; i++)
            {
                int cur = idx[i];
                double key = splitX ? xs[cur] : ys[cur];
                int j = i - 1;
                while (j >= lo)
                {
                    double vj = splitX ? xs[idx[j]] : ys[idx[j]];
                    if (vj <= key) break;
                    idx[j + 1] = idx[j];
                    j--;
                }
                idx[j + 1] = cur;
            }
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
