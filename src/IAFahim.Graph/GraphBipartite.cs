namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class IsBipartite
    {
        public static bool Run(int n, int* head, int* to, int* next)
        {
            int* color = stackalloc int[n];
            for (int i = 0; i < n; i++) color[i] = -1;
            for (int start = 0; start < n; start++)
            {
                if (color[start] != -1) continue;
                int* q = stackalloc int[n];
                int qh = 0, qt = 0;
                q[qt++] = start;
                color[start] = 0;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (color[v] == -1)
                        {
                            color[v] = 1 - color[u];
                            q[qt++] = v;
                        }
                        else if (color[v] == color[u])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    public static unsafe class ColorBipartite
    {
        public static bool Run(int n, int* head, int* to, int* next, int* color)
        {
            for (int i = 0; i < n; i++) color[i] = -1;
            for (int start = 0; start < n; start++)
            {
                if (color[start] != -1) continue;
                int* q = stackalloc int[n];
                int qh = 0, qt = 0;
                q[qt++] = start;
                color[start] = 0;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (color[v] == -1)
                        {
                            color[v] = 1 - color[u];
                            q[qt++] = v;
                        }
                    }
                }
            }
            return true;
        }
    }

    public static unsafe class ShortestPathUnweighted
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* dist, int* parent)
        {
            for (int i = 0; i < n; i++) dist[i] = -1;
            for (int i = 0; i < n; i++) parent[i] = -1;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            dist[start] = 0;
            q[qt++] = start;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (dist[v] == -1)
                    {
                        dist[v] = dist[u] + 1;
                        parent[v] = u;
                        q[qt++] = v;
                    }
                }
            }
        }
    }
}