namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumWeightClosure
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* nodeWeight, byte* inClosure)
        {
            // Edge ids must start at 2 so the reverse-edge XOR pairing (e ^ 1) in DinicDfs
            // maps (2,3),(4,5),... and never aliases the head[] sentinel slot 0.
            int nn = n + 2, edgeId = 2;
            int maxEdges = n * 4 + 2; // +2 for the two unused slots (0,1) skipped by edgeId starting at 2
            int* newHead = stackalloc int[nn];
            int* newTo = stackalloc int[maxEdges];
            int* newNext = stackalloc int[maxEdges];
            int* newCap = stackalloc int[maxEdges];
            int* newFlow = stackalloc int[maxEdges];
            for (int i = 0; i < nn; i++) newHead[i] = 0;
            for (int u = 0; u < n; u++)
            {
                if (nodeWeight[u] >= 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, s, u, 0, nodeWeight[u]);
                else MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u, t, 0, -nodeWeight[u]);
            }
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u, to[e], 0, int.MaxValue);
            DinicMaxFlow.Run(nn, s, t, newHead, newTo, newNext, newCap, newFlow);
            byte* reachable = stackalloc byte[nn];
            for (int i = 0; i < nn; i++) reachable[i] = 0;
            int* q = stackalloc int[nn];
            int qh = 0, qt = 0;
            reachable[s] = 1; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = newHead[u]; e != 0; e = newNext[e])
                {
                    int v = newTo[e];
                    if (newCap[e] - newFlow[e] > 0 && reachable[v] == 0)
                    {
                        reachable[v] = 1; q[qt++] = v;
                    }
                }
            }
            for (int u = 0; u < n; u++) inClosure[u] = reachable[u];
            return 0;
        }
    }
}