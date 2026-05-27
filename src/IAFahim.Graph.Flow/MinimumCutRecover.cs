namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumCutRecover
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* head, int* to, int* next, int* cap, int n, int s, byte* visited)
        {
            for (int i = 0; i < n; i++) visited[i] = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            visited[s] = 1;
            q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (cap[e] > 0 && visited[v] == 0)
                    {
                        visited[v] = 1;
                        q[qt++] = v;
                    }
                }
            }
        }
    }

    public static unsafe class MinCut
    {
        public static void Run(int n, int s, int* head, int* to, int* next, int* cap, byte* visited)
        {
            MinimumCutRecover.Run(head, to, next, cap, n, s, visited);
        }
    }
}
