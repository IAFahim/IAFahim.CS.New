namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class IncrementalCycleDetection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddEdge(int u, int v, int* head, int* next, int* to, int* edgeCount, int* visited, int n)
        {
            to[*edgeCount] = v;
            next[*edgeCount] = head[u];
            head[u] = *edgeCount;
            (*edgeCount)++;
            
            // Simple DFS to check cycle
            for(int i=0; i<n; i++) visited[i] = 0;
            return !Dfs(v, u, head, next, to, visited);
        }
        
        private static bool Dfs(int curr, int target, int* head, int* next, int* to, int* visited)
        {
            if (curr == target) return true;
            visited[curr] = 1;
            for (int e = head[curr]; e != -1; e = next[e])
            {
                int v = to[e];
                if (visited[v] == 0)
                {
                    if (Dfs(v, target, head, next, to, visited)) return true;
                }
            }
            return false;
        }
    }
}