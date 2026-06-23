namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PicardQueyranneClosure
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BuildClosureNetworkForSink(int i, int n, int s, int t, int* head, int* to, int* next, int* nodeWeight, int* newHead, int* newTo, int* newNext, int* newCap, int nn)
        {
            int edgeId = 2;
            for (int j = 0; j < nn; j++) newHead[j] = 0;
            MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, i, t, 0, int.MaxValue);
            for (int j = 0; j < n; j++)
            {
                if (nodeWeight[j] > 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, s, j, 0, nodeWeight[j]);
                else if (nodeWeight[j] < 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, j, t, 0, -nodeWeight[j]);
            }
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, u, to[e], 0, int.MaxValue);
            return edgeId;
        }

        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* nodeWeight)
        {
            int nn = n + 2;
            int maxEdges = n * 6;
            // Buffers are hoisted out of the i-loop: stackalloc inside a loop is not
            // freed per iteration, so allocating here once keeps stack usage O(maxEdges)
            // instead of O(n * maxEdges).
            int* newHead = stackalloc int[nn];
            int* newTo = stackalloc int[maxEdges];
            int* newNext = stackalloc int[maxEdges];
            int* newCap = stackalloc int[maxEdges];
            long minCut = long.MaxValue;
            for (int i = 0; i < n; i++)
            {
                int edgeId = BuildClosureNetworkForSink(i, n, s, t, head, to, next, nodeWeight, newHead, newTo, newNext, newCap, nn);
                // DinicMaxFlow does not zero flow on entry; clear the slots this iteration
                // uses so stale residual flow from a previous (differently-built) network
                // cannot leak in and corrupt the max-flow / cut value.
                for (int e = 0; e < edgeId; e++) flow[e] = 0;
                long cutVal = DinicMaxFlow.Run(nn, s, i, newHead, newTo, newNext, newCap, flow);
                if (cutVal < minCut) minCut = cutVal;
            }
            return minCut == long.MaxValue ? 0 : minCut;
        }
    }
}
