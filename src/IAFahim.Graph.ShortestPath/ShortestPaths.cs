namespace IAFahim.Graph.ShortestPath
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KthShortestPathEppstein
    {
        public static void Run(int n, int m, int k, int* eu, int* ev, long* ew, int s, long* dists)
        {
            for (int i = 0; i < n * k; i++) dists[i] = long.MaxValue;
            dists[s * k] = 0; bool changed = true;
            for (int step = 0; step < n * k && changed; step++)
            {
                changed = false;
                for (int e = 0; e < m; e++)
                {
                    int u = eu[e], v = ev[e]; long w = ew[e];
                    for (int i = 0; i < k; i++)
                    {
                        if (dists[u * k + i] == long.MaxValue) continue;
                        if (InsertIfBetter(k, v, dists[u * k + i] + w, dists)) changed = true;
                    }
                }
            }
        }
        private static bool InsertIfBetter(int k, int v, long cand, long* dists)
        {
            for (int j = 0; j < k; j++) if (cand < dists[v * k + j]) { for (int l = k - 1; l > j; l--) dists[v * k + l] = dists[v * k + l - 1]; dists[v * k + j] = cand; return true; }
            return false;
        }
    }

    public static unsafe class ReplacementPaths
    {
        public static void Run(int n, int m, int* eu, int* ev, long* ew, int s, int t, int pLen, int* pEdges, long* res)
        {
            for (int i = 0; i < pLen; i++)
            {
                long* d = stackalloc long[n]; bool* v = stackalloc bool[n];
                for (int j = 0; j < n; j++) { d[j] = long.MaxValue; v[j] = false; }
                d[s] = 0;
                for (int it = 0; it < n; it++)
                {
                    int cur = -1; for (int j = 0; j < n; j++) if (!v[j] && (cur == -1 || d[j] < d[cur])) cur = j;
                    if (cur == -1 || d[cur] == long.MaxValue) break;
                    v[cur] = true;
                    for (int e = 0; e < m; e++) if (e != pEdges[i] && eu[e] == cur && d[cur] + ew[e] < d[ev[e]]) d[ev[e]] = d[cur] + ew[e];
                }
                res[i] = d[t];
            }
        }
    }

    public static unsafe class AllPairsMinPlus
    {
        public static void Run(int n, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    long min = long.MaxValue;
                    for (int k = 0; k < n; k++)
                        if (a[i * n + k] != long.MaxValue && b[k * n + j] != long.MaxValue)
                        {
                            long v = a[i * n + k] + b[k * n + j]; if (v < min) min = v;
                        }
                    c[i * n + j] = min;
                }
        }
    }

    public static unsafe class DynamicShortestPathUpdate
    {
        public static void EdgeDecreased(int n, long* dist, int u, int v, long newW)
        {
            if (newW >= dist[u * n + v]) return;
            for (int i = 0; i < n; i++)
            {
                long d_iu = dist[i * n + u]; if (d_iu == long.MaxValue) continue;
                for (int j = 0; j < n; j++)
                {
                    long d_vj = dist[v * n + j];
                    if (d_vj != long.MaxValue) { long c = d_iu + newW + d_vj; if (c < dist[i * n + j]) dist[i * n + j] = c; }
                }
            }
        }
    }

    public static unsafe class MinimumCycleMean
    {
        public static double Run(int n, int m, int* eu, int* ev, long* ew, long* dp)
        {
            for (int i = 0; i <= n; i++) for (int j = 0; j < n; j++) dp[i * n + j] = long.MaxValue;
            for (int j = 0; j < n; j++) dp[j] = 0;
            for (int k = 1; k <= n; k++)
                for (int e = 0; e < m; e++)
                {
                    if (dp[(k - 1) * n + eu[e]] != long.MaxValue)
                    {
                        long v = dp[(k - 1) * n + eu[e]] + ew[e]; if (v < dp[k * n + ev[e]]) dp[k * n + ev[e]] = v;
                    }
                }
            double minMean = double.PositiveInfinity;
            for (int v = 0; v < n; v++)
            {
                if (dp[n * n + v] == long.MaxValue) continue;
                double maxVal = double.NegativeInfinity;
                for (int k = 0; k < n; k++)
                    if (dp[k * n + v] != long.MaxValue)
                    {
                        double val = (double)(dp[n * n + v] - dp[k * n + v]) / (n - k);
                        if (val > maxVal) maxVal = val;
                    }
                if (maxVal < minMean) minMean = maxVal;
            }
            return minMean;
        }
    }
}
