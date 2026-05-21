namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GraphColoring
    {
        public static int Run(int n, bool* adj, int* colors, int* sat)
        {
            for (int i = 0; i < n; i++) colors[i] = -1;
            for (int i = 0; i < n * n; i++) sat[i] = 1;
            return Search(n, adj, colors, sat, 0, 0);
        }

        private static int Search(int n, bool* adj, int* colors, int* sat, int v, int used)
        {
            if (v == n) return used;
            int maxSat = -1, bestColor = 0;
            for (int c = 0; c < used; c++)
            {
                if (sat[v * n + c] > maxSat) { maxSat = sat[v * n + c]; bestColor = c; }
            }
            int* order = stackalloc int[used + 1];
            int orderSize = 0;
            order[orderSize++] = bestColor;
            for (int c = 0; c < used; c++)
                if (c != bestColor) order[orderSize++] = c;
            if (used < n) order[orderSize++] = used;
            for (int oi = 0; oi < orderSize; oi++)
            {
                int c = order[oi];
                if (c == used)
                {
                    colors[v] = used;
                    for (int u = 0; u < n; u++) sat[v * n + u] = sat[u * n + v] = 0;
                    int res = Search(n, adj, colors, sat, v + 1, used + 1);
                    if (res > 0) return res;
                    colors[v] = -1;
                    for (int u = 0; u < n; u++) sat[v * n + u] = sat[u * n + v] = 1;
                }
                else
                {
                    bool ok = true;
                    for (int u = 0; u < n; u++)
                        if (adj[v * n + u] && colors[u] == c) { ok = false; break; }
                    if (!ok) continue;
                    colors[v] = c;
                    for (int u = 0; u < n; u++)
                        if (adj[v * n + u] && colors[u] == -1) sat[u * n + c] = 0;
                    int res = Search(n, adj, colors, sat, v + 1, used);
                    if (res > 0) return res;
                    colors[v] = -1;
                    for (int u = 0; u < n; u++)
                        if (adj[v * n + u] && colors[u] == -1) sat[u * n + c] = 1;
                }
            }
            return 0;
        }
    }
}
