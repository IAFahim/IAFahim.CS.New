namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RotatingCalipers
    {
        private const double CollinearEpsilon = 1e-12;

        private const int MinPointsForArea = 3;

        public struct Rect { public double X, Y, W, H, Angle; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Cross(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Dot(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return (x2 - x1) * (x3 - x1) + (y2 - y1) * (y3 - y1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double DistSq(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FindExtremalIndices(double* xs, double* ys, int n, out int bottom, out int top, out int left, out int right)
        {
            top = 1; bottom = 0; left = 0; right = 0;
            for (int i = 0; i < n; i++)
            {
                if (ys[i] < ys[bottom]) bottom = i;
                if (ys[i] > ys[top]) top = i;
                if (xs[i] < xs[left]) left = i;
                if (xs[i] > xs[right]) right = i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AdvanceTopCaliper(double* xs, double* ys, int n, int i, int nxt, ref int t)
        {
            while (Cross(xs[i], ys[i], xs[nxt], ys[nxt], xs[(t + 1) % n], ys[(t + 1) % n]) >= Cross(xs[i], ys[i], xs[nxt], ys[nxt], xs[t], ys[t]))
                t = (t + 1) % n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AdvanceRightCaliper(double* xs, double* ys, int n, int i, int nxt, ref int ri)
        {
            while (Dot(xs[i], ys[i], xs[nxt], ys[nxt], xs[(ri + 1) % n], ys[(ri + 1) % n]) >= Dot(xs[i], ys[i], xs[nxt], ys[nxt], xs[ri], ys[ri]))
                ri = (ri + 1) % n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AdvanceLeftCaliper(double* xs, double* ys, int n, int i, int nxt, ref int l)
        {
            while (Dot(xs[i], ys[i], xs[nxt], ys[nxt], xs[(l + 1) % n], ys[(l + 1) % n]) <= Dot(xs[i], ys[i], xs[nxt], ys[nxt], xs[l], ys[l]))
                l = (l + 1) % n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ConsiderRectangle(double width, double height, double dx, double dy, double originX, double originY, ref double minArea, ref Rect r)
        {
            double area = width * height;
            if (area < minArea)
            {
                minArea = area;
                r.W = width;
                r.H = height;
                r.Angle = Math.Atan2(dy, dx);
                r.X = originX;
                r.Y = originY;
            }
        }

        public static Rect MinArea(double* xs, double* ys, int n)
        {
            Rect r = default;
            if (n < MinPointsForArea) return r;
            FindExtremalIndices(xs, ys, n, out int bottom, out int top, out int left, out int right);
            double minArea = double.MaxValue;
            int t = top, l = left, ri = right;
            for (int i = 0; i < n; i++)
            {
                int nxt = (i + 1) % n;
                double dx = xs[nxt] - xs[i];
                double dy = ys[nxt] - ys[i];
                double len = Math.Sqrt(dx * dx + dy * dy);
                if (len < CollinearEpsilon) continue;
                AdvanceTopCaliper(xs, ys, n, i, nxt, ref t);
                AdvanceRightCaliper(xs, ys, n, i, nxt, ref ri);
                if (i == 0) l = ri;
                AdvanceLeftCaliper(xs, ys, n, i, nxt, ref l);
                double height = Cross(xs[i], ys[i], xs[nxt], ys[nxt], xs[t], ys[t]) / len;
                double width = (Dot(xs[i], ys[i], xs[nxt], ys[nxt], xs[ri], ys[ri]) - Dot(xs[i], ys[i], xs[nxt], ys[nxt], xs[l], ys[l])) / len;
                ConsiderRectangle(width, height, dx, dy, xs[i], ys[i], ref minArea, ref r);
            }
            return r;
        }

        public static double MinWidth(double* xs, double* ys, int n)
        {
            if (n < MinPointsForArea) return 0;
            double minW = double.MaxValue;
            int t = 1;
            for (int i = 0; i < n; i++)
            {
                int nxt = (i + 1) % n;
                double dx = xs[nxt] - xs[i];
                double dy = ys[nxt] - ys[i];
                double len = Math.Sqrt(dx * dx + dy * dy);
                if (len < CollinearEpsilon) continue;
                while (Cross(xs[i], ys[i], xs[nxt], ys[nxt], xs[(t + 1) % n], ys[(t + 1) % n]) >= Cross(xs[i], ys[i], xs[nxt], ys[nxt], xs[t], ys[t]))
                    t = (t + 1) % n;
                double w = Cross(xs[i], ys[i], xs[nxt], ys[nxt], xs[t], ys[t]) / len;
                if (w < minW) minW = w;
            }
            return minW;
        }
    }
}
