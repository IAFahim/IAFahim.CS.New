namespace IAFahim.Graph.TreeDecomposition
{
    using System;
    using System.Runtime.CompilerServices;

    public struct HldSegNode
    {
        public long Sum;
        public long Pref;
        public long Suff;
        public long Ans;
    }

    public static unsafe class HeavyLightDecomposition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TreePathDecompose(
            int n, int root,
            int* head, int* to, int* next,
            int* parent, int* depth, int* heavy, int* size,
            int* headChain, int* pos, ref int curPos)
        {
            Dfs1(root, -1, 0, head, to, next, parent, depth, heavy, size);
            Dfs2(root, root, head, to, next, parent, heavy, headChain, pos, ref curPos);
        }

        private static void Dfs1(
            int u, int p, int d,
            int* head, int* to, int* next,
            int* parent, int* depth, int* heavy, int* size)
        {
            parent[u] = p;
            depth[u] = d;
            size[u] = 1;
            heavy[u] = -1;
            int maxVSize = 0;

            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    Dfs1(v, u, d + 1, head, to, next, parent, depth, heavy, size);
                    size[u] += size[v];
                    if (size[v] > maxVSize)
                    {
                        maxVSize = size[v];
                        heavy[u] = v;
                    }
                }
            }
        }

        private static void Dfs2(
            int u, int h,
            int* head, int* to, int* next,
            int* parent, int* heavy, int* headChain, int* pos, ref int curPos)
        {
            headChain[u] = h;
            pos[u] = curPos++;

            if (heavy[u] != -1)
            {
                Dfs2(heavy[u], h, head, to, next, parent, heavy, headChain, pos, ref curPos);
            }

            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != parent[u] && v != heavy[u])
                {
                    Dfs2(v, v, head, to, next, parent, heavy, headChain, pos, ref curPos);
                }
            }
        }

        // --- SEGMENT TREE FOR SUM & ADD/ASSIGN ---
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildSumTree(long* tree, int node, int start, int end, long* initialValues)
        {
            if (start == end)
            {
                tree[node] = initialValues[start];
                return;
            }
            int mid = (start + end) >> 1;
            BuildSumTree(tree, node * 2, start, mid, initialValues);
            BuildSumTree(tree, node * 2 + 1, mid + 1, end, initialValues);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushDownSum(long* tree, long* lazyAdd, long* lazyAssign, byte* hasAssign, int node, int start, int end)
        {
            int mid = (start + end) >> 1;
            int lChild = node * 2;
            int rChild = node * 2 + 1;

            if (hasAssign[node] != 0)
            {
                long val = lazyAssign[node];
                tree[lChild] = val * (mid - start + 1);
                lazyAssign[lChild] = val;
                hasAssign[lChild] = 1;
                lazyAdd[lChild] = 0;

                tree[rChild] = val * (end - mid);
                lazyAssign[rChild] = val;
                hasAssign[rChild] = 1;
                lazyAdd[rChild] = 0;

                hasAssign[node] = 0;
            }

            if (lazyAdd[node] != 0)
            {
                long val = lazyAdd[node];
                tree[lChild] += val * (mid - start + 1);
                lazyAdd[lChild] += val;
                tree[rChild] += val * (end - mid);
                lazyAdd[rChild] += val;
                lazyAdd[node] = 0;
            }
        }

        public static void SumTreeAdd(
            long* tree, long* lazyAdd, long* lazyAssign, byte* hasAssign,
            int node, int start, int end, int l, int r, long val)
        {
            if (l <= start && end <= r)
            {
                tree[node] += val * (end - start + 1);
                lazyAdd[node] += val;
                return;
            }
            PushDownSum(tree, lazyAdd, lazyAssign, hasAssign, node, start, end);
            int mid = (start + end) >> 1;
            if (l <= mid)
            {
                SumTreeAdd(tree, lazyAdd, lazyAssign, hasAssign, node * 2, start, mid, l, r, val);
            }
            if (r > mid)
            {
                SumTreeAdd(tree, lazyAdd, lazyAssign, hasAssign, node * 2 + 1, mid + 1, end, l, r, val);
            }
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        public static void SumTreeAssign(
            long* tree, long* lazyAdd, long* lazyAssign, byte* hasAssign,
            int node, int start, int end, int l, int r, long val)
        {
            if (l <= start && end <= r)
            {
                tree[node] = val * (end - start + 1);
                lazyAssign[node] = val;
                hasAssign[node] = 1;
                lazyAdd[node] = 0;
                return;
            }
            PushDownSum(tree, lazyAdd, lazyAssign, hasAssign, node, start, end);
            int mid = (start + end) >> 1;
            if (l <= mid)
            {
                SumTreeAssign(tree, lazyAdd, lazyAssign, hasAssign, node * 2, start, mid, l, r, val);
            }
            if (r > mid)
            {
                SumTreeAssign(tree, lazyAdd, lazyAssign, hasAssign, node * 2 + 1, mid + 1, end, l, r, val);
            }
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        public static long SumTreeQuery(
            long* tree, long* lazyAdd, long* lazyAssign, byte* hasAssign,
            int node, int start, int end, int l, int r)
        {
            if (l <= start && end <= r)
            {
                return tree[node];
            }
            PushDownSum(tree, lazyAdd, lazyAssign, hasAssign, node, start, end);
            int mid = (start + end) >> 1;
            long sum = 0;
            if (l <= mid)
            {
                sum += SumTreeQuery(tree, lazyAdd, lazyAssign, hasAssign, node * 2, start, mid, l, r);
            }
            if (r > mid)
            {
                sum += SumTreeQuery(tree, lazyAdd, lazyAssign, hasAssign, node * 2 + 1, mid + 1, end, l, r);
            }
            return sum;
        }

        // --- SEGMENT TREE FOR MAX SUBARRAY ---
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HldSegNode MergeMaxSubarray(HldSegNode l, HldSegNode r)
        {
            HldSegNode res;
            res.Sum = l.Sum + r.Sum;
            res.Pref = Math.Max(l.Pref, l.Sum + r.Pref);
            res.Suff = Math.Max(r.Suff, r.Sum + l.Suff);
            res.Ans = Math.Max(Math.Max(l.Ans, r.Ans), l.Suff + r.Pref);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildMaxSubarrayTree(HldSegNode* tree, int node, int start, int end, long* initialValues)
        {
            if (start == end)
            {
                long val = initialValues[start];
                tree[node].Sum = val;
                tree[node].Pref = val;
                tree[node].Suff = val;
                tree[node].Ans = val;
                return;
            }
            int mid = (start + end) >> 1;
            BuildMaxSubarrayTree(tree, node * 2, start, mid, initialValues);
            BuildMaxSubarrayTree(tree, node * 2 + 1, mid + 1, end, initialValues);
            tree[node] = MergeMaxSubarray(tree[node * 2], tree[node * 2 + 1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyAssignMaxSubarray(HldSegNode* tree, long* lazyAssign, byte* hasAssign, int node, int start, int end, long val)
        {
            int len = end - start + 1;
            tree[node].Sum = val * len;
            long valTerm = val >= 0 ? val * len : val;
            tree[node].Pref = valTerm;
            tree[node].Suff = valTerm;
            tree[node].Ans = valTerm;
            lazyAssign[node] = val;
            hasAssign[node] = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushDownMaxSubarray(HldSegNode* tree, long* lazyAssign, byte* hasAssign, int node, int start, int end)
        {
            if (hasAssign[node] != 0)
            {
                int mid = (start + end) >> 1;
                ApplyAssignMaxSubarray(tree, lazyAssign, hasAssign, node * 2, start, mid, lazyAssign[node]);
                ApplyAssignMaxSubarray(tree, lazyAssign, hasAssign, node * 2 + 1, mid + 1, end, lazyAssign[node]);
                hasAssign[node] = 0;
            }
        }

        public static void MaxSubarrayTreeAssign(
            HldSegNode* tree, long* lazyAssign, byte* hasAssign,
            int node, int start, int end, int l, int r, long val)
        {
            if (l <= start && end <= r)
            {
                ApplyAssignMaxSubarray(tree, lazyAssign, hasAssign, node, start, end, val);
                return;
            }
            PushDownMaxSubarray(tree, lazyAssign, hasAssign, node, start, end);
            int mid = (start + end) >> 1;
            if (l <= mid)
            {
                MaxSubarrayTreeAssign(tree, lazyAssign, hasAssign, node * 2, start, mid, l, r, val);
            }
            if (r > mid)
            {
                MaxSubarrayTreeAssign(tree, lazyAssign, hasAssign, node * 2 + 1, mid + 1, end, l, r, val);
            }
            tree[node] = MergeMaxSubarray(tree[node * 2], tree[node * 2 + 1]);
        }

        public static HldSegNode MaxSubarrayTreeQuery(
            HldSegNode* tree, long* lazyAssign, byte* hasAssign,
            int node, int start, int end, int l, int r)
        {
            if (l <= start && end <= r)
            {
                return tree[node];
            }
            PushDownMaxSubarray(tree, lazyAssign, hasAssign, node, start, end);
            int mid = (start + end) >> 1;
            if (r <= mid)
            {
                return MaxSubarrayTreeQuery(tree, lazyAssign, hasAssign, node * 2, start, mid, l, r);
            }
            if (l > mid)
            {
                return MaxSubarrayTreeQuery(tree, lazyAssign, hasAssign, node * 2 + 1, mid + 1, end, l, r);
            }
            HldSegNode leftRes = MaxSubarrayTreeQuery(tree, lazyAssign, hasAssign, node * 2, start, mid, l, r);
            HldSegNode rightRes = MaxSubarrayTreeQuery(tree, lazyAssign, hasAssign, node * 2 + 1, mid + 1, end, l, r);
            return MergeMaxSubarray(leftRes, rightRes);
        }

        // --- PATH OPERATIONS VIA HLD ---
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PathAdd(
            int u, int v, long val,
            long* tree, long* lazyAdd, long* lazyAssign, byte* hasAssign,
            int* headChain, int* pos, int* parent, int* depth, int n)
        {
            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]])
                {
                    int tmp = u; u = v; v = tmp;
                }
                SumTreeAdd(tree, lazyAdd, lazyAssign, hasAssign, 1, 0, n - 1, pos[headChain[v]], pos[v], val);
                v = parent[headChain[v]];
            }
            int l = depth[u] < depth[v] ? pos[u] : pos[v];
            int r = depth[u] < depth[v] ? pos[v] : pos[u];
            SumTreeAdd(tree, lazyAdd, lazyAssign, hasAssign, 1, 0, n - 1, l, r, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PathAssign(
            int u, int v, long val,
            long* tree, long* lazyAdd, long* lazyAssign, byte* hasAssign,
            int* headChain, int* pos, int* parent, int* depth, int n)
        {
            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]])
                {
                    int tmp = u; u = v; v = tmp;
                }
                SumTreeAssign(tree, lazyAdd, lazyAssign, hasAssign, 1, 0, n - 1, pos[headChain[v]], pos[v], val);
                v = parent[headChain[v]];
            }
            int l = depth[u] < depth[v] ? pos[u] : pos[v];
            int r = depth[u] < depth[v] ? pos[v] : pos[u];
            SumTreeAssign(tree, lazyAdd, lazyAssign, hasAssign, 1, 0, n - 1, l, r, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PathSumQuery(
            int u, int v,
            long* tree, long* lazyAdd, long* lazyAssign, byte* hasAssign,
            int* headChain, int* pos, int* parent, int* depth, int n)
        {
            long sum = 0;
            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]])
                {
                    int tmp = u; u = v; v = tmp;
                }
                sum += SumTreeQuery(tree, lazyAdd, lazyAssign, hasAssign, 1, 0, n - 1, pos[headChain[v]], pos[v]);
                v = parent[headChain[v]];
            }
            int l = depth[u] < depth[v] ? pos[u] : pos[v];
            int r = depth[u] < depth[v] ? pos[v] : pos[u];
            sum += SumTreeQuery(tree, lazyAdd, lazyAssign, hasAssign, 1, 0, n - 1, l, r);
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HldSegNode PathMaxSubarray(
            int u, int v,
            HldSegNode* tree, long* lazyAssign, byte* hasAssign,
            int* headChain, int* pos, int* parent, int* depth, int n)
        {
            HldSegNode ansU;
            ansU.Sum = 0; ansU.Pref = long.MinValue / 2; ansU.Suff = long.MinValue / 2; ansU.Ans = long.MinValue / 2;
            bool hasU = false;

            HldSegNode ansV;
            ansV.Sum = 0; ansV.Pref = long.MinValue / 2; ansV.Suff = long.MinValue / 2; ansV.Ans = long.MinValue / 2;
            bool hasV = false;

            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]])
                {
                    HldSegNode segment = MaxSubarrayTreeQuery(tree, lazyAssign, hasAssign, 1, 0, n - 1, pos[headChain[u]], pos[u]);
                    long t = segment.Pref; segment.Pref = segment.Suff; segment.Suff = t; // Swap prefix and suffix
                    if (!hasU)
                    {
                        ansU = segment;
                        hasU = true;
                    }
                    else
                    {
                        ansU = MergeMaxSubarray(ansU, segment);
                    }
                    u = parent[headChain[u]];
                }
                else
                {
                    HldSegNode segment = MaxSubarrayTreeQuery(tree, lazyAssign, hasAssign, 1, 0, n - 1, pos[headChain[v]], pos[v]);
                    if (!hasV)
                    {
                        ansV = segment;
                        hasV = true;
                    }
                    else
                    {
                        ansV = MergeMaxSubarray(segment, ansV);
                    }
                    v = parent[headChain[v]];
                }
            }

            if (depth[u] > depth[v])
            {
                HldSegNode segment = MaxSubarrayTreeQuery(tree, lazyAssign, hasAssign, 1, 0, n - 1, pos[v], pos[u]);
                long t = segment.Pref; segment.Pref = segment.Suff; segment.Suff = t;
                if (!hasU)
                {
                    ansU = segment;
                    hasU = true;
                }
                else
                {
                    ansU = MergeMaxSubarray(ansU, segment);
                }
            }
            else
            {
                HldSegNode segment = MaxSubarrayTreeQuery(tree, lazyAssign, hasAssign, 1, 0, n - 1, pos[u], pos[v]);
                if (!hasV)
                {
                    ansV = segment;
                    hasV = true;
                }
                else
                {
                    ansV = MergeMaxSubarray(segment, ansV);
                }
            }

            if (!hasU)
            {
                return ansV;
            }
            if (!hasV)
            {
                return ansU;
            }
            return MergeMaxSubarray(ansU, ansV);
        }
    }
}
