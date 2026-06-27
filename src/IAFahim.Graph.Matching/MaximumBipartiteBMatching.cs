namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public static unsafe class MaximumBipartiteBMatching
    {
        // Maximum b-matching in a bipartite graph: left node i may take up to
        // capacitiesLeft[i] matched edges, right node j up to capacitiesRight[j].
        // Reduced to max-flow (Dinic): source -> left (cap=capacitiesLeft[i]),
        // each original edge left -> right (cap=1), right -> sink (cap=capacitiesRight[j]).
        // Returns total matched edges (= max flow). Uses Allocator.Temp scratch.
        //
        // Input adjacency is the repo's edge-list-by-left-node form:
        //   head[u] = index of first edge of left node u (0 = sentinel / none)
        //   next[e] = index of next edge in u's list (0 = end)
        //   to[e]   = right node of edge e, in [0, nRight)
        public static int Run(int* head, int* next, int* to, int* capacitiesLeft, int* capacitiesRight, int nLeft, int nRight)
        {
            if (nLeft <= 0 || nRight <= 0) return 0;

            // Count original bipartite edges.
            int origEdges = 0;
            for (int u = 0; u < nLeft; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    origEdges++;

            // Flow-graph nodes: source(0), left(1..nLeft), right(nLeft+1..nLeft+nRight), sink(last).
            int nNodes = nLeft + nRight + 2;
            int source = 0;
            int sink = nNodes - 1;

            // Directed arcs (each stored twice: forward + residual back arc):
            //   nLeft  source->left  + origEdges left->right + nRight right->sink
            int dirArcs = nLeft + origEdges + nRight;
            int arcCap = dirArcs * 2;

            int align = UnsafeUtility.AlignOf<int>();
            int* gHead = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * nNodes, align);
            int* gNext = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * arcCap, align);
            int* gTo = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * arcCap, align);
            int* gCap = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * arcCap, align);
            int* level = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * nNodes, align);
            int* iter = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * nNodes, align);
            int* queue = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * nNodes, align);

            int flow;
            try
            {
                for (int i = 0; i < nNodes; i++) gHead[i] = -1;
                int arcCount = 0;

                // source -> left
                for (int i = 0; i < nLeft; i++)
                {
                    int cap = capacitiesLeft[i];
                    if (cap < 0) cap = 0;
                    AddArc(gHead, gNext, gTo, gCap, ref arcCount, source, 1 + i, cap);
                }

                // left -> right (unit edges), preserving input adjacency
                for (int u = 0; u < nLeft; u++)
                {
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        AddArc(gHead, gNext, gTo, gCap, ref arcCount, 1 + u, 1 + nLeft + v, 1);
                    }
                }

                // right -> sink
                for (int j = 0; j < nRight; j++)
                {
                    int cap = capacitiesRight[j];
                    if (cap < 0) cap = 0;
                    AddArc(gHead, gNext, gTo, gCap, ref arcCount, 1 + nLeft + j, sink, cap);
                }

                flow = Dinic(gHead, gNext, gTo, gCap, level, iter, queue, nNodes, source, sink);
            }
            finally
            {
                AllocatorManager.Free(Allocator.Temp, queue);
                AllocatorManager.Free(Allocator.Temp, iter);
                AllocatorManager.Free(Allocator.Temp, level);
                AllocatorManager.Free(Allocator.Temp, gCap);
                AllocatorManager.Free(Allocator.Temp, gTo);
                AllocatorManager.Free(Allocator.Temp, gNext);
                AllocatorManager.Free(Allocator.Temp, gHead);
            }

            return flow;
        }

        // Adds a forward arc (cap) and its residual back arc (cap 0). Paired arcs
        // are adjacent: forward at even index, back at odd index, so xor 1 toggles.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddArc(int* gHead, int* gNext, int* gTo, int* gCap, ref int arcCount, int from, int dest, int cap)
        {
            gTo[arcCount] = dest;
            gCap[arcCount] = cap;
            gNext[arcCount] = gHead[from];
            gHead[from] = arcCount;
            arcCount++;

            gTo[arcCount] = from;
            gCap[arcCount] = 0;
            gNext[arcCount] = gHead[dest];
            gHead[dest] = arcCount;
            arcCount++;
        }

        private static int Dinic(int* gHead, int* gNext, int* gTo, int* gCap, int* level, int* iter, int* queue, int nNodes, int source, int sink)
        {
            int flow = 0;
            while (Bfs(gHead, gNext, gTo, gCap, level, queue, nNodes, source, sink))
            {
                for (int i = 0; i < nNodes; i++) iter[i] = gHead[i];
                int f;
                while ((f = Dfs(gHead, gNext, gTo, gCap, level, iter, source, sink, int.MaxValue)) > 0)
                    flow += f;
            }
            return flow;
        }

        // Builds level graph from source; returns whether sink is reachable.
        private static bool Bfs(int* gHead, int* gNext, int* gTo, int* gCap, int* level, int* queue, int nNodes, int source, int sink)
        {
            for (int i = 0; i < nNodes; i++) level[i] = -1;
            int qh = 0, qt = 0;
            level[source] = 0;
            queue[qt++] = source;
            while (qh < qt)
            {
                int u = queue[qh++];
                for (int e = gHead[u]; e != -1; e = gNext[e])
                {
                    int v = gTo[e];
                    if (gCap[e] > 0 && level[v] < 0)
                    {
                        level[v] = level[u] + 1;
                        queue[qt++] = v;
                    }
                }
            }
            return level[sink] >= 0;
        }

        // Sends blocking flow along level graph. iter is the current-arc pointer
        // per node so each arc is advanced past at most once per phase.
        private static int Dfs(int* gHead, int* gNext, int* gTo, int* gCap, int* level, int* iter, int u, int sink, int pushed)
        {
            if (u == sink) return pushed;
            for (; iter[u] != -1; iter[u] = gNext[iter[u]])
            {
                int e = iter[u];
                int v = gTo[e];
                if (gCap[e] <= 0 || level[v] != level[u] + 1) continue;
                int d = Dfs(gHead, gNext, gTo, gCap, level, iter, v, sink, pushed < gCap[e] ? pushed : gCap[e]);
                if (d > 0)
                {
                    gCap[e] -= d;
                    gCap[e ^ 1] += d;
                    return d;
                }
            }
            return 0;
        }
    }
}
