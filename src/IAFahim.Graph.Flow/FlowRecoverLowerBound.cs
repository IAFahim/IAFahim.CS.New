namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FlowRecoverLowerBound
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int m, int* edemand, int* flow, int* real_flow)
        {
            for (int i = 0; i < m; i++)
            {
                real_flow[i] = edemand[i] + flow[i * 2]; // Assuming even IDs are forward edges
            }
        }
    }
}