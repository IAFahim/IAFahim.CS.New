namespace IAFahim.Graph.Bridges
{
    using System.Runtime.CompilerServices;

    public static unsafe class BridgeAndArticulation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandleBackEdge(int* tin, ref int lowU, int v)
        {
            if (tin[v] < lowU) lowU = tin[v];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandleTreeEdge(int u, int v, int tu, int p, int* head, int* next, int* to,
                                           int* tin, int* low, ref int timer,
                                           byte* isArticulation, int* bridgesU, int* bridgesV, ref int bridgeCount, ref int lowU)
        {
            Dfs(v, u, head, next, to, tin, low, ref timer, isArticulation, bridgesU, bridgesV, ref bridgeCount);
            int lv = low[v];
            if (lv < lowU) lowU = lv;

            if (lv >= tu)
            {
                if (lv > tu)
                {
                    bridgesU[bridgeCount] = u;
                    bridgesV[bridgeCount] = v;
                    bridgeCount++;
                }
                if (p != -1) isArticulation[u] = 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dfs(int u, int p, int* head, int* next, int* to,
                               int* tin, int* low, ref int timer,
                               byte* isArticulation, int* bridgesU, int* bridgesV, ref int bridgeCount)
        {
            int tu = tin[u] = low[u] = ++timer;
            int children = 0;

            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;

                if (tin[v] != 0)
                {
                    HandleBackEdge(tin, ref low[u], v);
                }
                else
                {
                    children++;
                    HandleTreeEdge(u, v, tu, p, head, next, to, tin, low, ref timer, isArticulation, bridgesU, bridgesV, ref bridgeCount, ref low[u]);
                }
            }

            if (p == -1 && children > 1) isArticulation[u] = 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Find(int n, int* head, int* next, int* to,
                                int* tin, int* low,
                                byte* isArticulation, int* bridgesU, int* bridgesV, ref int bridgeCount)
        {
            int timer = 0;
            for (int i = 0; i < n; i++)
            {
                tin[i] = 0;
                low[i] = 0;
                isArticulation[i] = 0;
            }
            
            for (int i = 0; i < n; i++)
            {
                if (tin[i] == 0)
                {
                    Dfs(i, -1, head, next, to, tin, low, ref timer, isArticulation, bridgesU, bridgesV, ref bridgeCount);
                }
            }
        }
    }
}
