namespace IAFahim.Graph.DynamicTrees
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct EttNode
    {
        public int Parent;
        public int Left;
        public int Right;
        public uint Priority;
        public int Size;
        public long Val;
        public long SubSum;
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
            int l = nodes[u].Left;
            int r = nodes[u].Right;
            nodes[u].Size = 1 + (l != -1 ? nodes[l].Size : 0) + (r != -1 ? nodes[r].Size : 0);
            nodes[u].SubSum = nodes[u].Val + (l != -1 ? nodes[l].SubSum : 0) + (r != -1 ? nodes[r].SubSum : 0);
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
        public static void Reroot(EttNode* nodes, int u, ref uint randState)
        {
            int root = GetRoot(nodes, u);
            int idx = GetIndex(nodes, u);
            int l, r;
            Split(nodes, root, idx, out l, out r);
            Merge(nodes, r, l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Link(EttNode* nodes, int u, int v, int uv, int vu, ref uint randState)
        {
            if (GetRoot(nodes, u) == GetRoot(nodes, v))
            {
                return;
            }

            Reroot(nodes, u, ref randState);
            Reroot(nodes, v, ref randState);

            int rootU = GetRoot(nodes, u);
            int rootV = GetRoot(nodes, v);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SubtreeQuery(EttNode* nodes, int u)
        {
            int pred = GetPredecessor(nodes, u);
            if (pred == -1)
            {
                int root = GetRoot(nodes, u);
                return nodes[root].SubSum;
            }

            int twin = nodes[pred].Twin;
            int idxPred = GetIndex(nodes, pred);
            int idxTwin = GetIndex(nodes, twin);

            if (idxPred > idxTwin)
            {
                int t = idxPred; idxPred = idxTwin; idxTwin = t;
            }

            int rootTree = GetRoot(nodes, u);
            int l1, r1;
            Split(nodes, rootTree, idxTwin + 1, out l1, out r1);

            int l2, r2;
            Split(nodes, l1, idxPred, out l2, out r2);

            long ans = nodes[r2].SubSum;

            int mergedL1 = Merge(nodes, l2, r2);
            Merge(nodes, mergedL1, r1);

            return ans;
        }
    }
}
