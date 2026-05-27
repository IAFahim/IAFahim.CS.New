namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagHashCanonical
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* head, int* next, int* to, int* topoOrder, ulong* hashes, int n)
        {
            // Compute hashes in reverse topological order
            for (int i = n - 1; i >= 0; i--)
            {
                int u = topoOrder[i];
                ulong h = 14695981039346656037UL; // FNV offset basis
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    h ^= hashes[v];
                    h *= 1099511628211UL; // FNV prime
                }
                hashes[u] = h;
            }
        }
    }
}