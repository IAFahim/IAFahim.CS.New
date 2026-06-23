namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Bfs
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

    public static unsafe class ZeroOneBfs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RelaxOutgoing01(int* dq, byte* inDeque, ref int dh, ref int dt, ref int cnt, int u, int n, int* head, int* to, int* next, int* weight, int* dist)
        {
            int du = dist[u];
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                int w = weight[e];
                int nd = du + w;
                if (dist[v] > nd)
                {
                    dist[v] = nd;
                    if (inDeque[v] == 0)
                    {
                        inDeque[v] = 1;
                        cnt++;
                        if (w == 0)
                        {
                            dh--;
                            if (dh < 0) dh = n - 1;
                            dq[dh] = v;
                        }
                        else
                        {
                            dq[dt++] = v;
                            if (dt >= n) dt = 0;
                        }
                    }
                }
            }
        }

        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, int* dist)
        {
            for (int i = 0; i < n; i++) dist[i] = int.MaxValue;
            int* dq = stackalloc int[n];
            byte* inDeque = stackalloc byte[n];
            for (int i = 0; i < n; i++) inDeque[i] = 0;
            int dh = 0, dt = 0, cnt = 0;
            dist[start] = 0;
            dq[dt++] = start;
            inDeque[start] = 1;
            cnt++;
            while (cnt > 0)
            {
                int u = dq[dh++];
                if (dh >= n) dh = 0;
                cnt--;
                inDeque[u] = 0;
                RelaxOutgoing01(dq, inDeque, ref dh, ref dt, ref cnt, u, n, head, to, next, weight, dist);
            }
        }
    }

    public static unsafe class MultiSourceBfs
    {
        public static void Run(int n, int sourceCount, int* sources, int* head, int* to, int* next, int* dist)
        {
            for (int i = 0; i < n; i++) dist[i] = -1;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int i = 0; i < sourceCount; i++)
            {
                dist[sources[i]] = 0;
                q[qt++] = sources[i];
            }
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (dist[v] == -1)
                    {
                        dist[v] = dist[u] + 1;
                        q[qt++] = v;
                    }
                }
            }
        }
    }

    public static unsafe class Dfs
    {
        public static void Run(int u, int* head, int* to, int* next, int* parent, int* depth, bool* visited, ref int time)
        {
            visited[u] = true;
            time++;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (!visited[v])
                {
                    parent[v] = u;
                    depth[v] = depth[u] + 1;
                    Run(v, head, to, next, parent, depth, visited, ref time);
                }
            }
            time++;
        }
    }

    public static unsafe class IterativeDfs
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* parent, int* depth, int* order)
        {
            for (int i = 0; i < n; i++) parent[i] = -1;
            for (int i = 0; i < n; i++) depth[i] = 0;
            int* stack = stackalloc int[n];
            int* iter = stackalloc int[n];
            byte* visited = stackalloc byte[n];
            for (int i = 0; i < n; i++) visited[i] = 0;
            int top = 0;
            stack[top] = start;
            iter[top] = head[start];
            depth[start] = 0;
            visited[start] = 1;
            int count = 0;
            while (top >= 0)
            {
                int u = stack[top];
                if (iter[top] == 0)
                {
                    order[count++] = u;
                    top--;
                    continue;
                }
                int e = iter[top];
                iter[top] = next[e];
                int v = to[e];
                if (visited[v] == 0)
                {
                    visited[v] = 1;
                    parent[v] = u;
                    depth[v] = depth[u] + 1;
                    top++;
                    stack[top] = v;
                    iter[top] = head[v];
                }
            }
        }
    }
}