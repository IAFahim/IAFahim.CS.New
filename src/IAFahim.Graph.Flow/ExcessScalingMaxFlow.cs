namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ExcessScalingMaxFlow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int delta, int* head, int* to, int* next, int* cap, int* flow, int* excess)
        {
            // Outline of excess scaling loop
            for (int u = 0; u < n; u++)
            {
                if (u != s && u != t && excess[u] >= delta)
                {
                    for (int e = head[u]; e != -1 && excess[u] > 0; e = next[e])
                    {
                        if (cap[e] - flow[e] > 0)
                        {
                            int push = Math.Min(excess[u], cap[e] - flow[e]);
                            flow[e] += push;
                            flow[e ^ 1] -= push;
                            excess[u] -= push;
                            excess[to[e]] += push;
                        }
                    }
                }
            }
        }
    }
}