namespace IAFahim.Geometry.Advanced
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ConvexDiameter
    {
        public static long Run(int n, long* x, long* y)
        {
            if (n <= 1) return 0; if (n == 2) return DistSq(x[0], y[0], x[1], y[1]);
            FindExtremePoints(n, x, out int i, out int j);
            long maxD = 0; int ni = i, nj = j;
            while (ni != j || nj != i) { maxD = Math.Max(maxD, DistSq(x[ni], y[ni], x[nj], y[nj])); UpdateCaliper(n, x, y, ref ni, ref nj); }
            return maxD;
        }
        private static void FindExtremePoints(int n, long* x, out int i, out int j) { i = j = 0; for (int k = 1; k < n; k++) { if (x[k] > x[i]) i = k; if (x[k] < x[j]) j = k; } }
        private static void UpdateCaliper(int n, long* x, long* y, ref int ni, ref int nj) { int nni = ni + 1; if (nni == n) nni = 0; int nnj = nj + 1; if (nnj == n) nnj = 0; long cross = (x[nni] - x[ni]) * (y[nnj] - y[nj]) - (x[nnj] - x[nj]) * (y[nni] - y[ni]); if (cross >= 0) nj = nnj; else ni = nni; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static long DistSq(long x1, long y1, long x2, long y2) { long dx = x2 - x1, dy = y2 - y1; return dx * dx + dy * dy; }
    }

    public static unsafe class RotatingCalipers
    {
        public static long Run(int n, long* x, long* y, long* res)
        {
            if (n < 3) return 0;
            long maxD = 0; int j = 1;
            for (int i = 0; i < n; i++) { int ni = i + 1; if (ni == n) ni = 0; while (IsFurther(n, x, y, i, ni, j)) { j++; if (j == n) j = 0; } maxD = UpdateMaxDist(x, y, i, ni, j, maxD, res); }
            return maxD;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static bool IsFurther(int n, long* x, long* y, int i, int ni, int j) { int nj = j + 1; if (nj == n) nj = 0; return CrossProduct(x[ni] - x[i], y[ni] - y[i], x[nj] - x[j], y[nj] - y[j]) > 0; }
        private static long UpdateMaxDist(long* x, long* y, int i, int ni, int j, long maxD, long* res) { long d1 = DistSq(x[i], y[i], x[j], y[j]), d2 = DistSq(x[ni], y[ni], x[j], y[j]); if (d1 > maxD) { maxD = d1; if (res != null) { res[0] = i; res[1] = j; } } if (d2 > maxD) { maxD = d2; if (res != null) { res[0] = ni; res[1] = j; } } return maxD; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static long CrossProduct(long x1, long y1, long x2, long y2) => x1 * y2 - y1 * x2;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static long DistSq(long x1, long y1, long x2, long y2) { long dx = x2 - x1, dy = y2 - y1; return dx * dx + dy * dy; }
    }

    public static unsafe class MinkowskiSum
    {
        public static int Run(int n1, long* x1, long* y1, int n2, long* x2, long* y2, long* rx, long* ry)
        {
            long* p1 = stackalloc long[n1], p2 = stackalloc long[n1]; for (int k = 0; k < n1; k++) { int kn = k + 1; if (kn == n1) kn = 0; p1[k] = x1[kn] - x1[k]; p2[k] = y1[kn] - y1[k]; }
            long* q1 = stackalloc long[n2], q2 = stackalloc long[n2]; for (int k = 0; k < n2; k++) { int kn = k + 1; if (kn == n2) kn = 0; q1[k] = x2[kn] - x2[k]; q2[k] = y2[kn] - y2[k]; }
            int m = 0, i = 0, j = 0;
            long curX = x1[0] + x2[0];
            long curY = y1[0] + y2[0];
            while (i < n1 && j < n2)
            {
                rx[m] = curX; ry[m++] = curY;
                long cross = p1[i] * q2[j] - p2[i] * q1[j];
                curX += p1[i];
                curY += p2[i];
                if (cross <= 0) { i++; }
                else { curX += q1[j] - p1[i]; curY += q2[j] - p2[i]; j++; }
            }
            while (i < n1) { rx[m] = curX; ry[m++] = curY; curX += p1[i]; curY += p2[i]; i++; }
            while (j < n2) { rx[m] = curX; ry[m++] = curY; curX += q1[j]; curY += q2[j]; j++; }
            return m;
        }
    }

    public static unsafe class ClosestPair
    {
        private const int StripNeighbors = 7;

        public static long Run(int n, long* x, long* y)
        {
            if (n <= 1) return long.MaxValue;
            long* sx = stackalloc long[n], sy = stackalloc long[n];
            long* stx = stackalloc long[n], sty = stackalloc long[n];
            for (int i = 0; i < n; i++) { sx[i] = x[i]; sy[i] = y[i]; }
            HeapSort(sx, sy, n);
            return Solve(sx, sy, stx, sty, 0, n);
        }

        // Heapsort over parallel arrays, ordering ascending by key[]; payload[] is permuted identically.
        private static void HeapSort(long* key, long* payload, int n)
        {
            for (int i = (n >> 1) - 1; i >= 0; i--) SiftDown(key, payload, i, n);
            for (int end = n - 1; end > 0; end--) { Swap(key, payload, 0, end); SiftDown(key, payload, 0, end); }
        }
        private static void SiftDown(long* key, long* payload, int root, int len)
        {
            while (true)
            {
                int child = (root << 1) + 1;
                if (child >= len) break;
                if (child + 1 < len && key[child + 1] > key[child]) child++;
                if (key[root] >= key[child]) break;
                Swap(key, payload, root, child);
                root = child;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static void Swap(long* a, long* b, int i, int j) { long ta = a[i]; a[i] = a[j]; a[j] = ta; long tb = b[i]; b[i] = b[j]; b[j] = tb; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CombineStrip(long* stx, long* sty, int k, long d)
        {
            for (int i = 0; i < k; i++)
            {
                int last = i + StripNeighbors; if (last > k) last = k;
                for (int jj = i + 1; jj < last; jj++)
                {
                    long dy = sty[jj] - sty[i];
                    if (dy * dy >= d) break;
                    long dx = stx[jj] - stx[i];
                    long dist = dx * dx + dy * dy;
                    if (dist < d) d = dist;
                }
            }
            return d;
        }

        // x[] ascending by x over [l,r). stx/sty are scratch buffers of capacity >= n for the strip.
        private static long Solve(long* x, long* y, long* stx, long* sty, int l, int r)
        {
            int count = r - l;
            if (count <= 3) return Brute(x, y, l, r);
            int m = (l + r) >> 1; long mx = x[m];
            long d = Math.Min(Solve(x, y, stx, sty, l, m), Solve(x, y, stx, sty, m, r));
            int k = 0;
            for (int i = l; i < r; i++) { long dxm = x[i] - mx; if (dxm * dxm < d) { stx[k] = x[i]; sty[k] = y[i]; k++; } }
            HeapSort(sty, stx, k);
            return CombineStrip(stx, sty, k, d);
        }
        private static long Brute(long* x, long* y, int l, int r)
        {
            long best = long.MaxValue;
            for (int i = l; i < r; i++)
                for (int j = i + 1; j < r; j++)
                {
                    long dx = x[j] - x[i], dy = y[j] - y[i];
                    long dist = dx * dx + dy * dy;
                    if (dist < best) best = dist;
                }
            return best;
        }
    }
}
