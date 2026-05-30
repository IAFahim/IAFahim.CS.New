namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PicardQueyranneClosure
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* nodeWeight)
        {
            long minCut = long.MaxValue;
            for (int i = 0; i < n; i++)
            {
                int nn = n + 2, edgeId = 1;
                int* newHead = stackalloc int[nn];
                int* newTo = stackalloc int[n * 6];
                int* newNext = stackalloc int[n * 6];
                int* newCap = stackalloc int[n * 6];
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
                long cutVal = DinicMaxFlow.Run(nn, s, i, newHead, newTo, newNext, newCap, flow);
                if (cutVal < minCut) minCut = cutVal;
            }
            return minCut == long.MaxValue ? 0 : minCut;
        }
    }
}
