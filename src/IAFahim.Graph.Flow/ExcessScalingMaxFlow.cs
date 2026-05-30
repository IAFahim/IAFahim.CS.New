namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ExcessScalingMaxFlow
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            long totalFlow = 0;
            int* excess = stackalloc int[n];
            int* level = stackalloc int[n];
            int* q = stackalloc int[n];
            for (int i = 0; i < n; i++) { flow[i] = 0; excess[i] = 0; level[i] = 0; }

            int maxCap = 0;
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    if (cap[e] > maxCap) maxCap = cap[e];

            int scalingFactor = 1;
            while (scalingFactor <= maxCap) scalingFactor <<= 1;

            while (scalingFactor > 0)
            {
                int sf = scalingFactor >> 1;
                for (int u = 0; u < n; u++) level[u] = -1;
                int qh = 0, qt = 0;
                level[s] = 0; q[qt++] = s; excess[s] = int.MaxValue;

                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        int rcap = cap[e] - flow[e];
                        if (rcap >= sf && level[v] == -1)
                        {
                            level[v] = level[u] + 1;
                            q[qt++] = v;
                        }
                    }
                }

                qh = 0; qt = 0;
                q[qt++] = s;
                while (qh < qt && excess[s] > 0)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        int rcap = cap[e] - flow[e];
                        if (rcap >= sf && level[v] == level[u] + 1)
                        {
                            int pushed = Math.Min(excess[u], rcap);
                            excess[v] += pushed; excess[u] -= pushed;
                            flow[e] += pushed; flow[e ^ 1] -= pushed;
                            if (excess[v] >= sf && v != t) q[qt++] = v;
                        }
                    }
                }
                if (excess[s] == 0) scalingFactor = sf;
                else scalingFactor >>= 1;
            }
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    totalFlow += flow[e];
            return totalFlow / 2;
        }
    }
}
