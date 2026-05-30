namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumSTCutAll
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* cutU, int* cutV, int* cutCount)
        {
            long result = DinicMaxFlow.Run(n, s, t, head, to, next, cap, flow);
            byte* visited = stackalloc byte[n];
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int i = 0; i < n; i++) visited[i] = 0;
            visited[s] = 1; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    if (cap[e] - flow[e] > 0 && visited[to[e]] == 0)
                    {
                        visited[to[e]] = 1; q[qt++] = to[e];
                    }
                }
            }
            int cnt = 0;
            for (int u = 0; u < n; u++)
                if (visited[u] == 0)
                    for (int e = head[u]; e != 0; e = next[e])
                        if (visited[to[e]] == 1 && flow[e] > 0) { cutU[cnt] = u; cutV[cnt] = to[e]; cnt++; }
            *cutCount = cnt;
            return result;
        }
    }
}