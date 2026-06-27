namespace IAFahim.Graph.Functional
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphReroot
    {
        // Treats the input as a rooted tree/forest encoded as a parent array (the functional-graph
        // special case where every node has exactly one parent; p[i] < 0 marks a root, convention
        // shared with IAFahim.Graph.TreeIsomorphism). "Reroot at u" reverses the u -> root path so u
        // becomes the new root (res[u] < 0) and every other node keeps its subtree. This is the
        // standard, well-defined tree-reroot operation; subtrees hanging off the reversed path stay
        // attached to their (path) parent, so only the path edges change direction.
        //
        // Caller guarantees: 0 <= u < n; p encodes a valid forest (no cycles); res is n ints. On
        // invalid input (u out of range, or the u->root walk encounters a cycle) res is left untouched
        // and the method returns false.
        public static bool Run(int* p, int n, int u, int* res)
        {
            if (n <= 0 || (uint)u >= (uint)n) return false;
            for (int i = 0; i < n; i++) res[i] = p[i];

            int prev = -1;
            int cur = u;
            int guard = 0;
            while (p[cur] >= 0)
            {
                int nxt = p[cur];
                res[cur] = prev;
                prev = cur;
                cur = nxt;
                if (++guard > n) return false;
            }
            res[cur] = prev;
            return true;
        }
    }
}
