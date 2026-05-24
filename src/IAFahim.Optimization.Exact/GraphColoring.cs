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
            int bestColor = FindBestColor(n, v, used, sat);
            int* order = stackalloc int[used + 1];
            int orderSize = BuildColorOrder(used, n, bestColor, order);

            for (int oi = 0; oi < orderSize; oi++)
            {
                int c = order[oi];
                if (c == used)
                {
                    int res = TryNewColor(n, adj, colors, sat, v, used);
                    if (res > 0) return res;
                }
                else
                {
                    if (!CanUseColor(n, adj, colors, v, c)) continue;
                    int res = TryExistingColor(n, adj, colors, sat, v, c, used);
                    if (res > 0) return res;
                }
            }
            return 0;
        }

        private static int FindBestColor(int n, int v, int used, int* sat)
        {
            int maxSat = -1, bestColor = 0;
            for (int c = 0; c < used; c++)
                if (sat[v * n + c] > maxSat) { maxSat = sat[v * n + c]; bestColor = c; }
            return bestColor;
        }

        private static int BuildColorOrder(int used, int n, int bestColor, int* order)
        {
            int sz = 0; order[sz++] = bestColor;
            for (int c = 0; c < used; c++) if (c != bestColor) order[sz++] = c;
            if (used < n) order[sz++] = used;
            return sz;
        }

        private static int TryNewColor(int n, bool* adj, int* colors, int* sat, int v, int used)
        {
            colors[v] = used;
            for (int u = 0; u < n; u++) sat[v * n + u] = sat[u * n + v] = 0;
            int res = Search(n, adj, colors, sat, v + 1, used + 1);
            if (res > 0) return res;
            colors[v] = -1;
            for (int u = 0; u < n; u++) sat[v * n + u] = sat[u * n + v] = 1;
            return 0;
        }

        private static bool CanUseColor(int n, bool* adj, int* colors, int v, int c)
        {
            for (int u = 0; u < n; u++)
                if (adj[v * n + u] && colors[u] == c) return false;
            return true;
        }

        private static int TryExistingColor(int n, bool* adj, int* colors, int* sat, int v, int c, int used)
        {
            colors[v] = c;
            UpdateSaturation(n, adj, colors, sat, v, c, 0);
            int res = Search(n, adj, colors, sat, v + 1, used);
            if (res > 0) return res;
            colors[v] = -1;
            UpdateSaturation(n, adj, colors, sat, v, c, 1);
            return 0;
        }

        private static void UpdateSaturation(int n, bool* adj, int* colors, int* sat, int v, int c, int val)
        {
            for (int u = 0; u < n; u++)
                if (adj[v * n + u] && colors[u] == -1) sat[u * n + c] = val;
        }
    }
}
