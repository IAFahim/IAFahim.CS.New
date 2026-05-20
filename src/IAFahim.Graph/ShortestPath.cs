namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Dijkstra
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            for (int i = 0; i < n; i++) parent[i] = -1;
            dist[start] = 0;
            var pq = new System.Collections.Generic.SortedSet<(long d, int v)>();
            pq.Add((0, start));
            while (pq.Count > 0)
            {
                var cur = pq.Min;
                pq.Remove(cur);
                if (cur.d != dist[cur.v]) continue;
                int u = cur.v;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long nd = dist[u] + weight[e];
                    if (nd < dist[v])
                    {
                        pq.Remove((dist[v], v));
                        dist[v] = nd;
                        parent[v] = u;
                        pq.Add((nd, v));
                    }
                }
            }
        }
    }

    public static unsafe class DijkstraSparse
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            for (int i = 0; i < n; i++) parent[i] = -1;
            dist[start] = 0;
            var pq = new System.Collections.Generic.SortedSet<(long d, int v)>();
            pq.Add((0, start));
            while (pq.Count > 0)
            {
                var cur = pq.Min;
                pq.Remove(cur);
                if (cur.d != dist[cur.v]) continue;
                int u = cur.v;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long nd = dist[u] + weight[e];
                    if (nd < dist[v])
                    {
                        pq.Remove((dist[v], v));
                        dist[v] = nd;
                        parent[v] = u;
                        pq.Add((nd, v));
                    }
                }
            }
        }
    }

    public static unsafe class DijkstraDense
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            for (int i = 0; i < n; i++) parent[i] = -1;
            bool* used = stackalloc bool[n];
            for (int i = 0; i < n; i++) used[i] = false;
            dist[start] = 0;
            for (int iter = 0; iter < n; iter++)
            {
                int v = -1;
                long best = long.MaxValue;
                for (int i = 0; i < n; i++)
                {
                    if (!used[i] && dist[i] < best)
                    {
                        best = dist[i];
                        v = i;
                    }
                }
                if (v == -1 || best == long.MaxValue) break;
                used[v] = true;
                for (int e = head[v]; e != 0; e = next[e])
                {
                    int u = to[e];
                    long nd = dist[v] + weight[e];
                    if (nd < dist[u])
                    {
                        dist[u] = nd;
                        parent[u] = v;
                    }
                }
            }
        }
    }

    public static unsafe class DijkstraRestorePath
    {
        public static int Run(int* parent, int target, int* path)
        {
            int len = 0;
            int cur = target;
            while (cur != -1)
            {
                path[len++] = cur;
                cur = parent[cur];
            }
            for (int i = 0; i < len / 2; i++)
            {
                int tmp = path[i];
                path[i] = path[len - 1 - i];
                path[len - 1 - i] = tmp;
            }
            return len;
        }
    }

    public static unsafe class BellmanFord
    {
        public static bool Run(int n, int start, int m, int* eu, int* ev, int* ew, long* dist, int* parent)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            for (int i = 0; i < n; i++) parent[i] = -1;
            dist[start] = 0;
            for (int iter = 0; iter < n - 1; iter++)
            {
                bool changed = false;
                for (int i = 0; i < m; i++)
                {
                    if (dist[eu[i]] == long.MaxValue) continue;
                    long nd = dist[eu[i]] + ew[i];
                    if (nd < dist[ev[i]])
                    {
                        dist[ev[i]] = nd;
                        parent[ev[i]] = eu[i];
                        changed = true;
                    }
                }
                if (!changed) break;
            }
            for (int i = 0; i < m; i++)
            {
                if (dist[eu[i]] != long.MaxValue && dist[eu[i]] + ew[i] < dist[ev[i]])
                    return false;
            }
            return true;
        }
    }

    public static unsafe class Spfa
    {
        public static bool Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent, int* inqueue)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            for (int i = 0; i < n; i++) parent[i] = -1;
            for (int i = 0; i < n; i++) inqueue[i] = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            dist[start] = 0;
            q[qt++] = start;
            inqueue[start] = 1;
            int* count = stackalloc int[n];
            for (int i = 0; i < n; i++) count[i] = 0;
            count[start] = 1;
            while (qh < qt)
            {
                int u = q[qh++];
                if (qh >= n) qh = 0;
                inqueue[u] = 0;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long nd = dist[u] + weight[e];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        parent[v] = u;
                        if (inqueue[v] == 0)
                        {
                            q[qt++] = v;
                            if (qt >= n) qt = 0;
                            inqueue[v] = 1;
                            count[v]++;
                            if (count[v] > n) return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    public static unsafe class FloydWarshall
    {
        public static void Run(int n, long* dist, int* parent)
        {
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (dist[i * n + k] == long.MaxValue) continue;
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[k * n + j] == long.MaxValue) continue;
                        long nd = dist[i * n + k] + dist[k * n + j];
                        if (nd < dist[i * n + j])
                        {
                            dist[i * n + j] = nd;
                            parent[i * n + j] = k;
                        }
                    }
                }
            }
        }
    }

    public static unsafe class Johnson
    {
        public static bool Run(int n, int start, int m, int* eu, int* ev, int* ew, long* dist)
        {
            long* h = stackalloc long[n];
            int* parent = stackalloc int[n];
            long* augU = stackalloc long[m + n];
            long* augV = stackalloc long[m + n];
            long* augW = stackalloc long[m + n];
            for (int i = 0; i < m; i++)
            {
                augU[i] = eu[i];
                augV[i] = ev[i];
                augW[i] = ew[i];
            }
            for (int i = 0; i < n; i++)
            {
                augU[m + i] = n;
                augV[m + i] = i;
                augW[m + i] = 0;
            }
            long* bfDist = stackalloc long[n + 1];
            for (int i = 0; i <= n; i++) bfDist[i] = 0;
            for (int iter = 0; iter < n; iter++)
            {
                bool changed = false;
                for (int i = 0; i < m + n; i++)
                {
                    long uu = augU[i], vv = augV[i];
                    if (bfDist[uu] != long.MaxValue && bfDist[uu] + augW[i] < bfDist[vv])
                    {
                        bfDist[vv] = bfDist[uu] + augW[i];
                        changed = true;
                    }
                }
                if (!changed) break;
            }
            for (int i = 0; i < n; i++) h[i] = bfDist[i];
            for (int s = 0; s < n; s++)
            {
                for (int i = 0; i < n; i++) dist[s * n + i] = long.MaxValue;
                dist[s * n + s] = 0;
                var pq = new System.Collections.Generic.SortedSet<(long d, int v)>();
                pq.Add((0, s));
                while (pq.Count > 0)
                {
                    var cur = pq.Min;
                    pq.Remove(cur);
                    if (cur.d != dist[s * n + cur.v]) continue;
                    for (int i = 0; i < m; i++)
                    {
                        if (eu[i] != cur.v) continue;
                        int v = ev[i];
                        long nd = cur.d + ew[i] + h[s] - h[v];
                        if (nd < dist[s * n + v])
                        {
                            pq.Remove((dist[s * n + v], v));
                            dist[s * n + v] = nd;
                            pq.Add((nd, v));
                        }
                    }
                }
                for (int t = 0; t < n; t++)
                {
                    if (dist[s * n + t] != long.MaxValue)
                        dist[s * n + t] = dist[s * n + t] - h[s] + h[t];
                }
            }
            return true;
        }
    }

    public static unsafe class ZeroOneShortestPath
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            dist[start] = 0;
            int* dq = stackalloc int[n];
            int dh = 0, dt = 0;
            dq[dt++] = start;
            while (dh < dt)
            {
                int u = dq[dh++];
                if (dh >= n) dh = 0;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long nd = dist[u] + weight[e];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        if (weight[e] == 0)
                        {
                            dh--;
                            if (dh < 0) dh = n - 1;
                            dq[dh] = v;
                        }
                        else
                        {
                            dq[dt++] = v;
                            if (dt >= n) dt = 0;
                        }
                    }
                }
            }
        }
    }

    public static unsafe class PotentialDijkstra
    {
        public static void Run(int n, int start, int* head, int* to, int* next, long* weight, long* dist, int* parent, long* potential)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            for (int i = 0; i < n; i++) parent[i] = -1;
            dist[start] = 0;
            var pq = new System.Collections.Generic.SortedSet<(long d, int v)>();
            pq.Add((0, start));
            while (pq.Count > 0)
            {
                var cur = pq.Min;
                pq.Remove(cur);
                if (cur.d != dist[cur.v]) continue;
                int u = cur.v;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long reduced = weight[e] + potential[u] - potential[v];
                    long nd = dist[u] + reduced;
                    if (nd < dist[v])
                    {
                        pq.Remove((dist[v], v));
                        dist[v] = nd;
                        parent[v] = u;
                        pq.Add((nd, v));
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                if (dist[i] != long.MaxValue)
                    dist[i] = dist[i] - potential[start] + potential[i];
            }
        }
    }
}