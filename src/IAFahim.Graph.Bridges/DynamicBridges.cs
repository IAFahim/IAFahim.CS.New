namespace IAFahim.Graph.Bridges
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class IncrementalDynamicBridges
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Node
        {
            public int Parent;
            public int Left;
            public int Right;
            public byte Rev;
            public int Val;
            public int Sum;
            public int LazyCover;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(Node* nodes, int n)
        {
            for (int i = 0; i < n; i++)
            {
                nodes[i].Parent = -1;
                nodes[i].Left = -1;
                nodes[i].Right = -1;
                nodes[i].Rev = 0;
                nodes[i].Val = 0;
                nodes[i].Sum = 0;
                nodes[i].LazyCover = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRoot(Node* nodes, int u)
        {
            int p = nodes[u].Parent;
            return p == -1 || (nodes[p].Left != u && nodes[p].Right != u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushUp(Node* nodes, int u)
        {
            nodes[u].Sum = nodes[u].Val;
            int l = nodes[u].Left;
            int r = nodes[u].Right;
            if (l != -1) nodes[u].Sum += nodes[l].Sum;
            if (r != -1) nodes[u].Sum += nodes[r].Sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyCover(Node* nodes, int u)
        {
            if (u != -1)
            {
                nodes[u].Val = 0;
                nodes[u].Sum = 0;
                nodes[u].LazyCover = 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushDown(Node* nodes, int u)
        {
            if (nodes[u].Rev != 0)
            {
                int l = nodes[u].Left;
                int r = nodes[u].Right;
                nodes[u].Left = r;
                nodes[u].Right = l;
                if (l != -1) nodes[l].Rev ^= 1;
                if (r != -1) nodes[r].Rev ^= 1;
                nodes[u].Rev = 0;
            }
            if (nodes[u].LazyCover != 0)
            {
                ApplyCover(nodes, nodes[u].Left);
                ApplyCover(nodes, nodes[u].Right);
                nodes[u].LazyCover = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushAll(Node* nodes, int u)
        {
            if (!IsRoot(nodes, u)) PushAll(nodes, nodes[u].Parent);
            PushDown(nodes, u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Rotate(Node* nodes, int x)
        {
            int y = nodes[x].Parent;
            int z = nodes[y].Parent;
            int k = nodes[y].Left == x ? 1 : 0;
            if (!IsRoot(nodes, y))
            {
                if (nodes[z].Left == y) nodes[z].Left = x;
                else nodes[z].Right = x;
            }
            nodes[x].Parent = z;
            nodes[y].Parent = x;
            if (k != 0)
            {
                nodes[y].Left = nodes[x].Right;
                if (nodes[x].Right != -1) nodes[nodes[x].Right].Parent = y;
                nodes[x].Right = y;
            }
            else
            {
                nodes[y].Right = nodes[x].Left;
                if (nodes[x].Left != -1) nodes[nodes[x].Left].Parent = y;
                nodes[x].Left = y;
            }
            PushUp(nodes, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Splay(Node* nodes, int x)
        {
            PushAll(nodes, x);
            while (!IsRoot(nodes, x))
            {
                int y = nodes[x].Parent;
                int z = nodes[y].Parent;
                if (!IsRoot(nodes, y))
                {
                    if ((nodes[y].Left == x) ^ (nodes[z].Left == y)) Rotate(nodes, x);
                    else Rotate(nodes, y);
                }
                Rotate(nodes, x);
            }
            PushUp(nodes, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Access(Node* nodes, int u)
        {
            int v = -1;
            for (int x = u; x != -1; x = nodes[x].Parent)
            {
                Splay(nodes, x);
                nodes[x].Right = v;
                PushUp(nodes, x);
                v = x;
            }
            Splay(nodes, u);
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MakeRoot(Node* nodes, int u)
        {
            Access(nodes, u);
            nodes[u].Rev ^= 1;
            PushDown(nodes, u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindRoot(Node* nodes, int u)
        {
            Access(nodes, u);
            int x = u;
            PushDown(nodes, x);
            while (nodes[x].Left != -1)
            {
                x = nodes[x].Left;
                PushDown(nodes, x);
            }
            Splay(nodes, x);
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddEdge(Node* nodes, int u, int v, int edgeId, ref int bridgeCount)
        {
            MakeRoot(nodes, u);
            if (FindRoot(nodes, v) != u)
            {
                nodes[edgeId].Val = 1;
                nodes[edgeId].Sum = 1;
                MakeRoot(nodes, v);
                nodes[u].Parent = edgeId;
                nodes[edgeId].Parent = v;
                bridgeCount++;
            }
            else
            {
                MakeRoot(nodes, u);
                Access(nodes, v);
                Splay(nodes, v);
                bridgeCount -= nodes[v].Sum;
                ApplyCover(nodes, v);
            }
        }
    }
}
