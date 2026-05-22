namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumSTCutAll
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, bool* canReachFromS, bool* canReachToT)
        {
            MinimumCutRecover.Run(n, s, head, to, next, cap, flow, canReachFromS);
            
            for (int i = 0; i < n; i++) canReachToT[i] = false;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            q[qt++] = t;
            canReachToT[t] = true;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != -1; e = next[e])
                {
                    // reverse edge check
                    int rev = e ^ 1;
                    int v = to[e];
                    if (!canReachToT[v] && cap[rev] - flow[rev] > 0)
                    {
                        canReachToT[v] = true;
                        q[qt++] = v;
                    }
                }
            }
        }
    }
}