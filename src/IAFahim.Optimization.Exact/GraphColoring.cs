namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GraphColoring
    {
        public static int Run(int n, bool* adj, int* colors, int* sat)
        {
            for (int i = 0; i < n; i++) colors[i] = -1;
            int best = n;
            int* bestColors = stackalloc int[n];
            Search(n, adj, colors, 0, 0, ref best, bestColors);
            for (int i = 0; i < n; i++) colors[i] = bestColors[i];
            return best;
        }

        private static void Search(int n, bool* adj, int* colors, int v, int used, ref int best, int* bestColors)
        {
            if (used >= best) return;
            if (v == n)
            {
                if (used < best)
                {
                    best = used;
                    for (int i = 0; i < n; i++) bestColors[i] = colors[i];
                }
                return;
            }
            for (int c = 0; c < used; c++)
            {
                if (!CanUseColor(n, adj, colors, v, c)) continue;
                colors[v] = c;
                Search(n, adj, colors, v + 1, used, ref best, bestColors);
                colors[v] = -1;
            }
            if (used + 1 < best)
            {
                colors[v] = used;
                Search(n, adj, colors, v + 1, used + 1, ref best, bestColors);
                colors[v] = -1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanUseColor(int n, bool* adj, int* colors, int v, int c)
        {
            for (int u = 0; u < n; u++)
                if (adj[v * n + u] && colors[u] == c) return false;
            return true;
        }
    }
}
