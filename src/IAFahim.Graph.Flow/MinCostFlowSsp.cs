namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowSsp
    {
        private const int NoParent = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DijkstraSearch(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, long* dist, int* parent, int* parentEdge, long* pot, long* pqDist, int* pqV, int* pqPos, int* pqSize)
        {
            for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = NoParent; pqPos[i] = -1; }
            dist[s] = 0;
            PushHeap(pqV, pqDist, pqPos, pqSize, s, 0);
            while (*pqSize > 0)
            {
                int u = PopMin(pqV, pqDist, pqPos, pqSize);
                if (u == t) break;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    if (cap[e] - flow[e] <= 0) continue;
                    int v = to[e];
                    long nd = dist[u] + cost[e] + pot[u] - pot[v];
                    if (nd < dist[v])
                    {
                        dist[v] = nd; parent[v] = u; parentEdge[v] = e;
                        PushHeap(pqV, pqDist, pqPos, pqSize, v, nd);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdatePotentials(int n, long* pot, long* dist)
        {
            for (int i = 0; i < n; i++) if (dist[i] < long.MaxValue) pot[i] += dist[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PathCapacity(int s, int t, int* parent, int* parentEdge, int* cap, int* flow)
        {
            int add = int.MaxValue;
            int v = t;
            while (v != s) { int e = parentEdge[v]; add = Math.Min(add, cap[e] - flow[e]); v = parent[v]; }
            return add;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AugmentPath(int s, int t, int* parent, int* parentEdge, int* flow, int* cost, int add, ref long totalCost)
        {
            int v = t;
            while (v != s) { int e = parentEdge[v]; flow[e] += add; flow[e ^ 1] -= add; totalCost += (long)cost[e] * add; v = parent[v]; }
        }

        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow)
        {
            long totalCost = 0;
            int twoN = n << 1;
            for (int i = 0; i < twoN; i++) flow[i] = 0;
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            long* pot = stackalloc long[n];
            for (int i = 0; i < n; i++) pot[i] = 0;
            long* pqDist = stackalloc long[n];
            int* pqV = stackalloc int[n];
            int* pqPos = stackalloc int[n];
            while (true)
            {
                int pqSize = 0;
                DijkstraSearch(n, s, t, head, to, next, cap, cost, flow, dist, parent, parentEdge, pot, pqDist, pqV, pqPos, &pqSize);
                if (dist[t] == long.MaxValue) break;
                UpdatePotentials(n, pot, dist);
                int add = PathCapacity(s, t, parent, parentEdge, cap, flow);
                AugmentPath(s, t, parent, parentEdge, flow, cost, add, ref totalCost);
            }
            return totalCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushHeap(int* pqV, long* pqDist, int* pqPos, int* pqSize, int v, long d)
        {
            int i = pqPos[v];
            if (i == -1)
            {
                i = (*pqSize)++;
                pqV[i] = v; pqPos[v] = i;
            }
            pqDist[i] = d;
            while (i > 0)
            {
                int p = (i - 1) >> 1;
                if (pqDist[p] <= pqDist[i]) break;
                long td = pqDist[p]; pqDist[p] = pqDist[i]; pqDist[i] = td;
                int tv = pqV[p]; pqV[p] = pqV[i]; pqV[i] = tv;
                pqPos[pqV[p]] = p; pqPos[pqV[i]] = i;
                i = p;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopMin(int* pqV, long* pqDist, int* pqPos, int* pqSize)
        {
            int minV = pqV[0];
            pqPos[minV] = -1;
            (*pqSize)--;
            if (*pqSize > 0)
            {
                int lastV = pqV[*pqSize];
                long lastD = pqDist[*pqSize];
                pqV[0] = lastV; pqDist[0] = lastD; pqPos[lastV] = 0;
                int i = 0;
                while (true)
                {
                    int l = (i << 1) + 1, r = (i << 1) + 2, smallest = i;
                    if (l < *pqSize && pqDist[l] < pqDist[smallest]) smallest = l;
                    if (r < *pqSize && pqDist[r] < pqDist[smallest]) smallest = r;
                    if (smallest == i) break;
                    long td = pqDist[smallest]; pqDist[smallest] = pqDist[i]; pqDist[i] = td;
                    int tv = pqV[smallest]; pqV[smallest] = pqV[i]; pqV[i] = tv;
                    pqPos[pqV[smallest]] = smallest; pqPos[pqV[i]] = i;
                    i = smallest;
                }
            }
            return minV;
        }
    }
}
