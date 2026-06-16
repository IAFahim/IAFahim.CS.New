namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowDijkstra
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow)
        {
            long totalCost = 0;
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            long* pot = stackalloc long[n];
            for (int i = 0; i < n; i++) pot[i] = 0;
            int* pqV = stackalloc int[n];
            long* pqD = stackalloc long[n];

            while (true)
            {
                for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
                dist[s] = 0;
                int pqSize = 0;
                pqV[pqSize] = s; pqD[pqSize] = 0; pqSize++;
                while (pqSize > 0)
                {
                    int u = pqV[0]; long du = pqD[0];
                    pqSize--;
                    pqV[0] = pqV[pqSize]; pqD[0] = pqD[pqSize];
                    int idx = 0;
                    while (true)
                    {
                        int l = idx * 2 + 1, r = idx * 2 + 2, smallest = idx;
                        if (l < pqSize && pqD[l] < pqD[smallest]) smallest = l;
                        if (r < pqSize && pqD[r] < pqD[smallest]) smallest = r;
                        if (smallest == idx) break;
                        long td = pqD[idx]; pqD[idx] = pqD[smallest]; pqD[smallest] = td;
                        int tv = pqV[idx]; pqV[idx] = pqV[smallest]; pqV[smallest] = tv;
                        idx = smallest;
                    }
                    if (du != dist[u]) continue;
                    if (u == t) break;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int v = to[e];
                        long nd = dist[u] + cost[e] + pot[u] - pot[v];
                        if (nd < dist[v]) { dist[v] = nd; parent[v] = u; parentEdge[v] = e; pqV[pqSize] = v; pqD[pqSize] = nd; pqSize++; int pidx = pqSize - 1; while (pidx > 0) { int p = (pidx - 1) / 2; if (pqD[p] <= pqD[pidx]) break; long ttd = pqD[pidx]; pqD[pidx] = pqD[p]; pqD[p] = ttd; int ttv = pqV[pidx]; pqV[pidx] = pqV[p]; pqV[p] = ttv; pidx = p; } }
                    }
                }
                if (dist[t] == long.MaxValue) break;
                int add = int.MaxValue;
                for (int v = t; v != s; v = parent[v]) add = Math.Min(add, cap[parentEdge[v]] - flow[parentEdge[v]]);
                for (int v = t; v != s; v = parent[v]) { int e = parentEdge[v]; flow[e] += add; flow[e ^ 1] -= add; totalCost += (long)cost[e] * add; }
                for (int i = 0; i < n; i++) if (dist[i] < long.MaxValue) pot[i] += dist[i];
            }
            return totalCost;
        }
    }
}
