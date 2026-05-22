namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumEquivalentDigraph
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(bool* adjMatrix, int n)
        {
            // For a DAG, the minimum equivalent digraph is the transitive reduction
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (adjMatrix[i * n + j])
                    {
                        for (int k = 0; k < n; k++)
                        {
                            if (adjMatrix[j * n + k])
                            {
                                adjMatrix[i * n + k] = false;
                            }
                        }
                    }
                }
            }
        }
    }
}