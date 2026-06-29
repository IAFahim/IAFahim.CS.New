namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class FlowWithVertexCapacities
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* vCap)
        {
            int nn = n * 2;
            int* newHead = (int*)Marshal.AllocHGlobal((nint)((long)nn * sizeof(int)));
            int* newTo = (int*)Marshal.AllocHGlobal((nint)((long)n * 8 * sizeof(int)));
            int* newNext = (int*)Marshal.AllocHGlobal((nint)((long)n * 8 * sizeof(int)));
            int* newCap = (int*)Marshal.AllocHGlobal((nint)((long)n * 8 * sizeof(int)));
            int* newFlow = (int*)Marshal.AllocHGlobal((nint)((long)n * 8 * sizeof(int)));
            for (int i = 0; i < nn; i++) newHead[i] = 0;
            int edgeId = 2;

            for (int i = 0; i < n; i++)
            {
                int inV = i << 1, outV = (i << 1) | 1;
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
            Marshal.FreeHGlobal((nint)newFlow);
            Marshal.FreeHGlobal((nint)newCap);
            Marshal.FreeHGlobal((nint)newNext);
            Marshal.FreeHGlobal((nint)newTo);
            Marshal.FreeHGlobal((nint)newHead);
            return result;
        }
    }
}
