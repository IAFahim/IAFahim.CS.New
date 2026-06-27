namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public static unsafe class MinimumCostBipartiteBMatching
    {
        // Minimum-cost maximum b-matching in a bipartite graph: left node i may take
        // up to capacitiesLeft[i] matched edges, right node j up to capacitiesRight[j];
        // each original edge carries cost[e]. Among all MAXIMUM b-matchings, returns the
        // minimum total edge cost. Reduced to min-cost max-flow (SPFA / Bellman-Ford
        // potentials, then Dijkstra-style shortest-path augmentation):
        //   source -> left (cap=capacitiesLeft[i], cost 0)
        //   each original edge left -> right (cap=1, cost=cost[e])
        //   right -> sink (cap=capacitiesRight[j], cost 0)
        // Returns the min total cost. Uses Allocator.Temp scratch.
        //
        // Input adjacency is the repo's edge-list-by-left-node form:
        //   head[u] = index of first edge of left node u (0 = sentinel / none)
        //   next[e] = index of next edge in u's list (0 = end)
        //   to[e]   = right node of edge e, in [0, nRight)
        //   cost[e] = cost of edge e (parallel to to[]/next[])
        public static int Run(int* head, int* next, int* to, int* cost, int* capacitiesLeft, int* capacitiesRight, int nLeft, int nRight)
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
            int* gCost = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * arcCap, align);
            // SPFA / shortest-path scratch (one slot per node).
            int* dist = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * nNodes, align);
            int* prevArc = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * nNodes, align);
            byte* inQueue = (byte*)AllocatorManager.Allocate(Allocator.Temp, sizeof(byte) * nNodes, UnsafeUtility.AlignOf<byte>());
            // Circular SPFA queue: a node may be enqueued at most nNodes times before
            // the queue drains, so nNodes+1 slots suffice for the ring buffer.
            int* queue = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * (nNodes + 1), align);

            int totalCost;
            try
            {
                for (int i = 0; i < nNodes; i++) gHead[i] = -1;
                int arcCount = 0;

                // source -> left (cost 0)
                for (int i = 0; i < nLeft; i++)
                {
                    int cap = capacitiesLeft[i];
                    if (cap < 0) cap = 0;
                    AddArc(gHead, gNext, gTo, gCap, gCost, ref arcCount, source, 1 + i, cap, 0);
                }

                // left -> right (unit edges with cost), preserving input adjacency
                for (int u = 0; u < nLeft; u++)
                {
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        AddArc(gHead, gNext, gTo, gCap, gCost, ref arcCount, 1 + u, 1 + nLeft + v, 1, cost[e]);
                    }
                }

                // right -> sink (cost 0)
                for (int j = 0; j < nRight; j++)
                {
                    int cap = capacitiesRight[j];
                    if (cap < 0) cap = 0;
                    AddArc(gHead, gNext, gTo, gCap, gCost, ref arcCount, 1 + nLeft + j, sink, cap, 0);
                }

                totalCost = MinCostMaxFlow(gHead, gNext, gTo, gCap, gCost, dist, prevArc, inQueue, queue, nNodes, source, sink);
            }
            finally
            {
                AllocatorManager.Free(Allocator.Temp, queue);
                AllocatorManager.Free(Allocator.Temp, inQueue);
                AllocatorManager.Free(Allocator.Temp, prevArc);
                AllocatorManager.Free(Allocator.Temp, dist);
                AllocatorManager.Free(Allocator.Temp, gCost);
                AllocatorManager.Free(Allocator.Temp, gCap);
                AllocatorManager.Free(Allocator.Temp, gTo);
                AllocatorManager.Free(Allocator.Temp, gNext);
                AllocatorManager.Free(Allocator.Temp, gHead);
            }

            return totalCost;
        }

        // Adds a forward arc (cap, cost) and its residual back arc (cap 0, cost -cost).
        // Paired arcs are adjacent: forward at even index, back at odd index, so xor 1
        // toggles between them.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddArc(int* gHead, int* gNext, int* gTo, int* gCap, int* gCost, ref int arcCount, int from, int dest, int cap, int cost)
        {
            gTo[arcCount] = dest;
            gCap[arcCount] = cap;
            gCost[arcCount] = cost;
            gNext[arcCount] = gHead[from];
            gHead[from] = arcCount;
            arcCount++;

            gTo[arcCount] = from;
            gCap[arcCount] = 0;
            gCost[arcCount] = -cost;
            gNext[arcCount] = gHead[dest];
            gHead[dest] = arcCount;
            arcCount++;
        }

        // Successive shortest-path min-cost max-flow. Each phase finds the cheapest
        // residual source->sink path via SPFA (Bellman-Ford queue, tolerates the
        // negative-cost residual arcs), then saturates it. Accumulates total cost of
        // the maximum flow. Costs are summed in long to avoid intermediate overflow,
        // then returned as int.
        private static int MinCostMaxFlow(int* gHead, int* gNext, int* gTo, int* gCap, int* gCost, int* dist, int* prevArc, byte* inQueue, int* queue, int nNodes, int source, int sink)
        {
            long total = 0;
            int ringCap = nNodes + 1;

            while (Spfa(gHead, gNext, gTo, gCap, gCost, dist, prevArc, inQueue, queue, nNodes, source, sink, ringCap))
            {
                // Find the bottleneck residual capacity along the path.
                int push = int.MaxValue;
                for (int v = sink; v != source;)
                {
                    int e = prevArc[v];
                    if (gCap[e] < push) push = gCap[e];
                    v = gTo[e ^ 1];
                }

                // Apply the flow and accumulate cost.
                for (int v = sink; v != source;)
                {
                    int e = prevArc[v];
                    gCap[e] -= push;
                    gCap[e ^ 1] += push;
                    v = gTo[e ^ 1];
                }
                total += (long)push * dist[sink];
            }

            return (int)total;
        }

        // SPFA shortest path by cost from source over arcs with residual capacity.
        // Writes dist[] (min cost to each node) and prevArc[] (incoming arc on the
        // best path). Returns whether sink is reachable. queue is a circular ring
        // buffer of capacity ringCap (= nNodes + 1).
        private static bool Spfa(int* gHead, int* gNext, int* gTo, int* gCap, int* gCost, int* dist, int* prevArc, byte* inQueue, int* queue, int nNodes, int source, int sink, int ringCap)
        {
            for (int i = 0; i < nNodes; i++)
            {
                dist[i] = int.MaxValue;
                prevArc[i] = -1;
                inQueue[i] = 0;
            }

            int qh = 0, qt = 0;
            dist[source] = 0;
            queue[qt++] = source;
            inQueue[source] = 1;

            while (qh != qt)
            {
                int u = queue[qh++];
                if (qh == ringCap) qh = 0;
                inQueue[u] = 0;
                int du = dist[u];

                for (int e = gHead[u]; e != -1; e = gNext[e])
                {
                    if (gCap[e] <= 0) continue;
                    int v = gTo[e];
                    int nd = du + gCost[e];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        prevArc[v] = e;
                        if (inQueue[v] == 0)
                        {
                            inQueue[v] = 1;
                            queue[qt++] = v;
                            if (qt == ringCap) qt = 0;
                        }
                    }
                }
            }

            return dist[sink] != int.MaxValue;
        }
    }
}
