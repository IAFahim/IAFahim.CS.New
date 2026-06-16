namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FlowWithEdgeDemands
    {
        public static bool Run(int n, int s, int t, int* head, int* to, int* next, int* lower, int* upper, int* flow, int* result)
        {
            int nn = n + 2, ss = n, tt = n + 1, edgeId = 2;
            int* newHead = stackalloc int[nn];
            int* newTo = stackalloc int[n * 4 + 10];
            int* newNext = stackalloc int[n * 4 + 10];
            int* newCap = stackalloc int[n * 4 + 10];
            int* newFlow = stackalloc int[n * 4 + 10];
            for (int i = 0; i < nn; i++) newHead[i] = 0;

            long* b = stackalloc long[n];
            for (int i = 0; i < n; i++) b[i] = 0;

            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    int lo = lower[e], hi = upper[e];
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u, v, 0, hi - lo);
                    b[u] -= lo; b[v] += lo;
                }
            for (int i = 0; i < n; i++)
            {
                if (b[i] > 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, ss, i, 0, (int)b[i]);
                else if (b[i] < 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, i, tt, 0, (int)-b[i]);
            }
            MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, t, s, 0, int.MaxValue);
            DinicMaxFlow.Run(nn, ss, tt, newHead, newTo, newNext, newCap, newFlow);
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e]) flow[e] = newFlow[e] + lower[e];
            *result = flow[head[s]];
            return true;
        }
    }
}
