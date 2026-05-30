namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class IsapGapOptimization
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            int* level = stackalloc int[n];
            int* it = stackalloc int[n];
            int* cnt = stackalloc int[n];
            for (int i = 0; i < n; i++) { level[i] = 0; it[i] = head[i]; cnt[i] = 0; flow[i] = 0; }
            long result = 0;
            BfsLevel2(n, s, t, head, to, next, cap, flow, level, cnt);
            while (level[s] < n)
            {
                int pushed = SendFlow2(n, s, t, int.MaxValue, head, to, next, cap, flow, level, it, cnt, s);
                result += pushed;
                if (pushed == 0) break;
            }
            return result;
        }

        private static void BfsLevel2(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* level, int* cnt)
        {
            for (int i = 0; i < n; i++) { level[i] = n; cnt[i] = 0; }
            level[t] = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            q[qt++] = t;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (cap[e ^ 1] - flow[e ^ 1] > 0 && level[v] > level[u] + 1)
                    {
                        level[v] = level[u] + 1; q[qt++] = v;
                    }
                }
            }
            for (int i = 0; i < n; i++) cnt[level[i]]++;
        }

        private static int SendFlow2(int n, int u, int t, int f, int* head, int* to, int* next, int* cap, int* flow, int* level, int* it, int* cnt, int src)
        {
            if (u == t) return f;
            for (int e = it[u]; e != 0; e = next[e])
            {
                it[u] = e;
                int v = to[e];
                if (cap[e] - flow[e] > 0 && level[v] == level[u] + 1)
                {
                    int ret = SendFlow2(n, v, t, Math.Min(f, cap[e] - flow[e]), head, to, next, cap, flow, level, it, cnt, src);
                    if (ret > 0) { flow[e] += ret; flow[e ^ 1] -= ret; return ret; }
                }
            }
            cnt[level[u]]--;
            if (cnt[level[u]] == 0) level[src] = n;
            level[u]++;
            cnt[level[u]]++;
            it[u] = head[u];
            return 0;
        }
    }
}