namespace IAFahim.Graph.Connectivity
{
    using System.Runtime.CompilerServices;

    public static unsafe class DecrementalConnectivity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* queriesU, int* queriesV, int* queriesType, int q,
                               int* parent, int* size, int n, int* answers)
        {
            for (int i = q - 1; i >= 0; i--)
            {
                if (queriesType[i] == 0)
                {
                    IncrementalConnectivity.Union(parent, size, queriesU[i], queriesV[i]);
                }
                else if (queriesType[i] == 1)
                {
                    answers[i] = IncrementalConnectivity.Connected(parent, queriesU[i], queriesV[i]) ? 1 : 0;
                }
            }
        }
    }
}
