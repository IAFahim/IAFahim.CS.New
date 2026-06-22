namespace IAFahim.Graph.Tree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LcaBuild
    {
        public static void Run(int n, int root, int* head, int* to, int* next, int* parent, int* depth, int* ancestors, int logN)
        {
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int i = 0; i < n; i++) parent[i] = -1;
            depth[root] = 0;
            q[qt++] = root;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v != parent[u])
                    {
                        parent[v] = u;
                        depth[v] = depth[u] + 1;
                        q[qt++] = v;
                    }
                }
            }
            for (int i = 0; i < n; i++) ancestors[i * logN + 0] = parent[i] < 0 ? i : parent[i];
            for (int j = 1; j < logN; j++)
            {
                for (int i = 0; i < n; i++)
                    ancestors[i * logN + j] = ancestors[ancestors[i * logN + (j - 1)] * logN + (j - 1)];
            }
        }
    }

    public static unsafe class LcaQuery
    {
        public static int Run(int u, int v, int* depth, int* ancestors, int logN)
        {
            if (depth[u] < depth[v]) { int t = u; u = v; v = t; }
            int diff = depth[u] - depth[v];
            for (int j = 0; j < logN; j++)
            {
                if (((diff >> j) & 1) != 0)
                    u = ancestors[u * logN + j];
            }
            if (u == v) return u;
            for (int j = logN - 1; j >= 0; j--)
            {
                if (ancestors[u * logN + j] != ancestors[v * logN + j])
                {
                    u = ancestors[u * logN + j];
                    v = ancestors[v * logN + j];
                }
            }
            return ancestors[u * logN + 0];
        }
    }

    public static unsafe class LcaDistance
    {
        public static int Run(int u, int v, int* depth, int* ancestors, int logN)
        {
            int lca = LcaQuery.Run(u, v, depth, ancestors, logN);
            return depth[u] + depth[v] - 2 * depth[lca];
        }
    }

    public static unsafe class BinaryLiftBuild
    {
        public static void Run(int n, int root, int* parent, int* ancestors, int logN)
        {
            for (int i = 0; i < n; i++) ancestors[i * logN + 0] = parent[i] < 0 ? i : parent[i];
            for (int j = 1; j < logN; j++)
            {
                for (int i = 0; i < n; i++)
                    ancestors[i * logN + j] = ancestors[ancestors[i * logN + (j - 1)] * logN + (j - 1)];
            }
        }
    }

    public static unsafe class BinaryLiftKthAncestor
    {
        public static int Run(int node, int k, int* ancestors, int logN)
        {
            for (int j = 0; j < logN; j++)
            {
                if (((k >> j) & 1) != 0)
                    node = ancestors[node * logN + j];
            }
            return node;
        }
    }

    public static unsafe class CentroidFind
    {
        private const int NoParent = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BfsComponent(int n, int u, int* head, int* to, int* next, bool* removed, int* parent, int* q)
        {
            for (int i = 0; i < n; i++) parent[i] = NoParent;
            int qh = 0, qt = 0;
            q[qt++] = u;
            parent[u] = u;
            int totalSize = 0;
            while (qh < qt)
            {
                int cur = q[qh++];
                totalSize++;
                for (int e = head[cur]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (!removed[v] && parent[v] == NoParent) { parent[v] = cur; q[qt++] = v; }
                }
            }
            return totalSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeSubtreeSizes(int n, int* size, int* parent, int* q, int visitedCount)
        {
            for (int i = 0; i < n; i++) size[i] = 0;
            for (int i = 0; i < visitedCount; i++) size[q[i]] = 1;
            for (int i = visitedCount - 1; i > 0; i--) size[parent[q[i]]] += size[q[i]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DescendToCentroid(int u, int threshold, int* head, int* to, int* next, bool* removed, int* parent, int* size)
        {
            int centroid = u;
            bool found = false;
            while (!found)
            {
                found = true;
                for (int e = head[centroid]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (!removed[v] && parent[v] == centroid && size[v] > threshold)
                    {
                        centroid = v;
                        found = false;
                        break;
                    }
                }
            }
            return centroid;
        }

        public static int Run(int n, int u, int* head, int* to, int* next, bool* removed, int* size)
        {
            int* parent = stackalloc int[n];
            int* q = stackalloc int[n];
            int totalSize = BfsComponent(n, u, head, to, next, removed, parent, q);
            ComputeSubtreeSizes(n, size, parent, q, totalSize);
            return DescendToCentroid(u, totalSize / 2, head, to, next, removed, parent, size);
        }
    }

    public static unsafe class CentroidDecompose
    {
        public static void Run(int n, int u, int* head, int* to, int* next, bool* removed, int* size, int* cparent)
        {
            int c = IAFahim.Graph.Tree.CentroidFind.Run(n, u, head, to, next, removed, size);
            removed[c] = true;
            for (int e = head[c]; e != 0; e = next[e])
            {
                int v = to[e];
                if (!removed[v])
                {
                    Run(n, v, head, to, next, removed, size, cparent);
                    cparent[v] = c;
                }
            }
        }
    }

    public static unsafe class TreeDfs
    {
        public static void Run(int u, int* head, int* to, int* next, int* parent, int* depth, int* size)
        {
            size[u] = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != parent[u])
                {
                    parent[v] = u;
                    depth[v] = depth[u] + 1;
                    Run(v, head, to, next, parent, depth, size);
                    size[u] += size[v];
                }
            }
        }
    }

    public static unsafe class TreeParent
    {
        public static void Run(int n, int root, int* head, int* to, int* next, int* parent)
        {
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            parent[root] = -1;
            q[qt++] = root;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v != parent[u])
                    {
                        parent[v] = u;
                        q[qt++] = v;
                    }
                }
            }
        }
    }

    public static unsafe class TreeDepth
    {
        public static void Run(int n, int root, int* head, int* to, int* next, int* depth)
        {
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            depth[root] = 0;
            q[qt++] = root;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (depth[v] < 0)
                    {
                        depth[v] = depth[u] + 1;
                        q[qt++] = v;
                    }
                }
            }
        }
    }

    public static unsafe class TreeSize
    {
        public static void Run(int n, int root, int* head, int* to, int* next, int* size)
        {
            int* parent = stackalloc int[n];
            int* order = stackalloc int[n];
            int idx = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            parent[root] = -1;
            q[qt++] = root;
            while (qh < qt)
            {
                int u = q[qh++];
                order[idx++] = u;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v != parent[u])
                    {
                        parent[v] = u;
                        q[qt++] = v;
                    }
                }
            }
            for (int i = 0; i < n; i++) size[i] = 1;
            for (int i = idx - 1; i > 0; i--)
                size[parent[order[i]]] += size[order[i]];
        }
    }

    public static unsafe class TreeDiameter
    {
        private const int Unvisited = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BfsFrom(int n, int source, int* head, int* to, int* next, int* dist, int* q, out int farthest, out int maxDist)
        {
            for (int i = 0; i < n; i++) dist[i] = Unvisited;
            int qh = 0, qt = 0;
            dist[source] = 0;
            q[qt++] = source;
            farthest = source;
            maxDist = 0;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (dist[v] == Unvisited)
                    {
                        dist[v] = dist[u] + 1;
                        q[qt++] = v;
                        if (dist[v] > maxDist) { maxDist = dist[v]; farthest = v; }
                    }
                }
            }
        }

        public static int Run(int n, int root, int* head, int* to, int* next)
        {
            int* dist = stackalloc int[n];
            int* q = stackalloc int[n];
            BfsFrom(n, root, head, to, next, dist, q, out int farthest, out _);
            BfsFrom(n, farthest, head, to, next, dist, q, out _, out int maxDist);
            return maxDist;
        }
    }

    public static unsafe class TreeCenter
    {
        private const int MaxCenters = 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeDegrees(int n, int* head, int* next, int* degree)
        {
            for (int i = 0; i < n; i++) degree[i] = 0;
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e]) degree[u]++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SeedLeaves(int n, int* degree, int* q, out int qt)
        {
            qt = 0;
            for (int i = 0; i < n; i++)
                if (degree[i] <= 1) q[qt++] = i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PeelToTwoCenters(int n, int* head, int* to, int* next, int* degree, int* q, ref int qh, ref int qt, ref int remaining)
        {
            while (remaining > MaxCenters)
            {
                int sz = qt - qh;
                for (int i = 0; i < sz; i++)
                {
                    int u = q[qh++];
                    remaining--;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        degree[v]--;
                        if (degree[v] == 1) q[qt++] = v;
                    }
                }
            }
        }

        public static int Run(int n, int* head, int* to, int* next, int* centers)
        {
            int* degree = stackalloc int[n];
            int* q = stackalloc int[n];
            int qh = 0;
            int remaining = n;
            ComputeDegrees(n, head, next, degree);
            SeedLeaves(n, degree, q, out int qt);
            PeelToTwoCenters(n, head, to, next, degree, q, ref qh, ref qt, ref remaining);
            int count = 0;
            while (qh < qt) centers[count++] = q[qh++];
            return count;
        }
    }
}