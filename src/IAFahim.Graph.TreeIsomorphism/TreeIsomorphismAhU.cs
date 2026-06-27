namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class TreeIsomorphismAhU
    {
        // AHU (Aho-Hopcroft-Ullman) rooted-tree isomorphism test.
        // p1, p2 = parent arrays of two rooted trees on n nodes each; the (single) root is the node
        // with p[i] < 0, every other node's p[i] is its parent. Returns true iff the two rooted trees
        // are isomorphic as unordered trees.
        //
        // We assign exact canonical integer codes (not 64-bit hashes, so there is no collision risk)
        // level by level from the deepest level up. A node's signature is its child-count followed by
        // its children's codes sorted ascending; nodes at the same depth that share a signature share a
        // code. Both trees are coded in one shared code space, so the two roots (both at depth 0) end up
        // with the same code exactly when the trees are isomorphic. O(n) levels, near-linear overall.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* p1, int* p2, int n)
        {
            if (n <= 0) return true; // two empty trees are trivially isomorphic.

            // Locate both roots, validating the single-root convention.
            int root1 = FindRoot(p1, n);
            int root2 = FindRoot(p2, n);
            if (root1 < 0 || root2 < 0) return false; // not a valid single rooted tree.

            // Combined index space: tree-1 nodes are [0, n), tree-2 nodes are [n, 2n).
            int total = n * 2;

            // n-sized scratch on the heap (Temp-style), not stackalloc[n], to stay Burst/mobile-safe.
            // childStart: CSR offsets (total+1). children: child ids (total). order: BFS queue / cursor
            // (total). depth: per-node depth (total). code: canonical label (total).
            int* childStart = (int*)Marshal.AllocHGlobal((nint)((total + 1) * sizeof(int)));
            int* children = (int*)Marshal.AllocHGlobal((nint)(total * sizeof(int)));
            int* order = (int*)Marshal.AllocHGlobal((nint)(total * sizeof(int)));
            int* depth = (int*)Marshal.AllocHGlobal((nint)(total * sizeof(int)));
            int* code = (int*)Marshal.AllocHGlobal((nint)(total * sizeof(int)));
            try
            {
                // Count children per parent (childStart[parent+1]) in the combined space; validate ranges.
                for (int i = 0; i <= total; i++) childStart[i] = 0;
                if (!CountChildren(p1, n, 0, childStart)) return false;
                if (!CountChildren(p2, n, n, childStart)) return false;

                // Prefix sum: childStart[node] becomes the start offset of node's children block.
                for (int i = 0; i < total; i++) childStart[i + 1] += childStart[i];

                // Scatter child ids into the CSR layout via a per-node cursor (reuse 'order' as cursor).
                for (int i = 0; i < total; i++) order[i] = childStart[i];
                FillChildren(p1, n, 0, children, order);
                FillChildren(p2, n, n, children, order);

                // Depth of every node via BFS from each root (also validates full reachability: a valid
                // tree reaches all n of its nodes). 'order' is reused as the BFS queue.
                if (ComputeDepth(root1 + 0, childStart, children, order, depth) != n) return false;
                if (ComputeDepth(root2 + n, childStart, children, order, depth) != n) return false;

                // Counting-sort node indices by ascending depth so we can process levels deepest-first.
                int maxDepth = 0;
                for (int i = 0; i < total; i++) if (depth[i] > maxDepth) maxDepth = depth[i];

                int* depthCount = (int*)Marshal.AllocHGlobal((nint)((maxDepth + 2) * sizeof(int)));
                int* byDepth = (int*)Marshal.AllocHGlobal((nint)(total * sizeof(int)));
                try
                {
                    for (int d = 0; d <= maxDepth + 1; d++) depthCount[d] = 0;
                    for (int i = 0; i < total; i++) depthCount[depth[i] + 1]++;
                    for (int d = 0; d < maxDepth + 1; d++) depthCount[d + 1] += depthCount[d];
                    // depthCount[d] is the start offset of level d within byDepth; reuse it as the cursor.
                    for (int i = 0; i < total; i++) byDepth[depthCount[depth[i]]++] = i;
                    // byDepth is now grouped by ascending depth, so its tail run is the deepest level.

                    int nextCode = 0;
                    int li = total; // walk byDepth from the end (deepest) toward the root level.
                    while (li > 0)
                    {
                        int d = depth[byDepth[li - 1]];
                        // Find the contiguous run [runStart, li) of nodes at depth d.
                        int runStart = li - 1;
                        while (runStart > 0 && depth[byDepth[runStart - 1]] == d) runStart--;

                        // Sort each node's child block by (already final, deeper-level) code so its
                        // signature is order-independent.
                        for (int k = runStart; k < li; k++)
                            SortChildBlockByCode(byDepth[k], childStart, children, code);

                        // Sort this level's nodes by canonical signature (insertion sort, matching the
                        // module's small-fan-out house style).
                        InsertionSortBySignature(byDepth + runStart, li - runStart, childStart, children, code);

                        // Assign dense codes: equal consecutive signatures share a code.
                        for (int k = runStart; k < li; k++)
                        {
                            if (k > runStart && CompareSignature(byDepth[k - 1], byDepth[k], childStart, children, code) == 0)
                                code[byDepth[k]] = code[byDepth[k - 1]];
                            else
                                code[byDepth[k]] = nextCode++;
                        }

                        li = runStart;
                    }

                    return code[root1 + 0] == code[root2 + n];
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)byDepth);
                    Marshal.FreeHGlobal((nint)depthCount);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)code);
                Marshal.FreeHGlobal((nint)depth);
                Marshal.FreeHGlobal((nint)order);
                Marshal.FreeHGlobal((nint)children);
                Marshal.FreeHGlobal((nint)childStart);
            }
        }

        // Returns the single root index (p[i] < 0), or -1 if there is no root or more than one.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindRoot(int* p, int n)
        {
            int root = -1;
            for (int i = 0; i < n; i++)
            {
                if (p[i] < 0)
                {
                    if (root >= 0) return -1; // more than one root -> not a single rooted tree.
                    root = i;
                }
            }
            return root;
        }

        // Tally children per parent into childStart[offset + parent + 1]. Returns false on an
        // out-of-range parent index (malformed input).
        private static bool CountChildren(int* p, int n, int offset, int* childStart)
        {
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par < 0) continue;
                if (par >= n) return false;
                childStart[offset + par + 1]++;
            }
            return true;
        }

        // Scatter child ids (translated by 'offset') into the CSR layout using a per-node cursor.
        private static void FillChildren(int* p, int n, int offset, int* children, int* cursor)
        {
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par >= 0) children[cursor[offset + par]++] = offset + i;
            }
        }

        // BFS from 'start' over the CSR layout: fills depth[] (start at 0, children one deeper) and
        // returns the number of nodes reached (used to validate that the tree spans all n nodes).
        private static int ComputeDepth(int start, int* childStart, int* children, int* queue, int* depth)
        {
            int head = 0, tail = 0;
            depth[start] = 0;
            queue[tail++] = start;
            while (head < tail)
            {
                int node = queue[head++];
                int nd = depth[node];
                int s = childStart[node];
                int e = childStart[node + 1];
                for (int c = s; c < e; c++)
                {
                    int ch = children[c];
                    depth[ch] = nd + 1;
                    queue[tail++] = ch;
                }
            }
            return tail;
        }

        // Sort a node's children block ascending by their (already final) code, via insertion sort.
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

        // Insertion sort the level's nodes by canonical signature (child count, then sorted child codes).
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

        // Lexicographic comparison of two nodes' signatures: first by child count, then element-wise by
        // sorted child codes. Both nodes' child blocks must already be sorted by code.
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
    }
}
