namespace IAFahim.Graph.SCC
{
    using System.Runtime.CompilerServices;

    public static unsafe class TarjanScc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dfs(int u, int* head, int* next, int* to,
                               int* tin, int* low, ref int timer,
                               int* stack, ref int stackCount, byte* inStack,
                               int* sccId, ref int sccCount)
        {
            tin[u] = low[u] = ++timer;
            stack[stackCount++] = u;
            inStack[u] = 1;

            for (int e = head[u]; e != -1; e = next[e])
            {
                int v = to[e];
                if (tin[v] == 0)
                {
                    Dfs(v, head, next, to, tin, low, ref timer, stack, ref stackCount, inStack, sccId, ref sccCount);
                    if (low[v] < low[u]) low[u] = low[v];
                }
                else if (inStack[v] != 0)
                {
                    if (tin[v] < low[u]) low[u] = tin[v];
                }
            }

            if (low[u] == tin[u])
            {
                while (true)
                {
                    int v = stack[--stackCount];
                    inStack[v] = 0;
                    sccId[v] = sccCount;
                    if (u == v) break;
                }
                sccCount++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Find(int n, int* head, int* next, int* to,
                                int* tin, int* low, int* stack, byte* inStack, int* sccId, ref int sccCount)
        {
            int timer = 0;
            int stackCount = 0;
            sccCount = 0;
            for (int i = 0; i < n; i++)
            {
                tin[i] = 0;
                low[i] = 0;
                inStack[i] = 0;
                sccId[i] = -1;
            }

            for (int i = 0; i < n; i++)
            {
                if (tin[i] == 0)
                {
                    Dfs(i, head, next, to, tin, low, ref timer, stack, ref stackCount, inStack, sccId, ref sccCount);
                }
            }
        }
    }
}
