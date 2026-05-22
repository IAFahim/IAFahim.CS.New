namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Toposort
    {
        public static int Run(int n, int* head, int* to, int* next, int* order)
        {
            int* indeg = stackalloc int[n];
            for (int i = 0; i < n; i++) indeg[i] = 0;
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    indeg[to[e]]++;
                }
            }
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int i = 0; i < n; i++)
            {
                if (indeg[i] == 0) q[qt++] = i;
            }
            int idx = 0;
            while (qh < qt)
            {
                int u = q[qh++];
                order[idx++] = u;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    indeg[v]--;
                    if (indeg[v] == 0) q[qt++] = v;
                }
            }
            return idx == n ? idx : 0;
        }
    }

    public static unsafe class KahnToposort
    {
        public static int Run(int n, int* head, int* to, int* next, int* order)
        {
            return Toposort.Run(n, head, to, next, order);
        }
    }

    public static unsafe class DetectCycleDirected
    {
        public static bool Run(int n, int* head, int* to, int* next, int* parent, int* depth)
        {
            for (int i = 0; i < n; i++) parent[i] = -1;
            for (int i = 0; i < n; i++) depth[i] = 0;
            bool* onStack = stackalloc bool[n];
            for (int i = 0; i < n; i++) onStack[i] = false;
            bool* visited = stackalloc bool[n];
            for (int i = 0; i < n; i++) visited[i] = false;
            int* stack = stackalloc int[n];
            for (int start = 0; start < n; start++)
            {
                if (visited[start]) continue;
                int top = 0;
                stack[top] = start;
                while (top >= 0)
                {
                    int u = stack[top];
                    if (!visited[u])
                    {
                        visited[u] = true;
                        onStack[u] = true;
                    }
                    bool foundChild = false;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (!visited[v])
                        {
                            parent[v] = u;
                            depth[v] = depth[u] + 1;
                            stack[++top] = v;
                            foundChild = true;
                            break;
                        }
                        else if (onStack[v])
                        {
                            return true;
                        }
                    }
                    if (!foundChild)
                    {
                        onStack[u] = false;
                        top--;
                    }
                }
            }
            return false;
        }
    }

    public static unsafe class DetectCycleUndirected
    {
        public static bool Run(int n, int* head, int* to, int* next, int* parent)
        {
            bool* visited = stackalloc bool[n];
            for (int i = 0; i < n; i++) visited[i] = false;
            int* stack = stackalloc int[n];
            for (int start = 0; start < n; start++)
            {
                if (visited[start]) continue;
                int top = 0;
                stack[top] = start;
                parent[start] = -1;
                while (top >= 0)
                {
                    int u = stack[top--];
                    visited[u] = true;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (!visited[v])
                        {
                            parent[v] = u;
                            stack[++top] = v;
                        }
                        else if (v != parent[u])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    public static unsafe class ConnectedComponents
    {
        public static int Run(int n, int* head, int* to, int* next, int* comp)
        {
            for (int i = 0; i < n; i++) comp[i] = -1;
            int compCount = 0;
            int* q = stackalloc int[n];
            for (int start = 0; start < n; start++)
            {
                if (comp[start] != -1) continue;
                int qh = 0, qt = 0;
                comp[start] = compCount;
                q[qt++] = start;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (comp[v] == -1)
                        {
                            comp[v] = compCount;
                            q[qt++] = v;
                        }
                    }
                }
                compCount++;
            }
            return compCount;
        }
    }

    public static unsafe class Kosaraju
    {
        public static void FirstDfs(int u, int* head, int* to, int* next, bool* visited, int* order, int* top)
        {
            visited[u] = true;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (!visited[v]) FirstDfs(v, head, to, next, visited, order, top);
            }
            order[(*top)++] = u;
        }

        public static void SecondDfs(int u, int* head, int* to, int* next, bool* visited, int* comp, int id)
        {
            visited[u] = true;
            comp[u] = id;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (!visited[v]) SecondDfs(v, head, to, next, visited, comp, id);
            }
        }

        public static int Run(int n, int* head, int* to, int* next, int* revHead, int* revTo, int* revNext, int* comp)
        {
            int* order = stackalloc int[n];
            int top = 0;
            bool* visited = stackalloc bool[n];
            for (int i = 0; i < n; i++) visited[i] = false;
            for (int i = 0; i < n; i++)
            {
                if (!visited[i])
                    FirstDfs(i, head, to, next, visited, order, &top);
            }
            for (int i = 0; i < n; i++) visited[i] = false;
            int sccCount = 0;
            for (int i = n - 1; i >= 0; i--)
            {
                int v = order[i];
                if (!visited[v])
                {
                    SecondDfs(v, revHead, revTo, revNext, visited, comp, sccCount);
                    sccCount++;
                }
            }
            return sccCount;
        }
    }

    public static unsafe class TarjanScc
    {
        public static void Run(int u, int* head, int* to, int* next, int* index, int* lowlink, bool* onStack, int* stack, ref int stackSize, ref int idx, ref int sccCount, int* comp)
        {
            index[u] = idx;
            lowlink[u] = idx;
            idx++;
            stack[stackSize++] = u;
            onStack[u] = true;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (index[v] < 0)
                {
                    Run(v, head, to, next, index, lowlink, onStack, stack, ref stackSize, ref idx, ref sccCount, comp);
                    lowlink[u] = Math.Min(lowlink[u], lowlink[v]);
                }
                else if (onStack[v])
                {
                    lowlink[u] = Math.Min(lowlink[u], index[v]);
                }
            }
            if (lowlink[u] == index[u])
            {
                while (true)
                {
                    int w = stack[--stackSize];
                    onStack[w] = false;
                    comp[w] = sccCount;
                    if (w == u) break;
                }
                sccCount++;
            }
        }
    }

    public static unsafe class CondenseGraph
    {
        public static void Run(int n, int* head, int* to, int* next, int* comp, int sccCount, int* condHead, int* condTo, int* condNext, int* condEdgeId)
        {
            for (int i = 0; i < sccCount; i++) condHead[i] = 0;
            *condEdgeId = 0;
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    int cu = comp[u];
                    int cv = comp[v];
                    if (cu == cv) continue;
                    bool exists = false;
                    for (int ce = condHead[cu]; ce != 0; ce = condNext[ce])
                    {
                        if (condTo[ce] == cv) { exists = true; break; }
                    }
                    if (!exists)
                    {
                        int id = (*condEdgeId)++;
                        condTo[id] = cv;
                        condNext[id] = condHead[cu];
                        condHead[cu] = id;
                    }
                }
            }
        }
    }
}