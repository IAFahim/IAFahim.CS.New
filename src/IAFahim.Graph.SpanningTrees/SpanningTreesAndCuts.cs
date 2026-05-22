namespace IAFahim.Graph.SpanningTrees
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class MinimumFeedbackArcSetApprox
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int m, int* eu, int* ev, bool* remove)
        {
            int* inDegree = stackalloc int[n];
            int* outDegree = stackalloc int[n];
            for (int i = 0; i < n; i++) { inDegree[i] = 0; outDegree[i] = 0; }
            for (int i = 0; i < m; i++)
            {
                outDegree[eu[i]]++;
                inDegree[ev[i]]++;
            }
            int* score = stackalloc int[n];
            for (int i = 0; i < n; i++) score[i] = outDegree[i] - inDegree[i];

            int count = 0;
            for (int i = 0; i < m; i++)
            {
                if (score[eu[i]] < score[ev[i]])
                {
                    remove[i] = true;
                    count++;
                }
                else if (score[eu[i]] == score[ev[i]] && eu[i] >= ev[i])
                {
                    remove[i] = true;
                    count++;
                }
                else
                {
                    remove[i] = false;
                }
            }
            return count;
        }
    }

    public static unsafe class MinimumFeedbackVertexSet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int m, int* eu, int* ev, bool* remove)
        {
            for (int i = 0; i < n; i++) remove[i] = false;
            return 0; // NP-hard placeholder
        }
    }

    public static unsafe class MinimumPathCoverDag
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int m, int* eu, int* ev, int* matchL, int* matchR)
        {
            int* head = stackalloc int[n + 1];
            int* to = stackalloc int[m + 1];
            int* next = stackalloc int[m + 1];
            for (int i = 0; i <= n; i++) head[i] = 0;
            for (int i = 0; i < m; i++)
            {
                int u = eu[i] + 1;
                to[i + 1] = ev[i] + 1;
                next[i + 1] = head[u];
                head[u] = i + 1;
            }

            int* dist = stackalloc int[n + 1];
            for (int i = 0; i <= n; i++) { matchL[i] = 0; matchR[i] = 0; }

            int matching = 0;
            int* q = stackalloc int[n + 1];

            while (true)
            {
                int qh = 0, qt = 0;
                for (int i = 1; i <= n; i++)
                {
                    if (matchL[i] == 0)
                    {
                        dist[i] = 0;
                        q[qt++] = i;
                    }
                    else dist[i] = int.MaxValue;
                }
                dist[0] = int.MaxValue;
                while (qh < qt)
                {
                    int u = q[qh++];
                    if (dist[u] < dist[0])
                    {
                        for (int e = head[u]; e != 0; e = next[e])
                        {
                            int v = to[e];
                            if (dist[matchR[v]] == int.MaxValue)
                            {
                                dist[matchR[v]] = dist[u] + 1;
                                q[qt++] = matchR[v];
                            }
                        }
                    }
                }
                if (dist[0] == int.MaxValue) break;

                for (int i = 1; i <= n; i++)
                {
                    if (matchL[i] == 0 && Dfs(i, head, to, next, matchL, matchR, dist))
                    {
                        matching++;
                    }
                }
            }
            return n - matching;
        }

        private static bool Dfs(int u, int* head, int* to, int* next, int* matchL, int* matchR, int* dist)
        {
            if (u != 0)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (dist[matchR[v]] == dist[u] + 1)
                    {
                        if (Dfs(matchR[v], head, to, next, matchL, matchR, dist))
                        {
                            matchR[v] = u;
                            matchL[u] = v;
                            return true;
                        }
                    }
                }
                dist[u] = int.MaxValue;
                return false;
            }
            return true;
        }
    }

    public static unsafe class DilworthDecomposition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int m, int* eu, int* ev, int* matchL, int* matchR)
        {
            long byteCount = (long)n * n * sizeof(bool);
            bool* tc = (bool*)Marshal.AllocHGlobal((nint)byteCount);
            try
            {
                for (int i = 0; i < n * n; i++) tc[i] = false;
                for (int i = 0; i < m; i++) tc[eu[i] * n + ev[i]] = true;
                for (int k = 0; k < n; k++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (tc[i * n + k])
                        {
                            for (int j = 0; j < n; j++)
                            {
                                if (tc[k * n + j])
                                    tc[i * n + j] = true;
                            }
                        }
                    }
                }

                int tcEdges = 0;
                for (int i = 0; i < n * n; i++) if (tc[i]) tcEdges++;

                int* tceu = stackalloc int[tcEdges];
                int* tcev = stackalloc int[tcEdges];
                int idx = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (tc[i * n + j])
                        {
                            tceu[idx] = i;
                            tcev[idx] = j;
                            idx++;
                        }
                    }
                }

                return MinimumPathCoverDag.Run(n, tcEdges, tceu, tcev, matchL, matchR);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)tc);
            }
        }
    }

    public static unsafe class MaximumAntichain
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int m, int* eu, int* ev, bool* inAntichain)
        {
            int* matchL = stackalloc int[n + 1];
            int* matchR = stackalloc int[n + 1];
            int width = DilworthDecomposition.Run(n, m, eu, ev, matchL, matchR);

            long byteCount = (long)n * n * sizeof(bool);
            bool* tc = (bool*)Marshal.AllocHGlobal((nint)byteCount);
            try
            {
                for (int i = 0; i < n * n; i++) tc[i] = false;
                for (int i = 0; i < m; i++) tc[eu[i] * n + ev[i]] = true;
                for (int k = 0; k < n; k++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (tc[i * n + k])
                        {
                            for (int j = 0; j < n; j++) tc[i * n + j] |= tc[k * n + j];
                        }
                    }
                }

                int tcEdges = 0;
                for (int i = 0; i < n * n; i++) if (tc[i]) tcEdges++;
                int* head = stackalloc int[n + 1];
                int* to = stackalloc int[tcEdges + 1];
                int* next = stackalloc int[tcEdges + 1];
                for (int i = 0; i <= n; i++) head[i] = 0;
                int edgeIdx = 1;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (tc[i * n + j])
                        {
                            to[edgeIdx] = j + 1;
                            next[edgeIdx] = head[i + 1];
                            head[i + 1] = edgeIdx++;
                        }
                    }
                }

                bool* visitedL = stackalloc bool[n + 1];
                bool* visitedR = stackalloc bool[n + 1];
                for (int i = 0; i <= n; i++) { visitedL[i] = false; visitedR[i] = false; }

                int* q = stackalloc int[n + 1];
                int qh = 0, qt = 0;
                for (int i = 1; i <= n; i++)
                {
                    if (matchL[i] == 0)
                    {
                        visitedL[i] = true;
                        q[qt++] = i;
                    }
                }

                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (!visitedR[v])
                        {
                            visitedR[v] = true;
                            int leftNode = matchR[v];
                            if (leftNode != 0 && !visitedL[leftNode])
                            {
                                visitedL[leftNode] = true;
                                q[qt++] = leftNode;
                            }
                        }
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    inAntichain[i] = visitedL[i + 1] && !visitedR[i + 1];
                }

                return width;
            }
            finally
            {
                Marshal.FreeHGlobal((nint)tc);
            }
        }
    }

    public static unsafe class PosetWidth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int m, int* eu, int* ev)
        {
            int* matchL = stackalloc int[n + 1];
            int* matchR = stackalloc int[n + 1];
            return DilworthDecomposition.Run(n, m, eu, ev, matchL, matchR);
        }
    }

    public static unsafe class PosetChainDecomposition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int m, int* eu, int* ev, int* chainId)
        {
            int* matchL = stackalloc int[n + 1];
            int* matchR = stackalloc int[n + 1];
            int width = DilworthDecomposition.Run(n, m, eu, ev, matchL, matchR);

            for (int i = 0; i < n; i++) chainId[i] = -1;
            int currentChain = 0;

            for (int i = 1; i <= n; i++)
            {
                if (matchR[i] == 0)
                {
                    int curr = i;
                    while (curr != 0)
                    {
                        chainId[curr - 1] = currentChain;
                        curr = matchL[curr];
                    }
                    currentChain++;
                }
            }
            return width;
        }
    }

    public static unsafe class TransitiveReductionDag
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int m, int* eu, int* ev, bool* keepEdge)
        {
            long byteCount = (long)n * n * sizeof(bool);
            bool* tc = (bool*)Marshal.AllocHGlobal((nint)byteCount);
            try
            {
                for (int i = 0; i < n * n; i++) tc[i] = false;
                for (int i = 0; i < m; i++) tc[eu[i] * n + ev[i]] = true;

                for (int k = 0; k < n; k++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (tc[i * n + k])
                        {
                            for (int j = 0; j < n; j++)
                            {
                                if (tc[k * n + j]) tc[i * n + j] = true;
                            }
                        }
                    }
                }

                int count = 0;
                for (int i = 0; i < m; i++)
                {
                    int u = eu[i];
                    int v = ev[i];
                    bool redundant = false;
                    for (int k = 0; k < n; k++)
                    {
                        if (k != u && k != v && tc[u * n + k] && tc[k * n + v])
                        {
                            redundant = true;
                            break;
                        }
                    }
                    keepEdge[i] = !redundant;
                    if (!redundant) count++;
                }
                return count;
            }
            finally
            {
                Marshal.FreeHGlobal((nint)tc);
            }
        }
    }

    public static unsafe class TransitiveClosureBitset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int m, int* eu, int* ev, ulong* tc)
        {
            int words = (n + 63) >> 6;
            for (int i = 0; i < n * words; i++) tc[i] = 0;

            for (int i = 0; i < m; i++)
            {
                int u = eu[i];
                int v = ev[i];
                tc[u * words + (v >> 6)] |= (1UL << (v & 63));
            }

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    if ((tc[i * words + (k >> 6)] & (1UL << (k & 63))) != 0)
                    {
                        for (int w = 0; w < words; w++)
                        {
                            tc[i * words + w] |= tc[k * words + w];
                        }
                    }
                }
            }
        }
    }

    public static unsafe class ReachabilityIndexBuild
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int m, int* eu, int* ev, ulong* tcOut)
        {
            TransitiveClosureBitset.Run(n, m, eu, ev, tcOut);
        }
    }

    public static unsafe class ReachabilityIndexQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int u, int v, int words, ulong* tc)
        {
            return (tc[u * words + (v >> 6)] & (1UL << (v & 63))) != 0;
        }
    }
}
