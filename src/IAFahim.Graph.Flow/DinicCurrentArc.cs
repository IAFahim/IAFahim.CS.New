namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DinicCurrentArc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(int n, int* ptr, int* head, int* currentArc, int s)
        {
            for (int v = 0; v < n; v++) currentArc[v] = head[v];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Advance(int u, int* head, int* to, int* next, int* cap, int* flow, int* level, int* currentArc)
        {
            for (int e = currentArc[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (level[v] == level[u] + 1 && cap[e] - flow[e] > 0)
                {
                    currentArc[u] = e;
                    return e;
                }
            }
            currentArc[u] = 0;
            return 0;
        }
    }
}
