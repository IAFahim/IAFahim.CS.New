namespace IAFahim.Graph.Cactus
{
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public static unsafe class BridgeTreeDiameter
    {
        // Bridge-tree diameter.
        //
        // Input is an undirected graph in linked-adjacency form: head[u] is the id of u's first
        // incident arc (or -1), to[e] is arc e's endpoint, next[e] chains to the next arc out of the
        // same source (or -1). The m arcs are the 2*(edges) directed half-edges of the undirected
        // graph and are stored in reverse-paired order — arc e and arc e^1 are the two directions of
        // one undirected edge (the standard "add edge => push u->v then v->u" layout, which is why
        // to/next are sized m = arc count, not edge count). That pairing lets us skip exactly the
        // arc we arrived on while still treating a genuine second parallel edge as a non-bridge.
        //
        // We (1) find all bridges with an iterative Tarjan low-link pass (recursion-free, Burst-safe),
        // (2) contract each 2-edge-connected component to a node by BFS-flooding without crossing
        // bridges, giving each original vertex a component id, then (3) build the resulting tree
        // (one tree edge per bridge) and return its diameter — the maximum number of bridges on any
        // path — via the classic double-BFS (farthest node from an arbitrary start, then farthest
        // from that node). Returns 0 when the graph has no bridges (single 2ecc) or is empty; for a
        // disconnected graph it returns the largest diameter among the components' bridge trees.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* head, int* to, int* next, int n, int m)
        {
            if (n <= 0) return 0;

            int ai = UnsafeUtility.AlignOf<int>();

            // --- Phase 1: find bridges (mark each arc as bridge / not) via iterative low-link. ---
            var tin = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            var low = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            // isBridgeArc[e] = 1 iff arc e (and its twin e^1) is a bridge. m may be 0.
            byte* isBridgeArc = m > 0 ? (byte*)AllocatorManager.Allocate(Allocator.Temp, sizeof(byte) * m, 1) : null;
            // DFS-iteration scratch: explicit stack of (node, incoming-arc-id, iterator-arc-id).
            var stkNode = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            var stkParentArc = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            var stkIter = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);

            for (int i = 0; i < n; i++) tin[i] = -1;
            for (int e = 0; e < m; e++) isBridgeArc[e] = 0;

            int timer = 0;
            for (int s = 0; s < n; s++)
            {
                if (tin[s] != -1) continue;

                int sp = 0;
                stkNode[sp] = s;
                stkParentArc[sp] = -1;
                stkIter[sp] = head[s];
                tin[s] = low[s] = timer++;

                while (sp >= 0)
                {
                    int u = stkNode[sp];
                    int e = stkIter[sp];

                    if (e != -1)
                    {
                        stkIter[sp] = next[e];   // advance iterator before recursing/back-tracking.
                        int v = to[e];

                        // Skip exactly the arc we arrived on (the twin of the parent arc). A second
                        // parallel edge to the same vertex has a different id and is NOT skipped, so
                        // it correctly relaxes low[u] and prevents the edge from being a bridge.
                        if (e == (stkParentArc[sp] ^ 1) && stkParentArc[sp] != -1) continue;

                        if (tin[v] == -1)
                        {
                            tin[v] = low[v] = timer++;
                            sp++;
                            stkNode[sp] = v;
                            stkParentArc[sp] = e;
                            stkIter[sp] = head[v];
                        }
                        else if (tin[v] < low[u])
                        {
                            low[u] = tin[v];     // back edge.
                        }
                    }
                    else
                    {
                        // Done with u: propagate low to parent and decide if the parent arc is a bridge.
                        int pe = stkParentArc[sp];
                        sp--;
                        if (sp >= 0)
                        {
                            int p = stkNode[sp];
                            if (low[u] < low[p]) low[p] = low[u];
                            if (low[u] > tin[p])
                            {
                                isBridgeArc[pe] = 1;
                                isBridgeArc[pe ^ 1] = 1;
                            }
                        }
                    }
                }
            }

            // --- Phase 2: contract 2-edge-connected components (flood without crossing bridges). ---
            var comp = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            var bfs = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            for (int i = 0; i < n; i++) comp[i] = -1;

            int compCount = 0;
            for (int s = 0; s < n; s++)
            {
                if (comp[s] != -1) continue;
                int cid = compCount++;
                int qh = 0, qt = 0;
                bfs[qt++] = s;
                comp[s] = cid;
                while (qh < qt)
                {
                    int u = bfs[qh++];
                    for (int e = head[u]; e != -1; e = next[e])
                    {
                        if (isBridgeArc[e] == 1) continue;   // bridges separate components.
                        int v = to[e];
                        if (comp[v] != -1) continue;
                        comp[v] = cid;
                        bfs[qt++] = v;
                    }
                }
            }

            AllocatorManager.Free(Allocator.Temp, stkIter);
            AllocatorManager.Free(Allocator.Temp, stkParentArc);
            AllocatorManager.Free(Allocator.Temp, stkNode);
            AllocatorManager.Free(Allocator.Temp, low);
            AllocatorManager.Free(Allocator.Temp, tin);

            if (compCount <= 1)
            {
                if (isBridgeArc != null) AllocatorManager.Free(Allocator.Temp, isBridgeArc);
                AllocatorManager.Free(Allocator.Temp, bfs);
                AllocatorManager.Free(Allocator.Temp, comp);
                return 0;   // no bridges => bridge tree is a single node => diameter 0.
            }

            // --- Phase 3: build the bridge tree as CSR (each bridge => one tree edge), then ---
            // run double-BFS over it to get the diameter (edge count of the longest path).
            // Each bridge arc e contributes a directed tree arc comp[from] -> comp[to]; since both
            // e and e^1 are marked, both directions are emitted, giving an undirected CSR.
            int treeArcCap = 0;
            for (int e = 0; e < m; e++) if (isBridgeArc[e] == 1) treeArcCap++;

            var tStart = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * (compCount + 1), ai);
            var tAdj = treeArcCap > 0 ? (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * treeArcCap, ai) : null;
            var src = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * m, ai); // arc source vertex (indexed by arc id 0..m).

            for (int i = 0; i <= compCount; i++) tStart[i] = 0;
            // Record each arc's source vertex so we can map arc e to comp[src]->comp[to[e]].
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != -1; e = next[e])
                    src[e] = u;

            for (int e = 0; e < m; e++)
                if (isBridgeArc[e] == 1)
                    tStart[comp[src[e]] + 1]++;
            for (int i = 0; i < compCount; i++) tStart[i + 1] += tStart[i];

            var cur = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * compCount, ai);
            for (int i = 0; i < compCount; i++) cur[i] = tStart[i];
            for (int e = 0; e < m; e++)
                if (isBridgeArc[e] == 1)
                    tAdj[cur[comp[src[e]]]++] = comp[to[e]];

            // dist[] reused for both BFS sweeps; tbfs[] is the BFS queue over components.
            // The bridge structure of a disconnected graph is a FOREST, so we double-BFS each tree
            // separately and take the global maximum diameter. 'treeSeen' marks tree-nodes already
            // claimed by a previously processed tree so each tree is measured exactly once.
            var dist = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * compCount, ai);
            var tbfs = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * compCount, ai);
            var treeSeen = (byte*)AllocatorManager.Allocate(Allocator.Temp, sizeof(byte) * compCount, 1);
            for (int i = 0; i < compCount; i++) treeSeen[i] = 0;

            int diameter = 0;
            for (int root = 0; root < compCount; root++)
            {
                if (treeSeen[root] == 1) continue;
                // First sweep: farthest tree-node from this root (also marks the whole tree seen).
                int far0 = FarthestBfs(root, tStart, tAdj, dist, tbfs, compCount, treeSeen);
                // Second sweep from that endpoint gives this tree's diameter.
                int d = dist[FarthestBfs(far0, tStart, tAdj, dist, tbfs, compCount, null)];
                if (d > diameter) diameter = d;
            }

            AllocatorManager.Free(Allocator.Temp, treeSeen);
            AllocatorManager.Free(Allocator.Temp, tbfs);
            AllocatorManager.Free(Allocator.Temp, dist);
            AllocatorManager.Free(Allocator.Temp, cur);
            AllocatorManager.Free(Allocator.Temp, src);
            if (tAdj != null) AllocatorManager.Free(Allocator.Temp, tAdj);
            AllocatorManager.Free(Allocator.Temp, tStart);
            if (isBridgeArc != null) AllocatorManager.Free(Allocator.Temp, isBridgeArc);
            AllocatorManager.Free(Allocator.Temp, bfs);
            AllocatorManager.Free(Allocator.Temp, comp);

            return diameter;
        }

        // BFS over the bridge tree from 'start' (its connected component / tree only). Fills dist[]
        // (-1 outside this tree) and returns the farthest reached node; the caller reads its distance
        // from dist[]. When 'seen' is non-null, every visited tree-node is marked seen[v]=1 so the
        // forest driver visits each tree exactly once.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FarthestBfs(int start, int* tStart, int* tAdj, int* dist, int* q, int compCount, byte* seen)
        {
            for (int i = 0; i < compCount; i++) dist[i] = -1;
            int qh = 0, qt = 0;
            dist[start] = 0;
            q[qt++] = start;
            if (seen != null) seen[start] = 1;
            int best = start, bestD = 0;
            while (qh < qt)
            {
                int u = q[qh++];
                int du = dist[u];
                if (du > bestD) { bestD = du; best = u; }
                int s = tStart[u], e = tStart[u + 1];
                for (int a = s; a < e; a++)
                {
                    int v = tAdj[a];
                    if (dist[v] != -1) continue;
                    dist[v] = du + 1;
                    if (seen != null) seen[v] = 1;
                    q[qt++] = v;
                }
            }
            return best;
        }
    }
}
