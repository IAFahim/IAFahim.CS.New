namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AttractorSet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Solve(int n, bool* player, bool* adj, bool* even, int start)
        {
            bool* inAttr = stackalloc bool[n];
            for (int i = 0; i < n; i++) inAttr[i] = false;
            int* queue = stackalloc int[n];
            int head = 0, tail = 0;
            if (even[start]) { inAttr[start] = true; queue[tail++] = start; }
            else
            {
                for (int v = 0; v < n; v++)
                    if (adj[start * n + v]) { inAttr[v] = true; queue[tail++] = v; }
            }
            while (head < tail)
            {
                int v = queue[head++];
                for (int u = 0; u < n; u++)
                {
                    if (!adj[v * n + u] || inAttr[u]) continue;
                    if (even[u])
                    {
                        bool allEven = true;
                        for (int w = 0; w < n; w++)
                            if (adj[u * n + w] && !inAttr[w]) { allEven = false; break; }
                        if (allEven) { inAttr[u] = true; queue[tail++] = u; }
                    }
                    else
                    {
                        for (int w = 0; w < n; w++)
                            if (adj[u * n + w] && !inAttr[w]) { inAttr[w] = true; queue[tail++] = w; }
                    }
                }
            }
            return inAttr[start];
        }
    }
}
