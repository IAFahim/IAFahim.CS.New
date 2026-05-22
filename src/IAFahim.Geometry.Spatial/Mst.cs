namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Mst
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Manhattan(double* xs, double* ys, int n, int* outFrom, int* outTo, double* outW)
        {
            if (n <= 1) return 0;

            double* dist = stackalloc double[n];
            int* parent = stackalloc int[n];
            bool* vis = stackalloc bool[n];

            for (int i = 0; i < n; i++)
            {
                dist[i] = double.MaxValue;
                vis[i] = false;
                parent[i] = -1;
            }

            dist[0] = 0;
            double totalWeight = 0;
            int edgeCount = 0;

            for (int i = 0; i < n; i++)
            {
                int u = -1;
                double best = double.MaxValue;
                for (int j = 0; j < n; j++)
                {
                    if (!vis[j] && dist[j] < best)
                    {
                        best = dist[j];
                        u = j;
                    }
                }

                if (u < 0) break;
                vis[u] = true;

                if (parent[u] != -1)
                {
                    outFrom[edgeCount] = parent[u];
                    outTo[edgeCount] = u;
                    outW[edgeCount] = dist[u];
                    totalWeight += dist[u];
                    edgeCount++;
                }

                for (int v = 0; v < n; v++)
                {
                    if (!vis[v])
                    {
                        double w = Math.Abs(xs[u] - xs[v]) + Math.Abs(ys[u] - ys[v]);
                        if (w < dist[v])
                        {
                            dist[v] = w;
                            parent[v] = u;
                        }
                    }
                }
            }

            return totalWeight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Rectilinear(double* xs, double* ys, int n, int* outFrom, int* outTo, double* outW)
        {
            return Manhattan(xs, ys, n, outFrom, outTo, outW);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Euclidean(double* xs, double* ys, int n, int* outFrom, int* outTo, double* outW)
        {
            if (n <= 1) return 0;

            double* dist = stackalloc double[n];
            int* parent = stackalloc int[n];
            bool* vis = stackalloc bool[n];

            for (int i = 0; i < n; i++)
            {
                dist[i] = double.MaxValue;
                vis[i] = false;
                parent[i] = -1;
            }

            dist[0] = 0;
            double totalWeight = 0;
            int edgeCount = 0;

            for (int i = 0; i < n; i++)
            {
                int u = -1;
                double best = double.MaxValue;
                for (int j = 0; j < n; j++)
                {
                    if (!vis[j] && dist[j] < best)
                    {
                        best = dist[j];
                        u = j;
                    }
                }

                if (u < 0) break;
                vis[u] = true;

                if (parent[u] != -1)
                {
                    outFrom[edgeCount] = parent[u];
                    outTo[edgeCount] = u;
                    outW[edgeCount] = Math.Sqrt(dist[u]);
                    totalWeight += outW[edgeCount];
                    edgeCount++;
                }

                for (int v = 0; v < n; v++)
                {
                    if (!vis[v])
                    {
                        double dx = xs[u] - xs[v];
                        double dy = ys[u] - ys[v];
                        double w2 = dx * dx + dy * dy;
                        if (w2 < dist[v])
                        {
                            dist[v] = w2;
                            parent[v] = u;
                        }
                    }
                }
            }

            return totalWeight;
        }
    }
}
