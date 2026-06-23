namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinDominatingSet
    {
        public static int Run(int n, bool* adj, int* dom, int* best, int* tmp)
        {
            for (int i = 0; i < n; i++) dom[i] = 0;
            for (int i = 0; i < n; i++) tmp[i] = i;
            *best = n + 1;
            Search(n, adj, dom, 0, 0, best, tmp);
            return *best;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyDomination(int n, bool* adj, int* dom, int vi, int delta)
        {
            dom[vi] += delta;
            for (int u = 0; u < n; u++)
                if (adj[vi * n + u]) dom[u] += delta;
        }

        private static void Search(int n, bool* adj, int* dom, int idx, int used, int* best, int* order)
        {
            if (idx == n)
            {
                for (int vi = 0; vi < n; vi++)
                    if (dom[vi] == 0) return;
                if (used < *best) *best = used;
                return;
            }
            int vi2 = order[idx];
            if (dom[vi2] > 0)
            {
                Search(n, adj, dom, idx + 1, used, best, order);
                return;
            }
            ApplyDomination(n, adj, dom, vi2, 1);
            Search(n, adj, dom, idx + 1, used + 1, best, order);
            ApplyDomination(n, adj, dom, vi2, -1);
        }
    }
}