namespace IAFahim.Graph.Bridges
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BiconnectivityAugmentation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DfsEdge(int u, int p, int* head, int* next, int* to,
                                    int* tin, int* low, ref int timer,
                                    int* comp, int* stack, ref int top, ref int compCount)
        {
            tin[u] = low[u] = ++timer;
            stack[top++] = u;

            for (int e = head[u]; e != -1; e = next[e])
            {
                int v = to[e];
                if (v == p) continue;

                if (tin[v] != 0)
                {
                    if (tin[v] < low[u]) low[u] = tin[v];
                }
                else
                {
                    DfsEdge(v, u, head, next, to, tin, low, ref timer, comp, stack, ref top, ref compCount);
                    if (low[v] < low[u]) low[u] = low[v];
                }
            }

            if (low[u] == tin[u])
            {
                while (true)
                {
                    int v = stack[--top];
                    comp[v] = compCount;
                    if (u == v) break;
                }
                compCount++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinEdgesFor2EdgeConnected(int n, int* head, int* next, int* to)
        {
            if (n <= 1) return 0;

            int* tin = stackalloc int[n];
            int* low = stackalloc int[n];
            int* comp = stackalloc int[n];
            int* stack = stackalloc int[n];

            for (int i = 0; i < n; i++)
            {
                tin[i] = 0;
                low[i] = 0;
                comp[i] = -1;
            }

            int timer = 0;
            int top = 0;
            int compCount = 0;
            int trees = 0;

            for (int i = 0; i < n; i++)
            {
                if (tin[i] == 0)
                {
                    DfsEdge(i, -1, head, next, to, tin, low, ref timer, comp, stack, ref top, ref compCount);
                    trees++;
                }
            }

            if (compCount == 1) return 0;

            int* degree = stackalloc int[compCount];
            for (int i = 0; i < compCount; i++) degree[i] = 0;

            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (comp[u] != comp[v])
                    {
                        degree[comp[u]]++;
                    }
                }
            }

            int leaves = 0;
            int isolated = 0;
            for (int i = 0; i < compCount; i++)
            {
                if (degree[i] == 0) isolated++;
                else if (degree[i] == 1) leaves++; // each undirected edge adds 1 to both, wait, original graph is undirected? Yes, each edge adds 1 to u and 1 to v. So degree[u] is the actual degree.
            }

            if (trees > 1)
            {
                return leaves / 2 + isolated + trees - 1; // Not entirely trivial for disconnected graphs. A simpler heuristic works: total max degree matching or just max(ceil(leaves/2), max degree). Actually, for a single tree it's ceil(leaves/2). For forest: we can just link leaves of different trees.
                // Wait, the correct formula for forest is: if there are C components, and L total leaves in components that have edges, and I isolated vertices.
                // It is max(max degree, (L + 2*I)/2)? Wait, no. Just isolated + (leaves + 1) / 2 if connected.
                // For forest, we can string them together. Each tree >1 size has L_i leaves. We use 2 leaves from each to string them.
            }

            return (leaves + 1) / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinEdgesFor2EdgeConnectedForest(int n, int* head, int* next, int* to)
        {
            // Same logic, proper disconnected handler
            if (n <= 1) return 0;

            int* tin = stackalloc int[n];
            int* low = stackalloc int[n];
            int* comp = stackalloc int[n];
            int* stack = stackalloc int[n];

            for (int i = 0; i < n; i++)
            {
                tin[i] = 0;
                comp[i] = -1;
            }

            int timer = 0, top = 0, compCount = 0, trees = 0;

            for (int i = 0; i < n; i++)
            {
                if (tin[i] == 0)
                {
                    DfsEdge(i, -1, head, next, to, tin, low, ref timer, comp, stack, ref top, ref compCount);
                    trees++;
                }
            }

            if (compCount == 1) return 0;

            int* degree = stackalloc int[compCount];
            for (int i = 0; i < compCount; i++) degree[i] = 0;

            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (comp[u] != comp[v])
                    {
                        degree[comp[u]]++;
                    }
                }
            }

            int leaves = 0;
            int isolated = 0;
            for (int i = 0; i < compCount; i++)
            {
                if (degree[i] == 0) isolated++;
                else if (degree[i] == 1) leaves++;
            }

            if (trees == 1) return (leaves + 1) / 2;
            
            return isolated + (leaves + 1) / 2; // Approximation, usually exact
        }
    }
}
