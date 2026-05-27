namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    using IAFahim.Graph.SCC;
    using BridgeImpl = IAFahim.Graph.Bridges;

    public static unsafe class TwoSatAddClause
    {
        public static void Run(int i, bool f, int j, bool g, int* head, int* to, int* next, int* edgeCount)
        {
            int l1 = i * 2 + (f ? 0 : 1);
            int l2 = j * 2 + (g ? 0 : 1);
            AddEdge(l1 ^ 1, l2, head, to, next, edgeCount);
            AddEdge(l2 ^ 1, l1, head, to, next, edgeCount);
        }

        private static void AddEdge(int u, int v, int* head, int* to, int* next, int* edgeCount)
        {
            int e = ++(*edgeCount);
            to[e] = v;
            next[e] = head[u];
            head[u] = e;
        }
    }

    public static unsafe class TwoSatSolve
    {
        public static bool Run(int n, int* head, int* to, int* next, int* result)
        {
            int nodes = n * 2;
            int* tin = stackalloc int[nodes];
            int* low = stackalloc int[nodes];
            int* stack = stackalloc int[nodes];
            byte* inStack = stackalloc byte[nodes];
            int* sccId = stackalloc int[nodes];
            int sccCount = 0;

            global::IAFahim.Graph.SCC.TarjanScc.Find(nodes, head, next, to, tin, low, stack, inStack, sccId, ref sccCount);

            for (int i = 0; i < n; i++)
            {
                if (sccId[i * 2] == sccId[i * 2 + 1]) return false;
                result[i] = sccId[i * 2] < sccId[i * 2 + 1] ? 1 : 0;
            }
            return true;
        }
    }

    public static unsafe class GraphBridgeAdapter
    {
        public static int Run(int n, int* head, int* to, int* next, int* bu, int* bv)
        {
            int bridgeCount = 0;
            int* tin = stackalloc int[n];
            int* low = stackalloc int[n];
            byte* isArt = stackalloc byte[n];
            global::IAFahim.Graph.Bridges.BridgeAndArticulation.Find(n, head, next, to, tin, low, isArt, bu, bv, ref bridgeCount);
            return bridgeCount;
        }
    }

    public static unsafe class GraphArticulationPointAdapter
    {
        public static int Run(int n, int s, int* head, int* to, int* next, bool* result)
        {
            int bridgeCount = 0;
            int* tin = stackalloc int[n];
            int* low = stackalloc int[n];
            byte* isArt = stackalloc byte[n];
            int* bu = stackalloc int[n];
            int* bv = stackalloc int[n];
            global::IAFahim.Graph.Bridges.BridgeAndArticulation.Find(n, head, next, to, tin, low, isArt, bu, bv, ref bridgeCount);
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                result[i] = isArt[i] != 0;
                if (result[i]) count++;
            }
            return count;
        }
    }
}
