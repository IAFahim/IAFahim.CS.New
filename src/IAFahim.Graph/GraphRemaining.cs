namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal unsafe struct RemMinHeap
    {
        public long* Dist;
        public int* V;
        public int* Pos;
        public int Size;

        // Construct over caller-provided scratch buffers (each length >= n). This keeps
        // the heap allocation-free and Burst-compatible: no Marshal/native heap. The
        // caller owns the lifetime of dist/v/pos (typically stackalloc'd by the
        // top-level Run). pos is initialized to all -1 ("vertex not in heap").
        public RemMinHeap(int n, long* dist, int* v, int* pos)
        {
            Dist = dist;
            V = v;
            Pos = pos;
            for (int i = 0; i < n; i++) Pos[i] = -1;
            Size = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        // Computes the minimum-cost arborescence (directed MST) rooted at `root`.
        // On success returns the total weight and writes, for every vertex i != root,
        // result[i] = the SOURCE VERTEX (u[...]) of the original edge chosen as i's
        // incoming edge in the arborescence. result[root] is left untouched by this
        // routine (callers initialize it, typically to -1). Returns -1 if no
        // arborescence exists (some non-root vertex is unreachable). The chosen
        // source-vertex contract is relied upon by MstVariants.MaximumBranching.
        public static long Run(int n, int root, int* u, int* v, long* w, int m, long* result)
        {
            // resEdge[i] holds the ORIGINAL edge index selected as i's incoming edge.
            int* resEdge = stackalloc int[n];
            for (int i = 0; i < n; i++) resEdge[i] = -1;
            long total = Solve(n, root, u, v, w, m, resEdge);
            if (total == NoArborescence) return -1;
            for (int i = 0; i < n; i++)
            {
                if (i == root) continue;
                result[i] = u[resEdge[i]];
            }
            return total;
        }

        private const long NoArborescence = long.MinValue;

        // Recursive contraction. On success fills resEdge[i] (i != root) with the
        // ORIGINAL edge index chosen for vertex i and returns the total weight.
        // Returns NoArborescence if some non-root vertex has no incoming edge.
        private static long Solve(int n, int root, int* u, int* v, long* w, int m, int* resEdge)
        {
            if (n <= 1) return 0;

            int* inEdge = stackalloc int[n];
            for (int i = 0; i < n; i++) inEdge[i] = -1;
            for (int i = 0; i < m; i++)
            {
                if (v[i] != root && (inEdge[v[i]] == -1 || w[i] < w[inEdge[v[i]]]))
                    inEdge[v[i]] = i;
            }
            for (int i = 0; i < n; i++)
            {
                if (i != root && inEdge[i] == -1) return NoArborescence;
            }

            long totalWeight = 0;
            for (int i = 0; i < n; i++)
                if (i != root) totalWeight += w[inEdge[i]];

            // Detect cycles in the "chosen in-edge" functional graph.
            int* id = stackalloc int[n];   // contracted super-node id, -1 if unassigned
            int* vis = stackalloc int[n];  // visit stamp for cycle marking
            for (int i = 0; i < n; i++) { id[i] = -1; vis[i] = -1; }

            int cycles = 0;
            for (int i = 0; i < n; i++)
            {
                int vtx = i;
                while (vtx != root && vis[vtx] == -1 && id[vtx] == -1)
                {
                    vis[vtx] = i;
                    vtx = u[inEdge[vtx]];
                }
                // A cycle is found only if we returned to a vertex stamped in THIS walk.
                if (vtx != root && id[vtx] == -1 && vis[vtx] == i)
                {
                    int c = cycles++;
                    int x = vtx;
                    do
                    {
                        id[x] = c;
                        x = u[inEdge[x]];
                    } while (x != vtx);
                }
            }

            if (cycles == 0)
            {
                // No cycle: every chosen in-edge is final.
                for (int i = 0; i < n; i++)
                    if (i != root) resEdge[i] = inEdge[i];
                return totalWeight;
            }

            // Assign each non-cycle vertex its own fresh super-node id.
            for (int i = 0; i < n; i++)
                if (id[i] == -1) id[i] = cycles++;
            int newN = cycles;

            // Build the contracted edge set. For each original edge i with endpoints
            // in different super-nodes, record it and reduce its weight by the cost of
            // the in-edge currently entering its (cycle) target. Track which original
            // edge produced each contracted edge so the recursion's choice can be
            // mapped back.
            int* newU = stackalloc int[m];
            int* newV = stackalloc int[m];
            long* newW = stackalloc long[m];
            int* origEdge = stackalloc int[m];
            int newM = 0;
            for (int i = 0; i < m; i++)
            {
                int uu = id[u[i]];
                int vv = id[v[i]];
                if (uu == vv) continue;
                newU[newM] = uu;
                newV[newM] = vv;
                newW[newM] = w[i] - w[inEdge[v[i]]];
                origEdge[newM] = i;
                newM++;
            }

            int* subRes = stackalloc int[newN];
            for (int i = 0; i < newN; i++) subRes[i] = -1;
            long sub = Solve(newN, id[root], newU, newV, newW, newM, subRes);
            if (sub == NoArborescence) return NoArborescence;

            // Expand. For each super-node, the recursion picked a contracted edge
            // (subRes), which maps back to one original edge. That original edge's
            // real target vertex is the point where its cycle (if any) is broken:
            // that vertex takes the entering edge, all other cycle vertices keep
            // their in-cycle edge.
            for (int i = 0; i < n; i++)
                if (i != root) resEdge[i] = inEdge[i];

            for (int sn = 0; sn < newN; sn++)
            {
                if (sn == id[root]) continue;
                int ce = subRes[sn];          // contracted edge index chosen for super-node sn
                int oe = origEdge[ce];         // original edge index
                int target = v[oe];            // real target vertex inside super-node sn
                resEdge[target] = oe;          // break the cycle (if any) at this vertex
            }

            return totalWeight + sub;
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
            long* heapDist = stackalloc long[n];
            int* heapV = stackalloc int[n];
            int* heapPos = stackalloc int[n];
            RemMinHeap pq = new RemMinHeap(n, heapDist, heapV, heapPos);
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
            return result[target];
        }
    }

    public static unsafe class YenKShortestPaths
    {
        private const int MaxK = 64;

        public static int Run(int n, int src, int dst, int k, int* head, int* to, int* next, long* dist, long* pathCosts, long* work)
        {
            int maxK = MaxK;
            int* pathNodes = (int*)Marshal.AllocHGlobal((nint)((long)maxK * n * sizeof(int)));
            int* pathLens = (int*)Marshal.AllocHGlobal((nint)((long)maxK * sizeof(int)));
            long* pathCostsArr = (long*)Marshal.AllocHGlobal((nint)((long)maxK * sizeof(long)));
            int found = 0;

            long* distTo = (long*)Marshal.AllocHGlobal((nint)((long)n * sizeof(long)));
            int* prev = (int*)Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));
            int* spurPath = (int*)Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));
            int* combinedPath = (int*)Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));

            long shortestCost = Dijkstra(n, src, dst, head, to, next, dist, distTo, prev, (int*)0, (int*)0, 0);
            if (shortestCost == long.MaxValue) { Marshal.FreeHGlobal((nint)combinedPath); Marshal.FreeHGlobal((nint)spurPath); Marshal.FreeHGlobal((nint)prev); Marshal.FreeHGlobal((nint)distTo); Marshal.FreeHGlobal((nint)pathCostsArr); Marshal.FreeHGlobal((nint)pathLens); Marshal.FreeHGlobal((nint)pathNodes); return 0; }

            pathLens[0] = ReconstructPath(prev, src, dst, pathNodes, n);
            pathCostsArr[0] = shortestCost;
            pathCosts[0] = shortestCost;
            found = 1;

            bool* blockedU = (bool*)Marshal.AllocHGlobal((nint)((long)n));
            bool* blockedV = (bool*)Marshal.AllocHGlobal((nint)((long)n));
            int* blockedUList = (int*)Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));
            int* blockedVList = (int*)Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));

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
                        CollectBlockedEdge(pathNodes, pathLens, n, p, spurIdx, blockedU, blockedV, blockedUList, blockedVList, ref blockCount);

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
                    if (IsSamePath(pathNodes, pathLens, pathCostsArr, n, p, combinedPath, bestSpurLen, bestSpurCost))
                    {
                        isDuplicate = true;
                        break;
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

            Marshal.FreeHGlobal((nint)blockedVList);
            Marshal.FreeHGlobal((nint)blockedUList);
            Marshal.FreeHGlobal((nint)blockedV);
            Marshal.FreeHGlobal((nint)blockedU);
            Marshal.FreeHGlobal((nint)combinedPath);
            Marshal.FreeHGlobal((nint)spurPath);
            Marshal.FreeHGlobal((nint)prev);
            Marshal.FreeHGlobal((nint)distTo);
            Marshal.FreeHGlobal((nint)pathCostsArr);
            Marshal.FreeHGlobal((nint)pathLens);
            Marshal.FreeHGlobal((nint)pathNodes);
            return found;
        }
        private static long Dijkstra(int n, int src, int dst, int* head, int* to, int* next, long* dist, long* distTo, int* prev, int* blockedU, int* blockedV, int blockCount)
        {
            for (int i = 0; i < n; i++) { distTo[i] = long.MaxValue; prev[i] = -1; }
            distTo[src] = 0;
            long* heapDist = stackalloc long[n];
            int* heapV = stackalloc int[n];
            int* heapPos = stackalloc int[n];
            RemMinHeap pq = new RemMinHeap(n, heapDist, heapV, heapPos);
            pq.PushOrUpdate(src, 0);
            while (pq.Size > 0)
            {
                int u = pq.Pop(out long currentD);
                if (currentD > distTo[u]) continue;
                if (u == dst) break;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (IsEdgeBlocked(blockedU, blockedV, blockCount, u, v)) continue;
                    long nd = currentD + dist[e];
                    if (nd < distTo[v])
                    {
                        distTo[v] = nd;
                        prev[v] = u;
                        pq.PushOrUpdate(v, nd);
                    }
                }
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CollectBlockedEdge(int* pathNodes, int* pathLens, int n, int p, int spurIdx, bool* blockedU, bool* blockedV, int* blockedUList, int* blockedVList, ref int blockCount)
        {
            if (pathLens[p] <= spurIdx) return;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSamePath(int* pathNodes, int* pathLens, long* pathCostsArr, int n, int p, int* combinedPath, int bestSpurLen, long bestSpurCost)
        {
            if (pathCostsArr[p] != bestSpurCost || pathLens[p] != bestSpurLen) return false;
            bool same = true;
            for (int r = 0; r < bestSpurLen && same; r++)
            {
                if (pathNodes[p * n + r] != combinedPath[r])
                    same = false;
            }
            return same;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEdgeBlocked(int* blockedU, int* blockedV, int blockCount, int u, int v)
        {
            for (int b = 0; b < blockCount; b++)
            {
                if (blockedU[b] == u && blockedV[b] == v)
                    return true;
            }
            return false;
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
        // Lengauer-Tarjan immediate dominators. Edges use the head/to/next adjacency
        // where edge index 0 is the null sentinel. On return idom[v] is the immediate
        // dominator (a vertex id) of every vertex v reachable from root, with
        // idom[root] = -1 and idom[v] = -1 for vertices unreachable from root.
        //
        // The scratch buffers parent, semi, ancestor, label, bucket and parentNode
        // must each have length >= n; their contents on entry are ignored.
        // Internally semi/idom/ancestor/label/parent operate on DFS preorder numbers
        // (1..time); arr maps vertex -> dfs number and vertexOf maps dfs number -> vertex.
        public static void Run(int n, int root, int* head, int* to, int* next, int* parent, int* semi, int* idom, int* ancestor, int* label, int* bucket, int* parentNode)
        {
            // NOTE: idom (output, indexed by vertex) and parentNode (vertex -> DFS
            // parent vertex) use the caller's buffers (length >= n). All other
            // Lengauer-Tarjan state is indexed by 1-based DFS number (1..time, where
            // time can equal n), so it is allocated internally at size n+1 to avoid
            // an off-by-one overflow of the caller's length-n scratch buffers. The
            // parent/semi/ancestor/label/bucket parameters are retained for source
            // compatibility but are no longer read or written.
            for (int i = 0; i < n; i++)
            {
                idom[i] = -1;
                parentNode[i] = 0;
            }

            // DFS preorder. dfsNum is 1-based; index 0 is reserved as "no vertex".
            int* arr = stackalloc int[n];          // vertex -> dfs number (0 = unvisited)
            int* vertexOf = stackalloc int[n + 1]; // dfs number -> vertex
            for (int i = 0; i < n; i++) arr[i] = 0;

            int time = 0;
            Dfs(root, head, to, next, arr, vertexOf, parentNode, ref time);

            // Build a reverse (predecessor) adjacency. The semidominator computation
            // needs the IN-edges of each vertex, but head/to/next is a forward star
            // (OUT-edges). Count edges, then fill predecessor linked lists.
            int edgeCount = 0;
            for (int x = 0; x < n; x++)
                for (int e = head[x]; e != 0; e = next[e])
                    edgeCount++;

            int* predHead = stackalloc int[n];
            int* predNext = stackalloc int[edgeCount + 1];
            int* predFrom = stackalloc int[edgeCount + 1];
            for (int i = 0; i < n; i++) predHead[i] = -1;
            int pe = 0;
            for (int x = 0; x < n; x++)
            {
                for (int e = head[x]; e != 0; e = next[e])
                {
                    int y = to[e];      // edge x -> y
                    predFrom[pe] = x;   // record predecessor x of y
                    predNext[pe] = predHead[y];
                    predHead[y] = pe;
                    pe++;
                }
            }

            // Per-dfs-number LT state. Indices 1..time are live.
            int* semiNum = stackalloc int[n + 1];
            int* labelNum = stackalloc int[n + 1];
            int* ancestorNum = stackalloc int[n + 1];
            int* bucketHead = stackalloc int[n + 1];
            for (int i = 1; i <= time; i++)
            {
                semiNum[i] = i;
                labelNum[i] = i;
                ancestorNum[i] = 0;
                bucketHead[i] = 0; // bucket head: linked list over dfs numbers, 0 = empty
            }

            int* bucketNext = stackalloc int[n + 1];
            int* idomNum = stackalloc int[n + 1]; // provisional/final idom in DFS-number space
            for (int i = 0; i <= time; i++) idomNum[i] = 0;

            // Process vertices in reverse preorder, computing semidominators and
            // draining buckets immediately after each Link.
            for (int i = time; i >= 2; i--)
            {
                int w = vertexOf[i];
                // semi[i] = min over predecessors p of: dfs(p) if dfs(p) < i else semi[Eval(dfs(p))]
                for (int e = predHead[w]; e != -1; e = predNext[e])
                {
                    int pv = predFrom[e];
                    int pn = arr[pv];
                    if (pn == 0) continue; // predecessor not reachable (skip)
                    int candidate;
                    if (pn < i) candidate = pn;
                    else candidate = semiNum[Eval(pn, ancestorNum, labelNum, semiNum)];
                    if (candidate < semiNum[i]) semiNum[i] = candidate;
                }

                // Add i to bucket[semi[i]].
                int s = semiNum[i];
                bucketNext[i] = bucketHead[s];
                bucketHead[s] = i;

                int p = arr[parentNode[w]];
                LinkNode(p, i, ancestorNum);

                // Drain bucket[p]: for each v in bucket[p], idom is provisionally Eval(v).
                int vNum = bucketHead[p];
                bucketHead[p] = 0;
                while (vNum != 0)
                {
                    int evalV = Eval(vNum, ancestorNum, labelNum, semiNum);
                    idomNum[vNum] = (semiNum[evalV] < semiNum[vNum]) ? evalV : p;
                    vNum = bucketNext[vNum];
                }
            }

            // Finalize in DFS-number space (forward preorder), then translate to vertices.
            for (int i = 2; i <= time; i++)
            {
                if (idomNum[i] != semiNum[i]) idomNum[i] = idomNum[idomNum[i]];
            }
            for (int i = 2; i <= time; i++)
            {
                idom[vertexOf[i]] = vertexOf[idomNum[i]];
            }
            idom[root] = -1;
        }

        private static void Dfs(int u, int* head, int* to, int* next, int* arr, int* vertexOf, int* parentNode, ref int time)
        {
            arr[u] = ++time;
            vertexOf[time] = u;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (arr[v] == 0)
                {
                    parentNode[v] = u;
                    Dfs(v, head, to, next, arr, vertexOf, parentNode, ref time);
                }
            }
        }

        // All of v, ancestor[], label[], semi[] are indexed by dfs number.
        private static int Eval(int v, int* ancestor, int* label, int* semi)
        {
            if (ancestor[v] == 0) return label[v];
            Compress(v, ancestor, label, semi);
            return label[v];
        }

        private static void Compress(int v, int* ancestor, int* label, int* semi)
        {
            if (ancestor[ancestor[v]] == 0) return;
            Compress(ancestor[v], ancestor, label, semi);
            if (semi[label[ancestor[v]]] < semi[label[v]])
                label[v] = label[ancestor[v]];
            ancestor[v] = ancestor[ancestor[v]];
        }

        private static void LinkNode(int v, int w, int* ancestor)
        {
            ancestor[w] = v;
        }
    }
}
