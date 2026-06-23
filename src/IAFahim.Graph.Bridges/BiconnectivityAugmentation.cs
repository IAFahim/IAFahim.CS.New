namespace IAFahim.Graph.Bridges
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BiconnectivityAugmentation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DfsEdge(int u, int p, int* head, int* next, int* to, int* tin, int* low, ref int timer, int* comp, int* stack, ref int top, ref int compCount)
        {
            tin[u] = low[u] = ++timer; stack[top++] = u;
            for (int e = head[u]; e != -1; e = next[e])
            {
                int v = to[e]; if (v == p) continue;
                if (tin[v] != 0) { if (tin[v] < low[u]) low[u] = tin[v]; }
                else { DfsEdge(v, u, head, next, to, tin, low, ref timer, comp, stack, ref top, ref compCount); if (low[v] < low[u]) low[u] = low[v]; }
            }
            if (low[u] == tin[u]) { while (true) { int v = stack[--top]; comp[v] = compCount; if (u == v) break; } compCount++; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ClassifyComponents(int* degree, int compCount, out int leaves, out int isolated)
        {
            leaves = 0; isolated = 0;
            for (int i = 0; i < compCount; i++)
            {
                if (degree[i] == 0) isolated++;
                else if (degree[i] == 1) leaves++;
            }
        }

        public static int MinEdgesFor2EdgeConnected(int n, int* head, int* next, int* to)
        {
            if (n <= 1) return 0;
            int* tin = stackalloc int[n], low = stackalloc int[n], comp = stackalloc int[n], stack = stackalloc int[n];
            for (int i = 0; i < n; i++) { tin[i] = 0; low[i] = 0; comp[i] = -1; }
            int timer = 0, top = 0, compCount = 0, trees = 0;
            for (int i = 0; i < n; i++) if (tin[i] == 0) { DfsEdge(i, -1, head, next, to, tin, low, ref timer, comp, stack, ref top, ref compCount); trees++; }
            if (compCount <= 1) return 0;

            int* degree = stackalloc int[compCount];
            for (int i = 0; i < compCount; i++) degree[i] = 0;
            ComputeComponentDegrees(n, head, next, to, comp, degree);

            ClassifyComponents(degree, compCount, out int leaves, out int isolated);
            return trees > 1 ? leaves / 2 + isolated + trees - 1 : (leaves + 1) / 2;
        }

        private static void ComputeComponentDegrees(int n, int* head, int* next, int* to, int* comp, int* degree)
        {
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != -1; e = next[e])
                    if (comp[u] != comp[to[e]]) degree[comp[u]]++;
        }
    }
}
