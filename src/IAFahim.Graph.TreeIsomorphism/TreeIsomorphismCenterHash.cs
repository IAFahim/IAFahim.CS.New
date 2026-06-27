namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class TreeIsomorphismCenterHash
    {
        // Unrooted-tree isomorphism by centering.
        //
        // p1, p2 are parent arrays describing two UNROOTED trees on n nodes each. The tree's edge
        // set is { (i, p[i]) : p[i] >= 0 }; exactly one node has p[i] < 0 (it carries no parent
        // edge), giving the n-1 undirected edges of a tree. Which node is "rootless" is arbitrary
        // and does NOT define the rooting — the tree is unrooted.
        //
        // Two unrooted trees are isomorphic iff, after rooting each at its center (the 1 or 2
        // nodes of minimum eccentricity, found by iterative leaf-stripping), some center-rooting
        // of one matches some center-rooting of the other. We compute exact canonical integer
        // codes (AHU bottom-up relabelling — no hash collisions) for every center-rooting of both
        // trees in ONE shared code space, then test whether any tree-1 rooting code equals any
        // tree-2 rooting code. Because an isomorphism must map centers to centers, that test is
        // exact.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* p1, int* p2, int n)
        {
            if (n <= 0) return true;   // two empty trees are trivially isomorphic.
            if (n == 1) return true;   // both single-node trees.

            // Combined undirected index space: tree-1 nodes are [0, n), tree-2 nodes are [n, 2n).
            int total = n * 2;

            // Undirected CSR: every edge (i, p[i]) contributes two directed arcs, so 2*(n-1) per
            // tree => 4*(n-1) arc slots across both trees. Allocate the loose bound 2*total.
            int arcCap = (total) * 2;
            var adjStart = (int*)Marshal.AllocHGlobal(sizeof(int) * (total + 1));
            var adj = (int*)Marshal.AllocHGlobal(sizeof(int) * arcCap);
            var cursor = (int*)Marshal.AllocHGlobal(sizeof(int) * total);
            var deg = (int*)Marshal.AllocHGlobal(sizeof(int) * total);

            bool valid = true;

            for (int i = 0; i <= total; i++) adjStart[i] = 0;
            CountDegrees(p1, n, 0, adjStart, ref valid);
            CountDegrees(p2, n, n, adjStart, ref valid);

            bool isomorphic = false;
            // Centers: up to 2 per tree. centers1[k], centers2[k] in combined space; -1 = unused.
            int c1a = -1, c1b = -1, c2a = -1, c2b = -1;

            if (valid)
            {
                // Prefix sum -> adjStart[v] is the start of v's arc block.
                for (int i = 0; i < total; i++) adjStart[i + 1] += adjStart[i];

                for (int i = 0; i < total; i++) cursor[i] = adjStart[i];
                FillArcs(p1, n, 0, adj, cursor);
                FillArcs(p2, n, n, adj, cursor);

                // Find centers of each tree by leaf-stripping over its own combined-space slice.
                FindCenters(0, n, adjStart, adj, deg, cursor, ref c1a, ref c1b);
                FindCenters(n, n, adjStart, adj, deg, cursor, ref c2a, ref c2b);
            }

            // Number of candidate rootings.
            int r1 = (c1a < 0 ? 0 : 1) + (c1b < 0 ? 0 : 1);
            int r2 = (c2a < 0 ? 0 : 1) + (c2b < 0 ? 0 : 1);

            if (valid && r1 > 0 && r2 > 0)
            {
                // Build a single shared rooted forest containing all candidate rootings, in one
                // index space, then compute consistent AHU codes across all of them. Each rooting
                // re-numbers its n nodes into a fresh block so distinct rootings never alias.
                int rootings = r1 + r2;
                int slots = rootings * n;

                // Rooted CSR (parent -> children) over the rooting-expanded space.
                var childStart = (int*)Marshal.AllocHGlobal(sizeof(int) * (slots + 1));
                var children = (int*)Marshal.AllocHGlobal(sizeof(int) * slots);
                var depth = (int*)Marshal.AllocHGlobal(sizeof(int) * slots);
                var order = (int*)Marshal.AllocHGlobal(sizeof(int) * slots);
                var code = (int*)Marshal.AllocHGlobal(sizeof(int) * slots);
                // Per-rooting root slot (combined-space center mapped to its rooting block).
                var rootSlot = stackalloc int[4];     // at most r1+r2 <= 4 rootings.
                var rootBase = stackalloc int[4];     // block base index for each rooting.
                var rootTree = stackalloc int[4];     // 0 => belongs to tree 1, 1 => tree 2.

                for (int i = 0; i <= slots; i++) childStart[i] = 0;

                int ri = 0;
                if (c1a >= 0) { rootSlot[ri] = c1a; rootBase[ri] = ri * n; rootTree[ri] = 0; ri++; }
                if (c1b >= 0) { rootSlot[ri] = c1b; rootBase[ri] = ri * n; rootTree[ri] = 0; ri++; }
                if (c2a >= 0) { rootSlot[ri] = c2a; rootBase[ri] = ri * n; rootTree[ri] = 1; ri++; }
                if (c2b >= 0) { rootSlot[ri] = c2b; rootBase[ri] = ri * n; rootTree[ri] = 1; ri++; }

                // For each rooting, BFS from the center over the undirected graph, assigning each
                // visited combined-space node a dense slot in this rooting's block and recording the
                // parent/child relation. The center's combined-space offset (0 or n) tells us which
                // undirected slice we traverse.
                // local map: combined-space node -> slot in this rooting's block (or -1).
                var localMap = (int*)Marshal.AllocHGlobal(sizeof(int) * total);
                var parentSlot = (int*)Marshal.AllocHGlobal(sizeof(int) * slots);

                for (int rr = 0; rr < rootings; rr++)
                {
                    int center = rootSlot[rr];
                    int baseOff = rootBase[rr];
                    int sliceOff = center >= n ? n : 0;     // which undirected slice this center lives in.
                    for (int i = sliceOff; i < sliceOff + n; i++) localMap[i] = -1;

                    // BFS producing a dense ordering; node at BFS position k -> slot baseOff+k.
                    int head = 0, tail = 0;
                    int* q = order;                          // reuse order[] as the BFS queue (>= n entries).
                    localMap[center] = baseOff + 0;
                    parentSlot[baseOff + 0] = -1;
                    q[tail++] = center;
                    int filled = 1;
                    while (head < tail)
                    {
                        int u = q[head++];
                        int us = localMap[u];
                        int s = adjStart[u];
                        int e = adjStart[u + 1];
                        for (int a = s; a < e; a++)
                        {
                            int v = adj[a];
                            if (localMap[v] >= 0) continue;  // already visited (parent or earlier).
                            int vs = baseOff + filled;
                            filled++;
                            localMap[v] = vs;
                            parentSlot[vs] = us;
                            q[tail++] = v;
                        }
                    }

                    // Count children for this rooting's slots: childStart[parent+1]++.
                    for (int k = 0; k < n; k++)
                    {
                        int ps = parentSlot[baseOff + k];
                        if (ps >= 0) childStart[ps + 1]++;
                    }
                    rootSlot[rr] = baseOff;                   // the root is always at block base.
                }

                // Prefix sum over the whole expanded space, then fill children[] using a cursor.
                for (int i = 0; i < slots; i++) childStart[i + 1] += childStart[i];
                var fcursor = (int*)Marshal.AllocHGlobal(sizeof(int) * slots);
                for (int i = 0; i < slots; i++) fcursor[i] = childStart[i];
                for (int s = 0; s < slots; s++)
                {
                    int ps = parentSlot[s];
                    if (ps >= 0) children[fcursor[ps]++] = s;
                }

                // Bottom-up AHU coding in one shared code space, processing nodes by descending
                // depth across ALL rootings simultaneously so identical subtree shapes — wherever
                // they occur — receive the same code.
                for (int rr = 0; rr < rootings; rr++)
                    ComputeDepth(rootSlot[rr], childStart, children, q: order, depth);

                int maxDepth = 0;
                for (int i = 0; i < slots; i++) if (depth[i] > maxDepth) maxDepth = depth[i];

                var depthCount = (int*)Marshal.AllocHGlobal(sizeof(int) * (maxDepth + 2));
                for (int d = 0; d <= maxDepth + 1; d++) depthCount[d] = 0;
                for (int i = 0; i < slots; i++) depthCount[depth[i] + 1]++;
                for (int d = 0; d < maxDepth + 1; d++) depthCount[d + 1] += depthCount[d];
                var byDepth = (int*)Marshal.AllocHGlobal(sizeof(int) * slots);
                var dcursor = (int*)Marshal.AllocHGlobal(sizeof(int) * (maxDepth + 1));
                for (int d = 0; d <= maxDepth; d++) dcursor[d] = depthCount[d];
                for (int i = 0; i < slots; i++)
                {
                    int d = depth[i];
                    byDepth[dcursor[d]++] = i;
                }

                var levelNodes = (int*)Marshal.AllocHGlobal(sizeof(int) * slots);

                int nextCode = 0;
                int li = slots;
                while (li > 0)
                {
                    int d = depth[byDepth[li - 1]];
                    int cnt = 0;
                    int j = li - 1;
                    while (j >= 0 && depth[byDepth[j]] == d)
                    {
                        levelNodes[cnt++] = byDepth[j];
                        j--;
                    }
                    li = j + 1;

                    for (int k = 0; k < cnt; k++)
                        SortChildBlockByCode(levelNodes[k], childStart, children, code);

                    InsertionSortBySignature(levelNodes, cnt, childStart, children, code);

                    for (int k = 0; k < cnt; k++)
                    {
                        if (k > 0 && SignatureEqual(levelNodes[k - 1], levelNodes[k], childStart, children, code))
                            code[levelNodes[k]] = code[levelNodes[k - 1]];
                        else
                            code[levelNodes[k]] = nextCode++;
                    }
                }

                // Isomorphic iff some tree-1 rooting code equals some tree-2 rooting code.
                for (int a = 0; a < rootings && !isomorphic; a++)
                {
                    if (rootTree[a] != 0) continue;
                    int ca = code[rootBase[a]];
                    for (int b = 0; b < rootings; b++)
                    {
                        if (rootTree[b] != 1) continue;
                        if (code[rootBase[b]] == ca) { isomorphic = true; break; }
                    }
                }

                Marshal.FreeHGlobal(new System.IntPtr((void*)levelNodes));
                Marshal.FreeHGlobal(new System.IntPtr((void*)dcursor));
                Marshal.FreeHGlobal(new System.IntPtr((void*)byDepth));
                Marshal.FreeHGlobal(new System.IntPtr((void*)depthCount));
                Marshal.FreeHGlobal(new System.IntPtr((void*)fcursor));
                Marshal.FreeHGlobal(new System.IntPtr((void*)parentSlot));
                Marshal.FreeHGlobal(new System.IntPtr((void*)localMap));
                Marshal.FreeHGlobal(new System.IntPtr((void*)code));
                Marshal.FreeHGlobal(new System.IntPtr((void*)order));
                Marshal.FreeHGlobal(new System.IntPtr((void*)depth));
                Marshal.FreeHGlobal(new System.IntPtr((void*)children));
                Marshal.FreeHGlobal(new System.IntPtr((void*)childStart));
            }

            Marshal.FreeHGlobal(new System.IntPtr((void*)deg));
            Marshal.FreeHGlobal(new System.IntPtr((void*)cursor));
            Marshal.FreeHGlobal(new System.IntPtr((void*)adj));
            Marshal.FreeHGlobal(new System.IntPtr((void*)adjStart));

            return isomorphic;
        }

        // Count undirected degree of each endpoint of every edge (i, p[i]) into adjStart[v+1],
        // translated by 'offset' into the combined index space. Validates parent ranges and the
        // single-rootless-node convention.
        private static void CountDegrees(int* p, int n, int offset, int* adjStart, ref bool valid)
        {
            int rootless = 0;
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par < 0) { rootless++; continue; }
                if (par >= n || par == i) { valid = false; return; }
                adjStart[offset + i + 1]++;
                adjStart[offset + par + 1]++;
            }
            if (rootless != 1) valid = false;   // a tree's parent-array has exactly one rootless node.
        }

        // Fill undirected arcs for both directions of each edge (i, p[i]).
        private static void FillArcs(int* p, int n, int offset, int* adj, int* cursor)
        {
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par < 0) continue;
                int u = offset + i;
                int v = offset + par;
                adj[cursor[u]++] = v;
                adj[cursor[v]++] = u;
            }
        }

        // Find the 1 or 2 centers of the tree occupying combined slice [sliceOff, sliceOff+n) via
        // iterative leaf-stripping. Writes the surviving center(s) (in combined space) to ca/cb.
        // Uses deg[] for live degree and 'frontier' (cursor[] scratch) as the leaf queue.
        private static void FindCenters(int sliceOff, int n, int* adjStart, int* adj, int* deg, int* frontier, ref int ca, ref int cb)
        {
            if (n == 1) { ca = sliceOff; cb = -1; return; }

            for (int i = 0; i < n; i++)
            {
                int v = sliceOff + i;
                deg[v] = adjStart[v + 1] - adjStart[v];
            }

            // Initial leaf frontier.
            int qn = 0;
            for (int i = 0; i < n; i++)
            {
                int v = sliceOff + i;
                if (deg[v] <= 1) frontier[qn++] = v;
            }

            // removed[v] tracked via deg[v] == -1 sentinel. Strip whole leaf layers until 1 or 2
            // nodes remain unremoved — those are the center(s).
            int remaining = n;
            // Strip leaves layer by layer until <= 2 nodes remain. Layers are removed atomically so
            // a node that becomes the sole survivor is never itself stripped, even if its live
            // degree drops to 0 within the final layer.
            while (remaining > 2)
            {
                // No leaves left but >2 nodes remain => this slice contains a cycle (malformed, not a
                // tree). Stop instead of spinning forever; the post-loop check reports "no center".
                if (qn == 0) break;
                int qn2 = 0;
                for (int k = 0; k < qn; k++)
                {
                    int u = frontier[k];
                    int s = adjStart[u];
                    int e = adjStart[u + 1];
                    for (int a = s; a < e; a++)
                    {
                        int w = adj[a];
                        if (deg[w] < 0) continue;            // already removed in a prior layer.
                        deg[w]--;
                        if (deg[w] == 1) frontier[qn + qn2++] = w;   // becomes a leaf next layer.
                    }
                }
                // Remove this layer and compact the next frontier to the front of the buffer.
                for (int k = 0; k < qn; k++) deg[frontier[k]] = -1;
                remaining -= qn;
                for (int k = 0; k < qn2; k++) frontier[k] = frontier[qn + k];
                qn = qn2;
            }

            // Cycle detected (couldn't strip down to <=2): no valid center -> caller treats as non-tree.
            if (remaining > 2) { ca = -1; cb = -1; return; }

            // Survivors are the unremoved nodes (deg != -1): exactly 1 or 2 of them.
            ca = -1; cb = -1;
            for (int i = 0; i < n; i++)
            {
                int v = sliceOff + i;
                if (deg[v] >= 0)
                {
                    if (ca < 0) ca = v; else cb = v;
                }
            }
        }

        // depth[start]=0; BFS over rooted CSR so each node visited once (O(n)).
        private static void ComputeDepth(int start, int* childStart, int* children, int* q, int* depth)
        {
            int head = 0, tail = 0;
            depth[start] = 0;
            q[tail++] = start;
            while (head < tail)
            {
                int node = q[head++];
                int nd = depth[node];
                int s = childStart[node];
                int e = childStart[node + 1];
                for (int c = s; c < e; c++)
                {
                    int ch = children[c];
                    depth[ch] = nd + 1;
                    q[tail++] = ch;
                }
            }
        }

        // Sort a node's children block ascending by their (already final) code via insertion sort.
        private static void SortChildBlockByCode(int node, int* childStart, int* children, int* code)
        {
            int s = childStart[node];
            int e = childStart[node + 1];
            for (int a = s + 1; a < e; a++)
            {
                int cv = children[a];
                int key = code[cv];
                int b = a - 1;
                while (b >= s && code[children[b]] > key)
                {
                    children[b + 1] = children[b];
                    b--;
                }
                children[b + 1] = cv;
            }
        }

        private static void InsertionSortBySignature(int* nodes, int cnt, int* childStart, int* children, int* code)
        {
            for (int a = 1; a < cnt; a++)
            {
                int v = nodes[a];
                int b = a - 1;
                while (b >= 0 && CompareSignature(nodes[b], v, childStart, children, code) > 0)
                {
                    nodes[b + 1] = nodes[b];
                    b--;
                }
                nodes[b + 1] = v;
            }
        }

        // Lexicographic comparison: first by child count, then element-wise by sorted child codes.
        private static int CompareSignature(int x, int y, int* childStart, int* children, int* code)
        {
            int sx = childStart[x], ex = childStart[x + 1];
            int sy = childStart[y], ey = childStart[y + 1];
            int cx = ex - sx, cy = ey - sy;
            if (cx != cy) return cx < cy ? -1 : 1;
            for (int i = 0; i < cx; i++)
            {
                int a = code[children[sx + i]];
                int b = code[children[sy + i]];
                if (a != b) return a < b ? -1 : 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SignatureEqual(int x, int y, int* childStart, int* children, int* code)
        {
            return CompareSignature(x, y, childStart, children, code) == 0;
        }
    }
}
