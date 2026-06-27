namespace IAFahim.Graph.Cactus
{
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public static unsafe class CactusCycleDecompose
    {
        // Cactus cycle decomposition (per-edge / per-arc cycle id).
        //
        // Input is an undirected graph in linked-adjacency form: head[u] is the id of u's first
        // incident arc (or -1), to[e] is arc e's endpoint, next[e] chains to the next arc out of the
        // same source (or -1). The m arcs are the 2*(edges) directed half-edges of the undirected
        // graph in the standard reverse-paired layout — arc e and arc e^1 are the two directions of
        // one undirected edge (push u->v then v->u). cycleId is sized m (one slot per arc); the two
        // twin arcs of an edge always receive the same id.
        //
        // On a cactus (any two simple cycles share at most one vertex, so every edge lies in at most
        // one simple cycle) we label each arc with the id of the simple cycle its edge belongs to,
        // and bridge edges (in no cycle) with the sentinel -1. We return the number of cycles.
        //
        // We run a single recursion-free DFS (explicit stack, Burst-safe). Each non-twin back edge
        // u->v with v a strict DFS ancestor of u (tin[v] < tin[u]) closes exactly one simple cycle:
        // the tree path from v down to u plus that back edge. We mint a fresh cycle id, tag the back
        // arc and its twin, then walk up parent arcs from u until reaching v, tagging each tree arc
        // (and twin) with the same id. Because it is a cactus, every tree edge is reached by at most
        // one such climb, so no arc is relabeled. Arcs never tagged stay at the -1 sentinel (bridges).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* head, int* to, int* next, int n, int m, int* cycleId)
        {
            for (int e = 0; e < m; e++) cycleId[e] = -1;
            if (n <= 0) return 0;

            int ai = UnsafeUtility.AlignOf<int>();

            var tin = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            // parentArc[v] = the arc id by which v was first discovered (its tree edge to the parent),
            // or -1 for a DFS root. Lets us climb v..u along tree arcs when a back edge closes a cycle.
            var parentArc = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            // DFS scratch: explicit stack of (node, incoming-arc-id, iterator-arc-id).
            var stkNode = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            var stkParentArc = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);
            var stkIter = (int*)AllocatorManager.Allocate(Allocator.Temp, sizeof(int) * n, ai);

            for (int i = 0; i < n; i++) { tin[i] = -1; parentArc[i] = -1; }

            int timer = 0;
            int cycleCount = 0;
            for (int s = 0; s < n; s++)
            {
                if (tin[s] != -1) continue;

                int sp = 0;
                stkNode[sp] = s;
                stkParentArc[sp] = -1;
                stkIter[sp] = head[s];
                tin[s] = timer++;

                while (sp >= 0)
                {
                    int u = stkNode[sp];
                    int e = stkIter[sp];

                    if (e != -1)
                    {
                        stkIter[sp] = next[e];   // advance iterator before recursing.
                        int pe = stkParentArc[sp];

                        // Skip exactly the arc we arrived on (the twin of the parent arc). A genuine
                        // parallel edge has a different id and is processed as a real back edge.
                        if (pe != -1 && e == (pe ^ 1)) continue;

                        int v = to[e];

                        if (tin[v] == -1)
                        {
                            // Tree edge.
                            tin[v] = timer++;
                            parentArc[v] = e;
                            sp++;
                            stkNode[sp] = v;
                            stkParentArc[sp] = e;
                            stkIter[sp] = head[v];
                        }
                        else if (tin[v] < tin[u])
                        {
                            // Back edge to a strict ancestor: closes one simple cycle. Seeing it only
                            // from the descendant side (tin[v] < tin[u]) processes each undirected
                            // back edge exactly once; its twin (tin[v] > tin[u]) is ignored below.
                            int cid = cycleCount++;
                            cycleId[e] = cid;
                            cycleId[e ^ 1] = cid;
                            // Climb tree arcs from u up to v, tagging each edge of the cycle.
                            int w = u;
                            while (w != v)
                            {
                                int pa = parentArc[w];
                                cycleId[pa] = cid;
                                cycleId[pa ^ 1] = cid;
                                w = to[pa ^ 1];   // parent of w (source of the tree arc pa).
                            }
                        }
                        // else: forward/twin-direction edge to a descendant (tin[v] > tin[u]) — already
                        // handled from the other endpoint; do nothing.
                    }
                    else
                    {
                        sp--;   // done with u, backtrack.
                    }
                }
            }

            AllocatorManager.Free(Allocator.Temp, stkIter);
            AllocatorManager.Free(Allocator.Temp, stkParentArc);
            AllocatorManager.Free(Allocator.Temp, stkNode);
            AllocatorManager.Free(Allocator.Temp, parentArc);
            AllocatorManager.Free(Allocator.Temp, tin);

            return cycleCount;
        }
    }
}
