namespace IAFahim.Graph.Flow
{
    public static unsafe class DinicWithLinkCut
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            long result = 0;
            int* level = stackalloc int[n];
            int* it = stackalloc int[n];
            int* q = stackalloc int[n];
            while (BfsLayer(n, s, t, head, to, next, cap, flow, level, q, it))
            {
                while (true)
                {
                    int pushed = Dfs(s, t, int.MaxValue, to, next, cap, flow, level, it);
                    if (pushed == 0) break;
                    result += pushed;
                }
            }
            return result;
        }

        private static bool BfsLayer(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* level, int* q, int* it)
        {
            for (int i = 0; i < n; i++) { level[i] = -1; it[i] = head[i]; }
            int qh = 0, qt = 0;
            level[s] = 0;
            q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                int lu1 = level[u] + 1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int w = to[e];
                    if (cap[e] - flow[e] > 0 && level[w] == -1)
                    {
                        level[w] = lu1;
                        q[qt++] = w;
                    }
                }
            }
            return level[t] != -1;
        }

        private static int Dfs(int u, int t, int pushed, int* to, int* next, int* cap, int* flow, int* level, int* it)
        {
            if (u == t) return pushed;
            int lu1 = level[u] + 1;
            for (int e = it[u]; e != 0; e = next[e])
            {
                it[u] = e;
                int v = to[e];
                int residual = cap[e] - flow[e];
                if (level[v] != lu1 || residual <= 0) continue;
                int tr = Dfs(v, t, pushed < residual ? pushed : residual, to, next, cap, flow, level, it);
                if (tr == 0) continue;
                flow[e] += tr;
                flow[e ^ 1] -= tr;
                return tr;
            }
            return 0;
        }
    }
}
