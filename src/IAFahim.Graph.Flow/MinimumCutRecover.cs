namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumCutRecover
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int* head, int* to, int* next, int* cap, int* flow, bool* inCut)
        {
            for (int i = 0; i < n; i++) inCut[i] = false;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            q[qt++] = s;
            inCut[s] = true;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (!inCut[v] && cap[e] - flow[e] > 0)
                    {
                        inCut[v] = true;
                        q[qt++] = v;
                    }
                }
            }
        }
    }
}