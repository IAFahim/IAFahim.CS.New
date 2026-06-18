namespace IAFahim.Graph.Misc
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TopologicalDp
    {
        public static long Run(int n, int* order, long* dp, int* to, int* next, int* head)
        {
            for (int i = 0; i < n; i++)
            {
                int u = order[i];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e]; if (dp[u] + 1 > dp[v]) dp[v] = dp[u] + 1;
                }
            }
            long max = 0; for (int i = 0; i < n; i++) if (dp[i] > max) max = dp[i];
            return max;
        }
    }

    public static unsafe class CycleDp
    {
        public static long Run(int n, long* dp, int* next, long* values)
        {
            int* inDeg = stackalloc int[n];
            for (int i = 0; i < n; i++) inDeg[i] = 0;
            for (int i = 0; i < n; i++) inDeg[next[i]]++;
            bool* removed = stackalloc bool[n];
            for (int i = 0; i < n; i++) removed[i] = false;
            int* queue = stackalloc int[n]; int qh = 0, qt = 0;
            for (int i = 0; i < n; i++) if (inDeg[i] == 0) queue[qt++] = i;
            while (qh < qt)
            {
                int u = queue[qh++];
                removed[u] = true;
                int v = next[u];
                if (--inDeg[v] == 0) queue[qt++] = v;
            }
            long maxSum = 0;
            for (int i = 0; i < n; i++) dp[i] = 0;
            for (int start = 0; start < n; start++)
            {
                if (removed[start] || dp[start] != 0) continue;
                int cur = start; long sum = 0;
                while (dp[cur] == 0) { dp[cur] = 1; sum += values[cur]; cur = next[cur]; }
                if (sum > maxSum) maxSum = sum;
            }
            return maxSum;
        }
    }

    public static unsafe class SccDp
    {
        public static long Run(int n, int* sccId, long* sccSum, int* sccEdges, int* sccNext, int* sccHead, int sccCount)
        {
            long* dp = stackalloc long[sccCount]; int* inDeg = stackalloc int[sccCount];
            for (int i = 0; i < sccCount; i++) { dp[i] = sccSum[i]; inDeg[i] = 0; }
            ComputeInDegrees(sccCount, sccHead, sccNext, sccEdges, inDeg);
            int* q = stackalloc int[sccCount]; int qh = 0, qt = 0;
            for (int i = 0; i < sccCount; i++) if (inDeg[i] == 0) q[qt++] = i;
            while (qh < qt) ProcessScc(q[qh++], sccHead, sccNext, sccEdges, sccSum, dp, inDeg, q, ref qt);
            long max = 0; for (int i = 0; i < sccCount; i++) if (dp[i] > max) max = dp[i];
            return max;
        }
        private static void ComputeInDegrees(int cnt, int* head, int* next, int* edges, int* inDeg) { for (int u = 0; u < cnt; u++) for (int e = head[u]; e != 0; e = next[e]) inDeg[edges[e]]++; }
        private static void ProcessScc(int u, int* head, int* next, int* edges, long* sum, long* dp, int* inDeg, int* q, ref int qt) { for (int e = head[u]; e != 0; e = next[e]) { int v = edges[e]; if (dp[u] + sum[v] > dp[v]) dp[v] = dp[u] + sum[v]; if (--inDeg[v] == 0) q[qt++] = v; } }
    }

    public static unsafe class DagReachability
    {
        public static void Run(int n, int* order, long* reachable, int* to, int* next, int* head, long* bitsets, int bitsetSize)
        {
            for (int i = n - 1; i >= 0; i--)
            {
                int u = order[i]; bitsets[u * bitsetSize + (u >> 6)] |= 1L << (u & 63);
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e]; for (int j = 0; j < bitsetSize; j++) bitsets[u * bitsetSize + j] |= bitsets[v * bitsetSize + j];
                }
            }
        }
    }

    public static unsafe class TransitiveClosure
    {
        public static void Run(int n, int* adj, int* closure)
        {
            for (int i = 0; i < n * n; i++) closure[i] = adj[i];
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++) if (closure[i * n + k] != 0 && closure[k * n + j] != 0) closure[i * n + j] = 1;
        }
    }

    public static unsafe class WarshallBitset
    {
        public static void Run(int n, long* adj, long* closure, int wordsPerRow)
        {
            Buffer.MemoryCopy(adj, closure, n * wordsPerRow * sizeof(long), n * wordsPerRow * sizeof(long));
            for (int k = 0; k < n; k++)
            {
                long kMask = 1L << (k & 63); int kW = k >> 6;
                for (int i = 0; i < n; i++) if ((closure[i * wordsPerRow + kW] & kMask) != 0) PerformBitwiseOr(i, k, wordsPerRow, closure);
            }
        }
        private static void PerformBitwiseOr(int i, int k, int words, long* cls) { for (int j = 0; j < words; j++) cls[i * words + j] |= cls[k * words + j]; }
    }
}
