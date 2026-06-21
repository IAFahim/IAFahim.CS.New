namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumEquivalentDigraph
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(bool* adjMatrix, int n)
        {
            // Minimum equivalent digraph of a DAG = transitive reduction.
            // Compute reachability on the original graph, then drop every edge i->j
            // for which an alternate length>=2 path i->k->...->j exists.
            bool* reach = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) reach[i] = adjMatrix[i];
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    if (reach[i * n + k])
                        for (int j = 0; j < n; j++)
                            if (reach[k * n + j]) reach[i * n + j] = true;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j || !adjMatrix[i * n + j]) continue;
                    for (int k = 0; k < n; k++)
                    {
                        if (k == i || k == j) continue;
                        if (adjMatrix[i * n + k] && reach[k * n + j])
                        {
                            adjMatrix[i * n + j] = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}