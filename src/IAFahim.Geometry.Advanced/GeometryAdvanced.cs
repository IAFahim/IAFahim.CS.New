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
        private static void UpdateCaliper(int n, long* x, long* y, ref int ni, ref int nj) { int nni = (ni + 1) % n, nnj = (nj + 1) % n; long cross = (x[nni] - x[ni]) * (y[nnj] - y[nj]) - (x[nnj] - x[nj]) * (y[nni] - y[ni]); if (cross >= 0) nj = nnj; else ni = nni; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static long DistSq(long x1, long y1, long x2, long y2) { long dx = x2 - x1, dy = y2 - y1; return dx * dx + dy * dy; }
    }

    public static unsafe class RotatingCalipers
    {
        public static long Run(int n, long* x, long* y, long* res)
        {
            if (n < 3) return 0;
            long maxD = 0; int j = 1;
            for (int i = 0; i < n; i++) { int ni = (i + 1) % n; while (IsFurther(n, x, y, i, ni, j)) j = (j + 1) % n; maxD = UpdateMaxDist(x, y, i, ni, j, maxD, res); }
            return maxD;
        }
        private static bool IsFurther(int n, long* x, long* y, int i, int ni, int j) { int nj = (j + 1) % n; return CrossProduct(x[ni] - x[i], y[ni] - y[i], x[nj] - x[j], y[nj] - y[j]) > 0; }
        private static long UpdateMaxDist(long* x, long* y, int i, int ni, int j, long maxD, long* res) { long d1 = DistSq(x[i], y[i], x[j], y[j]), d2 = DistSq(x[ni], y[ni], x[j], y[j]); if (d1 > maxD) { maxD = d1; if (res != null) { res[0] = i; res[1] = j; } } if (d2 > maxD) { maxD = d2; if (res != null) { res[0] = ni; res[1] = j; } } return maxD; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static long CrossProduct(long x1, long y1, long x2, long y2) => x1 * y2 - y1 * x2;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static long DistSq(long x1, long y1, long x2, long y2) { long dx = x2 - x1, dy = y2 - y1; return dx * dx + dy * dy; }
    }

    public static unsafe class MinkowskiSum
    {
        public static int Run(int n1, long* x1, long* y1, int n2, long* x2, long* y2, long* rx, long* ry)
        {
            long* p1 = stackalloc long[n1], p2 = stackalloc long[n1]; for (int k = 0; k < n1; k++) { p1[k] = x1[(k + 1) % n1] - x1[k]; p2[k] = y1[(k + 1) % n1] - y1[k]; }
            long* q1 = stackalloc long[n2], q2 = stackalloc long[n2]; for (int k = 0; k < n2; k++) { q1[k] = x2[(k + 1) % n2] - x2[k]; q2[k] = y2[(k + 1) % n2] - y2[k]; }
            long* resX = stackalloc long[n1 + n2], resY = stackalloc long[n1 + n2];
            int m = 0, i = 0, j = 0;
            while (i < n1 && j < n2) { if (p1[i] * q2[j] - p2[i] * q1[j] <= 0) { resX[m] = x1[0] + x2[0] + (i > 0 ? p1[i - 1] : 0) + (j > 0 ? q1[j - 1] : 0); resY[m++] = y1[0] + y2[0] + (i > 0 ? p2[i - 1] : 0) + (j > 0 ? q2[j - 1] : 0); i++; } else { resX[m] = x1[0] + x2[0] + (i > 0 ? p1[i - 1] : 0) + (j > 0 ? q1[j - 1] : 0); resY[m++] = y1[0] + y2[0] + (i > 0 ? p2[i - 1] : 0) + (j > 0 ? q2[j - 1] : 0); j++; } }
            while (i < n1) { resX[m] = x1[0] + x2[0] + (i > 0 ? p1[i - 1] : 0) + (j > 0 ? q1[j - 1] : 0); resY[m++] = y1[0] + y2[0] + (i > 0 ? p2[i - 1] : 0) + (j > 0 ? q2[j - 1] : 0); i++; }
            while (j < n2) { resX[m] = x1[0] + x2[0] + (i > 0 ? p1[i - 1] : 0) + (j > 0 ? q1[j - 1] : 0); resY[m++] = y1[0] + y2[0] + (i > 0 ? p2[i - 1] : 0) + (j > 0 ? q2[j - 1] : 0); j++; }
            for (int k = 0; k < m; k++) { rx[k] = resX[k]; ry[k] = resY[k]; }
            return m;
        }
    }

    public static unsafe class ClosestPair
    {
        public static long Run(int n, long* x, long* y)
        {
            if (n <= 1) return long.MaxValue;
            long* sx = stackalloc long[n], sy = stackalloc long[n], yt = stackalloc long[n];
            for (int i = 0; i < n; i++) { sx[i] = x[i]; sy[i] = y[i]; }
            SortByX(sx, sy, n); return Solve(sx, sy, yt, 0, n);
        }
        private static void SortByX(long* sx, long* sy, int n) { for (int i = 1; i < n; i++) { long kx = sx[i], ky = sy[i]; int j = i - 1; while (j >= 0 && sx[j] > kx) { sx[j + 1] = sx[j]; sy[j + 1] = sy[j]; j--; } sx[j + 1] = kx; sy[j + 1] = ky; } }
        private static long Solve(long* x, long* y, long* yt, int l, int r)
        {
            if (r - l <= 1) return long.MaxValue;
            int m = (l + r) >> 1; long mx = x[m], d = Math.Min(Solve(x, y, yt, l, m), Solve(x, y, yt, m, r));
            int k = 0; for (int i = l; i < r; i++) if ((x[i] - mx) * (x[i] - mx) < d) yt[k++] = y[i];
            return d;
        }
    }
}
