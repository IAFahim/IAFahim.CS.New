namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class VisibilityGraph
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* ox, double* oy, int n, int* outFrom, int* outTo, double* outW)
        {
            int e = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    bool visible = true;
                    double x1 = ox[i], y1 = oy[i];
                    double x2 = ox[j], y2 = oy[j];
                    
                    for (int k = 0; k < n; k++)
                    {
                        int kNext = k + 1 == n ? 0 : k + 1;
                        if (k == i || k == j || kNext == i || kNext == j)
                            continue;

                        double x3 = ox[k], y3 = oy[k];
                        double x4 = ox[kNext], y4 = oy[kNext];

                        if (SegmentsIntersect(x1, y1, x2, y2, x3, y3, x4, y4))
                        {
                            visible = false;
                            break;
                        }
                    }

                    if (visible)
                    {
                        double mx = (x1 + x2) * 0.5;
                        double my = (y1 + y2) * 0.5;
                        bool inside = false;
                        for (int k = 0, l = n - 1; k < n; l = k++)
                        {
                            if ((oy[k] > my) != (oy[l] > my) &&
                                mx < (ox[l] - ox[k]) * (my - oy[k]) / (oy[l] - oy[k] + 1e-12) + ox[k])
                            {
                                inside = !inside;
                            }
                        }
                        
                        // For polygon vertices, a line segment is part of visibility graph if it's completely inside or is an edge.
                        // Since we just check midpoint, it handles general case for simple polygons.
                        if (inside || Math.Abs(j - i) == 1 || (i == 0 && j == n - 1))
                        {
                            outFrom[e] = i;
                            outTo[e] = j;
                            double dx = x1 - x2;
                            double dy = y1 - y2;
                            outW[e++] = Math.Sqrt(dx * dx + dy * dy);
                        }
                    }
                }
            }
            return e;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SegmentsIntersect(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            double d1 = Direction(x3, y3, x4, y4, x1, y1);
            double d2 = Direction(x3, y3, x4, y4, x2, y2);
            double d3 = Direction(x1, y1, x2, y2, x3, y3);
            double d4 = Direction(x1, y1, x2, y2, x4, y4);

            if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
                ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
                return true;

            if (d1 == 0 && OnSegment(x3, y3, x4, y4, x1, y1)) return true;
            if (d2 == 0 && OnSegment(x3, y3, x4, y4, x2, y2)) return true;
            if (d3 == 0 && OnSegment(x1, y1, x2, y2, x3, y3)) return true;
            if (d4 == 0 && OnSegment(x1, y1, x2, y2, x4, y4)) return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Direction(double xi, double yi, double xj, double yj, double xk, double yk)
        {
            return (xk - xi) * (yj - yi) - (xj - xi) * (yk - yi);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool OnSegment(double xi, double yi, double xj, double yj, double xk, double yk)
        {
            return Math.Min(xi, xj) <= xk && xk <= Math.Max(xi, xj) &&
                   Math.Min(yi, yj) <= yk && yk <= Math.Max(yi, yj);
        }
    }
}
