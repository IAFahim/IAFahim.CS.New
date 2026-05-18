namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinimumSpanningTreeKruskal
    {
        private static int Find(int* parent, int x)
        {
            while (parent[x] != x) x = parent[x];
            return x;
        }

        private static void Union(int* parent, int* size, int a, int b)
        {
            int ra = Find(parent, a);
            int rb = Find(parent, b);
            if (ra == rb) return;
            if (size[ra] < size[rb]) { int t = ra; ra = rb; rb = t; }
            parent[rb] = ra;
            size[ra] += size[rb];
        }

        public static long Run(int n, int m, int* eu, int* ev, int* ew, int* mstEdges)
        {
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            for (int i = 0; i < n; i++) { parent[i] = i; size[i] = 1; }
            int* idx = stackalloc int[m];
            for (int i = 0; i < m; i++) idx[i] = i;
            for (int i = 1; i < m; i++)
            {
                int key = ew[i];
                int j = i - 1;
                while (j >= 0 && ew[idx[j]] > key)
                {
                    idx[j + 1] = idx[j];
                    j--;
                }
                idx[j + 1] = i;
            }
            long mstWeight = 0;
            int edgeCount = 0;
            for (int i = 0; i < m && edgeCount < n - 1; i++)
            {
                int e = idx[i];
                int u = eu[e], v = ev[e];
                if (Find(parent, u) != Find(parent, v))
                {
                    mstEdges[edgeCount++] = e;
                    mstWeight += ew[e];
                    Union(parent, size, u, v);
                }
            }
            return edgeCount == n - 1 ? mstWeight : -1;
        }
    }

    public static unsafe class MinimumSpanningTreePrim
    {
        public static long Run(int n, int* head, int* to, int* next, int* weight, int* mstEdges)
        {
            bool* used = stackalloc bool[n];
            for (int i = 0; i < n; i++) used[i] = false;
            long mstWeight = 0;
            int edgeCount = 0;
            used[0] = true;
            var pq = new System.Collections.Generic.SortedSet<(int w, int u, int e)>();
            for (int e = head[0]; e != 0; e = next[e])
                pq.Add((weight[e], to[e], e));
            while (pq.Count > 0 && edgeCount < n - 1)
            {
                var cur = pq.Min;
                pq.Remove(cur);
                if (used[cur.u]) continue;
                used[cur.u] = true;
                mstEdges[edgeCount++] = cur.e;
                mstWeight += cur.w;
                for (int e = head[cur.u]; e != 0; e = next[e])
                {
                    if (!used[to[e]])
                        pq.Add((weight[e], to[e], e));
                }
            }
            return edgeCount == n - 1 ? mstWeight : -1;
        }
    }

    public static unsafe class SecondBestMst
    {
        private static int Find(int* parent, int x)
        {
            while (parent[x] != x) x = parent[x];
            return x;
        }

        public static long Run(int n, int m, int* eu, int* ev, int* ew)
        {
            long best = 0;
            {
                int* parent = stackalloc int[n];
                int* size = stackalloc int[n];
                for (int i = 0; i < n; i++) { parent[i] = i; size[i] = 1; }
                int* idx = stackalloc int[m];
                for (int i = 0; i < m; i++) idx[i] = i;
                for (int i = 1; i < m; i++)
                {
                    int key = ew[i];
                    int j = i - 1;
                    while (j >= 0 && ew[idx[j]] > key) { idx[j + 1] = idx[j]; j--; }
                    idx[j + 1] = i;
                }
                int count = 0;
                for (int i = 0; i < m && count < n - 1; i++)
                {
                    int e = idx[i];
                    int pu = Find(parent, eu[e]);
                    int pv = Find(parent, ev[e]);
                    if (pu != pv) { parent[pu] = pv; best += ew[e]; count++; }
                }
                if (count < n - 1) return -1;
            }
            long second = long.MaxValue;
            for (int skip = 0; skip < m; skip++)
            {
                int* parent = stackalloc int[n];
                for (int i = 0; i < n; i++) parent[i] = i;
                int count = 0;
                long alt = 0;
                for (int j = 0; j < m; j++)
                {
                    if (j == skip) continue;
                    int pu = Find(parent, eu[j]);
                    int pv = Find(parent, ev[j]);
                    if (pu != pv) { parent[pu] = pv; alt += ew[j]; count++; }
                }
                if (count == n - 1) second = Math.Min(second, alt);
            }
            return second == long.MaxValue ? -1 : second;
        }
    }
}