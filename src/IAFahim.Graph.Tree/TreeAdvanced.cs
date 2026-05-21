namespace IAFahim.Graph.Tree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HldBuild
    {
        public static void Run(int u, int p, int* head, int* to, int* next, int* parent, int* depth, int* heavy, int* size)
        {
            size[u] = 1;
            int maxSize = 0;
            heavy[u] = -1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;
                parent[v] = u;
                depth[v] = depth[u] + 1;
                Run(v, u, head, to, next, parent, depth, heavy, size);
                size[u] += size[v];
                if (size[v] > maxSize)
                {
                    maxSize = size[v];
                    heavy[u] = v;
                }
            }
        }

        public static void Decompose(int u, int h, int* head, int* to, int* next, int* parent, int* heavy, int* headChain, int* pos, ref int curPos)
        {
            headChain[u] = h;
            pos[u] = curPos++;
            if (heavy[u] != -1)
                Decompose(heavy[u], h, head, to, next, parent, heavy, headChain, pos, ref curPos);
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != parent[u] && v != heavy[u])
                    Decompose(v, v, head, to, next, parent, heavy, headChain, pos, ref curPos);
            }
        }
    }

    public static unsafe class HldPathQuery
    {
        public static long Run(int u, int v, long* segTree, int* headChain, int* pos, int* parent, int* depth, int n)
        {
            long result = 0;
            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]])
                {
                    int tmp = u; u = v; v = tmp;
                }
                result += SegTreeRangeQuery.Run(segTree, pos[headChain[v]], pos[v], n);
                v = parent[headChain[v]];
            }
            if (depth[u] > depth[v])
            {
                int tmp = u; u = v; v = tmp;
            }
            return result + SegTreeRangeQuery.Run(segTree, pos[u], pos[v], n);
        }
    }

    public static unsafe class HldPathUpdate
    {
        public static void Run(int u, int v, long val, long* segTree, int* headChain, int* pos, int* parent, int* depth, int n)
        {
            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]])
                {
                    int tmp = u; u = v; v = tmp;
                }
                for (int i = pos[headChain[v]]; i <= pos[v]; i++)
                    IAFahim.DS.SegmentTree.SegmentTreeSet.RunInt64(segTree, 1, 0, n - 1, i, val);
                v = parent[headChain[v]];
            }
            int l = depth[u] < depth[v] ? pos[u] : pos[v];
            int r = depth[u] < depth[v] ? pos[v] : pos[u];
            for (int i = l; i <= r; i++)
                IAFahim.DS.SegmentTree.SegmentTreeSet.RunInt64(segTree, 1, 0, n - 1, i, val);
        }
    }

    public static unsafe class HldSubtreeQuery
    {
        public static long Run(int u, int subtreeSize, long* segTree, int* pos, int n)
        {
            return SegTreeRangeQuery.Run(segTree, pos[u], pos[u] + subtreeSize - 1, n);
        }
    }

    public static unsafe class HldSubtreeUpdate
    {
        public static void Run(int u, int subtreeSize, long val, long* segTree, int* pos, int n)
        {
            for (int i = pos[u]; i < pos[u] + subtreeSize; i++)
                IAFahim.DS.SegmentTree.SegmentTreeSet.RunInt64(segTree, 1, 0, n - 1, i, val);
        }
    }

    public static unsafe class SegTreeRangeQuery
    {
        public static long Run(long* tree, int ql, int qr, int n)
        {
            return IAFahim.DS.SegmentTree.SegmentTreeQuery.RunInt64(tree, 1, 0, n - 1, ql, qr);
        }
    }

    public static unsafe class VirtualTreeBuild
    {
        public static int Run(int* nodes, int count, int* order, int* parent, int* depth, int** ancestors, int logN)
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    int w = IAFahim.Graph.Tree.LcaQuery.Run(nodes[i], nodes[j], depth, ancestors, logN);
                    order[i * count + j] = w;
                    order[j * count + i] = w;
                }
            }
            return count;
        }
    }

    public static unsafe class TreeDp
    {
        public static void Run(int u, int p, int* head, int* to, int* next, int* parent, long* dp)
        {
            dp[u] = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;
                Run(v, u, head, to, next, parent, dp);
                dp[u] += dp[v];
            }
        }
    }

    public static unsafe class RerootDp
    {
        public static void Run(int u, int p, int n, int* head, int* to, int* next, int* parent, long* dp, long* result)
        {
            long sum = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;
                Run(v, u, n, head, to, next, parent, dp, result);
                sum += dp[v];
            }
            dp[u] = sum;
            result[u] = sum;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;
                long withChild = sum - dp[v] + 1;
                result[v] = withChild;
            }
        }
    }

    public static unsafe class TreeCentroids
    {
        public static int Run(int n, int root, int* head, int* to, int* next, int* size, bool* removed)
        {
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            q[qt++] = root;
            int totalSize = 0;
            while (qh < qt)
            {
                int u = q[qh++];
                size[u] = 1;
                totalSize++;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (!removed[v])
                        q[qt++] = v;
                }
            }
            int threshold = totalSize / 2;
            int centroids = 0;
            for (int i = 0; i < qt; i++)
            {
                int u = q[i];
                bool ok = true;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (!removed[v] && size[v] > threshold)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok) centroids |= (1 << u);
            }
            return centroids;
        }
    }

    public static unsafe class EulerLcaBuild
    {
        public static int Run(int n, int root, int* head, int* to, int* next, int* euler, int* depth, int* first)
        {
            int timer = 0;
            void Dfs(int u, int p, int d)
            {
                depth[u] = d;
                first[u] = timer;
                euler[timer++] = u;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v == p) continue;
                    Dfs(v, u, d + 1);
                    euler[timer++] = u;
                }
            }
            Dfs(root, -1, 0);
            return timer;
        }
    }

    public static unsafe class RmqLcaQuery
    {
        public static int Run(int* euler, int* depth, int* first, int u, int v, int eulerSize)
        {
            int l = first[u];
            int r = first[v];
            if (l > r) { int t = l; l = r; r = t; }
            int minDepth = depth[euler[l]];
            int minNode = euler[l];
            for (int i = l + 1; i <= r; i++)
            {
                if (depth[euler[i]] < minDepth)
                {
                    minDepth = depth[euler[i]];
                    minNode = euler[i];
                }
            }
            return minNode;
        }
    }

    public static unsafe class TreeReroot
    {
        public static void Run(int u, int p, int n, int* head, int* to, int* next, long* dp, long* result)
        {
            result[u] = dp[u];
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;
                Run(v, u, n, head, to, next, dp, result);
            }
        }
    }

    public static unsafe class TreeHash
    {
        public static void Run(int u, int p, int* head, int* to, int* next, ulong* hash)
        {
            hash[u] = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;
                Run(v, u, head, to, next, hash);
                hash[u] = hash[u] * 31 + hash[v];
            }
        }
    }

    public static unsafe class RootedTreeHash
    {
        public static void Run(int root, int n, int* head, int* to, int* next, ulong* hash, ulong* dpUp)
        {
            int* order = stackalloc int[n];
            int idx = 0;
            int* stack = stackalloc int[n];
            int top = 0;
            stack[top] = root;
            int* parent = stackalloc int[n];
            for (int i = 0; i < n; i++) parent[i] = -1;
            while (top >= 0)
            {
                int u = stack[top--];
                order[idx++] = u;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v == parent[u]) continue;
                    parent[v] = u;
                    stack[++top] = v;
                }
            }
            for (int i = idx - 1; i >= 0; i--)
            {
                int u = order[i];
                hash[u] = 1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v == parent[u]) continue;
                    hash[u] = hash[u] * 31 + hash[v];
                }
            }
        }
    }

    public static unsafe class TreeIsomorphism
    {
        public static bool Run(int n1, int root1, int n2, int root2, int* head1, int* to1, int* next1, int* head2, int* to2, int* next2)
        {
            ulong* hash1 = stackalloc ulong[n1];
            ulong* hash2 = stackalloc ulong[n2];
            RunHash(root1, -1, head1, to1, next1, hash1);
            RunHash(root2, -1, head2, to2, next2, hash2);
            return hash1[root1] == hash2[root2];
        }

        private static void RunHash(int u, int p, int* head, int* to, int* next, ulong* hash)
        {
            hash[u] = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;
                RunHash(v, u, head, to, next, hash);
                hash[u] = hash[u] * 31 + hash[v];
            }
        }
    }

    public static unsafe class CartesianTreeBuild
    {
        public static void Run(int* arr, int n, int* parent, int* left, int* right)
        {
            int* stack = stackalloc int[n];
            int top = -1;
            for (int i = 0; i < n; i++)
            {
                int last = -1;
                while (top >= 0 && arr[stack[top]] < arr[i])
                {
                    last = stack[top--];
                }
                if (top >= 0)
                {
                    right[stack[top]] = i;
                    parent[i] = stack[top];
                }
                if (last != -1)
                {
                    left[i] = last;
                    parent[last] = i;
                }
                stack[++top] = i;
            }
            for (int i = 0; i < n; i++)
            {
                if (parent[i] == -1) parent[i] = 0;
            }
        }
    }
}