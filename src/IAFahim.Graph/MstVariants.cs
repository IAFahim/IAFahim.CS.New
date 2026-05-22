namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;
    using IAFahim.Graph.Flow;

    public static unsafe class MstVariants
    {
        public static long MinimumArborescenceDirected(int n, int root, int* u, int* v, long* w, int m, long* result)
        {
            return ChuLiuEdmonds.Run(n, root, u, v, w, m, result);
        }

        public static long MaximumBranching(int n, int* u, int* v, long* w, int m, int* resultEdges, int* resultCount)
        {
            int dummyRoot = n;
            int newN = n + 1;
            int newM = m + n;

            int* newU = stackalloc int[newM];
            int* newV = stackalloc int[newM];
            long* newW = stackalloc long[newM];

            long maxW = 0;
            for (int i = 0; i < m; i++)
            {
                if (w[i] > maxW)
                {
                    maxW = w[i];
                }
            }

            for (int i = 0; i < m; i++)
            {
                newU[i] = u[i];
                newV[i] = v[i];
                newW[i] = maxW - w[i];
            }

            for (int i = 0; i < n; i++)
            {
                newU[m + i] = dummyRoot;
                newV[m + i] = i;
                newW[m + i] = maxW;
            }

            long* result = stackalloc long[newN];
            for (int i = 0; i < newN; i++)
            {
                result[i] = -1;
            }

            long minArbWeight = ChuLiuEdmonds.Run(newN, dummyRoot, newU, newV, newW, newM, result);
            if (minArbWeight == -1)
            {
                return -1;
            }

            *resultCount = 0;
            long totalWeight = 0;
            for (int i = 0; i < newN; i++)
            {
                if (i != dummyRoot && result[i] != -1)
                {
                    int parent = (int)result[i];
                    if (parent != dummyRoot)
                    {
                        for (int e = 0; e < m; e++)
                        {
                            if (u[e] == parent && v[e] == i)
                            {
                                resultEdges[(*resultCount)++] = e;
                                totalWeight += w[e];
                                break;
                            }
                        }
                    }
                }
            }

            return totalWeight;
        }

        public static long BranchingMatroidIntersection(int n, int* u, int* v, long* w, int m, int* resultEdges, int* resultCount)
        {
            return MaximumBranching(n, u, v, w, m, resultEdges, resultCount);
        }

        public static long ArborescenceCount(int n, int root, int* u, int* v, int m)
        {
            const long Mod = 1000000007;
            long* laplacian = stackalloc long[(n - 1) * (n - 1)];
            for (int i = 0; i < (n - 1) * (n - 1); i++)
            {
                laplacian[i] = 0;
            }

            int* map = stackalloc int[n];
            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == root)
                {
                    map[i] = -1;
                }
                else
                {
                    map[i] = idx++;
                }
            }

            for (int i = 0; i < m; i++)
            {
                int fromNode = u[i];
                int toNode = v[i];
                if (toNode == root)
                {
                    continue;
                }
                int mappedTo = map[toNode];
                laplacian[mappedTo * (n - 1) + mappedTo]++;

                if (fromNode != root)
                {
                    int mappedFrom = map[fromNode];
                    laplacian[mappedTo * (n - 1) + mappedFrom] = (laplacian[mappedTo * (n - 1) + mappedFrom] - 1 + Mod) % Mod;
                }
            }

            long det = 1;
            int dim = n - 1;
            for (int i = 0; i < dim; i++)
            {
                int pivot = i;
                for (int j = i + 1; j < dim; j++)
                {
                    if (Math.Abs(laplacian[j * dim + i]) > Math.Abs(laplacian[pivot * dim + i]))
                    {
                        pivot = j;
                    }
                }
                if (laplacian[pivot * dim + i] == 0)
                {
                    return 0;
                }
                if (pivot != i)
                {
                    for (int k = 0; k < dim; k++)
                    {
                        long tmp = laplacian[i * dim + k];
                        laplacian[i * dim + k] = laplacian[pivot * dim + k];
                        laplacian[pivot * dim + k] = tmp;
                    }
                    det = (Mod - det) % Mod;
                }
                det = (det * laplacian[i * dim + i]) % Mod;
                long inv = Power(laplacian[i * dim + i], Mod - 2, Mod);
                for (int j = i + 1; j < dim; j++)
                {
                    long factor = (laplacian[j * dim + i] * inv) % Mod;
                    for (int k = i; k < dim; k++)
                    {
                        laplacian[j * dim + k] = (laplacian[j * dim + k] - factor * laplacian[i * dim + k] % Mod + Mod) % Mod;
                    }
                }
            }
            return det;
        }

        private static long Power(long baseVal, long exp, long mod)
        {
            long res = 1;
            baseVal %= mod;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                {
                    res = (res * baseVal) % mod;
                }
                baseVal = (baseVal * baseVal) % mod;
                exp /= 2;
            }
            return res;
        }

        public static bool DegreeConstrainedMst(int n, int m, int* u, int* v, long* w, int r, int k, int* resultEdges, int* resultCount)
        {
            int* parent = stackalloc int[n];
            int* rank = stackalloc int[n];
            int* edgeIndices = stackalloc int[m];
            for (int i = 0; i < m; i++)
            {
                edgeIndices[i] = i;
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = i + 1; j < m; j++)
                {
                    if (w[edgeIndices[i]] > w[edgeIndices[j]])
                    {
                        int tmp = edgeIndices[i];
                        edgeIndices[i] = edgeIndices[j];
                        edgeIndices[j] = tmp;
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                rank[i] = 0;
            }

            int Find(int x)
            {
                int root = x;
                while (root != parent[root])
                {
                    root = parent[root];
                }
                int curr = x;
                while (curr != root)
                {
                    int nxt = parent[curr];
                    parent[curr] = root;
                    curr = nxt;
                }
                return root;
            }

            bool Union(int x, int y)
            {
                int rx = Find(x);
                int ry = Find(y);
                if (rx == ry)
                {
                    return false;
                }
                if (rank[rx] < rank[ry])
                {
                    parent[rx] = ry;
                }
                else if (rank[rx] > rank[ry])
                {
                    parent[ry] = rx;
                }
                else
                {
                    parent[ry] = rx;
                    rank[rx]++;
                }
                return true;
            }

            byte* inTree = stackalloc byte[m];
            for (int i = 0; i < m; i++)
            {
                inTree[i] = 0;
            }

            int components = n - 1;
            for (int i = 0; i < m; i++)
            {
                int e = edgeIndices[i];
                if (u[e] == r || v[e] == r)
                {
                    continue;
                }
                if (Union(u[e], v[e]))
                {
                    inTree[e] = 1;
                    components--;
                }
            }

            int* minEdgeToR = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                minEdgeToR[i] = -1;
            }

            for (int i = 0; i < m; i++)
            {
                int e = edgeIndices[i];
                if (u[e] == r || v[e] == r)
                {
                    int other = (u[e] == r) ? v[e] : u[e];
                    int rootComp = Find(other);
                    if (minEdgeToR[rootComp] == -1 || w[e] < w[minEdgeToR[rootComp]])
                    {
                        minEdgeToR[rootComp] = e;
                    }
                }
            }

            int deg = 0;
            for (int i = 0; i < n; i++)
            {
                if (i != r && Find(i) == i)
                {
                    int e = minEdgeToR[i];
                    if (e == -1)
                    {
                        return false;
                    }
                    inTree[e] = 1;
                    deg++;
                }
            }

            if (deg > k)
            {
                return false;
            }

            while (deg < k)
            {
                int bestEdgeToAdd = -1;
                int bestEdgeToRemove = -1;
                long maxReduction = 0;

                for (int eAdd = 0; eAdd < m; eAdd++)
                {
                    if (inTree[eAdd] == 1)
                    {
                        continue;
                    }
                    if (u[eAdd] != r && v[eAdd] != r)
                    {
                        continue;
                    }

                    int other = (u[eAdd] == r) ? v[eAdd] : u[eAdd];

                    int* head = stackalloc int[n];
                    int* next = stackalloc int[n * 2];
                    int* to = stackalloc int[n * 2];
                    int* edgeId = stackalloc int[n * 2];
                    for (int i = 0; i < n; i++)
                    {
                        head[i] = -1;
                    }

                    int edgeIdx = 0;
                    for (int e = 0; e < m; e++)
                    {
                        if (inTree[e] == 1)
                        {
                            int x = u[e];
                            int y = v[e];
                            to[edgeIdx] = y; edgeId[edgeIdx] = e; next[edgeIdx] = head[x]; head[x] = edgeIdx++;
                            to[edgeIdx] = x; edgeId[edgeIdx] = e; next[edgeIdx] = head[y]; head[y] = edgeIdx++;
                        }
                    }

                    int* cycleEdges = stackalloc int[n];
                    int cycleEdgeCount = 0;
                    bool pathFound = false;

                    void DfsPath(int curr, int target, int parentEdge)
                    {
                        if (curr == target)
                        {
                            pathFound = true;
                            return;
                        }
                        for (int edge = head[curr]; edge != -1; edge = next[edge])
                        {
                            int neighbor = to[edge];
                            int idVal = edgeId[edge];
                            if (idVal != parentEdge)
                            {
                                cycleEdges[cycleEdgeCount++] = idVal;
                                DfsPath(neighbor, target, idVal);
                                if (pathFound)
                                {
                                    return;
                                }
                                cycleEdgeCount--;
                            }
                        }
                    }

                    DfsPath(r, other, -1);

                    int maxEdgeInCycle = -1;
                    for (int idxC = 0; idxC < cycleEdgeCount; idxC++)
                    {
                        int ec = cycleEdges[idxC];
                        if (u[ec] != r && v[ec] != r)
                        {
                            if (maxEdgeInCycle == -1 || w[ec] > w[maxEdgeInCycle])
                            {
                                maxEdgeInCycle = ec;
                            }
                        }
                    }

                    if (maxEdgeInCycle != -1)
                    {
                        long reduction = w[maxEdgeInCycle] - w[eAdd];
                        if (reduction > maxReduction)
                        {
                            maxReduction = reduction;
                            bestEdgeToAdd = eAdd;
                            bestEdgeToRemove = maxEdgeInCycle;
                        }
                    }
                }

                if (bestEdgeToAdd != -1 && maxReduction > 0)
                {
                    inTree[bestEdgeToAdd] = 1;
                    inTree[bestEdgeToRemove] = 0;
                    deg++;
                }
                else
                {
                    break;
                }
            }

            *resultCount = 0;
            for (int i = 0; i < m; i++)
            {
                if (inTree[i] == 1)
                {
                    resultEdges[(*resultCount)++] = i;
                }
            }

            return true;
        }

        public static void CapacitatedMst(int n, int m, int* u, int* v, long* w, int r, int capacity, int* resultEdges, int* resultCount)
        {
            byte* inTree = stackalloc byte[m];
            for (int i = 0; i < m; i++)
            {
                inTree[i] = 0;
            }

            int* componentSize = stackalloc int[n];
            int* parent = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                componentSize[i] = 1;
                parent[i] = i;
            }

            int Find(int x)
            {
                int root = x;
                while (root != parent[root])
                {
                    root = parent[root];
                }
                int curr = x;
                while (curr != root)
                {
                    int nxt = parent[curr];
                    parent[curr] = root;
                    curr = nxt;
                }
                return root;
            }

            int* rootEdge = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                rootEdge[i] = -1;
            }

            for (int i = 0; i < m; i++)
            {
                if (u[i] == r || v[i] == r)
                {
                    int other = (u[i] == r) ? v[i] : u[i];
                    if (rootEdge[other] == -1 || w[i] < w[rootEdge[other]])
                    {
                        rootEdge[other] = i;
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (i != r && rootEdge[i] != -1)
                {
                    inTree[rootEdge[i]] = 1;
                }
            }

            while (true)
            {
                int bestEdge = -1;
                long bestSavings = long.MinValue;
                int mergeU = -1;
                int mergeV = -1;

                for (int e = 0; e < m; e++)
                {
                    if (u[e] == r || v[e] == r)
                    {
                        continue;
                    }
                    if (inTree[e] == 1)
                    {
                        continue;
                    }

                    int ru = Find(u[e]);
                    int rv = Find(v[e]);
                    if (ru == rv)
                    {
                        continue;
                    }

                    if (componentSize[ru] + componentSize[rv] > capacity)
                    {
                        continue;
                    }

                    int reU = rootEdge[ru];
                    int reV = rootEdge[rv];
                    if (reU == -1 || reV == -1)
                    {
                        continue;
                    }

                    long rootCost = Math.Max(w[reU], w[reV]);
                    long savings = rootCost - w[e];
                    if (savings > bestSavings)
                    {
                        bestSavings = savings;
                        bestEdge = e;
                        mergeU = ru;
                        mergeV = rv;
                    }
                }

                if (bestEdge != -1 && bestSavings > 0)
                {
                    inTree[bestEdge] = 1;
                    int reU = rootEdge[mergeU];
                    int reV = rootEdge[mergeV];
                    if (w[reU] > w[reV])
                    {
                        inTree[reU] = 0;
                        rootEdge[mergeU] = reV;
                        parent[mergeU] = mergeV;
                        componentSize[mergeV] += componentSize[mergeU];
                    }
                    else
                    {
                        inTree[reV] = 0;
                        rootEdge[mergeV] = reU;
                        parent[mergeV] = mergeU;
                        componentSize[mergeU] += componentSize[mergeV];
                    }
                }
                else
                {
                    break;
                }
            }

            *resultCount = 0;
            for (int i = 0; i < m; i++)
            {
                if (inTree[i] == 1)
                {
                    resultEdges[(*resultCount)++] = i;
                }
            }
        }

        public static void MinimumDiameterSpanningTree(int n, int m, int* u, int* v, long* w, int* resultEdges, int* resultCount)
        {
            long* dist = stackalloc long[n * n];
            int* parent = stackalloc int[n * n];
            const long Inf = 999999999999;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dist[i * n + j] = (i == j) ? 0 : Inf;
                    parent[i * n + j] = -1;
                }
            }

            for (int i = 0; i < m; i++)
            {
                int x = u[i];
                int y = v[i];
                if (w[i] < dist[x * n + y])
                {
                    dist[x * n + y] = w[i];
                    dist[y * n + x] = w[i];
                }
            }

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[i * n + k] + dist[k * n + j] < dist[i * n + j])
                        {
                            dist[i * n + j] = dist[i * n + k] + dist[k * n + j];
                        }
                    }
                }
            }

            int bestCenter = 0;
            long minMaxDist = Inf;
            for (int i = 0; i < n; i++)
            {
                long maxDist = 0;
                for (int j = 0; j < n; j++)
                {
                    if (dist[i * n + j] > maxDist)
                    {
                        maxDist = dist[i * n + j];
                    }
                }
                if (maxDist < minMaxDist)
                {
                    minMaxDist = maxDist;
                    bestCenter = i;
                }
            }

            long* d = stackalloc long[n];
            int* p = stackalloc int[n];
            byte* vis = stackalloc byte[n];
            for (int i = 0; i < n; i++)
            {
                d[i] = Inf;
                p[i] = -1;
                vis[i] = 0;
            }

            d[bestCenter] = 0;
            for (int step = 0; step < n; step++)
            {
                int curr = -1;
                for (int i = 0; i < n; i++)
                {
                    if (vis[i] == 0 && (curr == -1 || d[i] < d[curr]))
                    {
                        curr = i;
                    }
                }
                if (curr == -1 || d[curr] == Inf)
                {
                    break;
                }
                vis[curr] = 1;

                for (int e = 0; e < m; e++)
                {
                    int x = u[e];
                    int y = v[e];
                    if (x == curr && d[curr] + w[e] < d[y])
                    {
                        d[y] = d[curr] + w[e];
                        p[y] = e;
                    }
                    if (y == curr && d[curr] + w[e] < d[x])
                    {
                        d[x] = d[curr] + w[e];
                        p[x] = e;
                    }
                }
            }

            *resultCount = 0;
            for (int i = 0; i < n; i++)
            {
                if (p[i] != -1)
                {
                    resultEdges[(*resultCount)++] = p[i];
                }
            }
        }

        public static void MinimumBottleneckSpanningTree(int n, int m, int* u, int* v, long* w, int* resultEdges, int* resultCount)
        {
            int* parent = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
            }

            int Find(int x)
            {
                int root = x;
                while (root != parent[root])
                {
                    root = parent[root];
                }
                int curr = x;
                while (curr != root)
                {
                    int nxt = parent[curr];
                    parent[curr] = root;
                    curr = nxt;
                }
                return root;
            }

            int* edgeIndices = stackalloc int[m];
            for (int i = 0; i < m; i++)
            {
                edgeIndices[i] = i;
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = i + 1; j < m; j++)
                {
                    if (w[edgeIndices[i]] > w[edgeIndices[j]])
                    {
                        int tmp = edgeIndices[i];
                        edgeIndices[i] = edgeIndices[j];
                        edgeIndices[j] = tmp;
                    }
                }
            }

            *resultCount = 0;
            for (int i = 0; i < m; i++)
            {
                int e = edgeIndices[i];
                int rx = Find(u[e]);
                int ry = Find(v[e]);
                if (rx != ry)
                {
                    parent[rx] = ry;
                    resultEdges[(*resultCount)++] = e;
                }
            }
        }

        public static long MinimumBottleneckPath(int n, int m, int* u, int* v, long* w, int src, int dest)
        {
            int* mstEdges = stackalloc int[n - 1];
            int mstCount = 0;
            MinimumBottleneckSpanningTree(n, m, u, v, w, mstEdges, &mstCount);

            int* head = stackalloc int[n];
            int* next = stackalloc int[n * 2];
            int* to = stackalloc int[n * 2];
            long* weight = stackalloc long[n * 2];
            for (int i = 0; i < n; i++)
            {
                head[i] = -1;
            }

            int edgeIdx = 0;
            for (int i = 0; i < mstCount; i++)
            {
                int e = mstEdges[i];
                int x = u[e];
                int y = v[e];
                long c = w[e];
                to[edgeIdx] = y; weight[edgeIdx] = c; next[edgeIdx] = head[x]; head[x] = edgeIdx++;
                to[edgeIdx] = x; weight[edgeIdx] = c; next[edgeIdx] = head[y]; head[y] = edgeIdx++;
            }

            long maxWeight = 0;
            bool found = false;

            void Dfs(int curr, int target, int parentNode, long currentMax)
            {
                if (curr == target)
                {
                    maxWeight = currentMax;
                    found = true;
                    return;
                }
                for (int edge = head[curr]; edge != -1; edge = next[edge])
                {
                    int neighbor = to[edge];
                    if (neighbor != parentNode)
                    {
                        Dfs(neighbor, target, curr, Math.Max(currentMax, weight[edge]));
                        if (found)
                        {
                            return;
                        }
                    }
                }
            }

            Dfs(src, dest, -1, 0);
            return found ? maxWeight : -1;
        }

        public static void MaximumCapacitySpanningTree(int n, int m, int* u, int* v, long* w, int* resultEdges, int* resultCount)
        {
            int* parent = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
            }

            int Find(int x)
            {
                int root = x;
                while (root != parent[root])
                {
                    root = parent[root];
                }
                int curr = x;
                while (curr != root)
                {
                    int nxt = parent[curr];
                    parent[curr] = root;
                    curr = nxt;
                }
                return root;
            }

            int* edgeIndices = stackalloc int[m];
            for (int i = 0; i < m; i++)
            {
                edgeIndices[i] = i;
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = i + 1; j < m; j++)
                {
                    if (w[edgeIndices[i]] < w[edgeIndices[j]])
                    {
                        int tmp = edgeIndices[i];
                        edgeIndices[i] = edgeIndices[j];
                        edgeIndices[j] = tmp;
                    }
                }
            }

            *resultCount = 0;
            for (int i = 0; i < m; i++)
            {
                int e = edgeIndices[i];
                int rx = Find(u[e]);
                int ry = Find(v[e]);
                if (rx != ry)
                {
                    parent[rx] = ry;
                    resultEdges[(*resultCount)++] = e;
                }
            }
        }

        public static long WidestPath(int n, int m, int* u, int* v, long* w, int src, int dest)
        {
            int* mstEdges = stackalloc int[n - 1];
            int mstCount = 0;
            MaximumCapacitySpanningTree(n, m, u, v, w, mstEdges, &mstCount);

            int* head = stackalloc int[n];
            int* next = stackalloc int[n * 2];
            int* to = stackalloc int[n * 2];
            long* weight = stackalloc long[n * 2];
            for (int i = 0; i < n; i++)
            {
                head[i] = -1;
            }

            int edgeIdx = 0;
            for (int i = 0; i < mstCount; i++)
            {
                int e = mstEdges[i];
                int x = u[e];
                int y = v[e];
                long c = w[e];
                to[edgeIdx] = y; weight[edgeIdx] = c; next[edgeIdx] = head[x]; head[x] = edgeIdx++;
                to[edgeIdx] = x; weight[edgeIdx] = c; next[edgeIdx] = head[y]; head[y] = edgeIdx++;
            }

            long minWeight = 999999999999;
            bool found = false;

            void Dfs(int curr, int target, int parentNode, long currentMin)
            {
                if (curr == target)
                {
                    minWeight = currentMin;
                    found = true;
                    return;
                }
                for (int edge = head[curr]; edge != -1; edge = next[edge])
                {
                    int neighbor = to[edge];
                    if (neighbor != parentNode)
                    {
                        Dfs(neighbor, target, curr, Math.Min(currentMin, weight[edge]));
                        if (found)
                        {
                            return;
                        }
                    }
                }
            }

            Dfs(src, dest, -1, minWeight);
            return found ? minWeight : -1;
        }

        public static long MaximumCapacityPath(int n, int m, int* u, int* v, long* w, int src, int dest)
        {
            return WidestPath(n, m, u, v, w, src, dest);
        }

        public static bool LexicographicShortestPath(int n, int m, int* u, int* v, long* w, int src, int dest, int* path, int* pathLength)
        {
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            byte* vis = stackalloc byte[n];
            const long Inf = 999999999999;
            for (int i = 0; i < n; i++)
            {
                dist[i] = Inf;
                parent[i] = -1;
                parentEdge[i] = -1;
                vis[i] = 0;
            }

            dist[src] = 0;
            for (int step = 0; step < n; step++)
            {
                int curr = -1;
                for (int i = 0; i < n; i++)
                {
                    if (vis[i] == 0 && (curr == -1 || dist[i] < dist[curr]))
                    {
                        curr = i;
                    }
                }
                if (curr == -1 || dist[curr] == Inf)
                {
                    break;
                }
                vis[curr] = 1;

                for (int e = 0; e < m; e++)
                {
                    int x = u[e];
                    int y = v[e];
                    if (x == curr)
                    {
                        if (dist[curr] + w[e] < dist[y])
                        {
                            dist[y] = dist[curr] + w[e];
                            parent[y] = curr;
                            parentEdge[y] = e;
                        }
                        else if (dist[curr] + w[e] == dist[y])
                        {
                            if (parent[y] == -1 || curr < parent[y])
                            {
                                parent[y] = curr;
                                parentEdge[y] = e;
                            }
                        }
                    }
                    if (y == curr)
                    {
                        if (dist[curr] + w[e] < dist[x])
                        {
                            dist[x] = dist[curr] + w[e];
                            parent[x] = curr;
                            parentEdge[x] = e;
                        }
                        else if (dist[curr] + w[e] == dist[x])
                        {
                            if (parent[x] == -1 || curr < parent[x])
                            {
                                parent[x] = curr;
                                parentEdge[x] = e;
                            }
                        }
                    }
                }
            }

            if (dist[dest] == Inf)
            {
                *pathLength = 0;
                return false;
            }

            int* tempPath = stackalloc int[n];
            int tempLen = 0;
            int node = dest;
            while (node != src)
            {
                tempPath[tempLen++] = node;
                node = parent[node];
            }
            tempPath[tempLen++] = src;

            *pathLength = tempLen;
            for (int i = 0; i < tempLen; i++)
            {
                path[i] = tempPath[tempLen - 1 - i];
            }
            return true;
        }

        public static void LexicographicMst(int n, int m, int* u, int* v, long* w, int* resultEdges, int* resultCount)
        {
            int* parent = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
            }

            int Find(int x)
            {
                int root = x;
                while (root != parent[root])
                {
                    root = parent[root];
                }
                int curr = x;
                while (curr != root)
                {
                    int nxt = parent[curr];
                    parent[curr] = root;
                    curr = nxt;
                }
                return root;
            }

            int* edgeIndices = stackalloc int[m];
            for (int i = 0; i < m; i++)
            {
                edgeIndices[i] = i;
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = i + 1; j < m; j++)
                {
                    int ei = edgeIndices[i];
                    int ej = edgeIndices[j];
                    bool swap = false;
                    if (w[ei] > w[ej])
                    {
                        swap = true;
                    }
                    else if (w[ei] == w[ej])
                    {
                        int minUi = Math.Min(u[ei], v[ei]);
                        int minUj = Math.Min(u[ej], v[ej]);
                        if (minUi > minUj)
                        {
                            swap = true;
                        }
                        else if (minUi == minUj)
                        {
                            int maxVi = Math.Max(u[ei], v[ei]);
                            int maxVj = Math.Max(u[ej], v[ej]);
                            if (maxVi > maxVj)
                            {
                                swap = true;
                            }
                        }
                    }
                    if (swap)
                    {
                        int tmp = edgeIndices[i];
                        edgeIndices[i] = edgeIndices[j];
                        edgeIndices[j] = tmp;
                    }
                }
            }

            *resultCount = 0;
            for (int i = 0; i < m; i++)
            {
                int e = edgeIndices[i];
                int rx = Find(u[e]);
                int ry = Find(v[e]);
                if (rx != ry)
                {
                    parent[rx] = ry;
                    resultEdges[(*resultCount)++] = e;
                }
            }
        }

        public static bool RandomizedMstVerify(int numVertices, int numEdges, int* u, int* v, long* weight, bool* inMst)
        {
            long* mstW = stackalloc long[numVertices];
            int* parent = stackalloc int[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                parent[i] = i;
            }

            int Find(int x)
            {
                int root = x;
                while (root != parent[root])
                {
                    root = parent[root];
                }
                int curr = x;
                while (curr != root)
                {
                    int nxt = parent[curr];
                    parent[curr] = root;
                    curr = nxt;
                }
                return root;
            }

            int mstCount = 0;
            for (int i = 0; i < numEdges; i++)
            {
                if (inMst[i])
                {
                    int rx = Find(u[i]);
                    int ry = Find(v[i]);
                    if (rx != ry)
                    {
                        parent[rx] = ry;
                        mstCount++;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (mstCount != numVertices - 1)
            {
                return false;
            }

            int* head = stackalloc int[numVertices];
            int* next = stackalloc int[numVertices * 2];
            int* to = stackalloc int[numVertices * 2];
            long* edgeW = stackalloc long[numVertices * 2];
            for (int i = 0; i < numVertices; i++)
            {
                head[i] = -1;
            }

            int edgeIdx = 0;
            for (int i = 0; i < numEdges; i++)
            {
                if (inMst[i])
                {
                    int x = u[i];
                    int y = v[i];
                    to[edgeIdx] = y; edgeW[edgeIdx] = weight[i]; next[edgeIdx] = head[x]; head[x] = edgeIdx++;
                    to[edgeIdx] = x; edgeW[edgeIdx] = weight[i]; next[edgeIdx] = head[y]; head[y] = edgeIdx++;
                }
            }

            for (int i = 0; i < numEdges; i++)
            {
                if (!inMst[i])
                {
                    int x = u[i];
                    int y = v[i];
                    long w = weight[i];

                    long maxW = 0;
                    bool pathFound = false;

                    void DfsPath(int curr, int target, int parentNode, long currentMax)
                    {
                        if (curr == target)
                        {
                            maxW = currentMax;
                            pathFound = true;
                            return;
                        }
                        for (int edge = head[curr]; edge != -1; edge = next[edge])
                        {
                            int neighbor = to[edge];
                            if (neighbor != parentNode)
                            {
                                DfsPath(neighbor, target, curr, Math.Max(currentMax, edgeW[edge]));
                                if (pathFound)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    DfsPath(x, y, -1, 0);
                    if (w < maxW)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void KargerSteinMinCut(int n, int m, int* u, int* v, int* bestCutU, int* bestCutV, int* bestCutCount)
        {
            int* parent = stackalloc int[n];
            int Find(int x)
            {
                int root = x;
                while (root != parent[root])
                {
                    root = parent[root];
                }
                int curr = x;
                while (curr != root)
                {
                    int nxt = parent[curr];
                    parent[curr] = root;
                    curr = nxt;
                }
                return root;
            }

            Random rng = new Random(42);
            int minCut = m + 1;

            int iterations = Math.Max(5, n * n);
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = 0; i < n; i++)
                {
                    parent[i] = i;
                }

                int activeVertices = n;
                while (activeVertices > 2)
                {
                    int e = rng.Next(m);
                    int rx = Find(u[e]);
                    int ry = Find(v[e]);
                    if (rx != ry)
                    {
                        parent[rx] = ry;
                        activeVertices--;
                    }
                }

                int cutCount = 0;
                int* cutU = stackalloc int[m];
                int* cutV = stackalloc int[m];
                for (int i = 0; i < m; i++)
                {
                    int rx = Find(u[i]);
                    int ry = Find(v[i]);
                    if (rx != ry)
                    {
                        cutU[cutCount] = u[i];
                        cutV[cutCount] = v[i];
                        cutCount++;
                    }
                }

                if (cutCount < minCut)
                {
                    minCut = cutCount;
                    *bestCutCount = cutCount;
                    for (int i = 0; i < cutCount; i++)
                    {
                        bestCutU[i] = cutU[i];
                        bestCutV[i] = cutV[i];
                    }
                }
            }
        }

        public static void NagamochiIbarakiSparseCertificate(int n, int m, int* u, int* v, int k, int* certEdges, int* certCount)
        {
            *certCount = 0;
            int* parent = stackalloc int[n];
            int Find(int x)
            {
                int root = x;
                while (root != parent[root])
                {
                    root = parent[root];
                }
                int curr = x;
                while (curr != root)
                {
                    int nxt = parent[curr];
                    parent[curr] = root;
                    curr = nxt;
                }
                return root;
            }

            byte* used = stackalloc byte[m];
            for (int i = 0; i < m; i++)
            {
                used[i] = 0;
            }

            for (int forest = 0; forest < k; forest++)
            {
                for (int i = 0; i < n; i++)
                {
                    parent[i] = i;
                }
                for (int i = 0; i < m; i++)
                {
                    if (used[i] == 0)
                    {
                        int rx = Find(u[i]);
                        int ry = Find(v[i]);
                        if (rx != ry)
                        {
                            parent[rx] = ry;
                            used[i] = 1;
                            certEdges[(*certCount)++] = i;
                        }
                    }
                }
            }
        }

        public static void SparseCertificateBuild(int n, int m, int* u, int* v, int k, int* certEdges, int* certCount)
        {
            NagamochiIbarakiSparseCertificate(n, m, u, v, k, certEdges, certCount);
        }

        public static long CutTreeQuery(int n, int* parent, int* weight, int src, int dest)
        {
            int* head = stackalloc int[n];
            int* next = stackalloc int[n * 2];
            int* to = stackalloc int[n * 2];
            int* cap = stackalloc int[n * 2];
            for (int i = 0; i < n; i++)
            {
                head[i] = -1;
            }

            int edgeIdx = 0;
            for (int i = 0; i < n; i++)
            {
                if (parent[i] != -1)
                {
                    int x = i;
                    int y = parent[i];
                    int c = weight[i];
                    to[edgeIdx] = y; cap[edgeIdx] = c; next[edgeIdx] = head[x]; head[x] = edgeIdx++;
                    to[edgeIdx] = x; cap[edgeIdx] = c; next[edgeIdx] = head[y]; head[y] = edgeIdx++;
                }
            }

            long minWeight = 999999999999;
            bool found = false;

            void Dfs(int curr, int target, int parentNode, long currentMin)
            {
                if (curr == target)
                {
                    minWeight = currentMin;
                    found = true;
                    return;
                }
                for (int edge = head[curr]; edge != -1; edge = next[edge])
                {
                    int neighbor = to[edge];
                    if (neighbor != parentNode)
                    {
                        Dfs(neighbor, target, curr, Math.Min(currentMin, cap[edge]));
                        if (found)
                        {
                            return;
                        }
                    }
                }
            }

            Dfs(src, dest, -1, minWeight);
            return found ? minWeight : -1;
        }

        public static int EdgeConnectivity(int n, int m, int* head, int* to, int* next, int* cap)
        {
            long minCut = 999999999999;
            for (int i = 1; i < n; i++)
            {
                int* tempCap = stackalloc int[m * 2];
                for (int j = 0; j < m * 2; j++)
                {
                    tempCap[j] = cap[j];
                }
                int* tempFlow = stackalloc int[m * 2];
                long flow = DinicMaxFlow.Run(n, 0, i, head, to, next, tempCap, tempFlow);
                if (flow < minCut)
                {
                    minCut = flow;
                }
            }
            return (int)minCut;
        }

        public static int VertexConnectivity(int n, int m, int* u, int* v)
        {
            if (n <= 1)
            {
                return 0;
            }

            int newN = n * 2;
            int newM = n + m;
            int* head = stackalloc int[newN];
            int* next = stackalloc int[newM * 4];
            int* to = stackalloc int[newM * 4];
            int* cap = stackalloc int[newM * 4];
            for (int i = 0; i < newN; i++)
            {
                head[i] = -1;
            }

            int edgeIdx = 0;
            void AddEdge(int fromNode, int toNode, int capacity)
            {
                to[edgeIdx] = toNode; cap[edgeIdx] = capacity; next[edgeIdx] = head[fromNode]; head[fromNode] = edgeIdx++;
                to[edgeIdx] = fromNode; cap[edgeIdx] = 0; next[edgeIdx] = head[toNode]; head[toNode] = edgeIdx++;
            }

            for (int i = 0; i < n; i++)
            {
                AddEdge(i, i + n, 1);
            }
            for (int i = 0; i < m; i++)
            {
                AddEdge(u[i] + n, v[i], 99999);
                AddEdge(v[i] + n, u[i], 99999);
            }

            long minCut = 999999999999;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    bool adjacent = false;
                    for (int e = 0; e < m; e++)
                    {
                        if ((u[e] == i && v[e] == j) || (u[e] == j && v[e] == i))
                        {
                            adjacent = true;
                            break;
                        }
                    }
                    if (adjacent)
                    {
                        continue;
                    }

                    int* tempCap = stackalloc int[edgeIdx];
                    for (int cIdx = 0; cIdx < edgeIdx; cIdx++)
                    {
                        tempCap[cIdx] = cap[cIdx];
                    }

                    int* tempFlow = stackalloc int[edgeIdx];
                    long flow = DinicMaxFlow.Run(newN, i + n, j, head, to, next, tempCap, tempFlow);
                    if (flow < minCut)
                    {
                        minCut = flow;
                    }
                }
            }

            if (minCut == 999999999999)
            {
                return n - 1;
            }
            return (int)minCut;
        }

        public static long StoerWagnerPhase(int n, int phase, int* head, int* to, int* next, int* weight, long* add, long* dist, int* vis, int* last, int* prev)
        {
            for (int i = 0; i < n; i++)
            {
                dist[i] = 0;
                vis[i] = -1;
            }
            *last = -1;
            *prev = -1;

            for (int iter = 0; iter < n - phase; iter++)
            {
                int v = -1;
                long maxDist = -1;
                for (int i = 0; i < n; i++)
                {
                    if (vis[i] == -1)
                    {
                        if (add[i] + dist[i] > maxDist)
                        {
                            maxDist = add[i] + dist[i];
                            v = i;
                        }
                    }
                }
                if (v == -1)
                {
                    break;
                }
                vis[v] = phase;
                *prev = *last;
                *last = v;

                for (int e = head[v]; e != 0; e = next[e])
                {
                    int toV = to[e];
                    if (vis[toV] == -1)
                    {
                        dist[toV] += weight[e];
                    }
                }
            }

            long phaseCut = add[*last] + dist[*last];
            return phaseCut;
        }

        public static long GomoryHuMinCutQuery(int n, int* parent, int* weight, int src, int dest)
        {
            return CutTreeQuery(n, parent, weight, src, dest);
        }
    }
}
