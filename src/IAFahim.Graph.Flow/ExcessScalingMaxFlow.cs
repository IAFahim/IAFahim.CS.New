namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ExcessScalingMaxFlow
    {
        private const int Unvisited = -1;

        private const int SourceSupply = int.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindMaxCapacity(int* head, int* to, int* next, int* cap, int n)
        {
            int maxCap = 0;
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    if (cap[e] > maxCap) maxCap = cap[e];
            return maxCap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ComputeInitialScale(int maxCap)
        {
            int scalingFactor = 1;
            while (scalingFactor <= maxCap) scalingFactor <<= 1;
            return scalingFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildLevelGraph(int n, int s, int* head, int* to, int* next, int* cap, int* flow, int* level, int* q, int sf, int* excess)
        {
            for (int u = 0; u < n; u++) level[u] = Unvisited;
            int qh = 0, qt = 0;
            level[s] = 0;
            q[qt++] = s;
            excess[s] = SourceSupply;
            while (qh < qt)
            {
                int u = q[qh++];
                int lu1 = level[u] + 1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    int rcap = cap[e] - flow[e];
                    if (rcap >= sf && level[v] == Unvisited)
                    {
                        level[v] = lu1;
                        q[qt++] = v;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushExcess(int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* level, int* q, int* excess, int sf)
        {
            int qh = 0, qt = 0;
            q[qt++] = s;
            while (qh < qt && excess[s] > 0)
            {
                int u = q[qh++];
                int lu1 = level[u] + 1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    int rcap = cap[e] - flow[e];
                    if (rcap >= sf && level[v] == lu1)
                    {
                        int pushed = Math.Min(excess[u], rcap);
                        excess[v] += pushed;
                        excess[u] -= pushed;
                        flow[e] += pushed;
                        flow[e ^ 1] -= pushed;
                        if (excess[v] >= sf && v != t) q[qt++] = v;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long SumSourceOutFlow(int* head, int* next, int* flow, int s)
        {
            long total = 0;
            for (int e = head[s]; e != 0; e = next[e])
                total += flow[e];
            return total;
        }

        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            int* excess = stackalloc int[n];
            int* level = stackalloc int[n];
            int* q = stackalloc int[n];
            for (int i = 0; i < n; i++) { flow[i] = 0; excess[i] = 0; level[i] = 0; }
            int maxCap = FindMaxCapacity(head, to, next, cap, n);
            int scalingFactor = ComputeInitialScale(maxCap);
            while (scalingFactor > 0)
            {
                int sf = scalingFactor >> 1;
                BuildLevelGraph(n, s, head, to, next, cap, flow, level, q, sf, excess);
                PushExcess(s, t, head, to, next, cap, flow, level, q, excess, sf);
                if (excess[s] == 0) scalingFactor = sf;
                else scalingFactor >>= 1;
            }
            return SumSourceOutFlow(head, next, flow, s);
        }
    }
}
