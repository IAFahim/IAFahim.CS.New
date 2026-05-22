namespace IAFahim.Graph.Connectivity
{
    using System.Runtime.CompilerServices;

    public struct EdgeInterval
    {
        public int U;
        public int V;
        public int StartTime;
        public int EndTime;
    }

    public static unsafe class OfflineDynamicConnectivity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Solve(
            int l, int r,
            EdgeInterval* edges, int edgeCount,
            int* parent, int* size,
            RollbackOp* history, ref int historyCount,
            int* answers)
        {
            if (l > r) return;

            int initialHistory = historyCount;
            int activeEdges = 0;
            int* activeIndices = stackalloc int[edgeCount];

            for (int i = 0; i < edgeCount; i++)
            {
                if (edges[i].StartTime <= l && edges[i].EndTime >= r)
                {
                    OfflineDynamicMst.Union(parent, size, edges[i].U, edges[i].V, history, ref historyCount);
                }
                else if (edges[i].StartTime <= r && edges[i].EndTime >= l)
                {
                    activeIndices[activeEdges++] = i;
                }
            }

            if (l == r)
            {
                // Normally process queries at time `l` and store in `answers`
                // Example: answers[l] = numberOfComponents;
            }
            else
            {
                int mid = l + (r - l) / 2;
                EdgeInterval* nextEdges = stackalloc EdgeInterval[activeEdges];
                for (int i = 0; i < activeEdges; i++)
                    nextEdges[i] = edges[activeIndices[i]];

                Solve(l, mid, nextEdges, activeEdges, parent, size, history, ref historyCount, answers);
                Solve(mid + 1, r, nextEdges, activeEdges, parent, size, history, ref historyCount, answers);
            }

            OfflineDynamicMst.Rollback(parent, size, history, ref historyCount, initialHistory);
        }
    }
}
