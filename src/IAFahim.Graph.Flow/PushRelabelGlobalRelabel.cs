namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PushRelabelGlobalRelabel
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            int* height = stackalloc int[n];
            int* excess = stackalloc int[n];
            for (int i = 0; i < n; i++) { height[i] = 0; excess[i] = 0; flow[i] = 0; }
            height[s] = n;
            excess[s] = int.MaxValue;
            for (int e = head[s]; e != 0; e = next[e])
            {
                flow[e] = cap[e]; flow[e ^ 1] = -cap[e];
                excess[to[e]] += cap[e]; excess[s] -= cap[e];
            }
            int* ptr = stackalloc int[n];
            for (int i = 0; i < n; i++) ptr[i] = head[i];
            int* q = stackalloc int[n];
            int* dist = stackalloc int[n];
            while (true)
            {
                for (int i = 0; i < n; i++) dist[i] = -1;
                int qh = 0, qt = 0;
                dist[t] = 0; q[qt++] = t;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int rev = e ^ 1;
                        if (flow[rev] > 0 && dist[to[e]] == -1)
                        {
                            dist[to[e]] = dist[u] + 1; q[qt++] = to[e];
                        }
                    }
                }
                for (int i = 0; i < n; i++) height[i] = dist[i] >= 0 ? dist[i] + 1 : 0;
                qh = 0; qt = 0;
                for (int i = 0; i < n; i++) if (excess[i] > 0 && i != s && i != t) q[qt++] = i;
                while (qh < qt)
                {
                    int u = q[qh++];
                    while (ptr[u] != 0)
                    {
                        int e = ptr[u];
                        int v = to[e];
                        int rc = cap[e] - flow[e];
                        if (rc > 0 && height[u] == height[v] + 1)
                        {
                            int push = Math.Min(excess[u], rc);
                            flow[e] += push; flow[e ^ 1] -= push;
                            excess[u] -= push; excess[v] += push;
                            if (excess[v] > 0 && v != s && v != t) q[qt++] = v;
                            if (excess[u] == 0) break;
                        }
                        ptr[u] = next[e];
                    }
                    if (ptr[u] == 0) ptr[u] = head[u];
                }
                bool any = false;
                for (int i = 0; i < n; i++) if (excess[i] > 0 && i != s) { any = true; break; }
                if (!any) break;
            }
            long result = 0;
            for (int e = head[s]; e != 0; e = next[e]) result += flow[e];
            return result;
        }
    }
}