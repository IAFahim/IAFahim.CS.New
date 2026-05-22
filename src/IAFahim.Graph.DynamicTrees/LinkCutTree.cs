namespace IAFahim.Graph.DynamicTrees
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LctNode
    {
        public int Parent;
        public int Left;
        public int Right;
        public byte Rev;
        public long Val;
        public long LazyAdd;
        public long PathMin;
        public long PathMax;
        public long PathSum;
        public int PathSize;
        public long VirSum;
        public long AllSum;
    }

    public static unsafe class LinkCutTree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(LctNode* nodes, int n)
        {
            for (int i = 0; i < n; i++)
            {
                nodes[i].Parent = -1;
                nodes[i].Left = -1;
                nodes[i].Right = -1;
                nodes[i].Rev = 0;
                nodes[i].Val = 0;
                nodes[i].LazyAdd = 0;
                nodes[i].PathMin = 0;
                nodes[i].PathMax = 0;
                nodes[i].PathSum = 0;
                nodes[i].PathSize = 1;
                nodes[i].VirSum = 0;
                nodes[i].AllSum = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRoot(LctNode* nodes, int u)
        {
            int p = nodes[u].Parent;
            if (p == -1)
            {
                return true;
            }
            return nodes[p].Left != u && nodes[p].Right != u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PushUp(LctNode* nodes, int u)
        {
            int l = nodes[u].Left;
            int r = nodes[u].Right;
            
            nodes[u].PathSize = 1;
            nodes[u].PathMin = nodes[u].Val;
            nodes[u].PathMax = nodes[u].Val;
            nodes[u].PathSum = nodes[u].Val;

            if (l != -1)
            {
                nodes[u].PathSize += nodes[l].PathSize;
                if (nodes[l].PathMin < nodes[u].PathMin)
                {
                    nodes[u].PathMin = nodes[l].PathMin;
                }
                if (nodes[l].PathMax > nodes[u].PathMax)
                {
                    nodes[u].PathMax = nodes[l].PathMax;
                }
                nodes[u].PathSum += nodes[l].PathSum;
            }

            if (r != -1)
            {
                nodes[u].PathSize += nodes[r].PathSize;
                if (nodes[r].PathMin < nodes[u].PathMin)
                {
                    nodes[u].PathMin = nodes[r].PathMin;
                }
                if (nodes[r].PathMax > nodes[u].PathMax)
                {
                    nodes[u].PathMax = nodes[r].PathMax;
                }
                nodes[u].PathSum += nodes[r].PathSum;
            }

            nodes[u].AllSum = nodes[u].PathSum + nodes[u].VirSum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ApplyAdd(LctNode* nodes, int u, long val)
        {
            if (u == -1)
            {
                return;
            }
            nodes[u].Val += val;
            nodes[u].PathMin += val;
            nodes[u].PathMax += val;
            nodes[u].PathSum += val * nodes[u].PathSize;
            nodes[u].LazyAdd += val;
            nodes[u].AllSum = nodes[u].PathSum + nodes[u].VirSum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PushDown(LctNode* nodes, int u)
        {
            if (nodes[u].LazyAdd != 0)
            {
                long val = nodes[u].LazyAdd;
                ApplyAdd(nodes, nodes[u].Left, val);
                ApplyAdd(nodes, nodes[u].Right, val);
                nodes[u].LazyAdd = 0;
            }
            if (nodes[u].Rev != 0)
            {
                int l = nodes[u].Left;
                int r = nodes[u].Right;
                nodes[u].Left = r;
                nodes[u].Right = l;
                if (r != -1)
                {
                    nodes[r].Rev ^= 1;
                }
                if (l != -1)
                {
                    nodes[l].Rev ^= 1;
                }
                nodes[u].Rev = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushAll(LctNode* nodes, int u)
        {
            if (!IsRoot(nodes, u))
            {
                PushAll(nodes, nodes[u].Parent);
            }
            PushDown(nodes, u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(LctNode* nodes, int x)
        {
            int y = nodes[x].Parent;
            int z = nodes[y].Parent;
            int k = nodes[y].Left == x ? 1 : 0;

            if (!IsRoot(nodes, y))
            {
                if (nodes[z].Left == y)
                {
                    nodes[z].Left = x;
                }
                else
                {
                    nodes[z].Right = x;
                }
            }

            if (k != 0)
            {
                nodes[y].Left = nodes[x].Right;
                if (nodes[x].Right != -1)
                {
                    nodes[nodes[x].Right].Parent = y;
                }
                nodes[x].Right = y;
            }
            else
            {
                nodes[y].Right = nodes[x].Left;
                if (nodes[x].Left != -1)
                {
                    nodes[nodes[x].Left].Parent = y;
                }
                nodes[x].Left = y;
            }

            nodes[y].Parent = x;
            nodes[x].Parent = z;
            PushUp(nodes, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Splay(LctNode* nodes, int x)
        {
            PushAll(nodes, x);
            while (!IsRoot(nodes, x))
            {
                int y = nodes[x].Parent;
                int z = nodes[y].Parent;
                if (!IsRoot(nodes, y))
                {
                    if ((nodes[y].Left == x) ^ (nodes[z].Left == y))
                    {
                        Rotate(nodes, x);
                    }
                    else
                    {
                        Rotate(nodes, y);
                    }
                }
                Rotate(nodes, x);
            }
            PushUp(nodes, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Access(LctNode* nodes, int u)
        {
            int v = -1;
            for (int x = u; x != -1; x = nodes[x].Parent)
            {
                Splay(nodes, x);
                if (nodes[x].Right != -1)
                {
                    nodes[x].VirSum += nodes[nodes[x].Right].AllSum;
                }
                if (v != -1)
                {
                    nodes[x].VirSum -= nodes[v].AllSum;
                }
                nodes[x].Right = v;
                PushUp(nodes, x);
                v = x;
            }
            Splay(nodes, u);
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MakeRoot(LctNode* nodes, int u)
        {
            Access(nodes, u);
            nodes[u].Rev ^= 1;
            PushDown(nodes, u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindRoot(LctNode* nodes, int u)
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
        public static void Link(LctNode* nodes, int u, int v)
        {
            MakeRoot(nodes, u);
            if (FindRoot(nodes, v) == u)
            {
                return;
            }
            Access(nodes, v);
            Splay(nodes, v);
            nodes[u].Parent = v;
            nodes[v].VirSum += nodes[u].AllSum;
            PushUp(nodes, v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cut(LctNode* nodes, int u, int v)
        {
            MakeRoot(nodes, u);
            Access(nodes, v);
            Splay(nodes, v);
            if (nodes[v].Left == u && nodes[u].Right == -1)
            {
                nodes[v].Left = -1;
                nodes[u].Parent = -1;
                PushUp(nodes, v);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PathAdd(LctNode* nodes, int u, int v, long val)
        {
            MakeRoot(nodes, u);
            Access(nodes, v);
            Splay(nodes, v);
            ApplyAdd(nodes, v, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PathMin(LctNode* nodes, int u, int v)
        {
            MakeRoot(nodes, u);
            Access(nodes, v);
            Splay(nodes, v);
            return nodes[v].PathMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PathMax(LctNode* nodes, int u, int v)
        {
            MakeRoot(nodes, u);
            Access(nodes, v);
            Splay(nodes, v);
            return nodes[v].PathMax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SubtreeQuery(LctNode* nodes, int u)
        {
            Access(nodes, u);
            return nodes[u].Val + nodes[u].VirSum;
        }
    }
}
