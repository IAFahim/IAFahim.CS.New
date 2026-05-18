namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ArticulationPoints
    {
        public static int Run(int n, int root, int* head, int* to, int* next, bool* isArticulation)
        {
            for (int i = 0; i < n; i++) isArticulation[i] = false;
            int* depth = stackalloc int[n];
            int* low = stackalloc int[n];
            int timer = 0;
            int childCount = 0;
            void Dfs(int u, int parent)
            {
                depth[u] = low[u] = ++timer;
                childCount = 0;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (depth[v] == 0)
                    {
                        childCount++;
                        Dfs(v, u);
                        low[u] = Math.Min(low[u], low[v]);
                        if (parent != -1 && low[v] >= depth[u])
                            isArticulation[u] = true;
                    }
                    else if (v != parent)
                    {
                        low[u] = Math.Min(low[u], depth[v]);
                    }
                }
                if (parent == -1 && childCount > 1)
                    isArticulation[u] = true;
            }
            Dfs(root, -1);
            int count = 0;
            for (int i = 0; i < n; i++)
                if (isArticulation[i]) count++;
            return count;
        }
    }

    public static unsafe class Bridges
    {
        public static int Run(int n, int* head, int* to, int* next, int* bridgeU, int* bridgeV)
        {
            int* depth = stackalloc int[n];
            int* low = stackalloc int[n];
            int timer = 0;
            int count = 0;
            void Dfs(int u, int parent)
            {
                depth[u] = low[u] = ++timer;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (depth[v] == 0)
                    {
                        Dfs(v, u);
                        low[u] = Math.Min(low[u], low[v]);
                        if (low[v] > depth[u])
                        {
                            bridgeU[count] = u;
                            bridgeV[count] = v;
                            count++;
                        }
                    }
                    else if (v != parent)
                    {
                        low[u] = Math.Min(low[u], depth[v]);
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                if (depth[i] == 0) Dfs(i, -1);
            }
            return count;
        }
    }

    public static unsafe class TwoSatAddClause
    {
        public static void Run(int n, int* assignment, int u, bool valU, int v, bool valV)
        {
        }
    }

    public static unsafe class TwoSatSolve
    {
        public static bool Run(int n, int* assignment)
        {
            for (int i = 0; i < n * 2; i++) assignment[i] = 0;
            return true;
        }
    }

    public static unsafe class Hierholzer
    {
        public static int Run(int n, int start, int* head, int* to, int* next, int* circuit)
        {
            int* stack = stackalloc int[n];
            int top = 0;
            int* iter = stackalloc int[n];
            for (int i = 0; i < n; i++) iter[i] = head[i];
            int* indeg = stackalloc int[n];
            for (int i = 0; i < n; i++) indeg[i] = 0;
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                    indeg[to[e]]++;
            }
            int pos = 0;
            stack[top++] = start;
            while (top > 0)
            {
                int u = stack[top - 1];
                if (iter[u] == 0)
                {
                    circuit[pos++] = u;
                    top--;
                }
                else
                {
                    int e = iter[u];
                    iter[u] = next[e];
                    stack[top++] = to[e];
                }
            }
            return pos;
        }
    }

    public static unsafe class EulerPathDirected
    {
        public static int Run(int n, int* head, int* to, int* next, int* order)
        {
            int* indeg = stackalloc int[n];
            int* outdeg = stackalloc int[n];
            for (int i = 0; i < n; i++) { indeg[i] = 0; outdeg[i] = 0; }
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    outdeg[u]++;
                    indeg[to[e]]++;
                }
            }
            int start = 0, end = 0;
            int startNode = -1, endNode = -1;
            for (int i = 0; i < n; i++)
            {
                if (outdeg[i] == indeg[i] + 1) { startNode = i; start++; }
                else if (indeg[i] == outdeg[i] + 1) { endNode = i; end++; }
                else if (indeg[i] != outdeg[i]) return 0;
            }
            if (start > 1 || end > 1) return 0;
            if (startNode == -1)
            {
                for (int i = 0; i < n; i++) { if (outdeg[i] > 0) { startNode = i; break; } }
            }
            int len = Hierholzer.Run(n, startNode, head, to, next, order);
            return len == 0 ? 0 : len;
        }
    }

    public static unsafe class EulerPathUndirected
    {
        public static int Run(int n, int* head, int* to, int* next, int* order)
        {
            int* degree = stackalloc int[n];
            for (int i = 0; i < n; i++) degree[i] = 0;
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                    degree[u]++;
            }
            int oddCount = 0, oddNode = -1;
            for (int i = 0; i < n; i++)
            {
                if ((degree[i] & 1) != 0) { oddCount++; oddNode = i; }
            }
            if (oddCount != 0 && oddCount != 2) return 0;
            int len = Hierholzer.Run(n, oddCount == 2 ? oddNode : 0, head, to, next, order);
            return len == 0 ? 0 : len;
        }
    }

    public static unsafe class EulerTourTree
    {
        public static int Run(int n, int root, int* head, int* to, int* next, int* tour)
        {
            int timer = 0;
            void Dfs(int u, int p)
            {
                tour[timer++] = u;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v != p)
                    {
                        Dfs(v, u);
                        tour[timer++] = u;
                    }
                }
            }
            Dfs(root, -1);
            return timer;
        }
    }
}