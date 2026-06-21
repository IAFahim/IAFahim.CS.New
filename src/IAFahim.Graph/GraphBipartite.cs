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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitArrays(int n, long* u, long* v, int* p, int* way)
        {
            for (int i = 0; i <= n; i++)
            {
                u[i] = 0;
                v[i] = 0;
                p[i] = 0;
                way[i] = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateDistances(int n, int i0, int j0, long* cost, long* u, long* v, long* minv, int* way, bool* used, ref long delta, ref int j1)
        {
            for (int j = 1; j <= n; j++)
            {
                if (!used[j])
                {
                    long cur = cost[(i0 - 1) * n + (j - 1)] - u[i0] - v[j];
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdatePotentials(int n, long delta, long* u, long* v, long* minv, int* p, bool* used)
        {
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FindAugmentingPath(int n, int i, long* cost, long* u, long* v, int* p, int* way)
        {
            p[0] = i;
            int j0 = 0;
            long* minv = stackalloc long[n + 1];
            for (int j = 0; j <= n; j++)
            {
                minv[j] = long.MaxValue;
            }
            bool* used = stackalloc bool[n + 1];
            for (int j = 0; j <= n; j++)
            {
                used[j] = false;
            }
            do
            {
                used[j0] = true;
                int i0 = p[j0];
                long delta = long.MaxValue;
                int j1 = 0;
                UpdateDistances(n, i0, j0, cost, u, v, minv, way, used, ref delta, ref j1);
                UpdatePotentials(n, delta, u, v, minv, p, used);
                j0 = j1;
            } while (p[j0] != 0);
            do
            {
                int j1 = way[j0];
                p[j0] = p[j1];
                j0 = j1;
            } while (j0 != 0);
        }

        public static long Run(int n, long* cost, long* assign)
        {
            long* u = stackalloc long[n + 1];
            long* v = stackalloc long[n + 1];
            int* p = stackalloc int[n + 1];
            int* way = stackalloc int[n + 1];
            InitArrays(n, u, v, p, way);
            for (int i = 1; i <= n; i++)
            {
                FindAugmentingPath(n, i, cost, u, v, p, way);
            }
            for (int j = 1; j <= n; j++)
            {
                assign[p[j] - 1] = j - 1;
            }
            long result = 0;
            for (int i = 1; i <= n; i++)
            {
                result += cost[(i - 1) * n + (int)assign[i - 1]];
            }
            return result;
        }
    }

    public static unsafe class HungarianMax
    {
        public static long Run(int n, long* cost, long* assign)
        {
            long* maxCost = stackalloc long[n * n];
            long maxVal = long.MinValue;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    maxCost[i * n + j] = cost[i * n + j];
                    if (cost[i * n + j] > maxVal)
                    {
                        maxVal = cost[i * n + j];
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    maxCost[i * n + j] = maxVal - maxCost[i * n + j];
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
        private static int GetLca(int n, int* base_, int* parent, int* match, int* inPath, int u, int v)
        {
            for (int i = 0; i < n; i++) inPath[i] = 0;
            while (true)
            {
                u = base_[u];
                inPath[u] = 1;
                if (match[u] == -1) break;
                u = base_[parent[match[u]]];
            }
            while (true)
            {
                v = base_[v];
                if (inPath[v] == 1) return v;
                v = base_[parent[match[v]]];
            }
        }

        private static void Contract(int n, int* base_, int* parent, int* match, int* color, int* q, ref int qt, int u, int v, int lca)
        {
            while (base_[u] != lca)
            {
                parent[u] = v;
                int mv = match[u];
                if (color[mv] == 1)
                {
                    color[mv] = 0;
                    q[qt++] = mv;
                }
                int oldBaseU = base_[u];
                int oldBaseMv = base_[mv];
                for (int i = 0; i < n; i++)
                {
                    if (base_[i] == oldBaseU || base_[i] == oldBaseMv)
                    {
                        base_[i] = lca;
                    }
                }
                v = mv;
                u = parent[v];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ProcessAugmentingPath(int v, int u, int* match, int* parent)
        {
            parent[v] = u;
            int cur = v;
            while (cur != -1)
            {
                int pNode = parent[cur];
                int nextMatched = match[pNode];
                match[cur] = pNode;
                match[pNode] = cur;
                cur = nextMatched;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ProcessNeighbor(int n, int u, int v, int* base_, int* match, int* parent, int* color, int* q, ref int qt, int* inPath)
        {
            if (base_[u] == base_[v] || match[u] == v) return false;
            if (color[v] == -1)
            {
                if (match[v] == -1)
                {
                    return ProcessAugmentingPath(v, u, match, parent);
                }
                color[v] = 1;
                parent[v] = u;
                int mv = match[v];
                color[mv] = 0;
                parent[mv] = v;
                q[qt++] = mv;
            }
            else if (color[v] == 0)
            {
                int lca = GetLca(n, base_, parent, match, inPath, u, v);
                Contract(n, base_, parent, match, color, q, ref qt, u, v, lca);
                Contract(n, base_, parent, match, color, q, ref qt, v, u, lca);
            }
            return false;
        }

        private static bool FindAugmentingPath(int n, int* head, int* to, int* next, int* match, int* parent, int* base_, int* color, int* q, int* inPath, int s)
        {
            for (int i = 0; i < n; i++)
            {
                color[i] = -1;
                parent[i] = -1;
                base_[i] = i;
            }
            int qh = 0, qt = 0;
            color[s] = 0;
            q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    if (ProcessNeighbor(n, u, to[e], base_, match, parent, color, q, ref qt, inPath))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int Run(int n, int m, int* eu, int* ev, int* match)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            int* head = stackalloc int[n];
            for (int i = 0; i < n; i++) head[i] = 0;
            int* to = stackalloc int[2 * m + 2];
            int* next = stackalloc int[2 * m + 2];
            int edgeId = 1;
            for (int i = 0; i < m; i++)
            {
                int u = eu[i];
                int v = ev[i];
                if (u == v) continue;
                to[edgeId] = v;
                next[edgeId] = head[u];
                head[u] = edgeId++;
                to[edgeId] = u;
                next[edgeId] = head[v];
                head[v] = edgeId++;
            }
            int* base_ = stackalloc int[n];
            int* p = stackalloc int[n];
            int* vArr = stackalloc int[n];
            int* blossom = stackalloc int[n];
            int* q = stackalloc int[n];
            int result = 0;
            for (int s = 0; s < n; s++)
            {
                if (match[s] != -1) continue;
                if (FindAugmentingPath(n, head, to, next, match, p, base_, vArr, q, blossom, s))
                {
                    result++;
                }
            }
            return result;
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
            int* womanRank = stackalloc int[n * n];
            for (int w = 0; w < n; w++)
            {
                for (int r = 0; r < n; r++)
                {
                    int mID = womanPref[w * n + r];
                    womanRank[w * n + mID] = r;
                }
            }
            int* stack = stackalloc int[n];
            int top = 0;
            for (int i = 0; i < n; i++) stack[top++] = i;
            while (top > 0)
            {
                int m = stack[--top];
                if (manNext[m] >= n) continue;
                int w = manPref[m * n + manNext[m]++];
                if (womanMatch[w] == -1)
                {
                    manMatch[m] = w;
                    womanMatch[w] = m;
                }
                else
                {
                    int m2 = womanMatch[w];
                    if (womanRank[w * n + m] < womanRank[w * n + m2])
                    {
                        manMatch[m2] = -1;
                        manMatch[m] = w;
                        womanMatch[w] = m;
                        stack[top++] = m2;
                    }
                    else
                    {
                        stack[top++] = m;
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
            int* receiverRank = stackalloc int[n * n];
            for (int r = 0; r < n; r++)
            {
                for (int rank = 0; rank < n; rank++)
                {
                    int pID = receiverPref[r * n + rank];
                    receiverRank[r * n + pID] = rank;
                }
            }
            int* stack = stackalloc int[n];
            int top = 0;
            for (int i = 0; i < n; i++) stack[top++] = i;
            while (top > 0)
            {
                int proposer = stack[--top];
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
                    if (receiverRank[receiver * n + proposer] < receiverRank[receiver * n + currentProposer])
                    {
                        proposerMatch[proposer] = receiver;
                        receiverMatch[receiver] = proposer;
                        proposerMatch[currentProposer] = -1;
                        stack[top++] = currentProposer;
                    }
                    else
                    {
                        stack[top++] = proposer;
                    }
                }
            }
        }
    }
}
