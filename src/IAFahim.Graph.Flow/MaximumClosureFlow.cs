namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaximumClosureFlow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int* eu, int* ev, int m, int* out_head, int* out_to, int* out_next, int* out_cap, ref int eId)
        {
            for (int i = 0; i < m; i++)
            {
                out_to[eId] = ev[i];
                out_cap[eId] = int.MaxValue;
                out_next[eId] = out_head[eu[i]];
                out_head[eu[i]] = eId++;
                
                out_to[eId] = eu[i];
                out_cap[eId] = 0;
                out_next[eId] = out_head[ev[i]];
                out_head[ev[i]] = eId++;
            }
        }
    }
}