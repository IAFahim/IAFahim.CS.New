namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class VisibilityGraph
    {
        private const double MidpointWeight = 0.5;

        private const double RayCastEpsilon = 1e-12;

        public static int Build(double* ox, double* oy, int n, int* outFrom, int* outTo, double* outW)
        {
            int e = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (IsVisible(i, j, n, ox, oy)) { outFrom[e] = i; outTo[e] = j; outW[e++] = Dist(ox[i], oy[i], ox[j], oy[j]); }
            return e;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPolygonBoundaryEdge(int i, int j, int n)
            => Math.Abs(j - i) == 1 || (i == 0 && j == n - 1);

        private static bool IsVisible(int i, int j, int n, double* ox, double* oy)
        {
            double x1 = ox[i], y1 = oy[i], x2 = ox[j], y2 = oy[j];
            for (int k = 0; k < n; k++)
            {
                int kn = (k + 1 == n) ? 0 : k + 1;
                if (k == i || k == j || kn == i || kn == j) continue;
                if (SegmentsIntersect(x1, y1, x2, y2, ox[k], oy[k], ox[kn], oy[kn])) return false;
            }
            double mx = (x1 + x2) * MidpointWeight, my = (y1 + y2) * MidpointWeight;
            return IsInside(mx, my, n, ox, oy) || IsPolygonBoundaryEdge(i, j, n);
        }

        private static bool IsInside(double x, double y, int n, double* ox, double* oy)
        {
            bool inside = false;
            for (int k = 0, l = n - 1; k < n; l = k++)
                if ((oy[k] > y) != (oy[l] > y) && x < (ox[l] - ox[k]) * (y - oy[k]) / (oy[l] - oy[k] + RayCastEpsilon) + ox[k]) inside = !inside;
            return inside;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Dist(double x1, double y1, double x2, double y2)
            => Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool OppositeSign(double a, double b)
            => (a > 0 && b < 0) || (a < 0 && b > 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ColinearHit(double d, double sxi, double syi, double sxj, double syj, double px, double py)
            => d == 0 && OnSeg(sxi, syi, sxj, syj, px, py);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SegmentsIntersect(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            double d1 = Dir(x3, y3, x4, y4, x1, y1), d2 = Dir(x3, y3, x4, y4, x2, y2);
            double d3 = Dir(x1, y1, x2, y2, x3, y3), d4 = Dir(x1, y1, x2, y2, x4, y4);
            if (OppositeSign(d1, d2) && OppositeSign(d3, d4)) return true;
            if (ColinearHit(d1, x3, y3, x4, y4, x1, y1)) return true;
            if (ColinearHit(d2, x3, y3, x4, y4, x2, y2)) return true;
            if (ColinearHit(d3, x1, y1, x2, y2, x3, y3)) return true;
            if (ColinearHit(d4, x1, y1, x2, y2, x4, y4)) return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Dir(double xi, double yi, double xj, double yj, double xk, double yk)
            => (xk - xi) * (yj - yi) - (xj - xi) * (yk - yi);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool OnSeg(double xi, double yi, double xj, double yj, double xk, double yk)
            => Math.Min(xi, xj) <= xk && xk <= Math.Max(xi, xj) && Math.Min(yi, yj) <= yk && yk <= Math.Max(yi, yj);
    }
}
