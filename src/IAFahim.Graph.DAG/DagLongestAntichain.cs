namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagLongestAntichain
    {
        private const int Unmatched = -1;

        // By Dilworth's theorem, the size of the longest antichain equals the minimum
        // path cover of the transitive closure, which equals n - (maximum bipartite
        // matching) on the bipartite graph where a left vertex u is joined to a right
        // vertex v whenever u can reach v (u != v).
        //
        // Unchecked: the caller guarantees reachabilityMatrix is a valid n*n row-major
        // matrix (reachabilityMatrix[u * n + v] == true iff u reaches v), and that
        // matchRight and visited each have room for at least n ints/bools.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(bool* reachabilityMatrix, int* matchRight, bool* visited, int n)
        {
            for (int v = 0; v < n; v++) matchRight[v] = Unmatched;

            int matching = 0;
            for (int u = 0; u < n; u++)
            {
                for (int v = 0; v < n; v++) visited[v] = false;
                if (TryAugment(reachabilityMatrix, matchRight, visited, u, n)) matching++;
            }

            return n - matching;
        }

        private static bool TryAugment(bool* reachabilityMatrix, int* matchRight, bool* visited, int u, int n)
        {
            int rowOffset = u * n;
            for (int v = 0; v < n; v++)
            {
                if (v == u) continue;
                if (!reachabilityMatrix[rowOffset + v]) continue;
                if (visited[v]) continue;

                visited[v] = true;
                int w = matchRight[v];
                if (w == Unmatched || TryAugment(reachabilityMatrix, matchRight, visited, w, n))
                {
                    matchRight[v] = u;
                    return true;
                }
            }

            return false;
        }
    }
}
