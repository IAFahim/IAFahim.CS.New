namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagTransitiveReduction
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(bool* adjMatrix, int n, bool* reach)
        {
            long total = (long)n * n;
            for (long i = 0; i < total; i++)
                reach[i] = adjMatrix[i];

            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (reach[i * n + k] && reach[k * n + j])
                            reach[i * n + j] = true;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (adjMatrix[i * n + j])
                        for (int m = 0; m < n; m++)
                            if (m != i && m != j && reach[i * n + m] && reach[m * n + j])
                            {
                                adjMatrix[i * n + j] = false;
                                break;
                            }
        }
    }
}
