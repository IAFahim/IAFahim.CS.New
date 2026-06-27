namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class RootedTreeCanonicalForm
    {
        // AHU (Aho-Hopcroft-Ullman) canonical subtree labelling.
        // p = parent array; root is the (single) node with p[i] < 0, every other node's p[i] is its parent.
        // outHash[i] receives a 32-bit canonical hash of the subtree rooted at i: two subtrees receive
        // equal hashes iff they are isomorphic as rooted (unordered) trees. Compare the root hashes of
        // two trees to test rooted-tree isomorphism.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* p, int n, int* outHash)
        {
            if (n <= 0) return;

            // Locate the root and validate the single-root + parent-range convention up front.
            // Malformed input (no/multiple roots, out-of-range parent, self-loop) leaves outHash
            // untouched rather than corrupting the heap via out-of-bounds CSR writes (childStart[par+1]).
            int root = -1;
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par < 0)
                {
                    if (root >= 0) return; // more than one root -> not a single rooted tree.
                    root = i;
                }
                else if (par >= n || par == i)
                {
                    return; // parent out of range or self-loop -> invalid input.
                }
            }
            if (root < 0) return; // no root -> not a rooted tree; leave outHash untouched.

            // n-sized scratch: heap (Temp-style) allocation, not stackalloc[n], to stay Burst/mobile-safe.
            // childStart: CSR offsets (n+1). children: child node ids (n). order: BFS node order (n).
            int* childStart = (int*)Marshal.AllocHGlobal((nint)((n + 1) * sizeof(int)));
            int* children = (int*)Marshal.AllocHGlobal((nint)(n * sizeof(int)));
            int* order = (int*)Marshal.AllocHGlobal((nint)(n * sizeof(int)));
            int* cursor = (int*)Marshal.AllocHGlobal((nint)(n * sizeof(int)));
            // Count children per parent into childStart[parent+1].
            for (int i = 0; i <= n; i++) childStart[i] = 0;
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par >= 0) childStart[par + 1]++;
            }
            // Prefix sum: childStart[par] becomes the start offset of par's children block.
            for (int i = 0; i < n; i++) childStart[i + 1] += childStart[i];

            // Scatter child ids into the CSR layout via a per-parent cursor.
            for (int i = 0; i < n; i++) cursor[i] = childStart[i];
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par >= 0) children[cursor[par]++] = i;
            }

            // BFS from the root -> parents appear before children in 'order'.
            int head = 0, tail = 0;
            order[tail++] = root;
            while (head < tail)
            {
                int node = order[head++];
                int s = childStart[node];
                int e = childStart[node + 1];
                for (int c = s; c < e; c++) order[tail++] = children[c];
            }

            // A valid single-rooted tree reaches every node exactly once. If BFS visited fewer
            // than n nodes the parent array encodes a cycle or disconnected component: bail before
            // the reverse pass indexes uninitialised 'order' slots (garbage -> AccessViolation).
            if (tail != n)
            {
                Marshal.FreeHGlobal((nint)cursor);
                Marshal.FreeHGlobal((nint)order);
                Marshal.FreeHGlobal((nint)children);
                Marshal.FreeHGlobal((nint)childStart);
                return;
            }

            // Process in reverse BFS order: each node is handled only after all its descendants,
            // so child hashes are final when combined at the parent.
            for (int oi = n - 1; oi >= 0; oi--)
            {
                int node = order[oi];
                int s = childStart[node];
                int e = childStart[node + 1];
                int cnt = e - s;

                // Sort the children slice by their already-computed hash so the combination is
                // independent of child order (unordered-tree canonicalisation). Insertion sort:
                // child fan-out is small in typical trees; worst case stays correct.
                for (int a = s + 1; a < e; a++)
                {
                    int cv = children[a];
                    uint key = (uint)outHash[cv];
                    int b = a - 1;
                    while (b >= s && (uint)outHash[children[b]] > key)
                    {
                        children[b + 1] = children[b];
                        b--;
                    }
                    children[b + 1] = cv;
                }

                // Fold the (count, sorted-child-hashes) into a 64-bit FNV-1a accumulator.
                ulong h = 1469598103934665603UL; // FNV offset basis
                h = (h ^ (uint)cnt) * 1099511628211UL;
                for (int c = s; c < e; c++)
                {
                    h = (h ^ (uint)outHash[children[c]]) * 1099511628211UL;
                }
                // splitmix64 finalizer for avalanche, then fold to 32 bits.
                h ^= h >> 30; h *= 0xbf58476d1ce4e5b9UL;
                h ^= h >> 27; h *= 0x94d049bb133111ebUL;
                h ^= h >> 31;
                outHash[node] = (int)(uint)(h ^ (h >> 32));
            }
            Marshal.FreeHGlobal((nint)cursor);
            Marshal.FreeHGlobal((nint)order);
            Marshal.FreeHGlobal((nint)children);
            Marshal.FreeHGlobal((nint)childStart);
        }
    }
}
