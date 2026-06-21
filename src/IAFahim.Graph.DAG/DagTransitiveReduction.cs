namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    internal static unsafe class DagShared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ComputeReachability(bool* adjMatrix, int n, bool* reach)
        {
            long total = (long)n * n;
            for (long i = 0; i < total; i++) reach[i] = adjMatrix[i];
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (reach[i * n + k] && reach[k * n + j]) reach[i * n + j] = true;
        }
    }

    public static unsafe class DagTransitiveReduction
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasRedundantEdge(bool* reach, int n, int i, int j)
        {
            for (int m = 0; m < n; m++)
                if (m != i && m != j && reach[i * n + m] && reach[m * n + j]) return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(bool* adjMatrix, int n, bool* reach)
        {
            DagShared.ComputeReachability(adjMatrix, n, reach);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (adjMatrix[i * n + j] && HasRedundantEdge(reach, n, i, j)) adjMatrix[i * n + j] = false;
        }
    }
}
