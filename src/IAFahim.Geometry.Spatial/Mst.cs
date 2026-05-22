namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Mst
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Manhattan(double* xs, double* ys, int n, int* outFrom, int* outTo, double* outW)
        {
            int e = 0;
            for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            {
                double w = Math.Abs(xs[i] - xs[j]) + Math.Abs(ys[i] - ys[j]);
                outFrom[e] = i; outTo[e] = j; outW[e++] = w;
            }
            return PrimMst(n, outFrom, outTo, outW, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Rectilinear(double* xs, double* ys, int n, int* outFrom, int* outTo, double* outW)
        {
            return Manhattan(xs, ys, n, outFrom, outTo, outW);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Euclidean(double* xs, double* ys, int n, int* outFrom, int* outTo, double* outW)
        {
            int e = 0;
            for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            {
                double dx = xs[i] - xs[j], dy = ys[i] - ys[j];
                outFrom[e] = i; outTo[e] = j; outW[e++] = Math.Sqrt(dx * dx + dy * dy);
            }
            return PrimMst(n, outFrom, outTo, outW, e);
        }

        private static double PrimMst(int n, int* from, int* to, double* w, int e)
        {
            double* dist = stackalloc double[n];
            int* parent = stackalloc int[n];
            bool* vis = stackalloc bool[n];
            for (int i = 0; i < n; i++) { dist[i] = double.MaxValue; vis[i] = false; parent[i] = -1; }
            dist[0] = 0;
            for (int it = 0; it < n; it++)
            {
                int v = -1;
                double best = double.MaxValue;
                for (int i = 0; i < n; i++)
                    if (!vis[i] && dist[i] < best) { best = dist[i]; v = i; }
                if (v < 0) break;
                vis[v] = true;
                for (int i = 0; i < e; i++)
                {
                    int u = from[i] == v ? to[i] : to[i] == v ? from[i] : -1;
                    if (u >= 0 && w[i] < dist[u]) { dist[u] = w[i]; parent[u] = v; }
                }
            }
            double total = 0;
            for (int i = 0; i < n; i++) total += dist[i];
            return total;
        }
    }
}
