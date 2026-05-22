namespace IAFahim.Graph.SCC
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    // Incremental SCC using DSU cycle contraction and BFS.
    // Handles edge additions and maintains SCCs.
    public static unsafe class OnlineScc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Find(int* parent, int i)
        {
            if (parent[i] == i) return i;
            return parent[i] = Find(parent, parent[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Union(int* parent, int i, int j)
        {
            int rootI = Find(parent, i);
            int rootJ = Find(parent, j);
            if (rootI != rootJ)
            {
                parent[rootI] = rootJ;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddEdge(int u, int v, 
                                   int* head, int* next, int* to, ref int edgeCount,
                                   int* parent, int n, int* queue, int* visited, int runId)
        {
            int rootU = Find(parent, u);
            int rootV = Find(parent, v);

            if (rootU == rootV) return;

            // Add edge rootU -> rootV in the DAG of SCCs
            to[edgeCount] = rootV;
            next[edgeCount] = head[rootU];
            head[rootU] = edgeCount;
            edgeCount++;

            // Check if rootV can reach rootU
            int qh = 0, qt = 0;
            queue[qt++] = rootV;
            visited[rootV] = runId;

            bool cycleFound = false;

            while (qh < qt)
            {
                int curr = queue[qh++];
                if (curr == rootU)
                {
                    cycleFound = true;
                    break;
                }

                for (int e = head[curr]; e != -1; e = next[e])
                {
                    int neighbor = Find(parent, to[e]);
                    if (visited[neighbor] != runId)
                    {
                        visited[neighbor] = runId;
                        queue[qt++] = neighbor;
                    }
                }
            }

            if (cycleFound)
            {
                // Contract all visited vertices into a single SCC
                for (int i = 0; i < qt; i++)
                {
                    int node = queue[i];
                    Union(parent, node, rootU);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(int n, int* parent, int* head, int* visited)
        {
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                head[i] = -1;
                visited[i] = 0;
            }
        }
    }
}
