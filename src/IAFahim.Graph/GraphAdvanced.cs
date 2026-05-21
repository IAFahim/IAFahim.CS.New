namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;
    using IAFahim.Graph.Flow;

    public static unsafe class MatroidIntersection
    {
        public static int Run(int n, int* set, int* rank1, int* rank2, int* basis, int* seen)
        {
            for (int i = 0; i < n; i++) basis[i] = -1;
            int result = 0;
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < n; i++) seen[i] = 0;
                int* queue = stackalloc int[n];
                int qh = 0, qt = 0;
                for (int i = 0; i < n; i++)
                {
                    if (basis[i] == -1) queue[qt++] = i;
                }
                while (qh < qt)
                {
                    int u = queue[qh++];
                    int e = u;
                    if (seen[e] != 0) continue;
                    seen[e] = 1;
                    for (int j = 0; j < n; j++)
                    {
                        if (basis[j] == -1) continue;
                        if (e == basis[j]) continue;
                        bool indep1 = true, indep2 = true;
                        if (indep1 && indep2)
                        {
                            int temp = basis[j];
                            basis[j] = e;
                            changed = true;
                            result++;
                        }
                    }
                }
            }
            return result;
        }
    }

    public static unsafe class StoerWagnerMinCut
    {
        public static long Run(int n, int* head, int* to, int* next, int* weight, long* add, long* dist, int* vis, int* parent)
        {
            long minCut = long.MaxValue;
            for (int i = 0; i < n; i++) add[i] = 0;
            for (int phase = 0; phase < n; phase++)
            {
                for (int i = 0; i < n; i++)
                {
                    dist[i] = 0;
                    vis[i] = -1;
                }
                int last = -1;
                for (int iter = 0; iter < n - phase; iter++)
                {
                    int v = -1;
                    long maxDist = long.MinValue;
                    for (int i = 0; i < n; i++)
                    {
                        if (vis[i] == -1)
                        {
                            add[i] += dist[i];
                            if (add[i] > maxDist)
                            {
                                maxDist = add[i];
                                v = i;
                            }
                        }
                    }
                    if (v == -1) break;
                    vis[v] = phase;
                    last = v;
                    for (int e = head[v]; e != 0; e = next[e])
                    {
                        int toV = to[e];
                        if (vis[toV] == -1)
                        {
                            dist[toV] += weight[e];
                        }
                    }
                }
                if (last != -1 && add[last] < minCut)
                {
                    minCut = add[last];
                }
                for (int e = head[last]; e != 0; e = next[e])
                {
                    int toV = to[e];
                    if (vis[toV] == phase)
                    {
                        int eRev = e ^ 1;
                        add[last] += weight[e] + weight[eRev];
                    }
                }
            }
            return minCut == long.MaxValue ? 0 : minCut;
        }
    }

    public static unsafe class GlobalMinCut
    {
        public static long Run(int n, int* head, int* to, int* next, int* weight)
        {
            long* add = stackalloc long[n];
            long* dist = stackalloc long[n];
            int* vis = stackalloc int[n];
            int* parent = stackalloc int[n];
            return StoerWagnerMinCut.Run(n, head, to, next, weight, add, dist, vis, parent);
        }
    }

    public static unsafe class GomoryHuTree
    {
        public static void Run(int n, int m, int* head, int* to, int* next, int* cap, int* treeHead, int* treeTo, int* treeNext, int* treeCap, int* parent)
        {
            for (int i = 0; i < n; i++) parent[i] = -1;
            for (int i = 0; i < n; i++)
            {
                long cut = DinicMaxFlow.Run(n, i, parent[i], head, to, next, cap, m);
                for (int j = i + 1; j < n; j++)
                {
                    if (parent[j] == i)
                    {
                        for (int e = head[j]; e != 0; e = next[e])
                        {
                            if (to[e] == parent[j])
                            {
                                cap[e] = (int)Math.Min(cut, cap[e]);
                                cap[e ^ 1] = (int)Math.Min(cut, cap[e ^ 1]);
                            }
                        }
                    }
                }
            }
        }
    }

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
        public static void Run(int u, bool valU, int v, bool valV, int* head, int* to, int* next, int* edgeCount)
        {
            int litU = valU ? (2 * u + 1) : (2 * u);
            int litV = valV ? (2 * v + 1) : (2 * v);
            AddEdge(litU ^ 1, litV, head, to, next, edgeCount);
            AddEdge(litV ^ 1, litU, head, to, next, edgeCount);
        }

        private static void AddEdge(int from, int toVal, int* head, int* to, int* next, int* edgeCount)
        {
            int id = ++(*edgeCount);
            to[id] = toVal;
            next[id] = head[from];
            head[from] = id;
        }
    }

    public static unsafe class TwoSatSolve
    {
        public static bool Run(int n, int* head, int* to, int* next, int* assignment)
        {
            int n2 = n * 2;
            int* index = stackalloc int[n2];
            int* lowlink = stackalloc int[n2];
            bool* onStack = stackalloc bool[n2];
            int* stack = stackalloc int[n2];
            int* comp = stackalloc int[n2];
            for (int i = 0; i < n2; i++) { index[i] = -1; onStack[i] = false; comp[i] = -1; }
            int stackSize = 0;
            int idx = 0;
            int sccCount = 0;
            for (int i = 0; i < n2; i++)
            {
                if (index[i] < 0)
                    IAFahim.Graph.TarjanScc.Run(i, head, to, next, index, lowlink, onStack, stack, ref stackSize, ref idx, ref sccCount, comp);
            }
            for (int i = 0; i < n; i++)
            {
                if (comp[2 * i] == comp[2 * i + 1]) return false;
            }
            for (int i = 0; i < n; i++)
                assignment[i] = comp[2 * i + 1] > comp[2 * i] ? 1 : 0;
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

    public static unsafe class DinicMaxFlow
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int m)
        {
            int* level = stackalloc int[n];
            int* it = stackalloc int[n];
            int* flow = stackalloc int[m + 1];
            for (int i = 0; i <= m; i++) flow[i] = 0;
            long result = 0;
            while (DinicBfs.Run(n, s, t, head, to, next, cap, flow, level, it))
            {
                while (true)
                {
                    int pushed = DinicDfs.Run(s, t, int.MaxValue, head, to, next, cap, flow, level, it);
                    if (pushed == 0) break;
                    result += pushed;
                }
            }
            return result;
        }
    }
}
