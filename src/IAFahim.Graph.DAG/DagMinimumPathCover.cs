namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagMinimumPathCover
    {
        private const int Unmatched = -1;
        private const int NullEdge = 0;
        private const int InfiniteDist = int.MaxValue;

        /// <summary>
        /// Minimum path cover of a DAG via Dilworth / König:
        /// build the bipartite graph (left node u -> right node v for each DAG edge u->v)
        /// and return n - maximumBipartiteMatching, computed with Hopcroft-Karp.
        /// Adjacency is the 1-indexed linked list head/next/to (edge index 0 = null).
        /// match must have length 2*n: match[0..n) holds left-side mates,
        /// match[n..2n) holds right-side mates. dist and queue must have length n.
        /// Unchecked: caller guarantees non-null pointers, valid lengths and a valid DAG.
        /// </summary>
        public static int Run(int* head, int* next, int* to, int* match, int* dist, int* queue, int n)
        {
            int* matchRight = match + n;
            for (int i = 0; i < n; i++)
            {
                match[i] = Unmatched;
                matchRight[i] = Unmatched;
            }

            int matching = 0;
            while (BuildLayers(head, next, to, match, matchRight, dist, queue, n))
            {
                for (int u = 0; u < n; u++)
                {
                    if (match[u] == Unmatched && TryAugment(head, next, to, match, matchRight, dist, u))
                        matching++;
                }
            }

            return n - matching;
        }

        // BFS over free left vertices to assign layer distances; returns true if any
        // augmenting path to a free right vertex exists in this phase.
        private static bool BuildLayers(int* head, int* next, int* to, int* matchLeft, int* matchRight, int* dist, int* queue, int n)
        {
            int qHead = 0;
            int qTail = 0;
            for (int u = 0; u < n; u++)
            {
                if (matchLeft[u] == Unmatched)
                {
                    dist[u] = 0;
                    queue[qTail++] = u;
                }
                else
                {
                    dist[u] = InfiniteDist;
                }
            }

            bool found = false;
            while (qHead < qTail)
            {
                int u = queue[qHead++];
                int nextDist = dist[u] + 1;
                for (int e = head[u]; e != NullEdge; e = next[e])
                {
                    int v = to[e];
                    int w = matchRight[v];
                    if (w == Unmatched)
                    {
                        found = true;
                    }
                    else if (dist[w] == InfiniteDist)
                    {
                        dist[w] = nextDist;
                        queue[qTail++] = w;
                    }
                }
            }

            return found;
        }

        // DFS to find/flip an augmenting path from left vertex u along the layered graph.
        private static bool TryAugment(int* head, int* next, int* to, int* matchLeft, int* matchRight, int* dist, int u)
        {
            int nextDist = dist[u] + 1;
            for (int e = head[u]; e != NullEdge; e = next[e])
            {
                int v = to[e];
                int w = matchRight[v];
                if (w == Unmatched || (dist[w] == nextDist && TryAugment(head, next, to, matchLeft, matchRight, dist, w)))
                {
                    matchRight[v] = u;
                    matchLeft[u] = v;
                    return true;
                }
            }

            dist[u] = InfiniteDist;
            return false;
        }
    }
}
