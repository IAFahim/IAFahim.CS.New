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

    public static unsafe class MinCostFlowAddEdge
    {
        public static void Run(int* head, int* to, int* next, int* cost, int* cap, int* edgeId, int u, int v, int c, int w)
        {
            to[*edgeId] = v; next[*edgeId] = head[u]; cost[*edgeId] = c; cap[*edgeId] = w; head[u] = (*edgeId)++;
            to[*edgeId] = u; next[*edgeId] = head[v]; cost[*edgeId] = -c; cap[*edgeId] = 0; head[v] = (*edgeId)++;
        }
    }

    public static unsafe class DinicBfs
    {
        public static bool Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* level, int* it)
        {
            for (int i = 0; i < n; i++) { level[i] = -1; it[i] = head[i]; }
            int* q = stackalloc int[n]; int qh = 0, qt = 0;
            level[s] = 0; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                    if (cap[e] - flow[e] > 0 && level[to[e]] == -1)
                    {
                        level[to[e]] = level[u] + 1;
                        q[qt++] = to[e];
                    }
            }
            return level[t] != -1;
        }
    }

    public static unsafe class DinicDfs
    {
        public static int Run(int u, int t, int pushed, int* head, int* to, int* next, int* cap, int* flow, int* level, int* it)
        {
            if (pushed == 0 || u == t) return pushed;
            for (int e = it[u]; e != 0; e = next[e])
            {
                it[u] = e;
                if (level[to[e]] != level[u] + 1 || cap[e] - flow[e] <= 0) continue;
                int tr = Run(to[e], t, Math.Min(pushed, cap[e] - flow[e]), head, to, next, cap, flow, level, it);
                if (tr == 0) continue;
                flow[e] += tr; flow[e ^ 1] -= tr;
                return tr;
            }
            return 0;
        }
    }

    public static unsafe class PotentialDijkstra
    {
        public static bool Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, long* pot, long* dist, int* parent, int* parentEdge, MinHeap* pq)
        {
            InitializeDistances(n, s, dist, pq);
            while (pq->Size > 0)
            {
                int u = pq->Pop(out long d);
                if (d != dist[u]) continue;
                if (u == t) break;
                RelaxEdges(u, head, to, next, cap, cost, pot, dist, parent, parentEdge, pq);
            }
            UpdatePotentials(n, pot, dist);
            return dist[t] < long.MaxValue;
        }

        private static void InitializeDistances(int n, int s, long* dist, MinHeap* pq)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            dist[s] = 0;
            pq->Size = 0;
            pq->PushOrUpdate(s, 0);
        }

        private static void RelaxEdges(int u, int* head, int* to, int* next, int* cap, int* cost, long* pot, long* dist, int* parent, int* parentEdge, MinHeap* pq)
        {
            for (int e = head[u]; e != 0; e = next[e])
            {
                if (cap[e] <= 0) continue;
                int v = to[e];
                long nd = dist[u] + pot[u] + cost[e] - pot[v];
                if (nd < dist[v])
                {
                    dist[v] = nd;
                    parent[v] = u;
                    parentEdge[v] = e;
                    pq->PushOrUpdate(v, nd);
                }
            }
        }

        private static void UpdatePotentials(int n, long* pot, long* dist)
        {
            for (int i = 0; i < n; i++)
                if (dist[i] < long.MaxValue) pot[i] += dist[i];
        }
    }

    public static unsafe class SuccessiveShortestPath
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost)
        {
            long minCost = 0;
            long* pot = stackalloc long[n];
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            
            MinHeap pq = CreateHeap(n);
            for (int i = 0; i < n; i++) pot[i] = 0;

            while (PotentialDijkstra.Run(n, s, t, head, to, next, cap, cost, pot, dist, parent, parentEdge, &pq))
            {
                int add = FindMinCapacity(s, t, parent, parentEdge, cap);
                minCost += UpdateFlow(s, t, add, parent, parentEdge, cap, cost);
            }
            return minCost;
        }

        private static MinHeap CreateHeap(int n)
        {
            long* pqDist = stackalloc long[n];
            int* pqV = stackalloc int[n];
            int* pqPos = stackalloc int[n];
            for (int i = 0; i < n; i++) pqPos[i] = -1;
            return new MinHeap { Dist = pqDist, V = pqV, Pos = pqPos, Size = 0 };
        }

        private static int FindMinCapacity(int s, int t, int* parent, int* parentEdge, int* cap)
        {
            int add = int.MaxValue;
            for (int v = t; v != s; v = parent[v])
                add = Math.Min(add, cap[parentEdge[v]]);
            return add;
        }

        private static long UpdateFlow(int s, int t, int add, int* parent, int* parentEdge, int* cap, int* cost)
        {
            long costInc = 0;
            for (int v = t; v != s; v = parent[v])
            {
                int e = parentEdge[v];
                cap[e] -= add;
                cap[e ^ 1] += add;
                costInc += (long)cost[e] * add;
            }
            return costInc;
        }
    }

    public static unsafe class MaxFlowLowerBounds
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* lower, int* upper, int* flow, int* newHead, int* newTo, int* newNext, int* newCap, int* newCost)
        {
            InitializeFlow(n, flow);
            int ss = n, tt = n + 1, nn = n + 2, edgeId = 2;
            for (int i = 0; i < nn; i++) newHead[i] = 0;

            long* b = stackalloc long[n];
            for (int i = 0; i < n; i++) b[i] = 0;

            BuildAuxiliaryGraph(n, head, to, next, lower, upper, b, newHead, newTo, newNext, newCost, newCap, &edgeId);
            AddSuperSourceSink(n, s, t, ss, tt, b, newHead, newTo, newNext, newCost, newCap, &edgeId);

            int* newFlow = stackalloc int[nn * 2 + edgeId]; // Sufficient size
            for (int i = 0; i < nn * 2 + edgeId; i++) newFlow[i] = 0;
            DinicMaxFlow.Run(nn, ss, tt, newHead, newTo, newNext, newCap, newFlow);
            return 0;
        }

        private static void InitializeFlow(int n, int* flow)
        {
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
        }

        private static void BuildAuxiliaryGraph(int n, int* head, int* to, int* next, int* lower, int* upper, long* b, int* newHead, int* newTo, int* newNext, int* newCost, int* newCap, int* edgeId)
        {
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int lb = lower[e], ub = upper[e];
                    MinCostFlowAddEdge.Run(newHead, newTo, newNext, newCost, newCap, edgeId, u, to[e], 0, ub - lb);
                    b[u] -= lb; b[to[e]] += lb;
                }
        }

        private static void AddSuperSourceSink(int n, int s, int t, int ss, int tt, long* b, int* newHead, int* newTo, int* newNext, int* newCost, int* newCap, int* edgeId)
        {
            for (int i = 0; i < n; i++)
            {
                if (b[i] > 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, newCost, newCap, edgeId, ss, i, 0, (int)b[i]);
                if (b[i] < 0) MinCostFlowAddEdge.Run(newHead, newTo, newNext, newCost, newCap, edgeId, i, tt, 0, (int)-b[i]);
            }
            MinCostFlowAddEdge.Run(newHead, newTo, newNext, newCost, newCap, edgeId, t, s, 0, int.MaxValue / 2);
        }
    }

    public static unsafe class EdmondsKarp
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            int maxEdge = 0;
            for (int i = 0; i < n; i++)
            {
                for (int e = head[i]; e != 0; e = next[e])
                {
                    if (e > maxEdge)
                    {
                        maxEdge = e;
                    }
                }
            }
            int limit = maxEdge | 1;
            for (int i = 0; i <= limit; i++)
            {
                flow[i] = 0;
            }
            long totalFlow = 0;
            int* parent = stackalloc int[n];
            int* q = stackalloc int[n];
            while (TryFindPath(n, s, t, head, to, next, cap, flow, parent, q))
            {
                int add = FindPathMinCapacity(s, t, to, parent, cap, flow);
                UpdatePathFlow(s, t, add, to, parent, flow);
                totalFlow += add;
            }
            return totalFlow;
        }

        private static bool TryFindPath(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* parent, int* q)
        {
            for (int i = 0; i < n; i++) parent[i] = -1;
            int qh = 0, qt = 0;
            parent[s] = s; q[qt++] = s;
            while (qh < qt && parent[t] == -1)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                    if (parent[to[e]] == -1 && cap[e] - flow[e] > 0)
                    {
                        parent[to[e]] = e;
                        q[qt++] = to[e];
                    }
            }
            return parent[t] != -1;
        }

        private static int FindPathMinCapacity(int s, int t, int* to, int* parent, int* cap, int* flow)
        {
            int add = int.MaxValue;
            for (int u = t; u != s; u = to[parent[u] ^ 1])
                add = Math.Min(add, cap[parent[u]] - flow[parent[u]]);
            return add;
        }

        private static void UpdatePathFlow(int s, int t, int add, int* to, int* parent, int* flow)
        {
            for (int u = t; u != s; u = to[parent[u] ^ 1])
            {
                int e = parent[u];
                flow[e] += add;
                flow[e ^ 1] -= add;
            }
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

    public static unsafe class MinCostMaxFlow
    {
        public static (long flow, long minCost) Run(int n, int s, int t, int* head, int* to, int* next, int* cost, int* cap)
        {
            long flow = 0, minCost = 0;
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n], parentEdge = stackalloc int[n];

            while (true)
            {
                if (!TryFindMinCostPath(n, s, t, head, to, next, cost, cap, dist, parent, parentEdge)) break;
                int add = FindPathMinCapacity(s, t, parent, parentEdge, cap);
                minCost += UpdateFlow(s, t, add, parent, parentEdge, cap, cost, out int f);
                flow += add;
            }
            return (flow, minCost);
        }

        // SPFA (Bellman-Ford queue): correct under negative-cost residual edges.
        // Min-cost-flow residual graphs contain no negative cycles, so this terminates.
        private static bool TryFindMinCostPath(int n, int s, int t, int* head, int* to, int* next, int* cost, int* cap, long* dist, int* parent, int* parentEdge)
        {
            for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = -1; }
            dist[s] = 0;
            int* q = stackalloc int[n];
            byte* inq = stackalloc byte[n];
            for (int i = 0; i < n; i++) inq[i] = 0;
            int qh = 0, qt = 0;
            q[qt++] = s; inq[s] = 1;
            while (qh != qt)
            {
                int u = q[qh++]; if (qh == n) qh = 0; inq[u] = 0;
                long du = dist[u];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    if (cap[e] <= 0) continue;
                    int v = to[e];
                    long nd = du + cost[e];
                    if (nd < dist[v])
                    {
                        dist[v] = nd; parent[v] = u; parentEdge[v] = e;
                        if (inq[v] == 0)
                        {
                            q[qt++] = v; if (qt == n) qt = 0; inq[v] = 1;
                        }
                    }
                }
            }
            return dist[t] != long.MaxValue;
        }

        private static int FindPathMinCapacity(int s, int t, int* parent, int* parentEdge, int* cap)
        {
            int add = int.MaxValue;
            for (int v = t; v != s; v = parent[v]) add = Math.Min(add, cap[parentEdge[v]]);
            return add;
        }

        private static long UpdateFlow(int s, int t, int add, int* parent, int* parentEdge, int* cap, int* cost, out int dummy)
        {
            long costInc = 0; dummy = 0;
            for (int v = t; v != s; v = parent[v])
            {
                int e = parentEdge[v];
                cap[e] -= add; cap[e ^ 1] += add;
                costInc += (long)cost[e] * add;
            }
            return costInc;
        }
    }
}
