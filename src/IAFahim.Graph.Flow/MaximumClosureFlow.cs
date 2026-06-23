namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaximumClosureFlow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BuildClosureGraph(int n, int s, int t, int* head, int* to, int* next, int* nodeWeight, int* newHead, int* newTo, int* newNext, int* newCap, int nn)
        {
            for (int i = 0; i < nn; i++) newHead[i] = 0;
            int edgeId = 2;
            for (int u = 0; u < n; u++)
            {
                if (nodeWeight[u] > 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, s, u, 0, nodeWeight[u]);
                else if (nodeWeight[u] < 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u, t, 0, -nodeWeight[u]);
            }
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u, to[e], 0, int.MaxValue);
            return edgeId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long BfsReachable(int n, int s, int* newHead, int* newTo, int* newNext, int* newCap, int* newFlow, int* nodeWeight, int nn)
        {
            long maxWeight = 0;
            byte* visited = stackalloc byte[nn];
            for (int i = 0; i < nn; i++) visited[i] = 0;
            int* q = stackalloc int[nn];
            int qh = 0, qt = 0;
            visited[s] = 1; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                if (u < n && nodeWeight[u] > 0) maxWeight += nodeWeight[u];
                for (int e = newHead[u]; e != 0; e = newNext[e])
                {
                    if (newCap[e] - newFlow[e] > 0 && visited[newTo[e]] == 0)
                    {
                        visited[newTo[e]] = 1; q[qt++] = newTo[e];
                    }
                }
            }
            return maxWeight;
        }

        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* nodeWeight)
        {
            // Edge ids must start at 2 so the reverse-edge XOR pairing (e ^ 1) maps
            // (2,3),(4,5),... and never aliases the head[] sentinel slot 0.
            int nn = n + 2;
            int maxEdges = n * 4 + 2; // +2 for the two unused slots (0,1) skipped by edgeId starting at 2
            int* newHead = stackalloc int[nn];
            int* newTo = stackalloc int[maxEdges];
            int* newNext = stackalloc int[maxEdges];
            int* newCap = stackalloc int[maxEdges];
            int* newFlow = stackalloc int[maxEdges];
            BuildClosureGraph(n, s, t, head, to, next, nodeWeight, newHead, newTo, newNext, newCap, nn);
            DinicMaxFlow.Run(nn, s, t, newHead, newTo, newNext, newCap, newFlow);
            return BfsReachable(n, s, newHead, newTo, newNext, newCap, newFlow, nodeWeight, nn);
        }
    }
}