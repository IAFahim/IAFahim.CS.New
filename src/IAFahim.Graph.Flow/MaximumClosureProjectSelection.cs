namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class MaximumClosureProjectSelection
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* profit, int* prerequisiteHead, int* prerequisiteTo, int* prerequisiteNext, int* reqEdgeId)
        {
            int nn = n + 2, edgeId = 2;
            int* newHead = (int*)Marshal.AllocHGlobal((nint)((long)nn * sizeof(int)));
            int* newTo = (int*)Marshal.AllocHGlobal((nint)((long)n * 6 * sizeof(int)));
            int* newNext = (int*)Marshal.AllocHGlobal((nint)((long)n * 6 * sizeof(int)));
            int* newCap = (int*)Marshal.AllocHGlobal((nint)((long)n * 6 * sizeof(int)));
            int* newFlow = (int*)Marshal.AllocHGlobal((nint)((long)n * 6 * sizeof(int)));
            for (int i = 0; i < nn; i++) newHead[i] = 0;

            long totalProfit = 0;
            for (int i = 0; i < n; i++)
            {
                if (profit[i] > 0) { MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, s, i, 0, profit[i]); totalProfit += profit[i]; }
                else if (profit[i] < 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, i, t, 0, -profit[i]);
            }
            for (int u = 0; u < n; u++)
                for (int e = prerequisiteHead[u]; e != 0; e = prerequisiteNext[e])
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, null, newCap, &edgeId, prerequisiteTo[e], u, 0, int.MaxValue);

            DinicMaxFlow.Run(nn, s, t, newHead, newTo, newNext, newCap, newFlow);
            Marshal.FreeHGlobal((nint)newFlow);
            Marshal.FreeHGlobal((nint)newCap);
            Marshal.FreeHGlobal((nint)newNext);
            Marshal.FreeHGlobal((nint)newTo);
            Marshal.FreeHGlobal((nint)newHead);
            return totalProfit;
        }
    }
}
