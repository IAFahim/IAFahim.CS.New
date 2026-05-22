namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagCountingPaths
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* head, int* next, int* to, int* topoOrder, long* pathCount, int n)
        {
            for (int i = 0; i < n; i++) pathCount[i] = 1;

            for (int i = n - 1; i >= 0; i--)
            {
                int u = topoOrder[i];
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    pathCount[u] += pathCount[v];
                }
            }
        }
    }
}