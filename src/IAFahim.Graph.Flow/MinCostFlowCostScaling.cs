namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowCostScaling
    {
        private const int NoParent = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RunSpfaSearch(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, long* dist, int* parent, int* parentEdge, long* potential, int* q, byte* inq, int* cnt)
        {
            for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = NoParent; inq[i] = 0; cnt[i] = 0; }
            dist[s] = 0;
            int qh = 0, qt = 0;
            q[qt++] = s; inq[s] = 1; cnt[s] = 1;
            while (qh != qt)
            {
                int u = q[qh++]; if (qh > n) qh = 0; inq[u] = 0;
                long pu = potential[u];
                long du = dist[u];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    if (cap[e] - flow[e] <= 0) continue;
                    int v = to[e];
                    long nd = du + cost[e] + pu - potential[v];
                    if (nd < dist[v])
                    {
                        dist[v] = nd; parent[v] = u; parentEdge[v] = e;
                        if (inq[v] == 0)
                        {
                            q[qt++] = v; if (qt > n) qt = 0; inq[v] = 1;
                            if (++cnt[v] > n) { qh = qt; break; }
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdatePotentials(long* potential, long* dist, int n)
        {
            for (int i = 0; i < n; i++) if (dist[i] < long.MaxValue) potential[i] += dist[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PathCapacity(int s, int t, int* parent, int* parentEdge, int* cap, int* flow)
        {
            int add = int.MaxValue;
            for (int v = t; v != s; v = parent[v])
            {
                int e = parentEdge[v];
                int res = cap[e] - flow[e];
                if (res < add) add = res;
            }
            return add;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AugmentPath(int s, int t, int* parent, int* parentEdge, int* flow, int* cost, int add, ref long totalCost)
        {
            for (int v = t; v != s; v = parent[v])
            {
                int e = parentEdge[v];
                flow[e] += add; flow[e ^ 1] -= add;
                totalCost += (long)cost[e] * add;
            }
        }

        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow)
        {
            long totalCost = 0;
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            long* potential = stackalloc long[n];
            int* q = stackalloc int[n + 1];
            byte* inq = stackalloc byte[n];
            int* cnt = stackalloc int[n];
            for (int i = 0; i < n; i++) potential[i] = 0;
            while (true)
            {
                RunSpfaSearch(n, s, t, head, to, next, cap, cost, flow, dist, parent, parentEdge, potential, q, inq, cnt);
                if (dist[t] == long.MaxValue) break;
                UpdatePotentials(potential, dist, n);
                int add = PathCapacity(s, t, parent, parentEdge, cap, flow);
                AugmentPath(s, t, parent, parentEdge, flow, cost, add, ref totalCost);
            }
            return totalCost;
        }
    }
}
