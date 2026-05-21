namespace IAFahim.Geometry.Basic
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GeometryPoint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* x, long* y, long px, long py)
        {
            x[0] = px;
            y[0] = py;
        }
    }

    public static unsafe class PointAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* ax, long* ay, long bx, long by, long* rx, long* ry)
        {
            rx[0] = ax[0] + bx;
            ry[0] = ay[0] + by;
        }
    }

    public static unsafe class PointSub
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* ax, long* ay, long bx, long by, long* rx, long* ry)
        {
            rx[0] = ax[0] - bx;
            ry[0] = ay[0] - by;
        }
    }

    public static unsafe class PointDot
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long ax, long ay, long bx, long by)
        {
            return ax * bx + ay * by;
        }
    }

    public static unsafe class PointCross
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long ax, long ay, long bx, long by)
        {
            return ax * by - ay * bx;
        }
    }

    public static unsafe class PointNorm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x, long y)
        {
            return x * x + y * y;
        }
    }

    public static unsafe class PointDist
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x1, long y1, long x2, long y2)
        {
            long dx = x2 - x1;
            long dy = y2 - y1;
            return dx * dx + dy * dy;
        }
    }

    public static unsafe class PointRotate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long x, long y, long angleSin, long angleCos, long* rx, long* ry)
        {
            rx[0] = x * angleCos - y * angleSin;
            ry[0] = x * angleSin + y * angleCos;
        }
    }

    public static unsafe class PointAngle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long ax, long ay, long bx, long by)
        {
            long dot = ax * bx + ay * by;
            long cross = ax * by - ay * bx;
            return cross == 0 ? dot : cross;
        }
    }

    public static unsafe class Orientation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long ax, long ay, long bx, long by, long cx, long cy)
        {
            long cross = (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
            if (cross > 0) return 1;
            if (cross < 0) return -1;
            return 0;
        }
    }

    public static unsafe class Ccw
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long ax, long ay, long bx, long by, long cx, long cy)
        {
            long cross = (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
            if (cross > 0) return 1;
            if (cross < 0) return -1;
            return 0;
        }
    }

    public static unsafe class OnSegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(long px, long py, long ax, long ay, long bx, long by)
        {
            if (Orientation.Run(ax, ay, bx, by, px, py) != 0) return false;
            long minX = ax < bx ? ax : bx;
            long maxX = ax > bx ? ax : bx;
            long minY = ay < by ? ay : by;
            long maxY = ay > by ? ay : by;
            return px >= minX && px <= maxX && py >= minY && py <= maxY;
        }
    }

    public static unsafe class SegmentIntersect
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(long ax, long ay, long bx, long by, long cx, long cy, long dx, long dy)
        {
            int o1 = Orientation.Run(ax, ay, bx, by, cx, cy);
            int o2 = Orientation.Run(ax, ay, bx, by, dx, dy);
            int o3 = Orientation.Run(cx, cy, dx, dy, ax, ay);
            int o4 = Orientation.Run(cx, cy, dx, dy, bx, by);
            if (o1 != o2 && o3 != o4) return true;
            if (o1 == 0 && OnSegment.Run(cx, cy, ax, ay, bx, by)) return true;
            if (o2 == 0 && OnSegment.Run(dx, dy, ax, ay, bx, by)) return true;
            if (o3 == 0 && OnSegment.Run(ax, ay, cx, cy, dx, dy)) return true;
            if (o4 == 0 && OnSegment.Run(bx, by, cx, cy, dx, dy)) return true;
            return false;
        }
    }

    public static unsafe class LineIntersect
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(long ax, long ay, long bx, long by, long cx, long cy, long dx, long dy, long* ix, long* iy)
        {
            long d = (bx - ax) * (dy - cy) - (by - ay) * (dx - cx);
            if (d == 0) return false;
            long t = ((cx - ax) * (dy - cy) - (cy - ay) * (dx - cx));
            ix[0] = ax + t * (bx - ax) / d;
            iy[0] = ay + t * (by - ay) / d;
            return true;
        }
    }

    public static unsafe class LineProjection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long px, long py, long ax, long ay, long bx, long by, long* rx, long* ry)
        {
            long dx = bx - ax;
            long dy = by - ay;
            long len = dx * dx + dy * dy;
            if (len == 0) { rx[0] = ax; ry[0] = ay; return; }
            long t = ((px - ax) * dx + (py - ay) * dy);
            rx[0] = ax + t * dx / len;
            ry[0] = ay + t * dy / len;
        }
    }

    public static unsafe class LineReflection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long px, long py, long ax, long ay, long bx, long by, long* rx, long* ry)
        {
            long projX, projY;
            long* pxPtr = &projX;
            long* pyPtr = &projY;
            LineProjection.Run(px, py, ax, ay, bx, by, pxPtr, pyPtr);
            rx[0] = 2 * projX - px;
            ry[0] = 2 * projY - py;
        }
    }

    public static unsafe class DistancePointLine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long px, long py, long ax, long ay, long bx, long by)
        {
            long dx = bx - ax;
            long dy = by - ay;
            long cross = (px - ax) * dy - (py - ay) * dx;
            long lenSq = dx * dx + dy * dy;
            if (lenSq == 0) return PointDist.Run(px, py, ax, ay);
            return cross * cross / lenSq;
        }
    }

    public static unsafe class DistancePointSegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long px, long py, long ax, long ay, long bx, long by)
        {
            long dx = bx - ax;
            long dy = by - ay;
            long lenSq = dx * dx + dy * dy;
            if (lenSq == 0) return PointDist.Run(px, py, ax, ay);
            long t = ((px - ax) * dx + (py - ay) * dy);
            if (t < 0) return PointDist.Run(px, py, ax, ay);
            if (t > lenSq) return PointDist.Run(px, py, bx, by);
            long projX = ax + t * dx / lenSq;
            long projY = ay + t * dy / lenSq;
            return PointDist.Run(px, py, projX, projY);
        }
    }

    public static unsafe class DistanceSegmentSegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long ax, long ay, long bx, long by, long cx, long cy, long dx, long dy)
        {
            if (SegmentIntersect.Run(ax, ay, bx, by, cx, cy, dx, dy)) return 0;
            long d1 = DistancePointSegment.Run(ax, ay, cx, cy, dx, dy);
            long d2 = DistancePointSegment.Run(bx, by, cx, cy, dx, dy);
            long d3 = DistancePointSegment.Run(cx, cy, ax, ay, bx, by);
            long d4 = DistancePointSegment.Run(dx, dy, ax, ay, bx, by);
            long min = d1;
            if (d2 < min) min = d2;
            if (d3 < min) min = d3;
            if (d4 < min) min = d4;
            return min;
        }
    }

    public static unsafe class PolygonArea
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long* x, long* y)
        {
            long area = 0;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += x[i] * y[j];
                area -= x[j] * y[i];
            }
            if (area < 0) area = -area;
            return area;
        }
    }

    public static unsafe class PolygonCentroid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, long* x, long* y, long* cx, long* cy)
        {
            long area = 0;
            cx[0] = 0;
            cy[0] = 0;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                long cross = x[i] * y[j] - x[j] * y[i];
                area += cross;
                cx[0] += (x[i] + x[j]) * cross;
                cy[0] += (y[i] + y[j]) * cross;
            }
            if (area != 0)
            {
                cx[0] = cx[0] * 3 / area;
                cy[0] = cy[0] * 3 / area;
            }
        }
    }

    public static unsafe class PolygonContains
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* x, long* y, long px, long py)
        {
            bool inside = false;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                long xi = x[i], yi = y[i], xj = x[j], yj = y[j];
                if ((yi > py) != (yj > py))
                {
                    long dy = yj - yi;
                    long rhs = xi * dy + (xj - xi) * (py - yi);
                    if (px * dy < rhs) inside = !inside;
                }
            }
            return inside ? 1 : 0;
        }
    }
}