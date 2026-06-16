namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FlowRecoverLowerBound
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* lower, int* upper, int* flow, int* newHead, int* newTo, int* newNext, int* newCap, int* newFlow)
        {
            long* b = stackalloc long[n];
            for (int i = 0; i < n; i++) b[i] = 0;
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int lo = lower[e], v = to[e];
                    b[u] -= lo;
                    b[v] += lo;
                }
            int ss = n, tt = n + 1, nn = n + 2, edgeId = 2;
            for (int i = 0; i < nn; i++) newHead[i] = 0;

            MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, t, s, 0, int.MaxValue);
            for (int i = 0; i < n; i++)
            {
                if (b[i] > 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, ss, i, 0, (int)b[i]);
                else if (b[i] < 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, i, tt, 0, (int)-b[i]);
            }
            long result = DinicMaxFlow.Run(nn, ss, tt, newHead, newTo, newNext, newCap, newFlow);
            long sumPos = 0;
            for (int i = 0; i < n; i++) if (b[i] > 0) sumPos += b[i];
            if (result < sumPos) return -1;
            long flowVal = 0;
            for (int e = head[s]; e != 0; e = next[e]) flowVal += flow[e] = newFlow[e];
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e]) flow[e] += lower[e];
            return flowVal;
        }
    }
}
