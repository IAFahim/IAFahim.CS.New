namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaximumClosureFlow
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* nodeWeight)
        {
            int nn = n + 2, edgeId = 1;
            int* newHead = stackalloc int[nn];
            int* newTo = stackalloc int[n * 4];
            int* newNext = stackalloc int[n * 4];
            int* newCap = stackalloc int[n * 4];
            int* newFlow = stackalloc int[n * 4];
            for (int i = 0; i < nn; i++) newHead[i] = 0;
            for (int u = 0; u < n; u++)
            {
                if (nodeWeight[u] > 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, s, u, 0, nodeWeight[u]);
                else if (nodeWeight[u] < 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u, t, 0, -nodeWeight[u]);
            }
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u, to[e], 0, int.MaxValue);
            DinicMaxFlow.Run(nn, s, t, newHead, newTo, newNext, newCap, newFlow);
            long maxWeight = 0;
            byte* visited = stackalloc byte[nn];
            for (int i = 0; i < nn; i++) visited[i] = 0;
            int* q = stackalloc int[nn];
            int qh = 0, qt = 0;
            visited[s] = 1; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = newHead[u]; e != 0; e = newNext[e])
                {
                    if (newCap[e] - newFlow[e] > 0 && visited[newTo[e]] == 0)
                    {
                        visited[newTo[e]] = 1; q[qt++] = newTo[e];
                    }
                }
            }
            for (int u = 0; u < n; u++) if (visited[u] == 1 && nodeWeight[u] > 0) maxWeight += nodeWeight[u];
            return maxWeight;
        }
    }
}