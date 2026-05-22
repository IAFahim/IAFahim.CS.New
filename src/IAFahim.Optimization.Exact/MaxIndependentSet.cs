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
            bool canUse = true;
            for (int u = 0; u < n; u++)
                if (adj[v * n + u] && used[u] != 0) { canUse = false; break; }
            int* currentTmp = tmp + depth * n;
            int sz = 0;
            for (int i = v + 1; i < n; i++) currentTmp[sz++] = i;
            if (canUse)
            {
                used[v] = 1;
                Search(n, adj, used, v + 1, best, cur + 1, depth + 1, tmp);
                used[v] = 0;
            }
            Search(n, adj, used, v + 1, best, cur, depth + 1, tmp);
        }
    }
}
