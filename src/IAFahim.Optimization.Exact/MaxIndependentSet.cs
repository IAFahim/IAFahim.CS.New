namespace IAFahim.Optimization.Exact
{
    using System;

    public static unsafe class MaxIndependentSet
    {
        public static int Run(int n, bool* adj, int* used, int* best, int* tmp)
        {
            *best = 0;
            Search(n, adj, used, 0, best, 0, 0, tmp);
            return *best;
        }

        private static void Search(int n, bool* adj, int* used, int v, int* best, int cur, int depth, int* tmp)
        {
            if (v == n) { if (cur > *best) *best = cur; return; }

            if (CanIncludeNode(n, adj, used, v))
            {
                used[v] = 1;
                Search(n, adj, used, v + 1, best, cur + 1, depth + 1, tmp);
                used[v] = 0;
            }
            Search(n, adj, used, v + 1, best, cur, depth + 1, tmp);
        }

        private static bool CanIncludeNode(int n, bool* adj, int* used, int v)
        {
            for (int u = 0; u < n; u++)
                if (adj[v * n + u] && used[u] != 0) return false;
            return true;
        }
    }
}
