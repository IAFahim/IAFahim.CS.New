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
                    for (int k = start; k < end; k++)
                    {
                        PropagateFromLose(radjFrom[k], win, lose, q, ref tail);
                    }
                }
                else
                {
                    for (int k = start; k < end; k++)
                    {
                        PropagateFromWin(radjFrom[k], win, lose, outdeg, q, ref tail);
                    }
                }
            }

            return tail;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PropagateFromLose(int u, bool* win, bool* lose, int* q, ref int tail)
        {
            if (win[u] || lose[u]) return;
            win[u] = true;
            q[tail++] = u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PropagateFromWin(int u, bool* win, bool* lose, int* outdeg, int* q, ref int tail)
        {
            if (win[u] || lose[u]) return;
            if (--outdeg[u] == 0)
            {
                lose[u] = true;
                q[tail++] = u;
            }
        }
    }
}
