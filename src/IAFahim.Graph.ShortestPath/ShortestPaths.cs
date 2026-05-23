namespace IAFahim.Graph.ShortestPath
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class KthShortestPathEppstein
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int m, int k, int* eu, int* ev, long* ew, int s, long* dists)
        {
            for (int i = 0; i < n * k; i++) dists[i] = long.MaxValue;
            dists[s * k + 0] = 0;

            bool changed = true;
            for (int step = 0; step < n * k && changed; step++)
            {
                changed = false;
                for (int e = 0; e < m; e++)
                {
                    int u = eu[e];
                    int v = ev[e];
                    long w = ew[e];

                    for (int i = 0; i < k; i++)
                    {
                        long dU = dists[u * k + i];
                        if (dU == long.MaxValue) continue;

                        long cand = dU + w;
                        for (int j = 0; j < k; j++)
                        {
                            if (cand < dists[v * k + j])
                            {
                                for (int l = k - 1; l > j; l--)
                                {
                                    dists[v * k + l] = dists[v * k + l - 1];
                                }
                                dists[v * k + j] = cand;
                                changed = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public static unsafe class ReplacementPaths
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int m, int* eu, int* ev, long* ew, int s, int t, int pathLen, int* pathEdges, long* resultDists)
        {
            long* dist = stackalloc long[n];
            bool* visited = stackalloc bool[n];

            for (int i = 0; i < pathLen; i++)
            {
                int avoidEdge = pathEdges[i];

                for (int j = 0; j < n; j++)
                {
                    dist[j] = long.MaxValue;
                    visited[j] = false;
                }
                dist[s] = 0;

                for (int iter = 0; iter < n; iter++)
                {
                    int u = -1;
                    long minDist = long.MaxValue;
                    for (int j = 0; j < n; j++)
                    {
                        if (!visited[j] && dist[j] < minDist)
                        {
                            minDist = dist[j];
                            u = j;
                        }
                    }

                    if (u == -1 || minDist == long.MaxValue) break;
                    visited[u] = true;

                    for (int e = 0; e < m; e++)
                    {
                        if (e == avoidEdge) continue;
                        if (eu[e] == u)
                        {
                            int v = ev[e];
                            long w = ew[e];
                            if (dist[u] + w < dist[v])
                            {
                                dist[v] = dist[u] + w;
                            }
                        }
                    }
                }
                resultDists[i] = dist[t];
            }
        }
    }

    public static unsafe class AllPairsMinPlus
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    long min = long.MaxValue;
                    for (int k = 0; k < n; k++)
                    {
                        if (a[i * n + k] != long.MaxValue && b[k * n + j] != long.MaxValue)
                        {
                            long val = a[i * n + k] + b[k * n + j];
                            if (val < min) min = val;
                        }
                    }
                    c[i * n + j] = min;
                }
            }
        }
    }

    public static unsafe class ApspRepeatedSquaring
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int maxEdges, long* adj, long* result, long* temp, long* current)
        {
            for (int i = 0; i < n * n; i++) result[i] = adj[i];
            for (int i = 0; i < n * n; i++) current[i] = adj[i];

            int p = maxEdges - 1;
            while (p > 0)
            {
                if ((p & 1) != 0)
                {
                    AllPairsMinPlus.Run(n, result, current, temp);
                    for (int i = 0; i < n * n; i++) result[i] = temp[i];
                }
                p >>= 1;
                if (p > 0)
                {
                    AllPairsMinPlus.Run(n, current, current, temp);
                    for (int i = 0; i < n * n; i++) current[i] = temp[i];
                }
            }
        }
    }

    public static unsafe class DynamicShortestPathUpdate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EdgeDecreased(int n, long* dist, int u, int v, long newW)
        {
            if (newW >= dist[u * n + v]) return;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (dist[i * n + u] != long.MaxValue && dist[v * n + j] != long.MaxValue)
                    {
                        long path1 = dist[i * n + u] + newW + dist[v * n + j];
                        if (path1 < dist[i * n + j])
                        {
                            dist[i * n + j] = path1;
                        }
                    }
                }
            }
        }
    }

    public static unsafe class ConstrainedShortestPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int m, int k, int* eu, int* ev, long* ew, int s, long* dist)
        {
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            dist[s] = 0;

            long* nextDist = stackalloc long[n];

            for (int step = 0; step < k; step++)
            {
                for (int i = 0; i < n; i++) nextDist[i] = dist[i];

                for (int e = 0; e < m; e++)
                {
                    int u = eu[e];
                    int v = ev[e];
                    long w = ew[e];
                    if (dist[u] != long.MaxValue)
                    {
                        if (dist[u] + w < nextDist[v])
                        {
                            nextDist[v] = dist[u] + w;
                        }
                    }
                }
                for (int i = 0; i < n; i++) dist[i] = nextDist[i];
            }
        }
    }

    public static unsafe class ResourceConstrainedShortestPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int m, int maxCost, int* eu, int* ev, long* weight, int* cost, int s, long* dp)
        {
            int dpSize = (maxCost + 1) * n;
            for (int i = 0; i < dpSize; i++) dp[i] = long.MaxValue;
            dp[s] = 0;

            for (int c = 0; c <= maxCost; c++)
            {
                for (int e = 0; e < m; e++)
                {
                    int u = eu[e];
                    int v = ev[e];
                    int ec = cost[e];
                    long ew = weight[e];

                    if (c + ec <= maxCost && dp[c * n + u] != long.MaxValue)
                    {
                        long val = dp[c * n + u] + ew;
                        if (val < dp[(c + ec) * n + v])
                        {
                            dp[(c + ec) * n + v] = val;
                        }
                    }
                }
            }
        }
    }

    public static unsafe class ShortestPathWithPotentials
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ComputePotentials(int n, int m, int* eu, int* ev, long* ew, long* pot)
        {
            for (int i = 0; i < n; i++) pot[i] = 0;
            for (int i = 0; i < n; i++)
            {
                bool any = false;
                for (int e = 0; e < m; e++)
                {
                    int u = eu[e], v = ev[e];
                    long w = ew[e];
                    if (pot[u] + w < pot[v])
                    {
                        pot[v] = pot[u] + w;
                        any = true;
                    }
                }
                if (!any) return true;
            }
            for (int e = 0; e < m; e++)
            {
                int u = eu[e], v = ev[e];
                if (pot[u] + ew[e] < pot[v]) return false;
            }
            return true;
        }
    }

    public static unsafe class MinimumCycleMean
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Run(int n, int m, int* eu, int* ev, long* ew, long* dp)
        {
            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dp[i * n + j] = long.MaxValue;
                }
            }
            for (int j = 0; j < n; j++) dp[j] = 0;

            for (int k = 1; k <= n; k++)
            {
                for (int e = 0; e < m; e++)
                {
                    int u = eu[e];
                    int v = ev[e];
                    long w = ew[e];
                    if (dp[(k - 1) * n + u] != long.MaxValue)
                    {
                        long val = dp[(k - 1) * n + u] + w;
                        if (val < dp[k * n + v])
                            dp[k * n + v] = val;
                    }
                }
            }

            double minMean = double.PositiveInfinity;
            for (int v = 0; v < n; v++)
            {
                if (dp[n * n + v] == long.MaxValue) continue;
                double maxVal = double.NegativeInfinity;
                for (int k = 0; k < n; k++)
                {
                    if (dp[k * n + v] != long.MaxValue)
                    {
                        double val = (double)(dp[n * n + v] - dp[k * n + v]) / (n - k);
                        if (val > maxVal) maxVal = val;
                    }
                }
                if (maxVal < minMean) minMean = maxVal;
            }
            return minMean;
        }
    }
}
