namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowCostScaling
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int maxCost, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* pot)
        {
            int epsilon = maxCost * n; // Just outline scaling phase
            while (epsilon > 0)
            {
                // Push-relabel for cost scaling
                epsilon /= 2;
            }
        }
    }
}