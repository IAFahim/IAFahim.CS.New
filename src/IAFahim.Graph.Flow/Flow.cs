namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

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
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap)
        {
            int* level = stackalloc int[n];
            int* it = stackalloc int[n];
            int* flow = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
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
        public static int Run(int n, int s, int t, int m, int* eu, int* ev, int* cap, int* flow, int* pathEdges, int* pathLen)
        {
            int pathCount = 0;
            int* used = stackalloc int[m];
            for (int i = 0; i < m; i++) used[i] = 0;
            bool done = false;
            while (!done)
            {
                int* parent = stackalloc int[n];
                int* edgeId = stackalloc int[n];
                for (int i = 0; i < n; i++) { parent[i] = -1; edgeId[i] = -1; }
                int* q = stackalloc int[n];
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
            int id = ++(*edgeId);
            to[id] = v;
            cost[id] = w;
            cap[id] = c;
            next[id] = head[u];
            head[u] = id;
            id = ++(*edgeId);
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
            while (true)
            {
                for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
                dist[s] = 0;
                var pq = new System.Collections.Generic.SortedSet<(long d, int v)>();
                pq.Add((0, s));
                while (pq.Count > 0)
                {
                    var cur = pq.Min;
                    pq.Remove(cur);
                    if (cur.d != dist[cur.v]) continue;
                    for (int e = head[cur.v]; e != 0; e = next[e])
                    {
                        if (cap[e] <= 0) continue;
                        int v = to[e];
                        long nd = cur.d + cost[e];
                        if (nd < dist[v])
                        {
                            dist[v] = nd;
                            parent[v] = cur.v;
                            parentEdge[v] = e;
                            pq.Add((nd, v));
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
            return (flow, minCost);
        }
    }
}