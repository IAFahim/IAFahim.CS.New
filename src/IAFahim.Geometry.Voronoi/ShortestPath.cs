namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ShortestPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Run(double* ox, double* oy, int n, int src, int dst, int* from, int* to, double* w, int e)
        {
            if (src == dst) return 0;
            double* dist = stackalloc double[n];
            bool* vis = stackalloc bool[n];
            for (int i = 0; i < n; i++) { dist[i] = double.MaxValue; vis[i] = false; }
            dist[src] = 0;
            for (int it = 0; it < n; it++)
            {
                int v = -1;
                double best = double.MaxValue;
                for (int i = 0; i < n; i++)
                    if (!vis[i] && dist[i] < best) { best = dist[i]; v = i; }
                if (v < 0) break;
                if (v == dst) return dist[dst];
                vis[v] = true;
                for (int i = 0; i < e; i++)
                {
                    int u = from[i] == v ? to[i] : from[i] == v ? to[i] : -1;
                    if (u < 0) continue;
                    if (dist[u] > dist[v] + w[i])
                        dist[u] = dist[v] + w[i];
                }
            }
            return dist[dst];
        }
    }
}
