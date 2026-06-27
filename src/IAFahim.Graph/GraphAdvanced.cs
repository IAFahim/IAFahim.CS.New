namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using IAFahim.Graph.Flow;

    public static unsafe class MinimumCutGomoryHu
    {
        // m is the length of the edge-indexed arrays (to/next/cap) and sizes the
        // residual-flow scratch buffer. Caller guarantees n, m and the pointers are valid.
        public static void Run(int n, int m, int* head, int* to, int* next, int* cap, int* p, int* w)
        {
            for (int i = 0; i < n; i++) p[i] = 0;
            int* flow = stackalloc int[m];
            bool* vis = stackalloc bool[n];
            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < m; j++) flow[j] = 0; // Reset residual flow
                int s = i, t = p[i];
                w[i] = (int)DinicMaxFlow.Run(n, s, t, head, to, next, cap, flow);
                Bfs(n, s, head, to, next, cap, flow, vis);
                for (int j = i + 1; j < n; j++)
                    if (p[j] == t && vis[j]) p[j] = i;
            }
        }

        private static void Bfs(int n, int s, int* head, int* to, int* next, int* cap, int* flow, bool* vis)
        {
            for (int i = 0; i < n; i++) vis[i] = false;
            int* q = stackalloc int[n]; int qh = 0, qt = 0;
            vis[s] = true; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                    if (cap[e] - flow[e] > 0 && !vis[to[e]]) { vis[to[e]] = true; q[qt++] = to[e]; }
            }
        }
    }

    public static unsafe class StoerWagner
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildWeightMatrix(int n, int* head, int* to, int* next, int* weight, long* w)
        {
            for (int i = 0; i < n * n; i++) w[i] = 0;
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    w[u * n + v] += weight[e];
                }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ContractVertex(int n, long* w, bool* merged, int s, int t)
        {
            merged[t] = true;
            for (int i = 0; i < n; i++)
            {
                if (merged[i] || i == s) continue;
                long add = w[t * n + i];
                w[s * n + i] += add;
                w[i * n + s] += add;
            }
        }

        // Global minimum cut of an undirected weighted graph (Stoer-Wagner).
        // Input is the library's adjacency-list form: head/to/next with edge index 0
        // as the sentinel and undirected edges stored as pairs (e, e^1). The list is
        // materialized into a dense n*n weight matrix, then classic Stoer-Wagner runs
        // over it with real node contraction. Returns long.MaxValue for n < 2.
        public static long Run(int n, int* head, int* to, int* next, int* weight)
        {
            if (n < 2) return long.MaxValue;
            long* w = (long*)Marshal.AllocHGlobal((IntPtr)((long)n * n * sizeof(long)));
            bool* merged = (bool*)Marshal.AllocHGlobal(sizeof(bool) * n);
            long* dist = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            bool* inA = (bool*)Marshal.AllocHGlobal(sizeof(bool) * n);
            BuildWeightMatrix(n, head, to, next, weight, w);
            for (int i = 0; i < n; i++) merged[i] = false;
            long minCut = long.MaxValue;
            int remaining = n;
            while (remaining > 1)
            {
                long cutOfPhase = MinimumCutPhase(n, w, merged, dist, inA, out int s, out int t);
                if (cutOfPhase < minCut) minCut = cutOfPhase;
                ContractVertex(n, w, merged, s, t);
                remaining--;
            }
            Marshal.FreeHGlobal((IntPtr)w);
            Marshal.FreeHGlobal((IntPtr)merged);
            Marshal.FreeHGlobal((IntPtr)dist);
            Marshal.FreeHGlobal((IntPtr)inA);
            return minCut;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountActive(int n, bool* merged)
        {
            int active = 0;
            for (int i = 0; i < n; i++) if (!merged[i]) active++;
            return active;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SelectMaxAdjacency(int n, long* dist, bool* merged, bool* inA)
        {
            int sel = -1;
            for (int i = 0; i < n; i++)
                if (!merged[i] && !inA[i] && (sel == -1 || dist[i] > dist[sel])) sel = i;
            return sel;
        }

        // Runs one maximum-adjacency ordering over the non-merged vertices,
        // returning the cut-of-the-phase (weighted degree of the last added vertex)
        // and the last two vertices added (s = second-to-last, t = last).
        private static long MinimumCutPhase(int n, long* w, bool* merged, long* dist, bool* inA, out int s, out int t)
        {
            for (int i = 0; i < n; i++) { dist[i] = 0; inA[i] = false; }
            s = -1;
            t = -1;
            long lastWeight = 0;
            int active = CountActive(n, merged);
            for (int added = 0; added < active; added++)
            {
                int sel = SelectMaxAdjacency(n, dist, merged, inA);
                inA[sel] = true;
                s = t;
                t = sel;
                lastWeight = dist[sel];
                for (int i = 0; i < n; i++)
                    if (!merged[i] && !inA[i]) dist[i] += w[sel * n + i];
            }
            return lastWeight;
        }
    }

    public static unsafe class Hierholzer
    {
        // m is the number of (directed) edges; the DFS stack depth is bounded by m + 1.
        // Caller guarantees n, m, start and the pointers are valid.
        public static int Run(int n, int m, int start, int* head, int* to, int* next, int* circuit)
        {
            int* stack = stackalloc int[m + 1];
            int top = 0;
            int* currentHead = stackalloc int[n];
            for (int i = 0; i < n; i++) currentHead[i] = head[i];

            int resCount = 0;
            stack[top++] = start;
            while (top > 0)
            {
                int u = stack[top - 1];
                if (currentHead[u] != 0)
                {
                    int e = currentHead[u];
                    currentHead[u] = next[e];
                    stack[top++] = to[e];
                }
                else
                {
                    circuit[resCount++] = u;
                    top--;
                }
            }
            return resCount;
        }
    }
}
