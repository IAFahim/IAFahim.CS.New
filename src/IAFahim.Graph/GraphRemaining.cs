namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    internal unsafe struct RemMinHeap
    {
        public long* Dist;
        public int* V;
        public int* Pos;
        public int Size;

        public RemMinHeap(int n)
        {
            Dist = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
            V = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
            Pos = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
            for (int i = 0; i < n; i++) Pos[i] = -1;
            Size = 0;
        }

        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)Dist);
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)V);
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)Pos);
        }

        public void PushOrUpdate(int v, long d)
        {
            int idx = Pos[v];
            if (idx == -1)
            {
                idx = Size++;
                V[idx] = v;
            }
            Dist[idx] = d;
            while (idx > 0)
            {
                int p = (idx - 1) / 2;
                if (Dist[p] <= Dist[idx]) break;
                long tmpD = Dist[p]; Dist[p] = Dist[idx]; Dist[idx] = tmpD;
                int tmpV = V[p]; V[p] = V[idx]; V[idx] = tmpV;
                Pos[V[p]] = p;
                Pos[V[idx]] = idx;
                idx = p;
            }
        }

        public int Pop(out long d)
        {
            int u = V[0];
            d = Dist[0];
            Pos[u] = -1;
            Size--;
            if (Size > 0)
            {
                Dist[0] = Dist[Size];
                V[0] = V[Size];
                Pos[V[0]] = 0;
                int idx = 0;
                while (idx * 2 + 1 < Size)
                {
                    int left = idx * 2 + 1;
                    int right = idx * 2 + 2;
                    int smallest = left;
                    if (right < Size && Dist[right] < Dist[left]) smallest = right;
                    if (Dist[idx] <= Dist[smallest]) break;
                    long tmpD = Dist[idx]; Dist[idx] = Dist[smallest]; Dist[smallest] = tmpD;
                    int tmpV = V[idx]; V[idx] = V[smallest]; V[smallest] = tmpV;
                    Pos[V[idx]] = idx;
                    Pos[V[smallest]] = smallest;
                    idx = smallest;
                }
            }
            return u;
        }
    }

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
            var pq = new RemMinHeap(n);
            try
            {
                pq.PushOrUpdate(start, h[start]);
                while (pq.Size > 0)
                {
                    int u = pq.Pop(out long currentF);
                    if (u == target) break;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        long g = result[u] + dist[e];
                        long f = g + h[v];
                        if (g < result[v])
                        {
                            result[v] = g;
                            pq.PushOrUpdate(v, f);
                        }
                    }
                }
            }
            finally { pq.Dispose(); }
            return result[target];
        }
    }

    public static unsafe class YenKShortestPaths
    {
        public static int Run(int n, int src, int dst, int k, int* head, int* to, int* next, long* dist, long* pathCosts, long* work)
        {
            int maxK = 64;
            int* pathNodes = stackalloc int[maxK * n];
            int* pathLens = stackalloc int[maxK];
            long* pathCostsArr = stackalloc long[maxK];
            int found = 0;

            long* distTo = stackalloc long[n];
            int* prev = stackalloc int[n];
            int* spurPath = stackalloc int[n];
            int* combinedPath = stackalloc int[n];

            long shortestCost = Dijkstra(n, src, dst, head, to, next, dist, distTo, prev, (int*)0, (int*)0, 0);
            if (shortestCost == long.MaxValue) return 0;

            pathLens[0] = ReconstructPath(prev, src, dst, pathNodes, n);
            pathCostsArr[0] = shortestCost;
            pathCosts[0] = shortestCost;
            found = 1;

            bool* blockedU = stackalloc bool[n];
            bool* blockedV = stackalloc bool[n];
            int* blockedUList = stackalloc int[n];
            int* blockedVList = stackalloc int[n];

            for (int ki = 1; ki < k && found < maxK; ki++)
            {
                int bestSpurCostIdx = -1;
                long bestSpurCost = long.MaxValue;
                int bestSpurLen = 0;

                int prevLen = pathLens[0];

                for (int spurIdx = 0; spurIdx < prevLen && spurIdx < n; spurIdx++)
                {
                    int spurNode = pathNodes[spurIdx];
                    long rootCost = 0;
                    for (int r = 0; r < spurIdx; r++)
                        rootCost += distTo[pathNodes[r + 1]] - distTo[pathNodes[r]];
                    if (spurIdx > 0 && distTo[pathNodes[spurIdx]] != long.MaxValue)
                        rootCost = distTo[pathNodes[spurIdx]] - distTo[src];

                    int blockCount = 0;
                    for (int b = 0; b < n; b++) { blockedU[b] = false; blockedV[b] = false; }

                    for (int p = 0; p < found; p++)
                    {
                        if (pathLens[p] > spurIdx)
                        {
                            bool match = true;
                            for (int r = 0; r < spurIdx && match; r++)
                            {
                                if (pathNodes[p * n + r] != pathNodes[r])
                                    match = false;
                            }
                            if (match && pathLens[p] > spurIdx)
                            {
                                int u = pathNodes[p * n + spurIdx];
                                int v = pathNodes[p * n + spurIdx + 1];
                                blockedU[u] = true;
                                blockedV[v] = true;
                                blockedUList[blockCount] = u;
                                blockedVList[blockCount] = v;
                                blockCount++;
                            }
                        }
                    }

                    long spurCost = Dijkstra(n, spurNode, dst, head, to, next, dist, distTo, prev, blockedUList, blockedVList, blockCount);
                    if (spurCost == long.MaxValue) continue;

                    long totalCost = rootCost + spurCost;
                    if (totalCost < bestSpurCost)
                    {
                        bestSpurCost = totalCost;
                        bestSpurCostIdx = spurIdx;
                        int spurLen = ReconstructPath(prev, spurNode, dst, spurPath, n);
                        bestSpurLen = spurIdx + spurLen;
                        for (int r = 0; r < spurIdx; r++)
                            combinedPath[r] = pathNodes[r];
                        for (int r = 0; r < spurLen; r++)
                            combinedPath[spurIdx + r] = spurPath[r];
                    }
                }

                if (bestSpurCostIdx == -1) break;

                bool isDuplicate = false;
                for (int p = 0; p < found; p++)
                {
                    if (pathCostsArr[p] == bestSpurCost && pathLens[p] == bestSpurLen)
                    {
                        bool same = true;
                        for (int r = 0; r < bestSpurLen && same; r++)
                        {
                            if (pathNodes[p * n + r] != combinedPath[r])
                                same = false;
                        }
                        if (same) { isDuplicate = true; break; }
                    }
                }

                if (!isDuplicate)
                {
                    for (int r = 0; r < bestSpurLen; r++)
                        pathNodes[found * n + r] = combinedPath[r];
                    pathLens[found] = bestSpurLen;
                    pathCostsArr[found] = bestSpurCost;
                    pathCosts[found] = bestSpurCost;
                    found++;
                }
            }

            return found;
        }

        private static long Dijkstra(int n, int src, int dst, int* head, int* to, int* next, long* dist, long* distTo, int* prev, int* blockedU, int* blockedV, int blockCount)
        {
            for (int i = 0; i < n; i++) { distTo[i] = long.MaxValue; prev[i] = -1; }
            distTo[src] = 0;
            var pq = new RemMinHeap(n);
            try
            {
                pq.PushOrUpdate(src, 0);
                while (pq.Size > 0)
                {
                    int u = pq.Pop(out long currentD);
                    if (currentD > distTo[u]) continue;
                    if (u == dst) break;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        bool isBlocked = false;
                        for (int b = 0; b < blockCount; b++)
                        {
                            if (blockedU[b] == u && blockedV[b] == v)
                            {
                                isBlocked = true;
                                break;
                            }
                        }
                        if (isBlocked) continue;
                        long nd = currentD + dist[e];
                        if (nd < distTo[v])
                        {
                            distTo[v] = nd;
                            prev[v] = u;
                            pq.PushOrUpdate(v, nd);
                        }
                    }
                }
            }
            finally { pq.Dispose(); }
            return distTo[dst];
        }

        private static int ReconstructPath(int* prev, int src, int dst, int* path, int n)
        {
            int* temp = stackalloc int[n];
            int len = 0;
            int cur = dst;
            while (cur != -1 && len < n)
            {
                temp[len++] = cur;
                if (cur == src) break;
                cur = prev[cur];
            }
            for (int i = 0; i < len; i++)
                path[i] = temp[len - 1 - i];
            return len;
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
