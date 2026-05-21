namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Retrograde
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Solve(int n, bool* win, bool* lose, int* from, int* to, int m)
        {
            int* indeg = stackalloc int[n];
            for (int i = 0; i < n; i++) indeg[i] = 0;
            for (int e = 0; e < m; e++) indeg[to[e]]++;
            int* q = stackalloc int[n];
            int head = 0, tail = 0;
            for (int i = 0; i < n; i++)
            {
                if (lose[i]) q[tail++] = i;
            }
            while (head < tail)
            {
                int v = q[head++];
                for (int e = 0; e < m; e++)
                {
                    if (to[e] != v) continue;
                    int u = from[e];
                    if (win[u]) continue;
                    indeg[u]--;
                    if (indeg[u] == 0)
                    {
                        win[u] = true;
                        q[tail++] = u;
                    }
                }
            }
            for (int i = 0; i < n; i++)
                if (!win[i] && !lose[i]) lose[i] = true;
            return tail;
        }
    }
}
