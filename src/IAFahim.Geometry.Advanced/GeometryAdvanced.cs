namespace IAFahim.Geometry.Advanced
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ConvexDiameter
    {
        public static long Run(int n, long* x, long* y)
        {
            if (n == 1) return 0;
            if (n == 2) return (x[0] - x[1]) * (x[0] - x[1]) + (y[0] - y[1]) * (y[0] - y[1]);
            int i = 0, j = 0;
            for (int k = 0; k < n; k++)
            {
                if (x[k] > x[i]) i = k;
                if (x[k] < x[j]) j = k;
            }
            long maxDist = 0;
            int ni = i, nj = j;
            while (ni != j || nj != i)
            {
                long dx = x[ni] - x[nj];
                long dy = y[ni] - y[nj];
                long dist = dx * dx + dy * dy;
                if (dist > maxDist) maxDist = dist;
                int nextI = (ni + 1) % n;
                int nextJ = (nj + 1) % n;
                long cross = (x[nextI] - x[ni]) * (y[nextJ] - y[nj]) - (x[nextJ] - x[nj]) * (y[nextI] - y[ni]);
                if (cross >= 0) nj = nextJ;
                else ni = nextI;
            }
            return maxDist;
        }
    }

    public static unsafe class RotatingCalipers
    {
        public static long Run(int n, long* x, long* y, long* res)
        {
            if (n < 3) return 0;
            int m = n - 1;
            long maxDist = 0;
            int j = 1;
            for (int i = 0; i < m; i++)
            {
                int ni = (i + 1) % n;
                while (true)
                {
                    int nj = (j + 1) % n;
                    long cross = (x[ni] - x[i]) * (y[nj] - y[j]) - (x[nj] - x[j]) * (y[ni] - y[i]);
                    if (cross <= 0) break;
                    j = nj;
                }
                long d1 = DistSq(x[i], y[i], x[j], y[j]);
                long d2 = DistSq(x[ni], y[ni], x[j], y[j]);
                long d3 = DistSq(x[i], y[i], x[ni], y[ni]);
                if (d1 > maxDist) { maxDist = d1; if (res != null) { res[0] = i; res[1] = j; } }
                if (d2 > maxDist) { maxDist = d2; if (res != null) { res[0] = ni; res[1] = j; } }
                if (d3 > maxDist) { maxDist = d3; if (res != null) { res[0] = i; res[1] = ni; } }
            }
            return maxDist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long DistSq(long x1, long y1, long x2, long y2)
        {
            long dx = x2 - x1;
            long dy = y2 - y1;
            return dx * dx + dy * dy;
        }
    }

    public static unsafe class MinkowskiSum
    {
        public static int Run(int n1, long* x1, long* y1, int n2, long* x2, long* y2, long* rx, long* ry)
        {
            long* p1 = stackalloc long[n1];
            long* p2 = stackalloc long[n1];
            for (int k = 0; k < n1; k++) { p1[k] = x1[(k + 1) % n1] - x1[k]; p2[k] = y1[(k + 1) % n1] - y1[k]; }
            long* q1 = stackalloc long[n2];
            long* q2 = stackalloc long[n2];
            for (int k = 0; k < n2; k++) { q1[k] = x2[(k + 1) % n2] - x2[k]; q2[k] = y2[(k + 1) % n2] - y2[k]; }
            long* resX = stackalloc long[n1 + n2];
            long* resY = stackalloc long[n1 + n2];
            int m = 0, i = 0, j = 0;
            while (i < n1 && j < n2)
            {
                long cross = p1[i] * q2[j] - p2[i] * q1[j];
                if (cross <= 0) { resX[m] = x1[0] + x2[0] + (i > 0 ? p1[i - 1] : 0) + (j > 0 ? q1[j - 1] : 0); resY[m] = y1[0] + y2[0] + (i > 0 ? p2[i - 1] : 0) + (j > 0 ? q2[j - 1] : 0); i++; }
                else { resX[m] = x1[0] + x2[0] + (i > 0 ? p1[i - 1] : 0) + (j > 0 ? q1[j - 1] : 0); resY[m] = y1[0] + y2[0] + (i > 0 ? p2[i - 1] : 0) + (j > 0 ? q2[j - 1] : 0); j++; }
                m++;
            }
            while (i < n1) { resX[m] = x1[0] + x2[0] + (i > 0 ? p1[i - 1] : 0) + (j > 0 ? q1[j - 1] : 0); resY[m] = y1[0] + y2[0] + (i > 0 ? p2[i - 1] : 0) + (j > 0 ? q2[j - 1] : 0); i++; m++; }
            while (j < n2) { resX[m] = x1[0] + x2[0] + (i > 0 ? p1[i - 1] : 0) + (j > 0 ? q1[j - 1] : 0); resY[m] = y1[0] + y2[0] + (i > 0 ? p2[i - 1] : 0) + (j > 0 ? q2[j - 1] : 0); j++; m++; }
            for (int k = 0; k < m; k++) { rx[k] = resX[k]; ry[k] = resY[k]; }
            return m;
        }
    }

    public static unsafe class ClosestPair
    {
        public static long Run(int n, long* x, long* y)
        {
            if (n <= 1) return long.MaxValue;
            long* sortedX = stackalloc long[n];
            long* sortedY = stackalloc long[n];
            long* yTemp = stackalloc long[n];
            for (int i = 0; i < n; i++) sortedX[i] = x[i];
            for (int i = 0; i < n; i++) sortedY[i] = y[i];
            for (int i = 1; i < n; i++)
            {
                int j = i;
                while (j > 0 && sortedX[j - 1] > sortedX[j])
                {
                    long t = sortedX[j]; sortedX[j] = sortedX[j - 1]; sortedX[j - 1] = t;
                    j--;
                }
            }
            return ClosestPairRecursive(sortedX, sortedY, yTemp, 0, n);
        }

        private static long ClosestPairRecursive(long* x, long* y, long* yTemp, int l, int r)
        {
            if (r - l <= 1) return long.MaxValue;
            int m = (l + r) >> 1;
            long midX = x[m];
            long d = Math.Min(ClosestPairRecursive(x, y, yTemp, l, m), ClosestPairRecursive(x, y, yTemp, m, r));
            int k = 0;
            for (int i = l; i < r; i++)
                if ((y[i] - midX) * (y[i] - midX) < d)
                    yTemp[k++] = y[i];
            return d;
        }
    }

    public static unsafe class CircleLineIntersection
    {
        public static int Run(long cx, long cy, long r, long x1, long y1, long x2, long y2, long* ix, long* iy)
        {
            long dx = x2 - x1;
            long dy = y2 - y1;
            long fx = x1 - cx;
            long fy = y1 - cy;
            long a = dx * dx + dy * dy;
            long b = 2 * (fx * dx + fy * dy);
            long c = fx * fx + fy * fy - r * r;
            long disc = b * b - 4 * a * c;
            if (disc < 0) return 0;
            disc = disc >= 0 ? disc : -disc;
            long sqrtDisc = (long)Math.Sqrt((double)disc);
            long t1 = (-b - sqrtDisc) / (2 * a);
            long t2 = (-b + sqrtDisc) / (2 * a);
            int cnt = 0;
            if (t1 >= 0 && t1 <= 1) { ix[cnt] = x1 + t1 * dx; iy[cnt] = y1 + t1 * dy; cnt++; }
            if (t2 >= 0 && t2 <= 1 && t2 != t1) { ix[cnt] = x1 + t2 * dx; iy[cnt] = y1 + t2 * dy; cnt++; }
            return cnt;
        }
    }

    public static unsafe class CircleCircleIntersection
    {
        public static int Run(long cx1, long cy1, long r1, long cx2, long cy2, long r2, long* ix, long* iy)
        {
            long dx = cx2 - cx1;
            long dy = cy2 - cy1;
            long dSq = dx * dx + dy * dy;
            long d = (long)Math.Sqrt((double)dSq);
            if (d > r1 + r2 || d < Math.Abs(r1 - r2)) return 0;
            long a = (dSq - r2 * r2 + r1 * r1) / (2 * d);
            long px = cx1 + dx * a / d;
            long py = cy1 + dy * a / d;
            long hSq = r1 * r1 - a * a;
            if (hSq < 0) return 0;
            long h = (long)Math.Sqrt((double)hSq);
            long rx = -dy * h / d;
            long ry = dx * h / d;
            ix[0] = px + rx; iy[0] = py + ry;
            ix[1] = px - rx; iy[1] = py - ry;
            return dSq == (r1 + r2) * (r1 + r2) ? 1 : 2;
        }
    }

    public static unsafe class CircleTangents
    {
        public static int Run(long cx1, long cy1, long r1, long cx2, long cy2, long r2, long* ax, long* ay, long* bx, long* by)
        {
            long dx = cx2 - cx1;
            long dy = cy2 - cy1;
            long d = (long)Math.Sqrt(dx * dx + dy * dy);
            if (d <= Math.Abs(r1 - r2)) return 0;
            long cos = (long)((double)r1 / d);
            long sin = (long)Math.Sqrt((double)(r1 * r1 - cos * cos)) / r1;
            long cosSign = r1 > r2 ? 1 : -1;
            ax[0] = cx1 + (dx * cos - dy * sin * cosSign) / d * r1;
            ay[0] = cy1 + (dy * cos + dx * sin * cosSign) / d * r1;
            ax[1] = cx1 + (dx * cos + dy * sin * cosSign) / d * r1;
            ay[1] = cy1 + (dy * cos - dx * sin * cosSign) / d * r1;
            bx[0] = cx2 + (dx * cos - dy * sin * cosSign) / d * r2;
            by[0] = cy2 + (dy * cos + dx * sin * cosSign) / d * r2;
            bx[1] = cx2 + (dx * cos + dy * sin * cosSign) / d * r2;
            by[1] = cy2 + (dy * cos - dx * sin * cosSign) / d * r2;
            return 2;
        }
    }

    public static unsafe class PointCircleTangents
    {
        public static int Run(long px, long py, long cx, long cy, long r, long* tx, long* ty)
        {
            long dx = px - cx;
            long dy = py - cy;
            long dSq = dx * dx + dy * dy;
            if (dSq < r * r) return 0;
            if (dSq == r * r) { tx[0] = px; ty[0] = py; return 1; }
            long d = (long)Math.Sqrt((double)dSq);
            long cos = (long)((double)r / d);
            long sin = (long)Math.Sqrt((double)(r * r - cos * cos)) / r;
            long ex = (long)((double)dx / d);
            long ey = (long)((double)dy / d);
            tx[0] = cx + (ex * cos - ey * sin) * r;
            ty[0] = cy + (ey * cos + ex * sin) * r;
            tx[1] = cx + (ex * cos + ey * sin) * r;
            ty[1] = cy + (ey * cos - ex * sin) * r;
            return 2;
        }
    }

    public static unsafe class Circumcenter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long ax, long ay, long bx, long by, long cx, long cy, long* rx, long* ry)
        {
            long d = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
            if (d == 0) { rx[0] = ax; ry[0] = ay; return; }
            long aSq = ax * ax + ay * ay;
            long bSq = bx * bx + by * by;
            long cSq = cx * cx + cy * cy;
            rx[0] = (aSq * (by - cy) + bSq * (cy - ay) + cSq * (ay - by)) / d;
            ry[0] = (aSq * (cx - bx) + bSq * (ax - cx) + cSq * (bx - ax)) / d;
        }
    }

    public static unsafe class Incenter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long ax, long ay, long bx, long by, long cx, long cy, long* rx, long* ry)
        {
            long da = Dist(cx, cy, bx, by);
            long db = Dist(ax, ay, cx, cy);
            long dc = Dist(ax, ay, bx, by);
            long perim = da + db + dc;
            if (perim == 0) { rx[0] = ax; ry[0] = ay; return; }
            rx[0] = (da * ax + db * bx + dc * cx) / perim;
            ry[0] = (da * ay + db * by + dc * cy) / perim;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Dist(long x1, long y1, long x2, long y2)
        {
            long dx = x2 - x1;
            long dy = y2 - y1;
            return (long)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}