namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PushRelabelGlobalRelabel
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            int* height = stackalloc int[n];
            int* excess = stackalloc int[n];
            for (int i = 0; i < n; i++) { height[i] = 0; excess[i] = 0; flow[i] = 0; }
            height[s] = n;
            excess[s] = int.MaxValue;
            for (int e = head[s]; e != 0; e = next[e])
            {
                flow[e] = cap[e]; flow[e ^ 1] = -cap[e];
                excess[to[e]] += cap[e]; excess[s] -= cap[e];
            }
            int* ptr = stackalloc int[n];
            for (int i = 0; i < n; i++) ptr[i] = head[i];
            // The discharge phase processes active vertices as a FIFO and may re-enqueue a vertex
            // after it regains excess. inQueue[] keeps at most one live copy of each vertex so the
            // number of queued-but-unprocessed vertices never exceeds n; the ring therefore needs
            // n + 1 slots (one spare to distinguish full from empty). The two relabel BFS passes
            // enqueue each vertex at most once and so also fit comfortably in this buffer.
            int queueCapacity = n + 1;
            int* q = stackalloc int[queueCapacity];
            int* inQueue = stackalloc int[n];
            int* dist = stackalloc int[n];
            // Sentinel height for nodes that, in the residual graph, can reach neither t nor s.
            // It is strictly above every real height (reachable heights are at most 2n-1), so it
            // never satisfies the admissibility test height[u] == height[v] + 1 for any reachable u.
            int unreachedHeight = 2 * n + 1;
            while (true)
            {
                for (int i = 0; i < n; i++) dist[i] = -1;
                int qh = 0, qt = 0;
                dist[t] = 0; q[qt++] = t;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int w = to[e];
                        // Reverse BFS over the residual graph: w can reach u (hence t) when the
                        // residual edge w->u (the reverse arc e^1) still has spare capacity.
                        if (cap[e ^ 1] - flow[e ^ 1] > 0 && dist[w] == -1)
                        {
                            dist[w] = dist[u] + 1; q[qt++] = w;
                        }
                    }
                }
                // Nodes reachable to t in the residual graph get their distance-to-t as height.
                // Nodes that cannot reach t (and the source) must drain their excess back toward s;
                // assign them heights n + (residual distance to s) via a second BFS seeded from s so
                // an admissible (downhill) residual path back to s always exists. Without this, such
                // stranded excess could never be pushed and the outer loop would spin forever.
                for (int i = 0; i < n; i++) height[i] = dist[i] >= 0 ? dist[i] : unreachedHeight;
                for (int i = 0; i < n; i++) dist[i] = -1;
                qh = 0; qt = 0;
                dist[s] = 0; q[qt++] = s;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int w = to[e];
                        // Reverse BFS over the residual graph from s: w can reach u (hence s) when the
                        // residual edge w->u (the reverse arc e^1) still has spare capacity.
                        if (cap[e ^ 1] - flow[e ^ 1] > 0 && dist[w] == -1)
                        {
                            dist[w] = dist[u] + 1; q[qt++] = w;
                        }
                    }
                }
                for (int i = 0; i < n; i++) if (dist[i] >= 0 && height[i] == unreachedHeight) height[i] = n + dist[i];
                // The source is never discharged; pin it at the canonical height n so that excess on
                // its residual neighbours (height n+1 from the s-BFS) has an admissible edge back into s.
                height[s] = n;
                for (int i = 0; i < n; i++) inQueue[i] = 0;
                int qcount = 0;
                qh = 0; qt = 0;
                for (int i = 0; i < n; i++)
                    if (excess[i] > 0 && i != s && i != t)
                    {
                        q[qt] = i; inQueue[i] = 1; qcount++;
                        qt++; if (qt == queueCapacity) qt = 0;
                    }
                while (qcount > 0)
                {
                    int u = q[qh];
                    qh++; if (qh == queueCapacity) qh = 0;
                    inQueue[u] = 0; qcount--;
                    int hu = height[u];
                    // hu and eu are invariant memory loads across this vertex's edge walk: height[u]
                    // is only changed by the relabel passes, and excess[u] is only changed by pushes
                    // out of u below (u is never a push target of its own discharge), so we hold both
                    // in registers and write excess[u] back once the walk ends.
                    int eu = excess[u];
                    while (ptr[u] != 0)
                    {
                        int e = ptr[u];
                        int v = to[e];
                        int rc = cap[e] - flow[e];
                        if (rc > 0 && hu == height[v] + 1)
                        {
                            int push = Math.Min(eu, rc);
                            flow[e] += push; flow[e ^ 1] -= push;
                            eu -= push; excess[v] += push;
                            if (excess[v] > 0 && v != s && v != t && inQueue[v] == 0)
                            {
                                q[qt] = v; inQueue[v] = 1; qcount++;
                                qt++; if (qt == queueCapacity) qt = 0;
                            }
                            if (eu == 0) break;
                        }
                        ptr[u] = next[e];
                    }
                    excess[u] = eu;
                    if (ptr[u] == 0) ptr[u] = head[u];
                }
                // An "active" vertex is one carrying excess other than the source or the sink. The
                // sink legitimately retains excess equal to the flow value, so it must be excluded
                // here exactly as it is when the active queue is built; otherwise the loop never ends.
                bool any = false;
                for (int i = 0; i < n; i++) if (excess[i] > 0 && i != s && i != t) { any = true; break; }
                if (!any) break;
            }
            long result = 0;
            for (int e = head[s]; e != 0; e = next[e]) result += flow[e];
            return result;
        }
    }
}