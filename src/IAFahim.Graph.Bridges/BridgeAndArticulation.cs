namespace IAFahim.Graph.Bridges
{
    using System.Runtime.CompilerServices;

    public static unsafe class BridgeAndArticulation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dfs(int u, int p, int* head, int* next, int* to,
                               int* tin, int* low, ref int timer,
                               byte* isArticulation, int* bridgesU, int* bridgesV, ref int bridgeCount)
        {
            tin[u] = low[u] = ++timer;
            int children = 0;
            
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;
                
                if (tin[v] != 0)
                {
                    if (tin[v] < low[u]) low[u] = tin[v];
                }
                else
                {
                    children++;
                    Dfs(v, u, head, next, to, tin, low, ref timer, isArticulation, bridgesU, bridgesV, ref bridgeCount);
                    if (low[v] < low[u]) low[u] = low[v];
                    
                    if (low[v] > tin[u])
                    {
                        bridgesU[bridgeCount] = u;
                        bridgesV[bridgeCount] = v;
                        bridgeCount++;
                    }
                    if (low[v] >= tin[u] && p != -1)
                    {
                        isArticulation[u] = 1;
                    }
                }
            }
            
            if (p == -1 && children > 1)
            {
                isArticulation[u] = 1;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Find(int n, int* head, int* next, int* to,
                                int* tin, int* low,
                                byte* isArticulation, int* bridgesU, int* bridgesV, ref int bridgeCount)
        {
            int timer = 0;
            for (int i = 0; i < n; i++) tin[i] = 0;
            for (int i = 0; i < n; i++) low[i] = 0;
            for (int i = 0; i < n; i++) isArticulation[i] = 0;
            
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
