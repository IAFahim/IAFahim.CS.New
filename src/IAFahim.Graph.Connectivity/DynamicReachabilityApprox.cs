namespace IAFahim.Graph.Connectivity
{
    using System.Runtime.CompilerServices;

    public static unsafe class DynamicReachabilityApprox
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* head, int* next, int* to, int n, bool* visited, int u, int v, int maxDepth)
        {
            if (u == v) return true;
            if (maxDepth <= 0) return false;
            for (int i = 0; i < n; i++) visited[i] = false;
            visited[u] = true;
            return Dfs(head, next, to, visited, u, v, maxDepth);
        }

        private static bool Dfs(int* head, int* next, int* to, bool* visited, int u, int v, int maxDepth)
        {
            if (maxDepth <= 0) return false;
            for (int e = head[u]; e != -1; e = next[e])
            {
                int w = to[e];
                if (w == v) return true;
                if (!visited[w])
                {
                    visited[w] = true;
                    if (Dfs(head, next, to, visited, w, v, maxDepth - 1)) return true;
                }
            }
            return false;
        }
    }
}
