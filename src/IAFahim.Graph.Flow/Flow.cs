namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public unsafe struct MinHeap
    {
        public long* Dist;
        public int* V;
        public int* Pos;
        public int Size;

        public MinHeap(int n)
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

    public static unsafe class PotentialDijkstra
    {
        public static bool Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, long* pot, long* dist, int* parent, int* parentEdge, MinHeap* pq)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            dist[s] = 0;
            pq->Size = 0;
            pq->PushOrUpdate(s, 0);
            while (pq->Size > 0)
            {
                int u = pq->Pop(out long d);
                if (d != dist[u]) continue;
                if (u == t) break;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    if (cap[e] <= 0) continue;
                    int v = to[e];
                    long nd = d + pot[u] + cost[e] - pot[v];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        parent[v] = u;
                        parentEdge[v] = e;
                        pq->PushOrUpdate(v, nd);
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                if (dist[i] < long.MaxValue) pot[i] += dist[i];
            }
            return dist[t] < long.MaxValue;
        }
    }

    public static unsafe class SuccessiveShortestPath
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost)
        {
            long flow = 0, minCost = 0;
            long* pot = stackalloc long[n];
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            var pq = new MinHeap(n);
            try
            {
                for (int i = 0; i < n; i++) pot[i] = 0;
                while (PotentialDijkstra.Run(n, s, t, head, to, next, cap, cost, pot, dist, parent, parentEdge, &pq))
                {
                    int add = int.MaxValue;
                    for (int v = t; v != s; v = parent[v])
                    {
                        int e = parentEdge[v];
                        add = Math.Min(add, cap[e]);
                    }
                    for (int v = t; v != s; v = parent[v])
                    {
                        int e = parentEdge[v];
                        cap[e] -= add;
                        cap[e ^ 1] += add;
                        minCost += (long)cost[e] * add;
                    }
                    flow += add;
                }
            }
            finally { pq.Dispose(); }
            return minCost;
        }
    }

    public static unsafe class CycleCanceling
    {
        public static long Run(int n, int m, int* from, int* to, int* cap, int* cost)
        {
            long minCost = 0;
            for (int i = 0; i < m; i++)
            {
                if (cap[i] > 0 && cost[i] < 0) minCost += (long)cap[i] * cost[i];
            }
            return minCost;
        }
    }

    public static unsafe class MaxFlowLowerBounds
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* lower, int* upper, int* flow, int* newHead, int* newTo, int* newNext, int* newCap, int* newCost)
        {
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
            int ss = n, tt = n + 1;
            int nn = n + 2;
            int newEdgeId = 2;
            for (int i = 0; i < nn; i++) newHead[i] = 0;
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    int lb = lower[e], ub = upper[e];
                    int cap = ub - lb;
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, newCost, newCap, &newEdgeId, u, v, 0, cap);
                    flow[e] = lb;
                }
            }
            long* b = stackalloc long[n];
            for (int i = 0; i < n; i++) b[i] = 0;
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    b[u] -= lower[e];
                    b[to[e]] += lower[e];
                }
            }
            long excess = 0, deficit = 0;
            for (int i = 0; i < n; i++)
            {
                if (b[i] > 0) excess += b[i];
                else deficit -= b[i];
                if (i != s && i != t) MinCostFlowAddEdge.Run(newHead, newTo, newNext, newCost, newCap, &newEdgeId, ss, i, 0, (int)Math.Max(0, b[i]));
                if (b[i] < 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, newCost, newCap, &newEdgeId, i, tt, 0, (int)Math.Max(0, -b[i]));
            }
            MinCostFlowAddEdge.Run(newHead, newTo, newNext, newCost, newCap, &newEdgeId, t, s, 0, int.MaxValue / 2);
            int* newFlow = stackalloc int[nn * 2];
            for (int i = 0; i < nn * 2; i++) newFlow[i] = 0;
            DinicMaxFlow.Run(nn, ss, tt, newHead, newTo, newNext, newCap, newFlow);
            return 0;
        }
    }

    public static unsafe class CirculationWithDemands
    {
        public static bool Run(int n, int* head, int* to, int* next, int* lower, int* upper, int* demand, int* flow)
        {
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
            int ss = n;
            int nn = n + 1;
            long totalDemand = 0;
            for (int i = 0; i < n; i++)
            {
                if (demand[i] > 0) totalDemand += demand[i];
            }
            return totalDemand == 0;
        }
    }


    public static unsafe class EdmondsKarp
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            for (int i = 0; i < n; i++) flow[i] = 0;
            long result = 0;
            while (true)
            {
                int* parent = stackalloc int[n];
                for (int i = 0; i < n; i++) parent[i] = -1;
                int* q = stackalloc int[n];
                int qh = 0, qt = 0;
                parent[s] = s;
                q[qt++] = s;
                while (qh < qt && parent[t] == -1)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (parent[v] == -1 && cap[e] - flow[e] > 0)
                        {
                            parent[v] = e;
                            q[qt++] = v;
                        }
                    }
                }
                if (parent[t] == -1) break;
                int add = int.MaxValue;
                for (int u = t; u != s; u = to[parent[u] ^ 1])
                {
                    int e = parent[u];
                    add = Math.Min(add, cap[e] - flow[e]);
                }
                for (int u = t; u != s; u = to[parent[u] ^ 1])
                {
                    int e = parent[u];
                    flow[e] += add;
                    flow[e ^ 1] -= add;
                }
                result += add;
            }
            return result;
        }
    }

    public static unsafe class DinicBfs
    {
        public static bool Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* level, int* it)
        {
            for (int i = 0; i < n; i++) level[i] = -1;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            level[s] = 0;
            q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (level[v] == -1 && cap[e] - flow[e] > 0)
                    {
                        level[v] = level[u] + 1;
                        q[qt++] = v;
                    }
                }
            }
            for (int i = 0; i < n; i++) it[i] = head[i];
            return level[t] != -1;
        }
    }

    public static unsafe class DinicDfs
    {
        public static int Run(int u, int t, int pushed, int* head, int* to, int* next, int* cap, int* flow, int* level, int* it)
        {
            if (u == t || pushed == 0) return pushed;
            for (int i = it[u]; i != 0; i = next[i])
            {
                it[u] = i;
                int v = to[i];
                if (level[v] == level[u] + 1 && cap[i] - flow[i] > 0)
                {
                    int tr = Run(v, t, Math.Min(pushed, cap[i] - flow[i]), head, to, next, cap, flow, level, it);
                    if (tr > 0)
                    {
                        flow[i] += tr;
                        flow[i ^ 1] -= tr;
                        return tr;
                    }
                }
            }
            return 0;
        }
    }

    public static unsafe class DinicMaxFlow
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            int* level = stackalloc int[n];
            int* it = stackalloc int[n];
            long result = 0;
            while (DinicBfs.Run(n, s, t, head, to, next, cap, flow, level, it))
            {
                while (true)
                {
                    int pushed = DinicDfs.Run(s, t, int.MaxValue, head, to, next, cap, flow, level, it);
                    if (pushed == 0) break;
                    result += pushed;
                }
            }
            return result;
        }
    }

    public static unsafe class MinCut
    {
        public static int Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, bool* visited)
        {
            for (int i = 0; i < n; i++) visited[i] = false;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            visited[s] = true;
            q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (!visited[v] && cap[e] - flow[e] > 0)
                    {
                        visited[v] = true;
                        q[qt++] = v;
                    }
                }
            }
            return 0;
        }
    }

    public static unsafe class FlowDecompose
    {
        public static int Run(int n, int s, int t, int m, int* eu, int* ev, int* cap, int* flow, int* pathEdges, int* pathLen, int* parent, int* edgeId, int* q)
        {
            int pathCount = 0;
            int* used = stackalloc int[m];
            for (int i = 0; i < m; i++) used[i] = 0;
            bool done = false;
            while (!done)
            {
                for (int i = 0; i < n; i++) { parent[i] = -1; edgeId[i] = -1; }
                int qh = 0, qt = 0;
                parent[s] = s;
                q[qt++] = s;
                while (qh < qt && parent[t] == -1)
                {
                    int u = q[qh++];
                    for (int i = 0; i < m; i++)
                    {
                        if (used[i] < cap[i])
                        {
                            int pu = eu[i], pv = ev[i];
                            if (pu == u && parent[pv] == -1)
                            {
                                parent[pv] = u;
                                edgeId[pv] = i;
                                q[qt++] = pv;
                            }
                        }
                    }
                }
                if (parent[t] == -1) { done = true; break; }
                int minCap = int.MaxValue;
                int cur = t;
                while (cur != s)
                {
                    int e = edgeId[cur];
                    minCap = Math.Min(minCap, cap[e] - used[e]);
                    cur = parent[cur];
                }
                cur = t;
                while (cur != s)
                {
                    int e = edgeId[cur];
                    used[e] += minCap;
                    cur = parent[cur];
                }
                pathCount++;
            }
            return pathCount;
        }
    }

    public static unsafe class MinCostFlowAddEdge
    {
        public static void Run(int* head, int* to, int* next, int* cost, int* cap, int* edgeId, int u, int v, int w, int c)
        {
            int id = (*edgeId)++;
            to[id] = v;
            cost[id] = w;
            cap[id] = c;
            next[id] = head[u];
            head[u] = id;
            id = (*edgeId)++;
            to[id] = u;
            cost[id] = -w;
            cap[id] = 0;
            next[id] = head[v];
            head[v] = id;
        }
    }

    public static unsafe class MinCostMaxFlow
    {
        public static (long flow, long minCost) Run(int n, int s, int t, int* head, int* to, int* next, int* cost, int* cap)
        {
            long flow = 0, minCost = 0;
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            var pq = new MinHeap(n);
            try
            {
                while (true)
                {
                    for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
                    dist[s] = 0;
                    pq.Size = 0;
                    pq.PushOrUpdate(s, 0);
                    while (pq.Size > 0)
                    {
                        int u = pq.Pop(out long d);
                        if (d != dist[u]) continue;
                        for (int e = head[u]; e != 0; e = next[e])
                        {
                            if (cap[e] <= 0) continue;
                            int v = to[e];
                            long nd = d + cost[e];
                            if (nd < dist[v])
                            {
                                dist[v] = nd;
                                parent[v] = u;
                                parentEdge[v] = e;
                                pq.PushOrUpdate(v, nd);
                            }
                        }
                    }
                    if (dist[t] == long.MaxValue) break;
                    int add = int.MaxValue;
                    for (int v = t; v != s; v = parent[v])
                    {
                        int e = parentEdge[v];
                        add = Math.Min(add, cap[e]);
                    }
                    for (int v = t; v != s; v = parent[v])
                    {
                        int e = parentEdge[v];
                        cap[e] -= add;
                        cap[e ^ 1] += add;
                        minCost += (long)cost[e] * add;
                    }
                    flow += add;
                }
            }
            finally { pq.Dispose(); }
            return (flow, minCost);
        }
    }
}