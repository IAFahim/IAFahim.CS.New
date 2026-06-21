using System.Runtime.CompilerServices;

namespace IAFahim.Graph.Flow
{
    public static unsafe class PushRelabelGap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Enqueue(int v, int queueCapacity, int* inQueue, int* q, ref int qt, ref int qcount)
        {
            inQueue[v] = 1;
            q[qt] = v; qt++; if (qt == queueCapacity) qt = 0;
            qcount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PushEdge(int e, int s, int t, int* to, int* cap, int* flow, int hu, int* height, ref int eu, int* excess, int* inQueue, int* q, int queueCapacity, ref int qt, ref int qcount)
        {
            int rc = cap[e] - flow[e];
            if (rc <= 0) return;
            int v = to[e];
            if (hu != height[v] + 1) return;
            int push = rc < eu ? rc : eu;
            flow[e] += push; flow[e ^ 1] -= push;
            eu -= push; excess[v] += push;
            if (v != s && v != t && inQueue[v] == 0)
            {
                Enqueue(v, queueCapacity, inQueue, q, ref qt, ref qcount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MinResidualHeight(int u, int* head, int* next, int* to, int* cap, int* flow, int* height)
        {
            int minH = int.MaxValue;
            for (int e = head[u]; e != 0; e = next[e])
            {
                if (cap[e] - flow[e] <= 0) continue;
                int hv = height[to[e]];
                if (hv < minH) minH = hv;
            }
            return minH;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyGap(int n, int s, int u, int hu, int* height, int* count)
        {
            for (int i = 0; i < n; i++)
            {
                int hi = height[i];
                if (hi > hu && hi <= n && i != s)
                {
                    count[hi] -= 1;
                    height[i] = n + 1;
                    count[n + 1] += 1;
                }
            }
            // u was at hu (already decremented out of its level) and sits below the gap, so
            // it is not touched by the sweep above; give it the parked height directly.
            height[u] = n + 1;
            count[n + 1] += 1;
        }

        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            if (s == t) return 0;

            // Heights of reachable nodes stay below 2n; the gap heuristic lifts stranded nodes to
            // n + 1, which is also below 2n for n >= 1. Size the height-frequency table accordingly
            // (one extra slot so index 2n is always valid).
            int heightLevels = 2 * n + 1;
            int* height = stackalloc int[n];
            int* excess = stackalloc int[n];
            int* count = stackalloc int[heightLevels];
            for (int i = 0; i < n; i++) { height[i] = 0; excess[i] = 0; flow[i] = 0; }
            for (int h = 0; h < heightLevels; h++) count[h] = 0;

            // The discharge phase processes active vertices as a FIFO and may re-enqueue a vertex
            // after it regains excess. inQueue[] keeps at most one live copy of each vertex so the
            // number of queued-but-unprocessed vertices never exceeds n; the ring therefore needs
            // n + 1 slots (one spare to distinguish full from empty).
            int queueCapacity = n + 1;
            int* q = stackalloc int[queueCapacity];
            int* inQueue = stackalloc int[n];
            for (int i = 0; i < n; i++) inQueue[i] = 0;

            // Initial heights: source pinned at n, everyone else at 0. Record the frequency of each
            // height so the gap heuristic can detect an empty level.
            height[s] = n;
            count[0] = n - 1;
            count[n] += 1;
            excess[s] = int.MaxValue;

            int qh = 0, qt = 0;
            // Saturate every edge out of the source and seed the active queue with the neighbours
            // that thereby gain excess.
            for (int e = head[s]; e != 0; e = next[e])
            {
                int c = cap[e];
                if (c <= 0) continue;
                flow[e] = c; flow[e ^ 1] = -c;
                int v = to[e];
                excess[v] += c; excess[s] -= c;
                if (v != t && inQueue[v] == 0)
                {
                    inQueue[v] = 1;
                    q[qt] = v; qt++; if (qt == queueCapacity) qt = 0;
                }
            }

            int qcount = qt >= qh ? qt - qh : queueCapacity - qh + qt;
            while (qcount > 0)
            {
                int u = q[qh];
                qh++; if (qh == queueCapacity) qh = 0;
                inQueue[u] = 0; qcount--;

                // hu is held in a register across u's edge walk; height[u] only changes on relabel
                // below, after which the walk has already ended.
                int hu = height[u];
                int eu = excess[u];

                // Push along every admissible residual edge until u runs out of excess.
                for (int e = head[u]; e != 0 && eu > 0; e = next[e])
                {
                    PushEdge(e, s, t, to, cap, flow, hu, height, ref eu, excess, inQueue, q, queueCapacity, ref qt, ref qcount);
                }

                if (eu == 0)
                {
                    excess[u] = 0;
                    continue;
                }

                // Relabel: lift u to one above the lowest residual neighbour.
                int minH = MinResidualHeight(u, head, next, to, cap, flow, height);

                excess[u] = eu;
                int newH = minH == int.MaxValue ? heightLevels - 1 : minH + 1;

                // Gap heuristic: if removing u from level hu empties that level (and the level is
                // below the source height n), every node strictly above hu can no longer reach the
                // sink, so park it at n + 1 where it can only drain back toward s. This both prunes
                // useless relabels and preserves correctness because such nodes have no admissible
                // edge toward t anyway.
                count[hu] -= 1;
                if (count[hu] == 0 && hu < n)
                {
                    ApplyGap(n, s, u, hu, height, count);
                }
                else
                {
                    if (newH >= heightLevels) newH = heightLevels - 1;
                    height[u] = newH;
                    count[newH] += 1;
                }

                // u still carries excess, so it stays active.
                if (inQueue[u] == 0)
                {
                    Enqueue(u, queueCapacity, inQueue, q, ref qt, ref qcount);
                }
            }

            long result = 0;
            for (int e = head[s]; e != 0; e = next[e]) result += flow[e];
            return result;
        }
    }
}
