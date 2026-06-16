namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    // Incremental cycle detection on a DAG built from a forward-star adjacency list.
    // Convention (matching OnlineScc): edge slots start at index 0, head[] sentinel is -1,
    // edgeCount starts at 0. Callers must initialize head[i] = -1 for all i and visited[i] = 0.
    public static unsafe class IncrementalCycleDetection
    {
        private const int NullEdge = -1;

        // Adds edge u -> v and returns true if the graph remains acyclic, false if the edge
        // would create a cycle (i.e. v can already reach u). 'stack' is a caller-provided scratch
        // buffer of at least n ints used for the iterative reachability search.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddEdge(int u, int v, int* head, int* next, int* to, int* edgeCount, int* visited, int* stack, int runId)
        {
            int ec = *edgeCount;
            to[ec] = v;
            next[ec] = head[u];
            head[u] = ec;
            *edgeCount = ec + 1;

            return !Reaches(v, u, head, next, to, visited, stack, runId);
        }

        // Returns true iff 'start' can reach 'target' following forward edges.
        private static bool Reaches(int start, int target, int* head, int* next, int* to, int* visited, int* stack, int runId)
        {
            int sp = 0;
            stack[sp++] = start;
            visited[start] = runId;

            while (sp != 0)
            {
                int curr = stack[--sp];
                if (curr == target) return true;

                for (int e = head[curr]; e != NullEdge; e = next[e])
                {
                    int w = to[e];
                    if (visited[w] != runId)
                    {
                        visited[w] = runId;
                        stack[sp++] = w;
                    }
                }
            }

            return false;
        }
    }
}
