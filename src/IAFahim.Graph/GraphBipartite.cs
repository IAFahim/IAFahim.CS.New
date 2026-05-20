namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class IsBipartite
    {
        public static bool Run(int n, int* head, int* to, int* next)
        {
            int* color = stackalloc int[n];
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

    public static unsafe class ColorBipartite
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
                    }
                }
            }
            return true;
        }
    }

    public static unsafe class ShortestPathUnweighted
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* dist, int* parent)
        {
            for (int i = 0; i < n; i++) dist[i] = -1;
            for (int i = 0; i < n; i++) parent[i] = -1;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            dist[start] = 0;
            q[qt++] = start;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (dist[v] == -1)
                    {
                        dist[v] = dist[u] + 1;
                        parent[v] = u;
                        q[qt++] = v;
                    }
                }
            }
        }
    }

    public static unsafe class BipartiteMatching
    {
        public static int Run(int nLeft, int nRight, int m, int* leftU, int* rightV, int* matchL, int* matchR)
        {
            for (int i = 0; i < nLeft; i++) matchL[i] = -1;
            for (int i = 0; i < nRight; i++) matchR[i] = -1;
            int result = 0;
            for (int u = 0; u < nLeft; u++)
            {
                bool* seen = stackalloc bool[nRight];
                for (int i = 0; i < nRight; i++) seen[i] = false;
                if (TryKuhn(u, m, leftU, rightV, matchR, seen, matchL))
                    result++;
            }
            return result;
        }

        private static bool TryKuhn(int u, int m, int* leftU, int* rightV, int* matchR, bool* seen, int* matchL)
        {
            for (int i = 0; i < m; i++)
            {
                if (leftU[i] != u) continue;
                int v = rightV[i];
                if (seen[v]) continue;
                seen[v] = true;
                if (matchR[v] == -1 || TryKuhn(matchR[v], m, leftU, rightV, matchR, seen, matchL))
                {
                    matchL[u] = v;
                    matchR[v] = u;
                    return true;
                }
            }
            return false;
        }
    }

    public static unsafe class KuhnMatch
    {
        public static int Run(int n, int m, int* eu, int* ev, int* matchL, int* matchR)
        {
            for (int i = 0; i < n; i++) matchL[i] = -1;
            for (int i = 0; i < m; i++) matchR[i] = -1;
            int result = 0;
            for (int u = 0; u < n; u++)
            {
                bool* used = stackalloc bool[m];
                for (int i = 0; i < m; i++) used[i] = false;
                if (TryKuhnBfs(u, m, eu, ev, matchR, used, matchL))
                    result++;
            }
            return result;
        }

        private static bool TryKuhnBfs(int u, int m, int* eu, int* ev, int* matchR, bool* used, int* matchL)
        {
            for (int i = 0; i < m; i++)
            {
                if (eu[i] != u) continue;
                int v = ev[i];
                if (used[v]) continue;
                used[v] = true;
                if (matchR[v] == -1 || TryKuhnBfs(matchR[v], m, eu, ev, matchR, used, matchL))
                {
                    matchL[u] = v;
                    matchR[v] = u;
                    return true;
                }
            }
            return false;
        }
    }

    public static unsafe class HopcroftKarp
    {
        public static int Run(int nLeft, int nRight, int m, int* eu, int* ev, int* matchL, int* matchR, int* dist)
        {
            for (int i = 0; i < nLeft; i++) matchL[i] = -1;
            for (int i = 0; i < nRight; i++) matchR[i] = -1;
            int result = 0;
            while (Bfs(nLeft, m, eu, ev, matchL, matchR, dist))
            {
                for (int u = 0; u < nLeft; u++)
                {
                    if (matchL[u] == -1 && Dfs(u, nLeft, nRight, m, eu, ev, matchL, matchR, dist))
                        result++;
                }
            }
            return result;
        }

        private static bool Bfs(int nLeft, int m, int* eu, int* ev, int* matchL, int* matchR, int* dist)
        {
            int* q = stackalloc int[nLeft];
            int qh = 0, qt = 0;
            for (int u = 0; u < nLeft; u++)
            {
                if (matchL[u] == -1)
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
                for (int i = 0; i < m; i++)
                {
                    if (eu[i] != u) continue;
                    int v = ev[i];
                    int nextU = matchR[v];
                    if (nextU != -1 && dist[nextU] == -1)
                    {
                        dist[nextU] = dist[u] + 1;
                        q[qt++] = nextU;
                    }
                    else if (nextU == -1)
                    {
                        found = true;
                    }
                }
            }
            return found;
        }

        private static bool Dfs(int u, int nLeft, int nRight, int m, int* eu, int* ev, int* matchL, int* matchR, int* dist)
        {
            for (int i = 0; i < m; i++)
            {
                if (eu[i] != u) continue;
                int v = ev[i];
                int nextU = matchR[v];
                if (nextU == -1 || (dist[nextU] == dist[u] + 1 && Dfs(nextU, nLeft, nRight, m, eu, ev, matchL, matchR, dist)))
                {
                    matchL[u] = v;
                    matchR[v] = u;
                    return true;
                }
            }
            dist[u] = -1;
            return false;
        }
    }

    public static unsafe class MinimumVertexCoverBipartite
    {
        public static int Run(int nLeft, int nRight, int m, int* eu, int* ev, int* matchL, int* matchR, int* coverL, int* coverR)
        {
            int* dist = stackalloc int[nLeft];
            HopcroftKarp.Run(nLeft, nRight, m, eu, ev, matchL, matchR, dist);
            bool* visitedL = stackalloc bool[nLeft];
            bool* visitedR = stackalloc bool[nRight];
            for (int i = 0; i < nLeft; i++) visitedL[i] = false;
            for (int i = 0; i < nRight; i++) visitedR[i] = false;
            int* q = stackalloc int[nLeft];
            int qh = 0, qt = 0;
            for (int u = 0; u < nLeft; u++)
            {
                if (matchL[u] == -1)
                {
                    q[qt++] = u;
                    visitedL[u] = true;
                }
            }
            while (qh < qt)
            {
                int u = q[qh++];
                for (int i = 0; i < m; i++)
                {
                    if (eu[i] != u) continue;
                    int v = ev[i];
                    if (!visitedR[v])
                    {
                        visitedR[v] = true;
                        if (matchR[v] != -1 && !visitedL[matchR[v]])
                        {
                            visitedL[matchR[v]] = true;
                            q[qt++] = matchR[v];
                        }
                    }
                }
            }
            int coverSize = 0;
            for (int u = 0; u < nLeft; u++)
            {
                coverL[u] = visitedL[u] ? 0 : 1;
                if (coverL[u] == 1) coverSize++;
            }
            for (int v = 0; v < nRight; v++)
            {
                coverR[v] = visitedR[v] ? 1 : 0;
                if (coverR[v] == 1) coverSize++;
            }
            return coverSize;
        }
    }

    public static unsafe class MaximumIndependentSetBipartite
    {
        public static int Run(int nLeft, int nRight, int m, int* eu, int* ev, int* matchL, int* matchR, int* indepL, int* indepR)
        {
            int* coverL = stackalloc int[nLeft];
            int* coverR = stackalloc int[nRight];
            MinimumVertexCoverBipartite.Run(nLeft, nRight, m, eu, ev, matchL, matchR, coverL, coverR);
            int indepSize = 0;
            for (int u = 0; u < nLeft; u++)
            {
                indepL[u] = coverL[u] == 0 ? 1 : 0;
                if (indepL[u] == 1) indepSize++;
            }
            for (int v = 0; v < nRight; v++)
            {
                indepR[v] = coverR[v] == 0 ? 1 : 0;
                if (indepR[v] == 1) indepSize++;
            }
            return indepSize;
        }
    }

    public static unsafe class HungarianMin
    {
        public static long Run(int n, long* cost, long* assign)
        {
            long* u = stackalloc long[n + 1];
            long* v = stackalloc long[n + 1];
            int* p = stackalloc int[n + 1];
            int* way = stackalloc int[n + 1];
            for (int i = 0; i <= n; i++) { u[i] = 0; v[i] = 0; p[i] = 0; way[i] = 0; }
            for (int i = 1; i <= n; i++)
            {
                p[0] = i;
                int j0 = 0;
                long* minv = stackalloc long[n + 1];
                for (int j = 0; j <= n; j++) minv[j] = long.MaxValue;
                bool* used = stackalloc bool[n + 1];
                for (int j = 0; j <= n; j++) used[j] = false;
                do
                {
                    used[j0] = true;
                    int i0 = p[j0];
                    long delta = long.MaxValue;
                    int j1 = 0;
                    for (int j = 1; j <= n; j++)
                    {
                        if (!used[j])
                        {
                            long cur = cost[i0 * (n + 1) + j] - u[i0] - v[j];
                            if (cur < minv[j])
                            {
                                minv[j] = cur;
                                way[j] = j0;
                            }
                            if (minv[j] < delta)
                            {
                                delta = minv[j];
                                j1 = j;
                            }
                        }
                    }
                    for (int j = 0; j <= n; j++)
                    {
                        if (used[j])
                        {
                            u[p[j]] += delta;
                            v[j] -= delta;
                        }
                        else
                        {
                            minv[j] -= delta;
                        }
                    }
                    j0 = j1;
                } while (p[j0] != 0);
                do
                {
                    int j1 = way[j0];
                    p[j0] = p[j1];
                    j0 = j1;
                } while (j0 != 0);
            }
            for (int j = 1; j <= n; j++) assign[p[j] - 1] = j - 1;
            long result = 0;
            for (int i = 1; i <= n; i++)
                result += cost[i * (n + 1) + (int)assign[i - 1] + 1];
            return result;
        }
    }

    public static unsafe class HungarianMax
    {
        public static long Run(int n, long* cost, long* assign)
        {
            long* maxCost = stackalloc long[(n + 1) * (n + 1)];
            long maxVal = long.MinValue;
            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    maxCost[i * (n + 1) + j] = cost[i * (n + 1) + j];
                    if (cost[i * (n + 1) + j] > maxVal) maxVal = cost[i * (n + 1) + j];
                }
            }
            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    maxCost[i * (n + 1) + j] = maxVal - maxCost[i * (n + 1) + j];
                }
            }
            long result = HungarianMin.Run(n, maxCost, assign);
            return (long)n * maxVal - result;
        }
    }

    public static unsafe class AssignmentSolve
    {
        public static int Run(int n, int m, long* cost, int* assign)
        {
            if (n <= m)
            {
                long* assignOut = stackalloc long[n];
                HungarianMin.Run(n, cost, assignOut);
                for (int i = 0; i < n; i++) assign[i] = (int)assignOut[i];
                return 0;
            }
            return -1;
        }
    }

    public static unsafe class GeneralMatchingBlossom
    {
        public static int Run(int n, int m, int* eu, int* ev, int* match)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            int matched = 0;
            for (int u = 0; u < n; u++)
            {
                if (match[u] != -1) continue;
                bool* used = stackalloc bool[n];
                for (int i = 0; i < n; i++) used[i] = false;
                int* q = stackalloc int[n];
                int* parent = stackalloc int[n];
                for (int i = 0; i < n; i++) parent[i] = -1;
                int qh = 0, qt = 0;
                q[qt++] = u;
                used[u] = true;
                while (qh < qt)
                {
                    int x = q[qh++];
                    for (int i = 0; i < m; i++)
                    {
                        int a = eu[i], b = ev[i];
                        if (a != x && b != x) continue;
                        int y = a == x ? b : a;
                        if (match[y] == -1)
                        {
                            int v = y, w = x;
                            while (v != -1)
                            {
                                int pv = parent[v];
                                int pw = parent[w];
                                int mv = match[v];
                                match[v] = w;
                                match[w] = v;
                                if (pv == -1) break;
                                w = pv;
                                v = pw;
                            }
                            matched++;
                            break;
                        }
                        else if (!used[match[y]])
                        {
                            used[y] = true;
                            used[match[y]] = true;
                            parent[y] = x;
                            parent[match[y]] = y;
                            q[qt++] = match[y];
                        }
                    }
                }
            }
            return matched;
        }
    }

    public static unsafe class StableMarriage
    {
        public static void Run(int n, int* manPref, int* womanPref, int* manMatch, int* womanMatch)
        {
            for (int i = 0; i < n; i++) manMatch[i] = -1;
            for (int i = 0; i < n; i++) womanMatch[i] = -1;
            int* manNext = stackalloc int[n];
            for (int i = 0; i < n; i++) manNext[i] = 0;
            int* queue = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int i = 0; i < n; i++) queue[qt++] = i;
            while (qh < n)
            {
                int man = queue[qh++];
                if (manNext[man] >= n) continue;
                int woman = manPref[man * n + manNext[man]++];
                if (womanMatch[woman] == -1)
                {
                    manMatch[man] = woman;
                    womanMatch[woman] = man;
                }
                else
                {
                    int currentMan = womanMatch[woman];
                    bool preferNew = false;
                    for (int i = 0; i < n; i++)
                    {
                        if (womanPref[woman * n + i] == man) { preferNew = true; break; }
                        if (womanPref[woman * n + i] == currentMan) break;
                    }
                    if (preferNew)
                    {
                        manMatch[man] = woman;
                        womanMatch[woman] = man;
                        manMatch[currentMan] = -1;
                        queue[qt++] = currentMan;
                    }
                    else
                    {
                        queue[qt++] = man;
                    }
                }
            }
        }
    }

    public static unsafe class GaleShapley
    {
        public static void Run(int n, int* proposerPref, int* receiverPref, int* proposerMatch, int* receiverMatch, bool* proposerIsMan)
        {
            for (int i = 0; i < n; i++) proposerMatch[i] = -1;
            for (int i = 0; i < n; i++) receiverMatch[i] = -1;
            int* nextIdx = stackalloc int[n];
            for (int i = 0; i < n; i++) nextIdx[i] = 0;
            int* free = stackalloc int[n];
            int fp = 0, bp = 0;
            for (int i = 0; i < n; i++) free[bp++] = i;
            while (fp < n)
            {
                int proposer = free[fp++];
                if (nextIdx[proposer] >= n) continue;
                int receiver = proposerPref[proposer * n + nextIdx[proposer]++];
                if (receiverMatch[receiver] == -1)
                {
                    proposerMatch[proposer] = receiver;
                    receiverMatch[receiver] = proposer;
                }
                else
                {
                    int currentProposer = receiverMatch[receiver];
                    bool preferNew = false;
                    for (int i = 0; i < n; i++)
                    {
                        if (receiverPref[receiver * n + i] == proposer) { preferNew = true; break; }
                        if (receiverPref[receiver * n + i] == currentProposer) break;
                    }
                    if (preferNew)
                    {
                        proposerMatch[proposer] = receiver;
                        receiverMatch[receiver] = proposer;
                        proposerMatch[currentProposer] = -1;
                        free[bp++] = currentProposer;
                    }
                    else
                    {
                        free[bp++] = proposer;
                    }
                }
            }
        }
    }
}
