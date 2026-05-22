namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PushRelabelGlobalRelabel
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int t, int* head, int* to, int* next, int* cap, int* flow, int* height, int* gap)
        {
            for (int i = 0; i < n; i++) height[i] = n;
            for (int i = 0; i <= n; i++) gap[i] = 0;
            height[t] = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            q[qt++] = t;
            while (qh < qt)
            {
                int u = q[qh++];
                gap[height[u]]++;
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (height[v] == n && cap[e ^ 1] - flow[e ^ 1] > 0)
                    {
                        height[v] = height[u] + 1;
                        q[qt++] = v;
                    }
                }
            }
        }
    }
}