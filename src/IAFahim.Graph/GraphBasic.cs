namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AddEdge
    {
        public static void Run(int* head, int* to, int* next, int* edgeId, int u, int v, int* edgeCount)
        {
            int id = (*edgeId)++;
            to[id] = v;
            next[id] = head[u];
            head[u] = id;
        }
    }

    public static unsafe class AddDirectedEdge
    {
        public static void Run(int* head, int* to, int* next, int* edgeId, int u, int v, int* edgeCount)
        {
            int id = (*edgeId)++;
            to[id] = v;
            next[id] = head[u];
            head[u] = id;
        }
    }

    public static unsafe class AddWeightedEdge
    {
        public static void Run(int* head, int* to, int* next, int* weight, int* edgeId, int u, int v, int w, int* edgeCount)
        {
            int id = (*edgeId)++;
            to[id] = v;
            weight[id] = w;
            next[id] = head[u];
            head[u] = id;
        }
    }

    public static unsafe class BuildAdjacency
    {
        public static void Run(int n, int m, int* edges, int* head, int* to, int* next, int* edgeId, bool directed)
        {
            for (int i = 0; i < n; i++) head[i] = 0;
            *edgeId = 1;
            for (int i = 0; i < m; i++)
            {
                int u = edges[i * 2];
                int v = edges[i * 2 + 1];
                AddEdge.Run(head, to, next, edgeId, u, v, &m);
                if (!directed)
                    AddEdge.Run(head, to, next, edgeId, v, u, &m);
            }
        }
    }

    public static unsafe class TransposeGraph
    {
        public static void Run(int n, int* head, int* to, int* next, int* revHead, int* revTo, int* revNext, int* revEdgeId)
        {
            for (int i = 0; i < n; i++) revHead[i] = 0;
            *revEdgeId = 1;
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    int id = (*revEdgeId)++;
                    revTo[id] = u;
                    revNext[id] = revHead[v];
                    revHead[v] = id;
                }
            }
        }
    }
}