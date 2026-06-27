namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphComponent
    {
        // Weakly-connected components of a functional graph (each node i has one out-edge f[i]).
        // Two nodes share a component iff joined through the undirected edges {i, f[i]}.
        // Fully zero-alloc: the caller's 'comp' buffer is reused as the DSU parent forest and then
        // rewritten in place to dense ids in [0, count). No stackalloc / no Temp allocation.
        // Returns the component count.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* f, int n, int* comp)
        {
            if (n <= 0) return 0;

            // 'comp' doubles as the DSU parent array: every node starts as its own root.
            for (int i = 0; i < n; i++) comp[i] = i;

            // Union each node with its successor (the underlying undirected edge {i, f[i]}).
            // Union-by-min-index keeps every set's root equal to its smallest member index,
            // which lets the relabel pass run in one in-place sweep without extra scratch.
            for (int i = 0; i < n; i++)
            {
                int t = f[i];
                if (t < 0 || t >= n) continue; // ignore out-of-range / sentinel edges
                Union(comp, i, t);
            }

            // Flatten so comp[i] holds i's canonical root directly (full path compression).
            for (int i = 0; i < n; i++) comp[i] = Find(comp, i);

            // In-place dense relabel without a side table:
            //   - a root r satisfies comp[r] == r and, being the min index of its set, is visited
            //     before any of its members; encode its id as a negative tag -(id+1).
            //   - a non-root i reads its root r (< i, already tagged) and copies the tag.
            // Negative tags are disjoint from the [0, n) index space, so reads stay unambiguous.
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                int r = comp[i];
                if (r == i) comp[i] = -(count++) - 1; // newly discovered root
                else comp[i] = comp[r];                // copy the root's (already negative) tag
            }

            // Decode tags back to plain dense ids in [0, count).
            for (int i = 0; i < n; i++) comp[i] = -comp[i] - 1;

            return count;
        }

        // Iterative find with full path compression over the parent array 'parent'.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Find(int* parent, int x)
        {
            int r = x;
            while (parent[r] != r) r = parent[r];
            while (parent[x] != r)
            {
                int next = parent[x];
                parent[x] = r;
                x = next;
            }
            return r;
        }

        // Union by smaller-root index: the surviving root is always the minimum member index.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Union(int* parent, int a, int b)
        {
            int ra = Find(parent, a);
            int rb = Find(parent, b);
            if (ra == rb) return;
            if (ra < rb) parent[rb] = ra;
            else parent[ra] = rb;
        }
    }
}
