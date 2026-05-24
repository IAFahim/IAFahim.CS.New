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
                if (v != p) { DfsVertexCover(v, u, head, to, next, dp0, dp1); UpdateVertexCoverDp(u, v, dp0, dp1); }
            }
        }

        private static void UpdateVertexCoverDp(int u, int v, int* dp0, int* dp1)
        {
            dp0[u] += dp1[v];
            dp1[u] += Math.Min(dp0[v], dp1[v]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinVertexCover(int n, int* head, int* to, int* next)
        {
            int* dp0 = stackalloc int[n], dp1 = stackalloc int[n];
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
                if (v != p) { DfsIndependentSet(v, u, head, to, next, dp0, dp1); UpdateIndependentSetDp(u, v, dp0, dp1); }
            }
        }

        private static void UpdateIndependentSetDp(int u, int v, int* dp0, int* dp1)
        {
            dp0[u] += Math.Max(dp0[v], dp1[v]);
            dp1[u] += dp0[v];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxIndependentSet(int n, int* head, int* to, int* next)
        {
            int* dp0 = stackalloc int[n], dp1 = stackalloc int[n];
            DfsIndependentSet(0, -1, head, to, next, dp0, dp1);
            return Math.Max(dp0[0], dp1[0]);
        }

        // --- MINIMUM DOMINATING SET ---
        private static void DfsDominatingSet(
            int u, int p,
            int* head, int* to, int* next,
            int* dp0, int* dp1, int* dp2)
        {
            dp0[u] = 1; dp2[u] = 0;
            int childCount = 0, sumMin01 = 0, minDiff = int.MaxValue / 2; bool hasMin0 = false;

            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    childCount++; DfsDominatingSet(v, u, head, to, next, dp0, dp1, dp2);
                    UpdateDominatingSetIntermediate(u, v, dp0, dp1, dp2, ref sumMin01, ref minDiff, ref hasMin0);
                }
            }
            FinalizeDominatingSetDp(u, childCount, sumMin01, minDiff, hasMin0, dp0, dp1, dp2);
        }

        private static void UpdateDominatingSetIntermediate(int u, int v, int* dp0, int* dp1, int* dp2, ref int sumMin01, ref int minDiff, ref bool hasMin0)
        {
            dp0[u] += Math.Min(Math.Min(dp0[v], dp1[v]), dp2[v]);
            dp2[u] += dp1[v];
            int min01 = Math.Min(dp0[v], dp1[v]);
            sumMin01 += min01;
            if (dp0[v] <= dp1[v]) hasMin0 = true;
            else minDiff = Math.Min(minDiff, dp0[v] - dp1[v]);
        }

        private static void FinalizeDominatingSetDp(int u, int childCount, int sumMin01, int minDiff, bool hasMin0, int* dp0, int* dp1, int* dp2)
        {
            if (childCount == 0) { dp0[u] = 1; dp1[u] = int.MaxValue / 2; dp2[u] = 0; return; }
            dp1[u] = hasMin0 ? sumMin01 : sumMin01 + minDiff;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DominatingSet(int n, int* head, int* to, int* next)
        {
            int* dp0 = stackalloc int[n], dp1 = stackalloc int[n], dp2 = stackalloc int[n];
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
            dp0[u] = 0; dp1[u] = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p) { DfsMatching(v, u, head, to, next, edgeWeight, dp0, dp1); dp0[u] += Math.Max(dp0[v], dp1[v]); }
            }
            dp1[u] = FindMaxMatchingCandidate(u, p, head, to, next, edgeWeight, dp0, dp1);
        }

        private static long FindMaxMatchingCandidate(int u, int p, int* head, int* to, int* next, long* edgeWeight, long* dp0, long* dp1)
        {
            long maxCand = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p) maxCand = Math.Max(maxCand, dp0[u] - Math.Max(dp0[v], dp1[v]) + dp0[v] + edgeWeight[e]);
            }
            return maxCand;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MatchingDp(int n, int* head, int* to, int* next, long* edgeWeight)
        {
            long* dp0 = stackalloc long[n], dp1 = stackalloc long[n];
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
                    if (termCount[v] > 0 && termCount[v] < totalTerminals) *totalCost += edgeWeight[e];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SteinerTree(int n, int* head, int* to, int* next, long* edgeWeight, byte* isTerminal, int terminalCount)
        {
            if (terminalCount <= 1) return 0;
            int* termCount = stackalloc int[n]; long totalCost = 0;
            DfsSteiner(0, -1, head, to, next, edgeWeight, isTerminal, termCount, &totalCost, terminalCount);
            return totalCost;
        }
    }
}
