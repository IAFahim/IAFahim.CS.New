namespace IAFahim.Optimization.Treewidth
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RankDp
    {
        public static void ComputeOrder(int n, int* adj, int* order, int* parent, int* rank)
        {
            bool* visited = stackalloc bool[n];
            for (int i = 0; i < n; i++) visited[i] = false;
            
            int ptr = 0; order[0] = 0; visited[0] = true;
            for (int oi = 0; oi <= ptr; oi++)
            {
                int v = order[oi];
                for (int u = 0; u < n; u++)
                    if (adj[v * n + u] != 0 && !visited[u]) { visited[u] = true; order[++ptr] = u; }
            }
            
            for (int i = 0; i < n; i++) rank[order[i]] = i;
            for (int i = 0; i < n; i++) parent[i] = -1;
            for (int i = 1; i < n; i++) parent[order[i]] = order[i - 1];
        }

        public static void FillBag(int n, int v, int* adj, int* bag, int* bagSize)
        {
            *bagSize = 1; bag[0] = v;
            for (int u = 0; u < n; u++)
                if (u != v && adj[v * n + u] != 0 && !IsInBag(bag, *bagSize, u))
                    if (*bagSize < n) bag[(*bagSize)++] = u;
        }

        private static bool IsInBag(int* bag, int size, int u)
        {
            for (int i = 0; i < size; i++) if (bag[i] == u) return true;
            return false;
        }

        public static long Run(int n, long* edgeW, int* order, int* parent, long* dp)
        {
            for (int i = 0; i < n; i++) dp[i] = 0;
            for (int i = n - 1; i >= 0; i--)
            {
                long wSum = ComputeEdgeSum(n, i, order, edgeW);
                if (parent[i] >= 0) dp[parent[i]] += dp[order[i]] + wSum;
            }
            return dp[order[0]];
        }

        private static long ComputeEdgeSum(int n, int i, int* order, long* edgeW)
        {
            long sum = 0;
            for (int j = i + 1; j < n; j++)
                if (edgeW[order[i] * n + order[j]] != 0) sum += edgeW[order[i] * n + order[j]];
            return sum;
        }
    }
}
