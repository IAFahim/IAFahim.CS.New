namespace IAFahim.Graph.DynamicTrees
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct EttNode
    {
        // 8-byte fields first to avoid internal alignment padding, then the
        // six 4-byte fields. Total = 16 + 24 = 40 bytes with no padding.
        public long Val;
        public long SubSum;
        public int Parent;
        public int Left;
        public int Right;
        public int Size;
        public uint Priority;
        public int Twin;
    }

    public static unsafe class EulerTourTree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(EttNode* nodes, int n)
        {
            for (int i = 0; i < n; i++)
            {
                nodes[i].Parent = -1;
                nodes[i].Left = -1;
                nodes[i].Right = -1;
                nodes[i].Priority = 0;
                nodes[i].Size = 1;
                nodes[i].Val = 0;
                nodes[i].SubSum = 0;
                nodes[i].Twin = -1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PushUp(EttNode* nodes, int u)
        {
            if (u == -1)
            {
                return;
            }
            EttNode* p = nodes + u;
            int l = p->Left;
            int r = p->Right;
            p->Size = 1 + (l != -1 ? nodes[l].Size : 0) + (r != -1 ? nodes[r].Size : 0);
            p->SubSum = p->Val + (l != -1 ? nodes[l].SubSum : 0) + (r != -1 ? nodes[r].SubSum : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRoot(EttNode* nodes, int u)
        {
            while (nodes[u].Parent != -1)
            {
                u = nodes[u].Parent;
            }
            return u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndex(EttNode* nodes, int u)
        {
            int idx = (nodes[u].Left != -1 ? nodes[nodes[u].Left].Size : 0);
            int curr = u;
            while (nodes[curr].Parent != -1)
            {
                int p = nodes[curr].Parent;
                if (nodes[p].Right == curr)
                {
                    idx += 1 + (nodes[p].Left != -1 ? nodes[nodes[p].Left].Size : 0);
                }
                curr = p;
            }
            return idx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint NextRand(ref uint state)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Split(EttNode* nodes, int root, int k, out int l, out int r)
        {
            if (root == -1)
            {
                l = -1;
                r = -1;
                return;
            }
            int leftSize = nodes[root].Left != -1 ? nodes[nodes[root].Left].Size : 0;
            if (leftSize < k)
            {
                l = root;
                Split(nodes, nodes[root].Right, k - leftSize - 1, out nodes[root].Right, out r);
                if (nodes[root].Right != -1)
                {
                    nodes[nodes[root].Right].Parent = root;
                }
                if (r != -1)
                {
                    nodes[r].Parent = -1;
                }
            }
            else
            {
                r = root;
                Split(nodes, nodes[root].Left, k, out l, out nodes[root].Left);
                if (nodes[root].Left != -1)
                {
                    nodes[nodes[root].Left].Parent = root;
                }
                if (l != -1)
                {
                    nodes[l].Parent = -1;
                }
            }
            PushUp(nodes, root);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Merge(EttNode* nodes, int l, int r)
        {
            if (l == -1)
            {
                return r;
            }
            if (r == -1)
            {
                return l;
            }
            if (nodes[l].Priority > nodes[r].Priority)
            {
                nodes[l].Right = Merge(nodes, nodes[l].Right, r);
                if (nodes[l].Right != -1)
                {
                    nodes[nodes[l].Right].Parent = l;
                }
                PushUp(nodes, l);
                return l;
            }
            else
            {
                nodes[r].Left = Merge(nodes, l, nodes[r].Left);
                if (nodes[r].Left != -1)
                {
                    nodes[nodes[r].Left].Parent = r;
                }
                PushUp(nodes, r);
                return r;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Reroot(EttNode* nodes, int u, ref uint randState)
        {
            int root = GetRoot(nodes, u);
            int idx = GetIndex(nodes, u);
            int l, r;
            Split(nodes, root, idx, out l, out r);
            return Merge(nodes, r, l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Link(EttNode* nodes, int u, int v, int uv, int vu, ref uint randState)
        {
            if (GetRoot(nodes, u) == GetRoot(nodes, v))
            {
                return;
            }

            int rootU = Reroot(nodes, u, ref randState);
            int rootV = Reroot(nodes, v, ref randState);

            nodes[uv].Priority = NextRand(ref randState);
            nodes[uv].Twin = vu;
            PushUp(nodes, uv);

            nodes[vu].Priority = NextRand(ref randState);
            nodes[vu].Twin = uv;
            PushUp(nodes, vu);

            int idxU = GetIndex(nodes, u);
            int lU, rU;
            Split(nodes, rootU, idxU + 1, out lU, out rU);

            int root = Merge(nodes, lU, uv);
            root = Merge(nodes, root, rootV);
            root = Merge(nodes, root, vu);
            Merge(nodes, root, rU);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cut(EttNode* nodes, int uv, int vu, ref uint randState)
        {
            int root = GetRoot(nodes, uv);
            int idx1 = GetIndex(nodes, uv);
            int idx2 = GetIndex(nodes, vu);

            if (idx1 > idx2)
            {
                int t = idx1; idx1 = idx2; idx2 = t;
                t = uv; uv = vu; vu = t;
            }

            int l1, r1;
            Split(nodes, root, idx2 + 1, out l1, out r1);

            int l2, r2;
            Split(nodes, l1, idx2, out l2, out r2); // r2 is vu

            int l3, r3;
            Split(nodes, l2, idx1 + 1, out l3, out r3); // r3 is the subtree/component of v

            int l4, r4;
            Split(nodes, l3, idx1, out l4, out r4); // r4 is uv

            Merge(nodes, l4, r1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Connected(EttNode* nodes, int u, int v)
        {
            return GetRoot(nodes, u) == GetRoot(nodes, v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPredecessor(EttNode* nodes, int u)
        {
            int idx = GetIndex(nodes, u);
            if (idx == 0)
            {
                return -1;
            }
            int root = GetRoot(nodes, u);
            int l, r;
            Split(nodes, root, idx, out l, out r);
            int curr = l;
            while (nodes[curr].Right != -1)
            {
                curr = nodes[curr].Right;
            }
            Merge(nodes, l, r);
            return curr;
        }

        // In-order predecessor of node u within its treap, by pointer navigation
        // only (no Split/Merge). Returns -1 if u is the first element of the tour.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PredNode(EttNode* nodes, int u)
        {
            if (nodes[u].Left != -1)
            {
                int c = nodes[u].Left;
                while (nodes[c].Right != -1)
                {
                    c = nodes[c].Right;
                }
                return c;
            }
            int curr = u;
            while (nodes[curr].Parent != -1)
            {
                int p = nodes[curr].Parent;
                if (nodes[p].Right == curr)
                {
                    return p;
                }
                curr = p;
            }
            return -1;
        }

        // Sum of Val over the subtree of vertex u under the CURRENT rooting of u's
        // tree. The Euler tour stored here keeps every tree-subtree as a contiguous
        // index interval, but (after Reroot/Link rotations) a vertex is not
        // necessarily the leftmost element of its own subtree, and the element
        // immediately preceding u is not necessarily u's parent edge. The subtree
        // of u is bracketed by u's parent edge (the innermost directed-edge pair
        // whose tour interval encloses u): [idx(enter) .. idx(twin(enter))]. We
        // locate that enclosing edge by climbing left from u over fully matched
        // sibling subtrees (each closing edge jumps directly to before its matching
        // open edge) until we reach an open edge whose twin lies after it.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SubtreeQuery(EttNode* nodes, int u)
        {
            int rootTree = GetRoot(nodes, u);
            int idxU = GetIndex(nodes, u);
            if (idxU == 0)
            {
                // u is the overall root of the tour: its subtree is the whole tree.
                return nodes[rootTree].SubSum;
            }

            int enter = -1;
            int idxEnter = -1;
            int idxExit = -1;
            int curr = PredNode(nodes, u);
            while (curr != -1)
            {
                int twin = nodes[curr].Twin;
                if (twin == -1)
                {
                    // Vertex node (no twin); two vertices are never adjacent in a
                    // valid tour, but skip defensively.
                    curr = PredNode(nodes, curr);
                    continue;
                }
                int idxCurr = GetIndex(nodes, curr);
                int idxTwin = GetIndex(nodes, twin);
                if (idxTwin > idxCurr)
                {
                    // Open edge whose match lies after it: this is u's parent edge,
                    // and [idxCurr .. idxTwin] brackets u's subtree.
                    enter = curr;
                    idxEnter = idxCurr;
                    idxExit = idxTwin;
                    break;
                }
                // Close edge: skip its entire matched pair by jumping to the
                // element just before its matching open edge.
                curr = PredNode(nodes, twin);
            }

            if (enter == -1)
            {
                // No enclosing edge found: u is effectively the root of its tour.
                return nodes[rootTree].SubSum;
            }

            int l1, r1;
            Split(nodes, rootTree, idxExit + 1, out l1, out r1);

            int l2, r2;
            Split(nodes, l1, idxEnter, out l2, out r2);

            long ans = r2 != -1 ? nodes[r2].SubSum : 0;

            int mergedL1 = Merge(nodes, l2, r2);
            Merge(nodes, mergedL1, r1);

            return ans;
        }
    }
}
