namespace IAFahim.Graph.TreeQueries
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TreeDp
    {
        // --- MINIMUM VERTEX COVER ---
        private static void DfsVertexCover(
            int u, int p,
            int* head, int* to, int* next,
            int* dp0, int* dp1)
        {
            dp0[u] = 0;
            dp1[u] = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    DfsVertexCover(v, u, head, to, next, dp0, dp1);
                    dp0[u] += dp1[v];
                    dp1[u] += Math.Min(dp0[v], dp1[v]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinVertexCover(int n, int* head, int* to, int* next)
        {
            int* dp0 = stackalloc int[n];
            int* dp1 = stackalloc int[n];
            DfsVertexCover(0, -1, head, to, next, dp0, dp1);
            return Math.Min(dp0[0], dp1[0]);
        }

        // --- MAXIMUM INDEPENDENT SET ---
        private static void DfsIndependentSet(
            int u, int p,
            int* head, int* to, int* next,
            int* dp0, int* dp1)
        {
            dp0[u] = 0;
            dp1[u] = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    DfsIndependentSet(v, u, head, to, next, dp0, dp1);
                    dp0[u] += Math.Max(dp0[v], dp1[v]);
                    dp1[u] += dp0[v];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxIndependentSet(int n, int* head, int* to, int* next)
        {
            int* dp0 = stackalloc int[n];
            int* dp1 = stackalloc int[n];
            DfsIndependentSet(0, -1, head, to, next, dp0, dp1);
            return Math.Max(dp0[0], dp1[0]);
        }

        // --- MINIMUM DOMINATING SET ---
        private static void DfsDominatingSet(
            int u, int p,
            int* head, int* to, int* next,
            int* dp0, int* dp1, int* dp2)
        {
            dp0[u] = 1;
            dp2[u] = 0;
            
            int childCount = 0;
            int sumMin01 = 0;
            int minDiff = int.MaxValue / 2;
            bool hasMin0 = false;

            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    childCount++;
                    DfsDominatingSet(v, u, head, to, next, dp0, dp1, dp2);
                    
                    dp0[u] += Math.Min(Math.Min(dp0[v], dp1[v]), dp2[v]);
                    dp2[u] += dp1[v];

                    int min01 = Math.Min(dp0[v], dp1[v]);
                    sumMin01 += min01;

                    if (dp0[v] <= dp1[v])
                    {
                        hasMin0 = true;
                    }
                    else
                    {
                        int diff = dp0[v] - dp1[v];
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                        }
                    }
                }
            }

            if (childCount == 0)
            {
                dp0[u] = 1;
                dp1[u] = int.MaxValue / 2;
                dp2[u] = 0;
                return;
            }

            if (hasMin0)
            {
                dp1[u] = sumMin01;
            }
            else
            {
                dp1[u] = sumMin01 + minDiff;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DominatingSet(int n, int* head, int* to, int* next)
        {
            int* dp0 = stackalloc int[n];
            int* dp1 = stackalloc int[n];
            int* dp2 = stackalloc int[n];
            DfsDominatingSet(0, -1, head, to, next, dp0, dp1, dp2);
            return Math.Min(dp0[0], dp1[0]);
        }

        // --- MAXIMUM WEIGHT MATCHING ---
        private static void DfsMatching(
            int u, int p,
            int* head, int* to, int* next,
            long* edgeWeight,
            long* dp0, long* dp1)
        {
            dp0[u] = 0;
            dp1[u] = 0;

            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    DfsMatching(v, u, head, to, next, edgeWeight, dp0, dp1);
                    dp0[u] += Math.Max(dp0[v], dp1[v]);
                }
            }

            long sum = dp0[u];
            long maxCand = 0;

            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    long w = edgeWeight[e];
                    long cand = sum - Math.Max(dp0[v], dp1[v]) + dp0[v] + w;
                    if (cand > maxCand)
                    {
                        maxCand = cand;
                    }
                }
            }
            dp1[u] = maxCand;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MatchingDp(
            int n, int* head, int* to, int* next,
            long* edgeWeight)
        {
            long* dp0 = stackalloc long[n];
            long* dp1 = stackalloc long[n];
            DfsMatching(0, -1, head, to, next, edgeWeight, dp0, dp1);
            return Math.Max(dp0[0], dp1[0]);
        }

        // --- STEINER TREE ---
        private static void DfsSteiner(
            int u, int p,
            int* head, int* to, int* next,
            long* edgeWeight,
            byte* isTerminal,
            int* termCount,
            long* totalCost,
            int totalTerminals)
        {
            termCount[u] = isTerminal[u] != 0 ? 1 : 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    DfsSteiner(v, u, head, to, next, edgeWeight, isTerminal, termCount, totalCost, totalTerminals);
                    termCount[u] += termCount[v];
                    if (termCount[v] > 0 && termCount[v] < totalTerminals)
                    {
                        *totalCost += edgeWeight[e];
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SteinerTree(
            int n, int* head, int* to, int* next,
            long* edgeWeight,
            byte* isTerminal,
            int terminalCount)
        {
            if (terminalCount <= 1) return 0;
            int* termCount = stackalloc int[n];
            long totalCost = 0;
            DfsSteiner(0, -1, head, to, next, edgeWeight, isTerminal, termCount, &totalCost, terminalCount);
            return totalCost;
        }
    }
}
