namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaximumClosureProjectSelection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, int* profit, int s, int t, int* out_head, int* out_to, int* out_next, int* out_cap, ref int eId)
        {
            long baseProfit = 0;
            for (int i = 0; i < n; i++)
            {
                if (profit[i] > 0)
                {
                    baseProfit += profit[i];
                    
                    out_to[eId] = i;
                    out_cap[eId] = profit[i];
                    out_next[eId] = out_head[s];
                    out_head[s] = eId++;
                    
                    out_to[eId] = s;
                    out_cap[eId] = 0;
                    out_next[eId] = out_head[i];
                    out_head[i] = eId++;
                }
                else if (profit[i] < 0)
                {
                    out_to[eId] = t;
                    out_cap[eId] = -profit[i];
                    out_next[eId] = out_head[i];
                    out_head[i] = eId++;
                    
                    out_to[eId] = i;
                    out_cap[eId] = 0;
                    out_next[eId] = out_head[t];
                    out_head[t] = eId++;
                }
            }
            return baseProfit;
        }
    }
}