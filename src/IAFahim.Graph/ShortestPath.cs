namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    internal unsafe struct SpMinHeap
    {
        public long* Dist;
        public int* V;
        public int* Pos;
        public int Size;

        public SpMinHeap(int n)
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
                Pos[v] = idx;
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

    public static unsafe class Dijkstra
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            for (int i = 0; i < n; i++) parent[i] = -1;
            dist[start] = 0;
            var pq = new SpMinHeap(n);
            try
            {
                pq.PushOrUpdate(start, 0);
                while (pq.Size > 0)
                {
                    int u = pq.Pop(out long d);
                    if (d != dist[u]) continue;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        long nd = dist[u] + weight[e];
                        if (nd < dist[v])
                        {
                            dist[v] = nd;
                            parent[v] = u;
                            pq.PushOrUpdate(v, nd);
                        }
                    }
                }
            }
            finally { pq.Dispose(); }
        }
    }

    public static unsafe class DijkstraSparse
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            // Identical heap-Dijkstra to Dijkstra.Run; forward to the single
            // implementation to avoid divergence. Public class kept for API stability.
            Dijkstra.Run(n, start, head, to, next, weight, dist, parent);
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
            int qh = 0, qt = 0, cnt = 0;
            dist[start] = 0;
            q[qt++] = start;
            if (qt >= n) qt = 0;
            cnt++;
            inqueue[start] = 1;
            int* count = stackalloc int[n];
            for (int i = 0; i < n; i++) count[i] = 0;
            count[start] = 1;
            while (cnt > 0)
            {
                int u = q[qh++];
                if (qh >= n) qh = 0;
                cnt--;
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
                            cnt++;
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
                int knBase = k * n;
                for (int i = 0; i < n; i++)
                {
                    int inBase = i * n;
                    long dik = dist[inBase + k];
                    if (dik == long.MaxValue) continue;
                    for (int j = 0; j < n; j++)
                    {
                        long dkj = dist[knBase + j];
                        if (dkj == long.MaxValue) continue;
                        long nd = dik + dkj;
                        if (nd < dist[inBase + j])
                        {
                            dist[inBase + j] = nd;
                            parent[inBase + j] = k;
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
            for (int i = 0; i < m + n; i++)
            {
                long uu = augU[i], vv = augV[i];
                if (bfDist[uu] != long.MaxValue && bfDist[uu] + augW[i] < bfDist[vv])
                    return false;
            }
            for (int i = 0; i < n; i++) h[i] = bfDist[i];
            for (int s = 0; s < n; s++)
            {
                for (int i = 0; i < n; i++) dist[s * n + i] = long.MaxValue;
                dist[s * n + s] = 0;
                var pq = new SpMinHeap(n);
                try
                {
                    pq.PushOrUpdate(s, 0);
                    while (pq.Size > 0)
                    {
                        int u = pq.Pop(out long d);
                        if (d != dist[s * n + u]) continue;
                        long hu = h[u];
                        for (int i = 0; i < m; i++)
                        {
                            if (eu[i] != u) continue;
                            int v = ev[i];
                            long nd = d + ew[i] + hu - h[v];
                            if (nd < dist[s * n + v])
                            {
                                dist[s * n + v] = nd;
                                pq.PushOrUpdate(v, nd);
                            }
                        }
                    }
                }
                finally { pq.Dispose(); }
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
            // A 0-1 BFS deque has no in-queue dedup, so a vertex may be enqueued
            // again on every strict improvement. Distances present in the deque
            // span at most {d, d+1}, so each vertex holds at most two live entries
            // (one per distance value): capacity 2*n is a safe upper bound. Stale
            // entries are skipped via the lazily-stored distance (dqDist).
            int cap = 2 * n;
            int* dq = stackalloc int[cap];
            long* dqDist = stackalloc long[cap];
            int dh = 0, dt = 0, cnt = 0;
            dq[dt] = start;
            dqDist[dt] = 0;
            dt++;
            cnt++;
            while (cnt > 0)
            {
                int u = dq[dh];
                long du = dqDist[dh];
                dh++;
                if (dh >= cap) dh = 0;
                cnt--;
                if (du != dist[u]) continue;
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
                            if (dh < 0) dh = cap - 1;
                            dq[dh] = v;
                            dqDist[dh] = nd;
                            cnt++;
                        }
                        else
                        {
                            dq[dt] = v;
                            dqDist[dt] = nd;
                            dt++;
                            if (dt >= cap) dt = 0;
                            cnt++;
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
            var pq = new SpMinHeap(n);
            try
            {
                pq.PushOrUpdate(start, 0);
                while (pq.Size > 0)
                {
                    int u = pq.Pop(out long d);
                    if (d != dist[u]) continue;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        long reduced = weight[e] + potential[u] - potential[v];
                        long nd = dist[u] + reduced;
                        if (nd < dist[v])
                        {
                            dist[v] = nd;
                            parent[v] = u;
                            pq.PushOrUpdate(v, nd);
                        }
                    }
                }
            }
            finally { pq.Dispose(); }
            for (int i = 0; i < n; i++)
            {
                if (dist[i] != long.MaxValue)
                    dist[i] = dist[i] - potential[start] + potential[i];
            }
        }
    }
}