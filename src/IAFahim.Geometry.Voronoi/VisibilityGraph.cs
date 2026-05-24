namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class VisibilityGraph
    {
        public static int Build(double* ox, double* oy, int n, int* outFrom, int* outTo, double* outW)
        {
            int e = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (IsVisible(i, j, n, ox, oy)) { outFrom[e] = i; outTo[e] = j; outW[e++] = Dist(ox[i], oy[i], ox[j], oy[j]); }
            return e;
        }

        private static bool IsVisible(int i, int j, int n, double* ox, double* oy)
        {
            double x1 = ox[i], y1 = oy[i], x2 = ox[j], y2 = oy[j];
            for (int k = 0; k < n; k++)
            {
                int kn = (k + 1 == n) ? 0 : k + 1;
                if (k == i || k == j || kn == i || kn == j) continue;
                if (SegmentsIntersect(x1, y1, x2, y2, ox[k], oy[k], ox[kn], oy[kn])) return false;
            }
            double mx = (x1 + x2) * 0.5, my = (y1 + y2) * 0.5;
            return IsInside(mx, my, n, ox, oy) || Math.Abs(j - i) == 1 || (i == 0 && j == n - 1);
        }

        private static bool IsInside(double x, double y, int n, double* ox, double* oy)
        {
            bool inside = false;
            for (int k = 0, l = n - 1; k < n; l = k++)
                if ((oy[k] > y) != (oy[l] > y) && x < (ox[l] - ox[k]) * (y - oy[k]) / (oy[l] - oy[k] + 1e-12) + ox[k]) inside = !inside;
            return inside;
        }

        private static double Dist(double x1, double y1, double x2, double y2) => Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SegmentsIntersect(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            double d1 = Dir(x3, y3, x4, y4, x1, y1), d2 = Dir(x3, y3, x4, y4, x2, y2);
            double d3 = Dir(x1, y1, x2, y2, x3, y3), d4 = Dir(x1, y1, x2, y2, x4, y4);
            if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0))) return true;
            if (d1 == 0 && OnSeg(x3, y3, x4, y4, x1, y1)) return true;
            if (d2 == 0 && OnSeg(x3, y3, x4, y4, x2, y2)) return true;
            if (d3 == 0 && OnSeg(x1, y1, x2, y2, x3, y3)) return true;
            if (d4 == 0 && OnSeg(x1, y1, x2, y2, x4, y4)) return true;
            return false;
        }
        private static double Dir(double xi, double yi, double xj, double yj, double xk, double yk) => (xk - xi) * (yj - yi) - (xj - xi) * (yk - yi);
        private static bool OnSeg(double xi, double yi, double xj, double yj, double xk, double yk) => Math.Min(xi, xj) <= xk && xk <= Math.Max(xi, xj) && Math.Min(yi, yj) <= yk && yk <= Math.Max(yi, yj);
    }
}
