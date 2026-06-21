namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AttractorSet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SeedStart(int n, int start, bool* adj, bool* even, bool* inAttr, int* queue, ref int tail)
        {
            if (even[start])
            {
                inAttr[start] = true;
                queue[tail++] = start;
            }
            else
            {
                for (int v = 0; v < n; v++)
                    if (adj[start * n + v]) { inAttr[v] = true; queue[tail++] = v; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AllSuccessorsAttracted(int u, int n, bool* adj, bool* inAttr)
        {
            for (int w = 0; w < n; w++)
                if (adj[u * n + w] && !inAttr[w]) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AttractAllSuccessors(int u, int n, bool* adj, bool* inAttr, int* queue, ref int tail)
        {
            for (int w = 0; w < n; w++)
                if (adj[u * n + w] && !inAttr[w]) { inAttr[w] = true; queue[tail++] = w; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Solve(int n, bool* player, bool* adj, bool* even, int start, bool* inAttr, int* queue)
        {
            for (int i = 0; i < n; i++) inAttr[i] = false;
            int head = 0, tail = 0;
            SeedStart(n, start, adj, even, inAttr, queue, ref tail);
            while (head < tail)
            {
                int v = queue[head++];
                for (int u = 0; u < n; u++)
                {
                    if (!adj[v * n + u] || inAttr[u]) continue;
                    if (even[u])
                    {
                        if (AllSuccessorsAttracted(u, n, adj, inAttr)) { inAttr[u] = true; queue[tail++] = u; }
                    }
                    else
                    {
                        AttractAllSuccessors(u, n, adj, inAttr, queue, ref tail);
                    }
                }
            }
            return inAttr[start];
        }
    }
}
