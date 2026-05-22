namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumWeightClosure
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int* cost, int s, int t, int* out_head, int* out_to, int* out_next, int* out_cap, ref int eId)
        {
            for (int i = 0; i < n; i++)
            {
                if (cost[i] < 0)
                {
                    out_to[eId] = i;
                    out_cap[eId] = -cost[i];
                    out_next[eId] = out_head[s];
                    out_head[s] = eId++;
                    
                    out_to[eId] = s;
                    out_cap[eId] = 0;
                    out_next[eId] = out_head[i];
                    out_head[i] = eId++;
                }
                else if (cost[i] > 0)
                {
                    out_to[eId] = t;
                    out_cap[eId] = cost[i];
                    out_next[eId] = out_head[i];
                    out_head[i] = eId++;
                    
                    out_to[eId] = i;
                    out_cap[eId] = 0;
                    out_next[eId] = out_head[t];
                    out_head[t] = eId++;
                }
            }
        }
    }
}