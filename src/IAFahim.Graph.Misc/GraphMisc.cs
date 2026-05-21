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
                    int v = to[e];
                    if (dp[u] + 1 > dp[v]) dp[v] = dp[u] + 1;
                }
            }
            long max = 0;
            for (int i = 0; i < n; i++) if (dp[i] > max) max = dp[i];
            return max;
        }
    }

    public static unsafe class CycleDp
    {
        public static long Run(int n, long* dp, int* next, long* values)
        {
            long* visited = stackalloc long[n];
            for (int i = 0; i < n; i++) visited[i] = 0;
            long maxSum = 0;
            for (int start = 0; start < n; start++)
            {
                if (visited[start] != 0) continue;
                int cur = start;
                long sum = 0;
                while (visited[cur] == 0)
                {
                    visited[cur] = 1;
                    sum += values[cur];
                    cur = next[cur];
                }
                if (sum > maxSum) maxSum = sum;
            }
            return maxSum;
        }
    }

    public static unsafe class SccDp
    {
        public static long Run(int n, int* sccId, long* sccSum, int* sccEdges, int* sccNext, int* sccHead, int sccCount)
        {
            long* dp = stackalloc long[sccCount];
            int* topoOrder = stackalloc int[sccCount];
            int* queue = stackalloc int[sccCount];
            int* inDeg = stackalloc int[sccCount];
            for (int i = 0; i < sccCount; i++) { dp[i] = sccSum[i]; inDeg[i] = 0; }
            for (int u = 0; u < sccCount; u++)
                for (int e = sccHead[u]; e != 0; e = sccNext[e])
                    inDeg[sccEdges[e]]++;
            int front = 0, rear = 0;
            for (int i = 0; i < sccCount; i++)
                if (inDeg[i] == 0) queue[rear++] = i;
            while (front < rear)
            {
                int u = queue[front++];
                for (int e = sccHead[u]; e != 0; e = sccNext[e])
                {
                    int v = sccEdges[e];
                    if (dp[u] + sccSum[v] > dp[v]) dp[v] = dp[u] + sccSum[v];
                    inDeg[v]--;
                    if (inDeg[v] == 0) queue[rear++] = v;
                }
            }
            long max = 0;
            for (int i = 0; i < sccCount; i++) if (dp[i] > max) max = dp[i];
            return max;
        }
    }

    public static unsafe class DagReachability
    {
        public static void Run(int n, int* order, long* reachable, int* to, int* next, int* head, long* bitsets, int bitsetSize)
        {
            for (int i = n - 1; i >= 0; i--)
            {
                int u = order[i];
                bitsets[u * bitsetSize + (u >> 6)] |= 1L << (u & 63);
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    for (int j = 0; j < bitsetSize; j++)
                        bitsets[u * bitsetSize + j] |= bitsets[v * bitsetSize + j];
                }
            }
        }
    }

    public static unsafe class TransitiveClosure
    {
        public static void Run(int n, int* adj, int* closure)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    closure[i * n + j] = adj[i * n + j];
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (closure[i * n + k] != 0 && closure[k * n + j] != 0)
                            closure[i * n + j] = 1;
        }
    }

    public static unsafe class WarshallBitset
    {
        public static void Run(int n, long* adj, long* closure, int wordsPerRow)
        {
            for (int i = 0; i < n * wordsPerRow; i++) closure[i] = adj[i];
            for (int k = 0; k < n; k++)
            {
                long kMask = 1L << (k & 63);
                int kW = k >> 6;
                for (int i = 0; i < n; i++)
                {
                    if ((closure[i * wordsPerRow + kW] & kMask) != 0)
                    {
                        for (int j = 0; j < wordsPerRow; j++)
                            closure[i * wordsPerRow + j] |= closure[k * wordsPerRow + j];
                    }
                }
            }
        }
    }
}