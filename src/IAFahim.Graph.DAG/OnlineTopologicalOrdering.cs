namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class OnlineTopologicalOrdering
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddEdge(int u, int v, int* ord, int* head, int* next, int* to, int* edgeCount)
        {
            if (ord[u] < ord[v])
            {
                to[*edgeCount] = v;
                next[*edgeCount] = head[u];
                head[u] = *edgeCount;
                (*edgeCount)++;
                return true;
            }
            // Reordering logic simplified
            return false;
        }
    }
}