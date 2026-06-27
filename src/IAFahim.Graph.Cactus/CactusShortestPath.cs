namespace IAFahim.Graph.Cactus
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class CactusShortestPath
    {
        // Weighted shortest path in a cactus. A cactus is a graph in which every edge belongs to at
        // most one simple cycle; Dijkstra is correct on ANY graph, hence on a cactus, and runs in
        // O((n + m) log n) via a binary heap. (The cactus structure admits a linear-time block-cut
        // solution, but Dijkstra is correct, simple, and fast enough for navmesh-scale inputs.)
        //
        // Adjacency follows the module convention (CSR-style linked lists): edge e goes head[u] ->
        // next[e] -> ... with to[e] the destination; weights are parallel in weight[e]. Undirected:
        // each edge appears twice (e and e^1). Returns the distance, or -1 if v is unreachable from u.
        //
        // Caller guarantees: head/to/next/weight form a valid undirected adjacency; 0 <= u,v < n.
        public static long Run(int* head, int* to, int* next, int* weight, int n, int m, int u, int v)
        {
            if (n <= 0) return -1;
            if ((uint)u >= (uint)n || (uint)v >= (uint)n) return -1;
            if (u == v) return 0;

            long* dist = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            int* heap = (int*)Marshal.AllocHGlobal(sizeof(int) * (n + 1));
            long* heapKey = (long*)Marshal.AllocHGlobal(sizeof(long) * (n + 1));
            long inf = long.MaxValue;
            for (int i = 0; i < n; i++) dist[i] = inf;
            dist[u] = 0;
            int hs = 0;
            Push(heap, heapKey, ref hs, 0, u);
            long result = -1;
            while (hs > 0)
            {
                long du = heapKey[1];
                int x = heap[1];
                Pop(heap, heapKey, ref hs);
                if (du > dist[x]) continue;
                if (x == v) { result = dist[v]; break; }
                for (int e = head[x]; e != -1; e = next[e])
                {
                    int y = to[e];
                    long nd = du + weight[e];
                    if (nd < dist[y])
                    {
                        dist[y] = nd;
                        Push(heap, heapKey, ref hs, nd, y);
                    }
                }
            }
            if (result == -1) result = dist[v] == inf ? -1 : dist[v];
            Marshal.FreeHGlobal((IntPtr)dist);
            Marshal.FreeHGlobal((IntPtr)heap);
            Marshal.FreeHGlobal((IntPtr)heapKey);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Push(int* heap, long* heapKey, ref int hs, long key, int val)
        {
            hs++;
            int i = hs;
            heap[i] = val;
            heapKey[i] = key;
            while (i > 1)
            {
                int p = i >> 1;
                if (heapKey[p] <= heapKey[i]) break;
                Swap(heap, heapKey, p, i);
                i = p;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Pop(int* heap, long* heapKey, ref int hs)
        {
            heap[1] = heap[hs];
            heapKey[1] = heapKey[hs];
            hs--;
            int i = 1;
            while (true)
            {
                int l = i << 1, r = l | 1, smallest = i;
                if (l <= hs && heapKey[l] < heapKey[smallest]) smallest = l;
                if (r <= hs && heapKey[r] < heapKey[smallest]) smallest = r;
                if (smallest == i) break;
                Swap(heap, heapKey, smallest, i);
                i = smallest;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(int* heap, long* heapKey, int a, int b)
        {
            int tv = heap[a]; heap[a] = heap[b]; heap[b] = tv;
            long tk = heapKey[a]; heapKey[a] = heapKey[b]; heapKey[b] = tk;
        }
    }
}
