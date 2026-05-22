namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DinicCurrentArc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int u, int t, int pushed, int* head, int* to, int* next, int* cap, int* flow, int* level, int* it)
        {
            if (pushed == 0 || u == t) return pushed;
            for (int e = it[u]; e != -1; e = next[e])
            {
                it[u] = e;
                int v = to[e];
                if (level[v] == level[u] + 1 && cap[e] - flow[e] > 0)
                {
                    int tr = Run(v, t, Math.Min(pushed, cap[e] - flow[e]), head, to, next, cap, flow, level, it);
                    if (tr > 0)
                    {
                        flow[e] += tr;
                        flow[e ^ 1] -= tr;
                        return tr;
                    }
                }
            }
            return 0;
        }
    }
}