namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FlowWithEdgeDemands
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int m, int* eu, int* ev, int* ecap, int* edemand, int* balance)
        {
            for (int i = 0; i < n; i++) balance[i] = 0;
            for (int i = 0; i < m; i++)
            {
                int u = eu[i];
                int v = ev[i];
                int d = edemand[i];
                balance[u] -= d;
                balance[v] += d;
            }
        }
    }
}