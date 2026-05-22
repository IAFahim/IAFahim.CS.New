namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FlowWithVertexCapacities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int* vcap, int* head, int* to, int* next, int* cap, int* out_head, int* out_to, int* out_next, int* out_cap, ref int out_edges)
        {
            int eId = 0;
            for (int i = 0; i < n; i++)
            {
                int u_in = i * 2;
                int u_out = i * 2 + 1;
                
                // Add internal edge
                out_to[eId] = u_out;
                out_cap[eId] = vcap[i];
                out_next[eId] = out_head[u_in];
                out_head[u_in] = eId++;
                
                out_to[eId] = u_in;
                out_cap[eId] = 0;
                out_next[eId] = out_head[u_out];
                out_head[u_out] = eId++;
                
                for (int e = head[i]; e != -1; e = next[e])
                {
                    int v = to[e];
                    int v_in = v * 2;
                    
                    out_to[eId] = v_in;
                    out_cap[eId] = cap[e];
                    out_next[eId] = out_head[u_out];
                    out_head[u_out] = eId++;
                    
                    out_to[eId] = u_out;
                    out_cap[eId] = 0;
                    out_next[eId] = out_head[v_in];
                    out_head[v_in] = eId++;
                }
            }
            out_edges = eId;
        }
    }
}