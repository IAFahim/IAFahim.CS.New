namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RotatingCalipers
    {
        public struct Rect { public double X, Y, W, H, Angle; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect MinArea(double* xs, double* ys, int n)
        {
            Rect r = default;
            if (n < 3) { r.W = r.H = 0; return r; }
            double minArea = double.MaxValue;
            int j = 1;
            for (int i = 0; i < n; i++)
            {
                int ni = (i + 1) % n;
                double dx = xs[ni] - xs[i], dy = ys[ni] - ys[i];
                double len = Math.Sqrt(dx * dx + dy * dy);
                if (len < 1e-12) continue;
                dx /= len; dy /= len;
                while (true)
                {
                    int nj = (j + 1) % n;
                    double proj = (xs[nj] - xs[i]) * dx + (ys[nj] - ys[i]) * dy;
                    double maxProj = (xs[j] - xs[i]) * dx + (ys[j] - ys[i]) * dy;
                    if (proj > maxProj) j = (j + 1) % n;
                    else break;
                }
                double area = len * Math.Abs((xs[j] - xs[i]) * dy - (ys[j] - ys[i]) * dx);
                if (area < minArea) { minArea = area; r.W = len; r.H = area / len; r.Angle = Math.Atan2(dy, dx); }
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double MinWidth(double* xs, double* ys, int n)
        {
            double minW = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                int ni = (i + 1) % n;
                double dx = xs[ni] - xs[i], dy = ys[ni] - ys[i];
                double len = Math.Sqrt(dx * dx + dy * dy);
                if (len < 1e-12) continue;
                double nx = dy / len, ny = -dx / len;
                double minP = double.MaxValue, maxP = double.MinValue;
                for (int k = 0; k < n; k++)
                {
                    double p = xs[k] * nx + ys[k] * ny;
                    if (p < minP) minP = p;
                    if (p > maxP) maxP = p;
                }
                double w = maxP - minP;
                if (w < minW) minW = w;
            }
            return minW == double.MaxValue ? 0 : minW;
        }
    }
}
