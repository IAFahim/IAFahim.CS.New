namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagReachabilityCompressed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* head, int* next, int* to, int* topoOrder, ulong* bitsets, int n, int ulongsPerNode)
        {
            for (int i = n - 1; i >= 0; i--)
            {
                int u = topoOrder[i];
                int offset = u * ulongsPerNode;
                bitsets[offset + (u >> 6)] |= (1UL << (u & 63));

                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    int vOffset = v * ulongsPerNode;
                    for (int w = 0; w < ulongsPerNode; w++)
                    {
                        bitsets[offset + w] |= bitsets[vOffset + w];
                    }
                }
            }
        }
    }
}