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
                int childCount = 0;
                for (int e = head[u]; e != 0; e = next[e]) childCount++;
                
                ulong* childHashes = stackalloc ulong[childCount];
                int idx = 0;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    childHashes[idx++] = hashes[v];
                }
                
                for (int c = 1; c < childCount; c++)
                {
                    ulong key = childHashes[c];
                    int d = c - 1;
                    while (d >= 0 && childHashes[d] > key)
                    {
                        childHashes[d + 1] = childHashes[d];
                        d--;
                    }
                    childHashes[d + 1] = key;
                }
                
                ulong h = 14695981039346656037UL;
                for (int c = 0; c < childCount; c++)
                {
                    h ^= childHashes[c];
                    h *= 1099511628211UL;
                }
                hashes[u] = h;
            }
        }
    }
}