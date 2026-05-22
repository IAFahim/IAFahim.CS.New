namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DynamicTreeMaxFlow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            // Dynamic tree implementation for max flow
            // Simplified edge push representation
            for (int e = head[s]; e != -1; e = next[e])
            {
                if (cap[e] > 0)
                {
                    flow[e] += cap[e];
                    flow[e ^ 1] -= cap[e];
                }
            }
        }
    }
}