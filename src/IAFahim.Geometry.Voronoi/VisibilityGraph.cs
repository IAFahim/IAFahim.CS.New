namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class VisibilityGraph
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* ox, double* oy, int n, int* outFrom, int* outTo, double* outW)
        {
            int e = 0;
            for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            {
                bool visible = true;
                for (int k = 0; k < n && visible; k++)
                {
                    if (k == i || k == j) continue;
                    double x1 = ox[i], y1 = oy[i], x2 = ox[j], y2 = oy[j];
                    double x3 = ox[k], y3 = oy[k], x4 = ox[(k + 1) % n], y4 = oy[(k + 1) % n];
                    double det = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
                    if (Math.Abs(det) < 1e-12) continue;
                    double px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / det;
                    double py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / det;
                    if (px > Math.Min(x1, x2) && px < Math.Max(x1, x2) && py > Math.Min(y1, y2) && py < Math.Max(y1, y2))
                        visible = false;
                }
                if (visible)
                {
                    outFrom[e] = i; outTo[e] = j;
                    double dx = ox[i] - ox[j], dy = oy[i] - oy[j];
                    outW[e++] = Math.Sqrt(dx * dx + dy * dy);
                }
            }
            return e;
        }
    }
}
