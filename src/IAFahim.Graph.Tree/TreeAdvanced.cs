namespace IAFahim.Graph.Tree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HldBuild
    {
        public static void Run(int u, int p, int* head, int* to, int* next, int* parent, int* depth, int* heavy, int* size)
        {
            size[u] = 1; int maxV = 0; heavy[u] = -1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e]; if (v == p) continue;
                parent[v] = u; depth[v] = depth[u] + 1;
                Run(v, u, head, to, next, parent, depth, heavy, size);
                size[u] += size[v]; if (size[v] > maxV) { maxV = size[v]; heavy[u] = v; }
            }
        }
        public static void Decompose(int u, int h, int* head, int* to, int* next, int* parent, int* heavy, int* headChain, int* pos, ref int curPos)
        {
            headChain[u] = h; pos[u] = curPos++;
            if (heavy[u] != -1) Decompose(heavy[u], h, head, to, next, parent, heavy, headChain, pos, ref curPos);
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e]; if (v != parent[u] && v != heavy[u]) Decompose(v, v, head, to, next, parent, heavy, headChain, pos, ref curPos);
            }
        }
    }

    public static unsafe class HldPathQuery
    {
        public static long Run(int u, int v, long* segTree, int* headChain, int* pos, int* parent, int* depth, int n)
        {
            long res = 0;
            while (headChain[u] != headChain[v])
            {
                if (depth[headChain[u]] > depth[headChain[v]]) { int t = u; u = v; v = t; }
                res += IAFahim.DS.SegmentTree.SegmentTreeQuery.RunInt64(segTree, 1, 0, n - 1, pos[headChain[v]], pos[v]);
                v = parent[headChain[v]];
            }
            if (depth[u] > depth[v]) { int t = u; u = v; v = t; }
            return res + IAFahim.DS.SegmentTree.SegmentTreeQuery.RunInt64(segTree, 1, 0, n - 1, pos[u], pos[v]);
        }
    }

    public static unsafe class TreeCentroids
    {
        public static int Run(int n, int root, int* head, int* to, int* next, int* size, bool* removed)
        {
            int* q = stackalloc int[n]; int qh = 0, qt = 0; q[qt++] = root; int total = 0;
            while (qh < qt) { int u = q[qh++]; size[u] = 1; total++; for (int e = head[u]; e != 0; e = next[e]) if (!removed[to[e]]) q[qt++] = to[e]; }
            int centroids = 0;
            for (int i = 0; i < qt; i++)
            {
                int u = q[i]; bool ok = true;
                for (int e = head[u]; e != 0; e = next[e]) if (!removed[to[e]] && size[to[e]] > total / 2) { ok = false; break; }
                if (ok && (total - size[u]) <= total / 2) centroids |= (1 << u);
            }
            return centroids;
        }
    }

    public static unsafe class RootedTreeHash
    {
        public static void Run(int root, int n, int* head, int* to, int* next, ulong* hash, ulong* dpUp)
        {
            int* order = stackalloc int[n]; int idx = 0;
            int* stack = stackalloc int[n]; int top = 0; stack[top] = root;
            int* parent = stackalloc int[n]; for (int i = 0; i < n; i++) parent[i] = -1;
            while (top >= 0)
            {
                int u = stack[top--]; order[idx++] = u;
                for (int e = head[u]; e != 0; e = next[e]) { int v = to[e]; if (v != parent[u]) { parent[v] = u; stack[++top] = v; } }
            }
            for (int i = idx - 1; i >= 0; i--)
            {
                int u = order[i]; hash[u] = 1;
                for (int e = head[u]; e != 0; e = next[e]) { int v = to[e]; if (v != parent[u]) hash[u] = hash[u] * 31 + hash[v]; }
            }
        }
    }

    public static unsafe class CartesianTreeBuild
    {
        public static void Run(int* arr, int n, int* parent, int* left, int* right)
        {
            int* stack = stackalloc int[n]; int top = -1;
            for (int i = 0; i < n; i++)
            {
                int last = -1; while (top >= 0 && arr[stack[top]] < arr[i]) last = stack[top--];
                if (top >= 0) { right[stack[top]] = i; parent[i] = stack[top]; }
                if (last != -1) { left[i] = last; parent[last] = i; }
                stack[++top] = i;
            }
            for (int i = 0; i < n; i++) if (parent[i] == -1) parent[i] = 0;
        }
    }
}
