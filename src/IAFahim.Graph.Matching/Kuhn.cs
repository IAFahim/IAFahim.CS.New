namespace IAFahim.Graph.Matching
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KuhnMatch
    {
        public static int Run(int nLeft, int nRight, int* head, int* to, int* next, int* matchRight)
        {
            for (int i = 0; i < nRight; i++) matchRight[i] = -1;
            int* seen = stackalloc int[nRight];
            int result = 0;
            for (int u = 0; u < nLeft; u++)
            {
                for (int i = 0; i < nRight; i++) seen[i] = 0;
                if (TryKuhn(u, head, to, next, matchRight, seen))
                    result++;
            }
            return result;
        }

        private static bool TryKuhn(int u, int* head, int* to, int* next, int* matchRight, int* seen)
        {
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (seen[v] != 0) continue;
                seen[v] = 1;
                if (matchRight[v] == -1 || TryKuhn(matchRight[v], head, to, next, matchRight, seen))
                {
                    matchRight[v] = u;
                    return true;
                }
            }
            return false;
        }

        public static int RunDense(int nLeft, int nRight, int* adj, int* matchRight)
        {
            for (int i = 0; i < nRight; i++) matchRight[i] = -1;
            int result = 0;
            int* seen = stackalloc int[nRight];
            for (int u = 0; u < nLeft; u++)
            {
                for (int i = 0; i < nRight; i++) seen[i] = 0;
                if (TryKuhnDense(u, adj, nRight, matchRight, seen))
                    result++;
            }
            return result;
        }

        private static bool TryKuhnDense(int u, int* adj, int nRight, int* matchRight, int* seen)
        {
            for (int v = 0; v < nRight; v++)
            {
                if (adj[u * nRight + v] == 0 || seen[v] != 0) continue;
                seen[v] = 1;
                if (matchRight[v] == -1 || TryKuhnDense(matchRight[v], adj, nRight, matchRight, seen))
                {
                    matchRight[v] = u;
                    return true;
                }
            }
            return false;
        }
    }

    public static unsafe class HopcroftKarp
    {
        public static int Run(int nLeft, int nRight, int* head, int* to, int* next, int* pairU, int* pairV)
        {
            int* dist = stackalloc int[nLeft];
            for (int i = 0; i < nLeft; i++) pairU[i] = -1;
            for (int i = 0; i < nRight; i++) pairV[i] = -1;
            int result = 0;
            while (Bfs(nLeft, head, to, next, pairU, pairV, dist))
            {
                for (int u = 0; u < nLeft; u++)
                {
                    if (pairU[u] == -1 && Dfs(u, head, to, next, pairU, pairV, dist))
                        result++;
                }
            }
            return result;
        }

        private static bool Bfs(int nLeft, int* head, int* to, int* next, int* pairU, int* pairV, int* dist)
        {
            int* q = stackalloc int[nLeft];
            int qh = 0, qt = 0;
            for (int u = 0; u < nLeft; u++)
            {
                if (pairU[u] == -1)
                {
                    dist[u] = 0;
                    q[qt++] = u;
                }
                else
                {
                    dist[u] = -1;
                }
            }
            bool found = false;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    int pu = pairV[v];
                    if (pu != -1 && dist[pu] == -1)
                    {
                        dist[pu] = dist[u] + 1;
                        q[qt++] = pu;
                    }
                    if (pu == -1)
                        found = true;
                }
            }
            return found;
        }

        private static bool Dfs(int u, int* head, int* to, int* next, int* pairU, int* pairV, int* dist)
        {
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                int pu = pairV[v];
                if (pu == -1 || (dist[pu] == dist[u] + 1 && Dfs(pu, head, to, next, pairU, pairV, dist)))
                {
                    pairU[u] = v;
                    pairV[v] = u;
                    return true;
                }
            }
            dist[u] = -1;
            return false;
        }
    }

    public static unsafe class KuhnMatchBipartiteCheck
    {
        public static bool Run(int n, int* head, int* to, int* next, int* color)
        {
            for (int i = 0; i < n; i++) color[i] = -1;
            for (int start = 0; start < n; start++)
            {
                if (color[start] != -1) continue;
                int* q = stackalloc int[n];
                int qh = 0, qt = 0;
                q[qt++] = start;
                color[start] = 0;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (color[v] == -1)
                        {
                            color[v] = 1 - color[u];
                            q[qt++] = v;
                        }
                        else if (color[v] == color[u])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    public static unsafe class BipartiteMaximumMatch
    {
        public static int Run(int nLeft, int nRight, int* head, int* to, int* next, int* matchLeft, int* matchRight)
        {
            for (int i = 0; i < nLeft; i++) matchLeft[i] = -1;
            for (int i = 0; i < nRight; i++) matchRight[i] = -1;
            return HopcroftKarp.Run(nLeft, nRight, head, to, next, matchLeft, matchRight);
        }
    }

    public static unsafe class MinimumVertexCoverBipartite
    {
        public static int Run(int n, int* head, int* to, int* next, int* matchLeft, int* matchRight, int* cover)
        {
            int* vis = stackalloc int[n];
            for (int i = 0; i < n; i++) vis[i] = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int u = 0; u < n; u++)
            {
                if (matchLeft[u] == -1)
                {
                    q[qt++] = u;
                    vis[u] = 1;
                }
            }
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (vis[v] != 0) continue;
                    vis[v] = 1;
                    int mu = matchRight[v];
                    if (mu != -1 && vis[mu] == 0)
                    {
                        vis[mu] = 1;
                        q[qt++] = mu;
                    }
                }
            }
            for (int u = 0; u < n; u++)
                cover[u] = (vis[u] == 0) ? 1 : 0;
            int count = 0;
            for (int u = 0; u < n; u++)
                if (cover[u] == 1) count++;
            return count;
        }
    }

    public static unsafe class MaximumIndependentSetBipartite
    {
        public static int Run(int n, int* cover)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                if (cover[i] == 0) count++;
            }
            return count;
        }
    }
}