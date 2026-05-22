namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class IncrementalCycleDetection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddEdge(int u, int v, int* head, int* next, int* to, int* edgeCount, int* visited, int runId)
        {
            to[*edgeCount] = v;
            next[*edgeCount] = head[u];
            head[u] = *edgeCount;
            (*edgeCount)++;
            
            return !Dfs(v, u, head, next, to, visited, runId);
        }
        
        private static bool Dfs(int curr, int target, int* head, int* next, int* to, int* visited, int runId)
        {
            if (curr == target) return true;
            visited[curr] = runId;
            for (int e = head[curr]; e != -1; e = next[e])
            {
                int v = to[e];
                if (visited[v] != runId)
                {
                    if (Dfs(v, target, head, next, to, visited, runId)) return true;
                }
            }
            return false;
        }
    }
}