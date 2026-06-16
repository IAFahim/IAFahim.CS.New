namespace IAFahim.Optimization.Games
{
    using System.Runtime.CompilerServices;

    public static unsafe class Retrograde
    {
        public static int Solve(int n, bool* win, bool* lose, int* from, int* to, int m)
        {
            // outdeg[u] = number of outgoing moves of u (edges with from[e] == u).
            // Used as the remaining-unresolved-children counter for the LOSE rule.
            int* outdeg = stackalloc int[n];
            // radjStart is built as a histogram of in-edges per node, then prefix-summed
            // into CSR start offsets (radjStart[v]..radjStart[v+1]) over radjFrom.
            int* radjStart = stackalloc int[n + 1];
            for (int i = 0; i < n; i++)
            {
                outdeg[i] = 0;
                radjStart[i] = 0;
            }
            radjStart[n] = 0;
            for (int e = 0; e < m; e++)
            {
                outdeg[from[e]]++;
                radjStart[to[e] + 1]++;
            }
            // Prefix sum to turn per-node in-edge counts into CSR start offsets.
            for (int i = 0; i < n; i++) radjStart[i + 1] += radjStart[i];
            // radjFrom holds, grouped by target node v, the source endpoints u of edges u -> v.
            int* radjFrom = stackalloc int[m];
            int* fill = stackalloc int[n];
            for (int i = 0; i < n; i++) fill[i] = radjStart[i];
            for (int e = 0; e < m; e++)
            {
                int v = to[e];
                radjFrom[fill[v]++] = from[e];
            }

            int* q = stackalloc int[n];
            int head = 0, tail = 0;
            // Seed the queue with all already-resolved (terminal) nodes.
            for (int i = 0; i < n; i++)
            {
                if (win[i] || lose[i]) q[tail++] = i;
            }

            while (head < tail)
            {
                int v = q[head++];
                int start = radjStart[v];
                int end = radjStart[v + 1];
                if (lose[v])
                {
                    // v is a losing position: any predecessor can move into it and win.
                    for (int k = start; k < end; k++)
                    {
                        int u = radjFrom[k];
                        if (win[u] || lose[u]) continue;
                        win[u] = true;
                        q[tail++] = u;
                    }
                }
                else
                {
                    // v is a winning position: it removes one option from each predecessor.
                    // A predecessor loses only once all of its moves lead to winning nodes.
                    for (int k = start; k < end; k++)
                    {
                        int u = radjFrom[k];
                        if (win[u] || lose[u]) continue;
                        if (--outdeg[u] == 0)
                        {
                            lose[u] = true;
                            q[tail++] = u;
                        }
                    }
                }
            }

            return tail;
        }
    }
}
