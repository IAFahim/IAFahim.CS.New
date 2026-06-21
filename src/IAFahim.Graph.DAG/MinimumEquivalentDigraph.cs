namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumEquivalentDigraph
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasAlternatePath(bool* adjMatrix, bool* reach, int n, int i, int j)
        {
            for (int k = 0; k < n; k++)
            {
                if (k == i || k == j) continue;
                if (adjMatrix[i * n + k] && reach[k * n + j]) return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(bool* adjMatrix, int n)
        {
            bool* reach = stackalloc bool[n * n];
            DagShared.ComputeReachability(adjMatrix, n, reach);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j || !adjMatrix[i * n + j]) continue;
                    if (HasAlternatePath(adjMatrix, reach, n, i, j)) adjMatrix[i * n + j] = false;
                }
            }
        }
    }
}
