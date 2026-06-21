namespace IAFahim.Graph.SCC
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SccAugmentation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Dfs1(int u, int* head, int* next, int* to, bool* visited, int* order, ref int time)
        {
            visited[u] = true;
            for (int e = head[u]; e != -1; e = next[e])
            {
                int v = to[e];
                if (!visited[v]) Dfs1(v, head, next, to, visited, order, ref time);
            }
            order[time++] = u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Dfs2(int u, int* revHead, int* revNext, int* revTo, int* comp, int c)
        {
            comp[u] = c;
            for (int e = revHead[u]; e != -1; e = revNext[e])
            {
                int v = revTo[e];
                if (comp[v] == -1) Dfs2(v, revHead, revNext, revTo, comp, c);
            }
        }

        private const int NilEdge = -1;
        private const int Unassigned = -1;
        private const int SingleComponent = 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitHeads(int n, int* head, int* revHead)
        {
            for (int i = 0; i < n; i++)
            {
                head[i] = NilEdge;
                revHead[i] = NilEdge;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildAdjacency(int m, int* u, int* v, int* head, int* next, int* to, int* revHead, int* revNext, int* revTo)
        {
            for (int i = 0; i < m; i++)
            {
                int from = u[i], dest = v[i];
                to[i] = dest;
                next[i] = head[from];
                head[from] = i;

                revTo[i] = from;
                revNext[i] = revHead[dest];
                revHead[dest] = i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ForwardOrder(int n, int* head, int* next, int* to, bool* visited, int* order)
        {
            for (int i = 0; i < n; i++) visited[i] = false;

            int time = 0;
            for (int i = 0; i < n; i++)
            {
                if (!visited[i]) Dfs1(i, head, next, to, visited, order, ref time);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AssignComponents(int n, int* order, int* revHead, int* revNext, int* revTo, int* comp)
        {
            for (int i = 0; i < n; i++) comp[i] = Unassigned;

            int sccCount = 0;
            for (int i = n - 1; i >= 0; i--)
            {
                int curr = order[i];
                if (comp[curr] == Unassigned)
                {
                    Dfs2(curr, revHead, revNext, revTo, comp, sccCount);
                    sccCount++;
                }
            }

            return sccCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CondensationDegrees(int sccCount, int m, int* u, int* v, int* comp, int* inDegree, int* outDegree)
        {
            for (int i = 0; i < sccCount; i++)
            {
                inDegree[i] = 0;
                outDegree[i] = 0;
            }

            for (int i = 0; i < m; i++)
            {
                int fromComp = comp[u[i]];
                int toComp = comp[v[i]];
                if (fromComp != toComp)
                {
                    outDegree[fromComp]++;
                    inDegree[toComp]++;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RequiredEdges(int sccCount, int* inDegree, int* outDegree)
        {
            int zeroIn = 0, zeroOut = 0;
            for (int i = 0; i < sccCount; i++)
            {
                if (inDegree[i] == 0) zeroIn++;
                if (outDegree[i] == 0) zeroOut++;
            }

            return zeroIn > zeroOut ? zeroIn : zeroOut;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinEdgesForStronglyConnected(int n, int m, int* u, int* v)
        {
            if (n <= 1) return 0;

            int* head = stackalloc int[n];
            int* next = stackalloc int[m];
            int* to = stackalloc int[m];

            int* revHead = stackalloc int[n];
            int* revNext = stackalloc int[m];
            int* revTo = stackalloc int[m];

            InitHeads(n, head, revHead);
            BuildAdjacency(m, u, v, head, next, to, revHead, revNext, revTo);

            bool* visited = stackalloc bool[n];
            int* order = stackalloc int[n];
            ForwardOrder(n, head, next, to, visited, order);

            int* comp = stackalloc int[n];
            int sccCount = AssignComponents(n, order, revHead, revNext, revTo, comp);

            if (sccCount == SingleComponent) return 0;

            int* inDegree = stackalloc int[sccCount];
            int* outDegree = stackalloc int[sccCount];
            CondensationDegrees(sccCount, m, u, v, comp, inDegree, outDegree);

            return RequiredEdges(sccCount, inDegree, outDegree);
        }
    }
}
