namespace IAFahim.Graph.TreeQueries
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TreeCentroid
    {
        private static void DfsCentroid(
            int u, int p,
            int* head, int* to, int* next,
            int* size, int* maxChild)
        {
            size[u] = 1;
            maxChild[u] = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    DfsCentroid(v, u, head, to, next, size, maxChild);
                    size[u] += size[v];
                    if (size[v] > maxChild[u]) maxChild[u] = size[v];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AllCentroids(
            int n, int* head, int* to, int* next,
            int* centroids, ref int centroidCount)
        {
            int* size = stackalloc int[n];
            int* maxChild = stackalloc int[n];
            DfsCentroid(0, -1, head, to, next, size, maxChild);

            centroidCount = 0;
            for (int i = 0; i < n; i++)
            {
                if (IsCentroid(i, n, size, maxChild))
                    centroids[centroidCount++] = i;
            }
        }

        private static bool IsCentroid(int i, int n, int* size, int* maxChild)
        {
            int rem = n - size[i];
            int maxComp = maxChild[i] > rem ? maxChild[i] : rem;
            return maxComp <= n / 2;
        }

        private static void DfsWeight(
            int u, int p,
            int* head, int* to, int* next,
            int* parent, long* weights, long* subtreeWeight)
        {
            parent[u] = p;
            subtreeWeight[u] = weights[u];
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    DfsWeight(v, u, head, to, next, parent, weights, subtreeWeight);
                    subtreeWeight[u] += subtreeWeight[v];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int WeightedMedian(
            int n, int* head, int* to, int* next,
            long* weights)
        {
            int* parent = stackalloc int[n];
            long* subtreeWeight = stackalloc long[n];
            DfsWeight(0, -1, head, to, next, parent, weights, subtreeWeight);

            long totalWeight = subtreeWeight[0];
            int curr = 0;
            while (true)
            {
                int nextNode = FindNextWeightedMedian(curr, parent, head, to, next, subtreeWeight, totalWeight);
                if (nextNode == -1) return curr;
                curr = nextNode;
            }
        }

        private static int FindNextWeightedMedian(int curr, int* parent, int* head, int* to, int* next, long* subtreeWeight, long totalWeight)
        {
            for (int e = head[curr]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != parent[curr] && subtreeWeight[v] > totalWeight / 2) return v;
            }
            return -1;
        }
    }
}
