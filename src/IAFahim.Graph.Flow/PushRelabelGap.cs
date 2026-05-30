namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PushRelabelGap
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            int* height = stackalloc int[n];
            int* excess = stackalloc int[n];
            int* count = stackalloc int[n];
            for (int i = 0; i < n; i++) { height[i] = 0; excess[i] = 0; flow[i] = 0; count[i] = 0; }
            height[s] = n; excess[s] = int.MaxValue;
            for (int e = head[s]; e != 0; e = next[e])
            {
                flow[e] = cap[e]; flow[e ^ 1] = -cap[e];
                excess[to[e]] += cap[e]; excess[s] -= cap[e];
            }
            int* ptr = stackalloc int[n];
            for (int i = 0; i < n; i++) ptr[i] = head[i];
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            q[qt++] = t;
            while (qh < qt)
            {
                int v = q[qh++];
                for (int i = 0; i < n; i++) count[i] = 0;
                qh = 0; qt = 0; q[qt++] = t;
                while (qh < qt)
                {
                    int u = q[qh++];
                    count[height[u]]++;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v2 = to[e];
                        if (flow[e ^ 1] > 0 && height[u] > height[v2] + 1) count[height[u]]--;
                        else if (flow[e] > 0 && height[u] == height[v2] + 1)
                        {
                            Push(n, u, v2, head, to, next, cap, flow, excess);
                            if (excess[u] == 0) break;
                        }
                    }
                    if (excess[u] > 0)
                    {
                        height[u]++;
                        if (count[height[u]] == 0) return -1;
                        count[height[u]]++;
                    }
                    if (excess[u] > 0 && u != s && u != t) q[qt++] = u;
                }
            }
            long result = 0;
            for (int e = head[s]; e != 0; e = next[e]) result += flow[e];
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Push(int n, int u, int v, int* head, int* to, int* next, int* cap, int* flow, int* excess)
        {
            int push = Math.Min(excess[u], cap[head[u]] - flow[head[u]]);
            excess[u] -= push; excess[v] += push;
            flow[head[u]] += push; flow[head[u] ^ 1] -= push;
        }
    }
}
