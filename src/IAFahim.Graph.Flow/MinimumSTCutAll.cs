namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumSTCutAll
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BfsReachable(int n, int s, int* head, int* to, int* next, int* cap, int* flow, byte* visited)
        {
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CollectCutEdges(int n, int* head, int* next, int* to, int* cap, int* flow, byte* visited, int* cutU, int* cutV)
        {
            int cnt = 0;
            for (int u = 0; u < n; u++)
                if (visited[u] == 1)
                    for (int e = head[u]; e != 0; e = next[e])
                        if (visited[to[e]] == 0 && cap[e] > 0 && cap[e] - flow[e] == 0) { cutU[cnt] = u; cutV[cnt] = to[e]; cnt++; }
            return cnt;
        }

        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* cutU, int* cutV, int* cutCount)
        {
            long result = DinicMaxFlow.Run(n, s, t, head, to, next, cap, flow);
            byte* visited = stackalloc byte[n];
            BfsReachable(n, s, head, to, next, cap, flow, visited);
            *cutCount = CollectCutEdges(n, head, next, to, cap, flow, visited, cutU, cutV);
            return result;
        }
    }
}