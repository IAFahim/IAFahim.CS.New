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
        private const int NoVertex = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitDistances(int n, long* d, bool* visited)
        {
            for (int j = 0; j < n; j++) { d[j] = long.MaxValue; visited[j] = false; }
            d[0] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindClosestUnvisited(int n, long* d, bool* visited)
        {
            int cur = NoVertex;
            for (int j = 0; j < n; j++)
                if (!visited[j] && (cur == NoVertex || d[j] < d[cur])) cur = j;
            return cur;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RelaxEdgesExcluding(int m, int excludedEdge, int* eu, int* ev, long* ew, int cur, long* d)
        {
            for (int e = 0; e < m; e++)
                if (e != excludedEdge && eu[e] == cur && d[cur] + ew[e] < d[ev[e]]) d[ev[e]] = d[cur] + ew[e];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ShortestAvoidingEdge(int n, int m, int* eu, int* ev, long* ew, int s, int t, int excludedEdge)
        {
            long* d = stackalloc long[n];
            bool* v = stackalloc bool[n];
            InitDistances(n, d, v);
            d[s] = 0;
            for (int it = 0; it < n; it++)
            {
                int cur = FindClosestUnvisited(n, d, v);
                if (cur == NoVertex || d[cur] == long.MaxValue) break;
                v[cur] = true;
                RelaxEdgesExcluding(m, excludedEdge, eu, ev, ew, cur, d);
            }
            return d[t];
        }

        public static void Run(int n, int m, int* eu, int* ev, long* ew, int s, int t, int pLen, int* pEdges, long* res)
        {
            for (int i = 0; i < pLen; i++)
                res[i] = ShortestAvoidingEdge(n, m, eu, ev, ew, s, t, pEdges[i]);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitDp(int n, long* dp)
        {
            for (int i = 0; i <= n; i++) for (int j = 0; j < n; j++) dp[i * n + j] = long.MaxValue;
            for (int j = 0; j < n; j++) dp[j] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RelaxLayer(int n, int m, int k, int* eu, int* ev, long* ew, long* dp)
        {
            for (int e = 0; e < m; e++)
            {
                if (dp[(k - 1) * n + eu[e]] != long.MaxValue)
                {
                    long v = dp[(k - 1) * n + eu[e]] + ew[e];
                    if (v < dp[k * n + ev[e]]) dp[k * n + ev[e]] = v;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double BestMeanForVertex(int n, int v, long* dp)
        {
            double maxVal = double.NegativeInfinity;
            for (int k = 0; k < n; k++)
                if (dp[k * n + v] != long.MaxValue)
                {
                    double val = (double)(dp[n * n + v] - dp[k * n + v]) / (n - k);
                    if (val > maxVal) maxVal = val;
                }
            return maxVal;
        }

        public static double Run(int n, int m, int* eu, int* ev, long* ew, long* dp)
        {
            InitDp(n, dp);
            for (int k = 1; k <= n; k++) RelaxLayer(n, m, k, eu, ev, ew, dp);
            double minMean = double.PositiveInfinity;
            for (int v = 0; v < n; v++)
            {
                if (dp[n * n + v] == long.MaxValue) continue;
                double maxVal = BestMeanForVertex(n, v, dp);
                if (maxVal < minMean) minMean = maxVal;
            }
            return minMean;
        }
    }
}
