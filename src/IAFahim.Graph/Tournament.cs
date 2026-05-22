namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Tournament
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TournamentHamiltonianPath(int n, byte* adj, int* path)
        {
            for (int i = 0; i < n; i++)
            {
                path[i] = i;
            }
            for (int i = 1; i < n; i++)
            {
                int val = path[i];
                int j = i - 1;
                while (j >= 0 && adj[val * n + path[j]] == 1)
                {
                    path[j + 1] = path[j];
                    j--;
                }
                path[j + 1] = val;
            }
        }

        public static bool TournamentHamiltonianCycle(int n, byte* adj, int* cycle)
        {
            if (n <= 2)
            {
                if (n == 1)
                {
                    cycle[0] = 0;
                    return true;
                }
                return false;
            }

            int* path = stackalloc int[n];
            TournamentHamiltonianPath(n, adj, path);

            int cycleLen = 0;
            int last = path[n - 1];
            int firstIdx = -1;
            for (int i = 0; i < n - 1; i++)
            {
                if (adj[last * n + path[i]] == 1)
                {
                    firstIdx = i;
                    break;
                }
            }

            if (firstIdx == -1)
            {
                return false;
            }

            for (int i = firstIdx; i < n; i++)
            {
                cycle[cycleLen++] = path[i];
            }

            byte* inCycle = stackalloc byte[n];
            for (int i = 0; i < n; i++)
            {
                inCycle[i] = 0;
            }
            for (int i = 0; i < cycleLen; i++)
            {
                inCycle[cycle[i]] = 1;
            }

            for (int i = 0; i < n; i++)
            {
                if (inCycle[i] == 1)
                {
                    continue;
                }

                int u = i;
                int insertPos = -1;
                for (int j = 0; j < cycleLen; j++)
                {
                    int curr = cycle[j];
                    int next = cycle[(j + 1) % cycleLen];
                    if (adj[curr * n + u] == 1 && adj[u * n + next] == 1)
                    {
                        insertPos = j + 1;
                        break;
                    }
                }

                if (insertPos != -1)
                {
                    for (int j = cycleLen; j > insertPos; j--)
                    {
                        cycle[j] = cycle[j - 1];
                    }
                    cycle[insertPos] = u;
                    cycleLen++;
                    inCycle[u] = 1;
                }
                else
                {
                    return false;
                }
            }

            return cycleLen == n;
        }

        public static void TournamentMedianOrder(int n, byte* adj, int* bestOrder)
        {
            int* current = stackalloc int[n];
            int* best = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                current[i] = i;
                best[i] = i;
            }

            int maxScore = -1;
            byte* used = stackalloc byte[n];
            for (int i = 0; i < n; i++)
            {
                used[i] = 0;
            }

            MedianOrderBacktrack(0, n, adj, current, used, best, &maxScore);

            for (int i = 0; i < n; i++)
            {
                bestOrder[i] = best[i];
            }
        }

        private static void MedianOrderBacktrack(int step, int n, byte* adj, int* current, byte* used, int* best, int* maxScore)
        {
            if (step == n)
            {
                int score = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        if (adj[current[i] * n + current[j]] == 1)
                        {
                            score++;
                        }
                    }
                }
                if (score > *maxScore)
                {
                    *maxScore = score;
                    for (int i = 0; i < n; i++)
                    {
                        best[i] = current[i];
                    }
                }
                return;
            }

            for (int i = 0; i < n; i++)
            {
                if (used[i] == 0)
                {
                    used[i] = 1;
                    current[step] = i;
                    MedianOrderBacktrack(step + 1, n, adj, current, used, best, maxScore);
                    used[i] = 0;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TournamentKingFind(int n, byte* adj)
        {
            for (int u = 0; u < n; u++)
            {
                bool isKing = true;
                for (int v = 0; v < n; v++)
                {
                    if (u == v)
                    {
                        continue;
                    }
                    if (adj[u * n + v] == 1)
                    {
                        continue;
                    }

                    bool reachable2 = false;
                    for (int w = 0; w < n; w++)
                    {
                        if (adj[u * n + w] == 1 && adj[w * n + v] == 1)
                        {
                            reachable2 = true;
                            break;
                        }
                    }
                    if (!reachable2)
                    {
                        isKing = false;
                        break;
                    }
                }
                if (isKing)
                {
                    return u;
                }
            }
            return 0;
        }

        public static bool EulerianOrientation(int numNodes, int numEdges, int* head, int* next, int* to, int* edgeU, int* edgeV, int* orientedU, int* orientedV)
        {
            int* degree = stackalloc int[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                degree[i] = 0;
            }
            for (int i = 0; i < numEdges; i++)
            {
                degree[edgeU[i]]++;
                degree[edgeV[i]]++;
            }
            for (int i = 0; i < numNodes; i++)
            {
                if (degree[i] % 2 != 0)
                {
                    return false;
                }
            }

            byte* visitedEdge = stackalloc byte[numEdges];
            for (int i = 0; i < numEdges; i++)
            {
                visitedEdge[i] = 0;
            }

            int* edgeIndex = stackalloc int[numEdges * 2];
            int* curEdge = stackalloc int[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                curEdge[i] = head[i];
            }

            int* tempTo = to;
            int* tempNext = next;

            for (int start = 0; start < numNodes; start++)
            {
                if (curEdge[start] == -1)
                {
                    continue;
                }

                int* stack = stackalloc int[numEdges + 1];
                int* path = stackalloc int[numEdges + 1];
                int stackSize = 0;
                int pathSize = 0;

                stack[stackSize++] = start;

                while (stackSize > 0)
                {
                    int u = stack[stackSize - 1];
                    int e = curEdge[u];
                    while (e != -1 && visitedEdge[e / 2] == 1)
                    {
                        e = tempNext[e];
                    }
                    curEdge[u] = e;

                    if (e != -1)
                    {
                        int v = tempTo[e];
                        visitedEdge[e / 2] = 1;
                        orientedU[e / 2] = u;
                        orientedV[e / 2] = v;
                        stack[stackSize++] = v;
                        curEdge[u] = tempNext[e];
                    }
                    else
                    {
                        path[pathSize++] = u;
                        stackSize--;
                    }
                }
            }

            return true;
        }

        public static bool StrongOrientation(int numNodes, int numEdges, int* head, int* next, int* to, int* edgeU, int* edgeV, int* orientedU, int* orientedV)
        {
            byte* visited = stackalloc byte[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                visited[i] = 0;
            }

            int* parent = stackalloc int[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                parent[i] = -1;
            }

            byte* usedEdge = stackalloc byte[numEdges];
            for (int i = 0; i < numEdges; i++)
            {
                usedEdge[i] = 0;
            }

            StrongDfs(0, head, next, to, visited, parent, orientedU, orientedV, usedEdge);

            int* oHead = stackalloc int[numNodes];
            int* oNext = stackalloc int[numEdges];
            int* oTo = stackalloc int[numEdges];

            int* rHead = stackalloc int[numNodes];
            int* rNext = stackalloc int[numEdges];
            int* rTo = stackalloc int[numEdges];

            for (int i = 0; i < numNodes; i++)
            {
                oHead[i] = -1;
                rHead[i] = -1;
            }

            for (int i = 0; i < numEdges; i++)
            {
                int u = orientedU[i];
                int v = orientedV[i];
                oTo[i] = v; oNext[i] = oHead[u]; oHead[u] = i;
                rTo[i] = u; rNext[i] = rHead[v]; rHead[v] = i;
            }

            byte* visitedO = stackalloc byte[numNodes];
            byte* visitedR = stackalloc byte[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                visitedO[i] = visitedR[i] = 0;
            }

            int reachO = 0;
            int reachR = 0;

            DfsReach(0, oHead, oNext, oTo, visitedO, &reachO);
            DfsReach(0, rHead, rNext, rTo, visitedR, &reachR);

            return reachO == numNodes && reachR == numNodes;
        }

        private static void StrongDfs(int u, int* head, int* next, int* to, byte* visited, int* parent, int* orientedU, int* orientedV, byte* usedEdge)
        {
            visited[u] = 1;
            for (int e = head[u]; e != -1; e = next[e])
            {
                int edgeIdx = e / 2;
                if (usedEdge[edgeIdx] == 1)
                {
                    continue;
                }
                int v = to[e];
                if (visited[v] == 0)
                {
                    parent[v] = u;
                    orientedU[edgeIdx] = u;
                    orientedV[edgeIdx] = v;
                    usedEdge[edgeIdx] = 1;
                    StrongDfs(v, head, next, to, visited, parent, orientedU, orientedV, usedEdge);
                }
                else if (v != parent[u])
                {
                    orientedU[edgeIdx] = u;
                    orientedV[edgeIdx] = v;
                    usedEdge[edgeIdx] = 1;
                }
            }
        }

        private static void DfsReach(int u, int* head, int* next, int* to, byte* visited, int* reachCount)
        {
            visited[u] = 1;
            (*reachCount)++;
            for (int e = head[u]; e != -1; e = next[e])
            {
                int v = to[e];
                if (visited[v] == 0)
                {
                    DfsReach(v, head, next, to, visited, reachCount);
                }
            }
        }

        public static bool MinimumStrongOrientation(int numNodes, int numEdges, int* head, int* next, int* to, int* edgeU, int* edgeV, int* orientedU, int* orientedV)
        {
            return StrongOrientation(numNodes, numEdges, head, next, to, edgeU, edgeV, orientedU, orientedV);
        }

        public static void OrientEdgesStrongly(int numNodes, int numEdges, int* head, int* next, int* to, int* edgeU, int* edgeV, int* orientedU, int* orientedV)
        {
            StrongOrientation(numNodes, numEdges, head, next, to, edgeU, edgeV, orientedU, orientedV);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OrientEdgesAcyclic(int numEdges, int* edgeU, int* edgeV, int* orientedU, int* orientedV)
        {
            for (int i = 0; i < numEdges; i++)
            {
                int u = edgeU[i];
                int v = edgeV[i];
                if (u < v)
                {
                    orientedU[i] = u;
                    orientedV[i] = v;
                }
                else
                {
                    orientedU[i] = v;
                    orientedV[i] = u;
                }
            }
        }

        public static int FeedbackArcTournament(int n, byte* adj, int* reversedU, int* reversedV, int* reversedCount)
        {
            int* order = stackalloc int[n];
            int* bestOrder = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                order[i] = i;
                bestOrder[i] = i;
            }

            int minReversals = 999999;
            byte* used = stackalloc byte[n];
            for (int i = 0; i < n; i++)
            {
                used[i] = 0;
            }

            FeedbackArcBacktrack(0, n, adj, order, used, bestOrder, &minReversals);

            *reversedCount = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    int u = bestOrder[i];
                    int v = bestOrder[j];
                    if (adj[v * n + u] == 1)
                    {
                        reversedU[*reversedCount] = v;
                        reversedV[*reversedCount] = u;
                        (*reversedCount)++;
                    }
                }
            }

            return minReversals;
        }

        private static void FeedbackArcBacktrack(int step, int n, byte* adj, int* order, byte* used, int* bestOrder, int* minReversals)
        {
            if (step == n)
            {
                int reversals = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        int u = order[i];
                        int v = order[j];
                        if (adj[v * n + u] == 1)
                        {
                            reversals++;
                        }
                    }
                }
                if (reversals < *minReversals)
                {
                    *minReversals = reversals;
                    for (int i = 0; i < n; i++)
                    {
                        bestOrder[i] = order[i];
                    }
                }
                return;
            }

            for (int i = 0; i < n; i++)
            {
                if (used[i] == 0)
                {
                    used[i] = 1;
                    order[step] = i;
                    FeedbackArcBacktrack(step + 1, n, adj, order, used, bestOrder, minReversals);
                    used[i] = 0;
                }
            }
        }
    }
}
