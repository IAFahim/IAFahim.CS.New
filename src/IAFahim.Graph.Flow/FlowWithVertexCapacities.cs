namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FlowWithVertexCapacities
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* vCap)
        {
            int nn = n * 2;
            int* newHead = stackalloc int[nn];
            int* newTo = stackalloc int[n * 8];
            int* newNext = stackalloc int[n * 8];
            int* newCap = stackalloc int[n * 8];
            int* newFlow = stackalloc int[n * 8];
            for (int i = 0; i < nn; i++) newHead[i] = 0;
            int edgeId = 1;

            for (int i = 0; i < n; i++)
            {
                int inV = i * 2, outV = i * 2 + 1;
                int vc = i == s || i == t ? int.MaxValue : vCap[i];
                MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, inV, outV, 0, vc);
            }

            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u * 2 + 1, v * 2, 0, cap[e]);
                }

            long result = DinicMaxFlow.Run(nn, s * 2 + 1, t * 2, newHead, newTo, newNext, newCap, newFlow);
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e]) flow[e] = newFlow[e];
            return result;
        }
    }
}
