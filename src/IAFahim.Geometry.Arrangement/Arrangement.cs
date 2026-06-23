namespace IAFahim.Geometry.Arrangement
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PointLocationBuild
    {
        public static int Run(int* xs, int* ys, int n, int* grid, int gridSize)
        {
            FindMinMax(xs, ys, n, out int minX, out int maxX, out int minY, out int maxY);
            int cellW = (maxX - minX) / gridSize + 1, cellH = (maxY - minY) / gridSize + 1;
            for (int i = 0; i < n; i++) grid[((ys[i] - minY) / cellH) * gridSize + ((xs[i] - minX) / cellW)]++;
            return gridSize * gridSize;
        }
        private static void FindMinMax(int* xs, int* ys, int n, out int minX, out int maxX, out int minY, out int maxY) { minX = maxX = xs[0]; minY = maxY = ys[0]; for (int i = 1; i < n; i++) { if (xs[i] < minX) minX = xs[i]; if (xs[i] > maxX) maxX = xs[i]; if (ys[i] < minY) minY = ys[i]; if (ys[i] > maxY) maxY = ys[i]; } }
        public static void BuildKdTree(long* points, int* tree, int node, int l, int r, int depth) { if (l > r) return; int axis = depth & 1, m = Partition(points, l, r, axis); tree[node] = m; BuildKdTree(points, tree, node * 2, l, m - 1, depth + 1); BuildKdTree(points, tree, node * 2 + 1, m + 1, r, depth + 1); }
        private static int Partition(long* pts, int l, int r, int axis) { for (int i = l; i <= r; i++) { int m = i; for (int j = i + 1; j <= r; j++) if (axis == 0 ? pts[j * 2] < pts[m * 2] : pts[j * 2 + 1] < pts[m * 2 + 1]) m = j; if (m != i) { long tx = pts[i * 2], ty = pts[i * 2 + 1]; pts[i * 2] = pts[m * 2]; pts[i * 2 + 1] = pts[m * 2 + 1]; pts[m * 2] = tx; pts[m * 2 + 1] = ty; } } return (l + r) >> 1; }
    }

    public static unsafe class PointLocationQuery
    {
        public static int Run(int* grid, int gridSize, int minX, int minY, int cellW, int cellH, int px, int py) { int cx = (px - minX) / cellW, cy = (py - minY) / cellH; if (cx < 0 || cx >= gridSize || cy < 0 || cy >= gridSize) return -1; return grid[cy * gridSize + cx]; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long AxisValue(long* points, int idx, int axis)
        {
            return axis == 0 ? points[idx * 2] : points[idx * 2 + 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PickNearChild(int node, int axis, long px, long py, long val)
        {
            bool goLeft = axis == 0 ? px < val : py < val;
            return goLeft ? node * 2 : node * 2 + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MergeBest(long* points, int cand, long px, long py, ref int best, ref double dist)
        {
            if (cand >= 0)
            {
                double d = SqDist(points, cand, px, py);
                if (d < dist) { best = cand; dist = d; }
            }
        }

        public static int QueryKdTree(long* points, int* tree, int node, int depth, long px, long py)
        {
            if (node == 0) return -1;
            int idx = tree[node], axis = depth & 1; long val = AxisValue(points, idx, axis);
            int near = PickNearChild(node, axis, px, py, val);
            int far = near == node * 2 ? node * 2 + 1 : node * 2;
            int best = idx;
            double dist = SqDist(points, idx, px, py);
            int next = QueryKdTree(points, tree, near, depth + 1, px, py);
            MergeBest(points, next, px, py, ref best, ref dist);
            long diff = axis == 0 ? px - val : py - val;
            if ((double)diff * diff <= dist)
            {
                int cand = QueryKdTree(points, tree, far, depth + 1, px, py);
                MergeBest(points, cand, px, py, ref best, ref dist);
            }
            return best;
        }

        private static double SqDist(long* points, int idx, long px, long py)
        {
            double dx = (double)points[idx * 2] - px;
            double dy = (double)points[idx * 2 + 1] - py;
            return dx * dx + dy * dy;
        }
    }

    public static unsafe class VerticalDecomposition
    {
        public static int Run(int* xs, int* ys, int n, int* outX, int* outY) { int* order = stackalloc int[n]; for (int i = 0; i < n; i++) order[i] = i; SortByX(xs, order, n); for (int i = 0; i < n; i++) { outX[i] = xs[order[i]]; outY[i] = ys[order[i]]; } return n; }
        private static void SortByX(int* xs, int* order, int n) { for (int i = 1; i < n; i++) { int key = order[i], j = i - 1; while (j >= 0 && xs[order[j]] > xs[key]) { order[j + 1] = order[j]; j--; } order[j + 1] = key; } }
    }

    public static unsafe class TrapezoidalMapBuild
    {
        public static int Run(int* sx1, int* sy1, int* sx2, int* sy2, int n, int* tx1, int* ty1, int* tx2, int* ty2) { int m = 0; for (int i = 0; i < n; i++) { int loX = sx1[i] < sx2[i] ? sx1[i] : sx2[i], hiX = sx1[i] < sx2[i] ? sx2[i] : sx1[i]; tx1[m] = loX; ty1[m] = sy1[i]; tx2[m] = hiX; ty2[m] = sy2[i]; m++; tx1[m] = loX; ty1[m] = sy2[i]; tx2[m] = hiX; ty2[m] = sy1[i]; m++; } return m; }
    }

    public static unsafe class TrapezoidalMapQuery
    {
        public static int Run(int* tx1, int* ty1, int* tx2, int* ty2, int n, int px, int py) { for (int i = 0; i < n; i++) if (tx1[i] <= px && px <= tx2[i]) { int yL = ty1[i] < ty2[i] ? ty1[i] : ty2[i], yH = ty1[i] < ty2[i] ? ty2[i] : ty1[i]; if (yL <= py && py <= yH) return i; } return -1; }
    }

    public static unsafe class ArrangementFaces
    {
        public static int Run(int n, int* head, int* to, int* next, int* visited, int* outFace) { int count = 0; for (int i = 0; i < n; i++) for (int e = head[i]; e != 0; e = next[e]) if (visited[e] == 0) { visited[e] = visited[e ^ 1] = 1; outFace[count++] = i; break; } return count; }
    }

    public static unsafe class PolygonBooleanUnion
    {
        public static int Run(int* x1, int* y1, int* x2, int* y2, int n, int* outX, int* outY) { int count = 0; for (int i = 0; i < n; i++) { int xLo = Math.Min(x1[i], x2[i]), xHi = Math.Max(x1[i], x2[i]), yLo = Math.Min(y1[i], y2[i]), yHi = Math.Max(y1[i], y2[i]); outX[count] = xLo; outY[count++] = yLo; outX[count] = xLo; outY[count++] = yHi; outX[count] = xHi; outY[count++] = yLo; outX[count] = xHi; outY[count++] = yHi; } return count; }
    }

    public static unsafe class PolygonBooleanIntersection
    {
        public static int Run(int* x1, int* y1, int* x2, int* y2, int n, int* outX, int* outY) { for (int i = 0; i < n; i++) for (int j = i + 1; j < n; j++) { int l = Math.Max(x1[i], x1[j]), r = Math.Min(x2[i], x2[j]), b = Math.Max(y1[i], y1[j]), t = Math.Min(y2[i], y2[j]); if (l < r && b < t) { outX[0] = l; outY[0] = b; outX[1] = l; outY[1] = t; outX[2] = r; outY[2] = b; outX[3] = r; outY[3] = t; return 4; } } return 0; }
    }
}
