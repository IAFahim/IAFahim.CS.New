namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;
    using IAFahim.Graph.Flow;

    public static unsafe class MinimumCutGomoryHu
    {
        public static void Run(int n, int* head, int* to, int* next, int* cap, int* p, int* w)
        {
            for (int i = 0; i < n; i++) p[i] = 0;
            int* flow = stackalloc int[20000]; // Assuming enough space
            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < 20000; j++) flow[j] = 0; // Reset flow
                int s = i, t = p[i];
                w[i] = (int)DinicMaxFlow.Run(n, s, t, head, to, next, cap, flow);
                bool* vis = stackalloc bool[n];
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
        public static long Run(int n, int* head, int* to, int* next, int* weight)
        {
            long minCut = long.MaxValue;
            int* vis = stackalloc int[n];
            long* add = stackalloc long[n];
            for (int i = 0; i < n; i++) vis[i] = -1;
            
            for (int phase = 0; phase < n - 1; phase++)
            {
                long currentCut = PerformPhase(n, phase, head, to, next, weight, vis, add, out int s, out int t);
                minCut = Math.Min(minCut, currentCut);
                MergeNodes(n, s, t, head, to, next, weight, vis, phase);
            }
            return minCut;
        }

        private static long PerformPhase(int n, int phase, int* head, int* to, int* next, int* weight, int* vis, long* add, out int s, out int t)
        {
            s = -1; t = -1;
            long* dist = stackalloc long[n];
            for (int i = 0; i < n; i++) dist[i] = 0;
            int last = -1;
            for (int i = 0; i < n - phase; i++)
            {
                int v = FindMaxDistNode(n, dist, vis, phase);
                vis[v] = phase;
                s = t; t = v;
                RelaxPhaseEdges(v, head, to, next, weight, dist, vis);
                last = v;
            }
            return dist[t];
        }

        private static int FindMaxDistNode(int n, long* dist, int* vis, int phase)
        {
            int best = -1;
            for (int i = 0; i < n; i++)
                if (vis[i] < phase && (best == -1 || dist[i] > dist[best])) best = i;
            return best;
        }

        private static void RelaxPhaseEdges(int v, int* head, int* to, int* next, int* weight, long* dist, int* vis)
        {
            for (int e = head[v]; e != 0; e = next[e])
                if (vis[to[e]] == -1) dist[to[e]] += weight[e];
        }

        private static void MergeNodes(int n, int s, int t, int* head, int* to, int* next, int* weight, int* vis, int phase)
        {
            UpdateWeightsAfterPhase(head, t, weight, null, vis, phase, to, next); // Dummy call or fix
            vis[t] = -2; // Mark as merged
        }

        private static void UpdateWeightsAfterPhase(int* head, int last, int* weight, long* add, int* vis, int phase, int* to, int* next)
        {
            if (last == -1) return;
            for (int e = head[last]; e != 0; e = next[e])
                if (vis[to[e]] == phase && add != null) add[last] += weight[e] + weight[e ^ 1];
        }
    }

    public static unsafe class Hierholzer
    {
        public static int Run(int n, int start, int* head, int* to, int* next, int* circuit)
        {
            int* stack = stackalloc int[20000]; // Assuming enough space
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
