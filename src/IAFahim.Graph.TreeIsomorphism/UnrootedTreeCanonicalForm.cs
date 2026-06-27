namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class UnrootedTreeCanonicalForm
    {
        // Canonical form of an UNROOTED tree.
        //
        // p = parent array on n nodes; the tree's edge set is { (i, p[i]) : p[i] >= 0 }, with exactly
        // one node carrying p[i] < 0 (it owns no parent edge). Which node is "rootless" is arbitrary and
        // does NOT root the tree.
        //
        // Algorithm: find the tree's center (1 or 2 nodes of minimum eccentricity, via iterative
        // leaf-stripping); root at the center; compute an AHU subtree hash for every node of that
        // rooting; the rooting's canonical value is the root's hash. With two centers we hash both
        // center-rootings and combine them order-independently (the two are joined by an edge, so an
        // isomorphism may swap them), yielding a single value invariant under tree isomorphism.
        //
        // Output layout: a single canonical hash for the whole tree is written to outHash[0]. Two
        // unrooted trees are isomorphic iff their outHash[0] values are equal (FNV-1a + splitmix64
        // folding; collisions are possible in principle but astronomically unlikely, matching the
        // sibling RootedTreeCanonicalForm contract).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* p, int n, int* outHash)
        {
            if (n <= 0) return;
            if (n == 1) { outHash[0] = LeafHash(0); return; } // single node: a 0-child leaf shape.

            // Undirected CSR over the tree's n-1 edges (two arcs each => 2*(n-1) arc slots).
            int arcCap = (n - 1) * 2;
            if (arcCap < 0) arcCap = 0;
            var adjStart = (int*)Marshal.AllocHGlobal(sizeof(int) * (n + 1));
            var adj = (int*)Marshal.AllocHGlobal(sizeof(int) * (arcCap < 1 ? 1 : arcCap));
            var cursor = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            var deg = (int*)Marshal.AllocHGlobal(sizeof(int) * n);

            // Rooted scratch (reused per center-rooting).
            var order = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            var parent = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            var childStart = (int*)Marshal.AllocHGlobal(sizeof(int) * (n + 1));
            var children = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            var ccursor = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            var subHash = (int*)Marshal.AllocHGlobal(sizeof(int) * n);

            bool valid = true;

            for (int i = 0; i <= n; i++) adjStart[i] = 0;

            // Count undirected degrees; validate the single-rootless-node convention and parent ranges.
            int rootless = 0;
            for (int i = 0; i < n && valid; i++)
            {
                int par = p[i];
                if (par < 0) { rootless++; continue; }
                if (par >= n || par == i) { valid = false; break; }
                adjStart[i + 1]++;
                adjStart[par + 1]++;
            }
            if (rootless != 1) valid = false;

            if (valid)
            {
                for (int i = 0; i < n; i++) adjStart[i + 1] += adjStart[i];
                for (int i = 0; i < n; i++) cursor[i] = adjStart[i];
                for (int i = 0; i < n; i++)
                {
                    int par = p[i];
                    if (par < 0) continue;
                    adj[cursor[i]++] = par;
                    adj[cursor[par]++] = i;
                }

                int ca, cb;
                FindCenters(n, adjStart, adj, deg, cursor, out ca, out cb);

                int hA = HashRootedAtCenter(ca, n, adjStart, adj, order, parent, childStart, children, ccursor, subHash);
                if (cb < 0)
                {
                    outHash[0] = hA;
                }
                else
                {
                    int hB = HashRootedAtCenter(cb, n, adjStart, adj, order, parent, childStart, children, ccursor, subHash);
                    // Combine the two center-rootings order-independently (centers are symmetric under
                    // an isomorphism that swaps them across their shared edge).
                    uint lo = (uint)hA, hi = (uint)hB;
                    if (lo > hi) { uint t = lo; lo = hi; hi = t; }
                    ulong h = 1469598103934665603UL;
                    h = (h ^ lo) * 1099511628211UL;
                    h = (h ^ hi) * 1099511628211UL;
                    h ^= h >> 30; h *= 0xbf58476d1ce4e5b9UL;
                    h ^= h >> 27; h *= 0x94d049bb133111ebUL;
                    h ^= h >> 31;
                    outHash[0] = (int)(uint)(h ^ (h >> 32));
                }
            }

            Marshal.FreeHGlobal(new System.IntPtr((void*)subHash));
            Marshal.FreeHGlobal(new System.IntPtr((void*)ccursor));
            Marshal.FreeHGlobal(new System.IntPtr((void*)children));
            Marshal.FreeHGlobal(new System.IntPtr((void*)childStart));
            Marshal.FreeHGlobal(new System.IntPtr((void*)parent));
            Marshal.FreeHGlobal(new System.IntPtr((void*)order));
            Marshal.FreeHGlobal(new System.IntPtr((void*)deg));
            Marshal.FreeHGlobal(new System.IntPtr((void*)cursor));
            Marshal.FreeHGlobal(new System.IntPtr((void*)adj));
            Marshal.FreeHGlobal(new System.IntPtr((void*)adjStart));
        }

        // Find the 1 or 2 centers via iterative leaf-stripping. deg[] holds live degree (-1 = removed),
        // 'frontier' (cursor scratch) is the leaf queue.
        private static void FindCenters(int n, int* adjStart, int* adj, int* deg, int* frontier, out int ca, out int cb)
        {
            for (int i = 0; i < n; i++) deg[i] = adjStart[i + 1] - adjStart[i];

            int qn = 0;
            for (int i = 0; i < n; i++) if (deg[i] <= 1) frontier[qn++] = i;

            int remaining = n;
            // Strip whole leaf layers atomically until <= 2 nodes remain; those are the center(s).
            while (remaining > 2)
            {
                int qn2 = 0;
                for (int k = 0; k < qn; k++)
                {
                    int u = frontier[k];
                    int s = adjStart[u];
                    int e = adjStart[u + 1];
                    for (int a = s; a < e; a++)
                    {
                        int w = adj[a];
                        if (deg[w] < 0) continue;
                        deg[w]--;
                        if (deg[w] == 1) frontier[qn + qn2++] = w;
                    }
                }
                for (int k = 0; k < qn; k++) deg[frontier[k]] = -1;
                remaining -= qn;
                for (int k = 0; k < qn2; k++) frontier[k] = frontier[qn + k];
                qn = qn2;
            }

            ca = -1; cb = -1;
            for (int i = 0; i < n; i++)
            {
                if (deg[i] >= 0)
                {
                    if (ca < 0) ca = i; else cb = i;
                }
            }
        }

        // Root the undirected tree at 'center', AHU-hash every subtree, return the root's hash.
        private static int HashRootedAtCenter(int center, int n, int* adjStart, int* adj,
            int* order, int* parent, int* childStart, int* children, int* ccursor, int* subHash)
        {
            // BFS from center over the undirected graph -> rooted parent + BFS order (parents first).
            // Visitation is tracked via parent[]: -2 = unseen, -1 = root, >=0 = parent node.
            for (int i = 0; i <= n; i++) childStart[i] = 0;
            for (int i = 0; i < n; i++) parent[i] = -2;
            int head = 0, tail = 0;
            parent[center] = -1;
            order[tail++] = center;
            while (head < tail)
            {
                int u = order[head++];
                int s = adjStart[u];
                int e = adjStart[u + 1];
                for (int a = s; a < e; a++)
                {
                    int v = adj[a];
                    if (parent[v] != -2) continue; // already visited (its parent, or earlier sibling path).
                    parent[v] = u;
                    childStart[u + 1]++;
                    order[tail++] = v;
                }
            }

            // CSR children blocks (parent -> children) from the recorded parents.
            for (int i = 0; i < n; i++) childStart[i + 1] += childStart[i];
            for (int i = 0; i < n; i++) ccursor[i] = childStart[i];
            for (int oi = 1; oi < n; oi++)
            {
                int node = order[oi];
                int par = parent[node];
                children[ccursor[par]++] = node;
            }

            // Reverse BFS: a node is hashed only after all its descendants are final.
            for (int oi = n - 1; oi >= 0; oi--)
            {
                int node = order[oi];
                int s = childStart[node];
                int e = childStart[node + 1];

                // Sort children by their subtree hash so the fold is order-independent (unordered tree).
                for (int a = s + 1; a < e; a++)
                {
                    int cv = children[a];
                    uint key = (uint)subHash[cv];
                    int b = a - 1;
                    while (b >= s && (uint)subHash[children[b]] > key)
                    {
                        children[b + 1] = children[b];
                        b--;
                    }
                    children[b + 1] = cv;
                }

                subHash[node] = SubtreeHash(e - s, s, e, children, subHash);
            }

            return subHash[center];
        }

        // Hash for a 0-child leaf (childCount == 0).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeafHash(int childCount)
        {
            ulong h = 1469598103934665603UL;
            h = (h ^ (uint)childCount) * 1099511628211UL;
            h ^= h >> 30; h *= 0xbf58476d1ce4e5b9UL;
            h ^= h >> 27; h *= 0x94d049bb133111ebUL;
            h ^= h >> 31;
            return (int)(uint)(h ^ (h >> 32));
        }

        // Fold (childCount, sorted-child-hashes) into a 32-bit subtree hash.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SubtreeHash(int childCount, int s, int e, int* children, int* subHash)
        {
            ulong h = 1469598103934665603UL;
            h = (h ^ (uint)childCount) * 1099511628211UL;
            for (int c = s; c < e; c++)
                h = (h ^ (uint)subHash[children[c]]) * 1099511628211UL;
            h ^= h >> 30; h *= 0xbf58476d1ce4e5b9UL;
            h ^= h >> 27; h *= 0x94d049bb133111ebUL;
            h ^= h >> 31;
            return (int)(uint)(h ^ (h >> 32));
        }
    }
}
