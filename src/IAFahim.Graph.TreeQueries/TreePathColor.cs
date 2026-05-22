namespace IAFahim.Graph.TreeQueries
{
    using System;
    using System.Runtime.CompilerServices;

    public struct PathColorNode
    {
        public int SegCount;
        public int LeftColor;
        public int RightColor;
        public int LazyColor;
    }

    public static unsafe class TreePathColor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PathColorNode Merge(PathColorNode left, PathColorNode right)
        {
            if (left.SegCount == 0) return right;
            if (right.SegCount == 0) return left;
            
            PathColorNode res;
            res.SegCount = left.SegCount + right.SegCount;
            if (left.RightColor == right.LeftColor && left.RightColor != 0)
            {
                res.SegCount--;
            }
            res.LeftColor = left.LeftColor;
            res.RightColor = right.RightColor;
            res.LazyColor = 0;
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Apply(PathColorNode* tree, int node, int color)
        {
            if (color == 0) return;
            tree[node].SegCount = 1;
            tree[node].LeftColor = color;
            tree[node].RightColor = color;
            tree[node].LazyColor = color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Push(PathColorNode* tree, int node)
        {
            int lc = tree[node].LazyColor;
            if (lc != 0)
            {
                Apply(tree, node * 2, lc);
                Apply(tree, node * 2 + 1, lc);
                tree[node].LazyColor = 0;
            }
        }

        public static void Build(PathColorNode* tree, int node, int start, int end, int* initialColors)
        {
            if (start == end)
            {
                tree[node].SegCount = 1;
                tree[node].LeftColor = initialColors[start];
                tree[node].RightColor = initialColors[start];
                tree[node].LazyColor = 0;
                return;
            }
            int mid = start + (end - start) / 2;
            Build(tree, node * 2, start, mid, initialColors);
            Build(tree, node * 2 + 1, mid + 1, end, initialColors);
            tree[node] = Merge(tree[node * 2], tree[node * 2 + 1]);
        }

        public static void Update(PathColorNode* tree, int node, int start, int end, int l, int r, int color)
        {
            if (l <= start && end <= r)
            {
                Apply(tree, node, color);
                return;
            }
            Push(tree, node);
            int mid = start + (end - start) / 2;
            if (l <= mid) Update(tree, node * 2, start, mid, l, r, color);
            if (r > mid) Update(tree, node * 2 + 1, mid + 1, end, l, r, color);
            tree[node] = Merge(tree[node * 2], tree[node * 2 + 1]);
        }

        public static PathColorNode Query(PathColorNode* tree, int node, int start, int end, int l, int r)
        {
            if (l <= start && end <= r)
            {
                return tree[node];
            }
            Push(tree, node);
            int mid = start + (end - start) / 2;
            if (r <= mid) return Query(tree, node * 2, start, mid, l, r);
            if (l > mid) return Query(tree, node * 2 + 1, mid + 1, end, l, r);
            return Merge(
                Query(tree, node * 2, start, mid, l, r),
                Query(tree, node * 2 + 1, mid + 1, end, l, r)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PathColorUpdate(
            int u, int v, int color,
            PathColorNode* tree, int* headChain, int* pos, int* parent, int* depth, int n)
        {
            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]])
                {
                    Update(tree, 1, 0, n - 1, pos[headChain[u]], pos[u], color);
                    u = parent[headChain[u]];
                }
                else
                {
                    Update(tree, 1, 0, n - 1, pos[headChain[v]], pos[v], color);
                    v = parent[headChain[v]];
                }
            }
            if (depth[u] > depth[v])
            {
                Update(tree, 1, 0, n - 1, pos[v], pos[u], color);
            }
            else
            {
                Update(tree, 1, 0, n - 1, pos[u], pos[v], color);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PathColorQueryCount(
            int u, int v,
            PathColorNode* tree, int* headChain, int* pos, int* parent, int* depth, int n)
        {
            PathColorNode leftRes = default;
            PathColorNode rightRes = default;

            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]])
                {
                    PathColorNode q = Query(tree, 1, 0, n - 1, pos[headChain[u]], pos[u]);
                    PathColorNode swapQ = default;
                    swapQ.SegCount = q.SegCount;
                    swapQ.LeftColor = q.RightColor;
                    swapQ.RightColor = q.LeftColor;
                    leftRes = Merge(leftRes, swapQ);
                    u = parent[headChain[u]];
                }
                else
                {
                    PathColorNode q = Query(tree, 1, 0, n - 1, pos[headChain[v]], pos[v]);
                    rightRes = Merge(q, rightRes);
                    v = parent[headChain[v]];
                }
            }
            
            if (depth[u] > depth[v])
            {
                PathColorNode q = Query(tree, 1, 0, n - 1, pos[v], pos[u]);
                PathColorNode swapQ = default;
                swapQ.SegCount = q.SegCount;
                swapQ.LeftColor = q.RightColor;
                swapQ.RightColor = q.LeftColor;
                leftRes = Merge(leftRes, swapQ);
            }
            else
            {
                PathColorNode q = Query(tree, 1, 0, n - 1, pos[u], pos[v]);
                rightRes = Merge(q, rightRes);
            }
            
            PathColorNode finalRes = Merge(leftRes, rightRes);
            return finalRes.SegCount;
        }
    }
}
