namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ChuLiuEdmonds
    {
        public static long Run(int n, int root, int* u, int* v, long* w, int m, long* result)
        {
            if (n <= 1) return 0;
            long totalWeight = 0;
            int* inEdge = stackalloc int[n];
            for (int i = 0; i < n; i++) inEdge[i] = -1;
            for (int i = 0; i < m; i++)
            {
                if (v[i] != root && (inEdge[v[i]] == -1 || w[i] < w[inEdge[v[i]]]))
                    inEdge[v[i]] = i;
            }
            for (int i = 0; i < n; i++)
            {
                if (i != root && inEdge[i] == -1) return -1;
            }
            int* pre = stackalloc int[n];
            int* id = stackalloc int[n];
            int* vis = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                totalWeight += (inEdge[i] >= 0) ? w[inEdge[i]] : 0;
                pre[i] = root;
                id[i] = 0;
                vis[i] = -1;
            }
            int b = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == root) continue;
                result[i] = u[inEdge[i]];
                int vtx = i;
                vis[vtx] = i;
                while (vis[pre[vtx]] != i && pre[vtx] != root && id[pre[vtx]] == 0)
                {
                    vtx = pre[vtx];
                    vis[vtx] = i;
                }
                if (pre[vtx] != root && id[pre[vtx]] == 0 && vis[pre[vtx]] == i)
                {
                    id[vtx] = ++b;
                    int u2 = pre[vtx];
                    while (u2 != vtx)
                    {
                        id[u2] = b;
                        u2 = pre[u2];
                    }
                }
            }
            if (b == 0) return totalWeight;
            for (int i = 0; i < n; i++)
                if (id[i] == 0) id[i] = ++b;
            int* newU = stackalloc int[m + n];
            int* newV = stackalloc int[m + n];
            long* newW = stackalloc long[m + n];
            int newM = 0;
            for (int i = 0; i < m; i++)
            {
                int uu = id[u[i]];
                int vv = id[v[i]];
                long ww = w[i];
                if (uu != vv)
                {
                    newU[newM] = uu;
                    newV[newM] = vv;
                    newW[newM] = ww - (inEdge[v[i]] >= 0 ? w[inEdge[v[i]]] : 0);
                    newM++;
                }
            }
            long subRes = Run(b, id[root], newU, newV, newW, newM, result);
            for (int i = 1; i < n; i++)
            {
                if (id[i] == id[pre[i]] || i == root) continue;
                int pi = inEdge[i];
                int uu = u[pi], vv = v[pi];
                int j = 0;
                while (j < n && (id[j] != id[vv] || j == vv))
                    for (int k = j + 1; k < n; k++)
                        if (id[k] == id[uu]) break;
                break;
            }
            return totalWeight + subRes;
        }
    }

    public static unsafe class Boruvka
    {
        public static long Run(int n, int* u, int* v, long* w, int m, int* mstEdges, int* used, int* edgeCount)
        {
            for (int i = 0; i < n; i++) used[i] = 0;
            long totalWeight = 0;
            *edgeCount = 0;
            int* comp = stackalloc int[n];
            for (int i = 0; i < n; i++) comp[i] = i;
            int* bestU = stackalloc int[n];
            int* bestV = stackalloc int[n];
            long* bestW = stackalloc long[n];
            for (int i = 0; i < n; i++) { bestU[i] = -1; bestV[i] = -1; bestW[i] = long.MaxValue; }
            bool changed = true;
            int iterations = 0;
            while (changed && iterations < n)
            {
                changed = false;
                for (int i = 0; i < n; i++)
                {
                    bestU[i] = -1; bestV[i] = -1; bestW[i] = long.MaxValue;
                }
                for (int i = 0; i < m; i++)
                {
                    int cu = comp[u[i]];
                    int cv = comp[v[i]];
                    if (cu != cv && w[i] < bestW[cu])
                    {
                        bestU[cu] = u[i];
                        bestV[cu] = v[i];
                        bestW[cu] = w[i];
                    }
                    if (cu != cv && w[i] < bestW[cv])
                    {
                        bestU[cv] = u[i];
                        bestV[cv] = v[i];
                        bestW[cv] = w[i];
                    }
                }
                for (int i = 0; i < n; i++)
                {
                    if (bestU[i] >= 0 && comp[bestU[i]] != comp[bestV[i]])
                    {
                        mstEdges[(*edgeCount)++] = bestU[i];
                        int cu = comp[bestU[i]];
                        int cv = comp[bestV[i]];
                        for (int j = 0; j < n; j++)
                            if (comp[j] == cu) comp[j] = cv;
                        totalWeight += bestW[i];
                        changed = true;
                    }
                }
                iterations++;
            }
            return totalWeight;
        }
    }

    public static unsafe class KruskalReconstructionTree
    {
        public static int Build(int n, int* u, int* v, int m, int* parent, int* left, int* right, int* label)
        {
            int nodeCount = n;
            for (int i = 1; i < n; i++)
            {
                left[i] = u[i - 1];
                right[i] = v[i - 1];
                parent[left[i]] = n + i - 1;
                parent[right[i]] = n + i - 1;
                label[n + i - 1] = Math.Min(label[left[i]], label[right[i]]);
                nodeCount++;
            }
            return nodeCount;
        }
    }

    public static unsafe class AStar
    {
        public static long Run(int n, int start, int target, int* head, int* to, int* next, long* dist, long* h, long* result)
        {
            for (int i = 0; i < n; i++) result[i] = long.MaxValue;
            result[start] = 0;
            var pq = new System.Collections.Generic.SortedSet<(long f, int v)>();
            pq.Add((h[start], start));
            while (pq.Count > 0)
            {
                var cur = pq.Min;
                pq.Remove(cur);
                if (cur.v == target) break;
                for (int e = head[cur.v]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long g = result[cur.v] + dist[e];
                    long f = g + h[v];
                    if (g < result[v])
                    {
                        result[v] = g;
                        pq.Add((f, v));
                    }
                }
            }
            return result[target];
        }
    }

    public static unsafe class YenKShortestPaths
    {
        public static int Run(int n, int src, int dst, int k, int* head, int* to, int* next, long* dist, long* pathCosts, long* work)
        {
            int count = 0;
            long* distTo = stackalloc long[n];
            for (int i = 0; i < n; i++) distTo[i] = long.MaxValue;
            distTo[src] = 0;
            var pq = new System.Collections.Generic.SortedSet<(long d, int v)>();
            pq.Add((0, src));
            while (pq.Count > 0)
            {
                var cur = pq.Min;
                pq.Remove(cur);
                if (cur.d > distTo[cur.v]) continue;
                for (int e = head[cur.v]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long nd = cur.d + dist[e];
                    if (nd < distTo[v])
                    {
                        distTo[v] = nd;
                        pq.Add((nd, v));
                    }
                }
            }
            if (distTo[dst] == long.MaxValue) return 0;
            pathCosts[count++] = distTo[dst];
            return count;
        }
    }

    public static unsafe class BiconnectedComponents
    {
        public static int Run(int n, int* head, int* to, int* next, int* disc, int* low, int* bccId, int* stackEdges, int* bccCount)
        {
            int timer = 0;
            int top = 0;
            int count = 0;
            for (int i = 0; i < n; i++) { disc[i] = 0; low[i] = 0; bccId[i] = -1; }
            void Dfs(int u, int parent)
            {
                disc[u] = low[u] = ++timer;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (disc[v] == 0)
                    {
                        stackEdges[top++] = e;
                        Dfs(v, u);
                        low[u] = Math.Min(low[u], low[v]);
                        if (low[v] >= disc[u])
                        {
                            count++;
                            while (stackEdges[--top] != e) { }
                        }
                    }
                    else if (parent != v && disc[v] < disc[u])
                    {
                        stackEdges[top++] = e;
                        low[u] = Math.Min(low[u], disc[v]);
                    }
                }
            }
            for (int i = 0; i < n; i++)
                if (disc[i] == 0) Dfs(i, -1);
            *bccCount = count;
            return count;
        }
    }

    public static unsafe class EdgeBiconnectedComponents
    {
        public static int Run(int n, int* head, int* to, int* next, int* disc, int* low, int* parent, int* ebcId)
        {
            int timer = 0;
            int count = 0;
            for (int i = 0; i < n; i++) { disc[i] = 0; low[i] = 0; parent[i] = -1; ebcId[i] = -1; }
            void Dfs(int u)
            {
                disc[u] = low[u] = ++timer;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (disc[v] == 0)
                    {
                        parent[v] = u;
                        Dfs(v);
                        low[u] = Math.Min(low[u], low[v]);
                        if (low[v] > disc[u])
                            ebcId[e] = ebcId[e ^ 1] = count++;
                    }
                    else if (v != parent[u] && disc[v] < disc[u])
                    {
                        low[u] = Math.Min(low[u], disc[v]);
                        ebcId[e] = ebcId[e ^ 1] = count++;
                    }
                }
            }
            for (int i = 0; i < n; i++)
                if (disc[i] == 0) Dfs(i);
            return count;
        }
    }

    public static unsafe class DominatorTree
    {
        public static void Run(int n, int root, int* head, int* to, int* next, int* parent, int* semi, int* idom, int* ancestor, int* label, int* bucket, int* parentNode)
        {
            for (int i = 0; i < n; i++)
            {
                semi[i] = i;
                idom[i] = -1;
                ancestor[i] = -1;
                label[i] = i;
                bucket[i] = -1;
                parentNode[i] = -1;
            }
            int* vertex = stackalloc int[n];
            int* arr = stackalloc int[n];
            int time = 0;
            void Dfs(int u)
            {
                arr[u] = ++time;
                vertex[time] = u;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (arr[v] == 0)
                    {
                        parentNode[v] = u;
                        Dfs(v);
                    }
                }
            }
            Dfs(root);
            for (int i = time; i >= 1; i--)
            {
                int w = vertex[i];
                for (int e = head[w]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (arr[v] == 0) continue;
                    int u = Eval(v, ancestor, label, semi);
                    if (semi[u] < semi[w]) semi[w] = semi[u];
                }
                if (w > 1) AddToBucket(semi[w], w, bucket);
                Link(parentNode[w], w, ancestor, semi);
            }
            for (int i = 2; i <= time; i++)
            {
                int w = vertex[i];
                int u = idom[w];
                while (u != -1 && bucket[u] != -1)
                {
                    Link(idom[u], w, ancestor, label);
                    u = idom[u];
                }
            }
        }

        private static int Eval(int v, int* ancestor, int* label, int* semi)
        {
            if (ancestor[v] == -1) return label[v];
            Compress(v, ancestor, label, semi);
            return label[v];
        }

        private static void Compress(int v, int* ancestor, int* label, int* semi)
        {
            if (ancestor[ancestor[v]] == -1) return;
            Compress(ancestor[v], ancestor, label, semi);
            if (semi[label[ancestor[v]]] < semi[label[v]])
                label[v] = label[ancestor[v]];
            ancestor[v] = ancestor[ancestor[v]];
        }

        private static void Link(int v, int w, int* ancestor, int* semi)
        {
            ancestor[w] = v;
        }

        private static void AddToBucket(int v, int w, int* bucket)
        {
            bucket[v] = w;
        }
    }
}
