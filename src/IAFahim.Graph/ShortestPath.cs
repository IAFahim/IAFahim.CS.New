namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    internal static unsafe class SpArrays
    {
        public const long Infinity = long.MaxValue;

        public const int NoParent = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitDist(long* dist, int n)
        {
            for (int i = 0; i < n; i++) dist[i] = Infinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitParent(int* parent, int n)
        {
            for (int i = 0; i < n; i++) parent[i] = NoParent;
        }
    }

    internal unsafe struct SpMinHeap
    {
        private const int HeapRoot = 0;

        private const int NotInHeap = -1;

        public long* Dist;

        public int* V;

        public int* Pos;

        public int Size;

        public SpMinHeap(int n)
        {
            Dist = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal((nint)((long)n * sizeof(long)));
            V = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));
            Pos = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));
            for (int i = 0; i < n; i++) Pos[i] = NotInHeap;
            Size = 0;
        }

        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)Dist);
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)V);
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)Pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SwapEntries(int a, int b)
        {
            long tmpD = Dist[a]; Dist[a] = Dist[b]; Dist[b] = tmpD;
            int tmpV = V[a]; V[a] = V[b]; V[b] = tmpV;
            Pos[V[a]] = a;
            Pos[V[b]] = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SiftUp(int idx)
        {
            while (idx > HeapRoot)
            {
                int p = (idx - 1) / 2;
                if (Dist[p] <= Dist[idx]) break;
                SwapEntries(p, idx);
                idx = p;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SiftDown()
        {
            int idx = HeapRoot;
            while (idx * 2 + 1 < Size)
            {
                int left = idx * 2 + 1;
                int right = idx * 2 + 2;
                int smallest = left;
                if (right < Size && Dist[right] < Dist[left]) smallest = right;
                if (Dist[idx] <= Dist[smallest]) break;
                SwapEntries(idx, smallest);
                idx = smallest;
            }
        }

        public void PushOrUpdate(int v, long d)
        {
            int idx = Pos[v];
            if (idx == NotInHeap)
            {
                idx = Size++;
                V[idx] = v;
                Pos[v] = idx;
            }
            Dist[idx] = d;
            SiftUp(idx);
        }

        public int Pop(out long d)
        {
            int u = V[HeapRoot];
            d = Dist[HeapRoot];
            Pos[u] = NotInHeap;
            Size--;
            if (Size > 0)
            {
                Dist[HeapRoot] = Dist[Size];
                V[HeapRoot] = V[Size];
                Pos[V[HeapRoot]] = HeapRoot;
                SiftDown();
            }
            return u;
        }
    }

    public static unsafe class Dijkstra
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            SpArrays.InitDist(dist, n);
            SpArrays.InitParent(parent, n);
            dist[start] = 0;
            SpMinHeap pq = new SpMinHeap(n);
            pq.PushOrUpdate(start, 0);
            while (pq.Size > 0)
            {
                int u = pq.Pop(out long d);
                if (d != dist[u]) continue;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long nd = dist[u] + weight[e];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        parent[v] = u;
                        pq.PushOrUpdate(v, nd);
                    }
                }
            }
            pq.Dispose();
        }
    }

    public static unsafe class DijkstraSparse
    {
        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            Dijkstra.Run(n, start, head, to, next, weight, dist, parent);
        }
    }

    public static unsafe class DijkstraDense
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ScanMinVertex(long* dist, bool* used, int n, out long best)
        {
            int v = SpArrays.NoParent;
            best = SpArrays.Infinity;
            for (int i = 0; i < n; i++)
            {
                if (!used[i] && dist[i] < best)
                {
                    best = dist[i];
                    v = i;
                }
            }
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RelaxOutEdges(int* head, int* to, int* next, int* weight, long* dist, int* parent, int src, long srcDist)
        {
            for (int e = head[src]; e != 0; e = next[e])
            {
                int neighbor = to[e];
                long nd = srcDist + weight[e];
                if (nd < dist[neighbor])
                {
                    dist[neighbor] = nd;
                    parent[neighbor] = src;
                }
            }
        }

        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent)
        {
            SpArrays.InitDist(dist, n);
            SpArrays.InitParent(parent, n);
            bool* used = stackalloc bool[n];
            for (int i = 0; i < n; i++) used[i] = false;
            dist[start] = 0;
            for (int iter = 0; iter < n; iter++)
            {
                int v = ScanMinVertex(dist, used, n, out long best);
                if (v == SpArrays.NoParent || best == SpArrays.Infinity) break;
                used[v] = true;
                RelaxOutEdges(head, to, next, weight, dist, parent, v, dist[v]);
            }
        }
    }

    public static unsafe class DijkstraRestorePath
    {
        public static int Run(int* parent, int target, int* path)
        {
            int len = 0;
            int cur = target;
            while (cur != SpArrays.NoParent)
            {
                path[len++] = cur;
                cur = parent[cur];
            }
            int half = len / 2;
            for (int i = 0; i < half; i++)
            {
                int tmp = path[i];
                path[i] = path[len - 1 - i];
                path[len - 1 - i] = tmp;
            }
            return len;
        }
    }

    public static unsafe class BellmanFord
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool RelaxEdges(int* eu, int* ev, int* ew, int m, long* dist, int* parent)
        {
            bool changed = false;
            for (int i = 0; i < m; i++)
            {
                if (dist[eu[i]] == SpArrays.Infinity) continue;
                long nd = dist[eu[i]] + ew[i];
                if (nd < dist[ev[i]])
                {
                    dist[ev[i]] = nd;
                    parent[ev[i]] = eu[i];
                    changed = true;
                }
            }
            return changed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasNegativeCycle(int* eu, int* ev, int* ew, int m, long* dist)
        {
            for (int i = 0; i < m; i++)
            {
                if (dist[eu[i]] != SpArrays.Infinity && dist[eu[i]] + ew[i] < dist[ev[i]])
                    return true;
            }
            return false;
        }

        public static bool Run(int n, int start, int m, int* eu, int* ev, int* ew, long* dist, int* parent)
        {
            SpArrays.InitDist(dist, n);
            SpArrays.InitParent(parent, n);
            dist[start] = 0;
            for (int iter = 0; iter < n - 1; iter++)
            {
                if (!RelaxEdges(eu, ev, ew, m, dist, parent)) break;
            }
            return !HasNegativeCycle(eu, ev, ew, m, dist);
        }
    }

    public static unsafe class Spfa
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Enqueue(int* q, int cap, ref int tail, ref int cnt, int* inqueue, int v)
        {
            q[tail] = v;
            tail++;
            if (tail >= cap) tail = 0;
            cnt++;
            inqueue[v] = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Dequeue(int* q, int cap, ref int head, ref int cnt)
        {
            int u = q[head];
            head++;
            if (head >= cap) head = 0;
            cnt--;
            return u;
        }

        public static bool Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent, int* inqueue)
        {
            SpArrays.InitDist(dist, n);
            SpArrays.InitParent(parent, n);
            for (int i = 0; i < n; i++) inqueue[i] = 0;
            int* q = stackalloc int[n];
            int* count = stackalloc int[n];
            for (int i = 0; i < n; i++) count[i] = 0;
            int qh = 0, qt = 0, cnt = 0;
            dist[start] = 0;
            Enqueue(q, n, ref qt, ref cnt, inqueue, start);
            count[start] = 1;
            while (cnt > 0)
            {
                int u = Dequeue(q, n, ref qh, ref cnt);
                inqueue[u] = 0;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long nd = dist[u] + weight[e];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        parent[v] = u;
                        if (inqueue[v] == 0)
                        {
                            Enqueue(q, n, ref qt, ref cnt, inqueue, v);
                            count[v]++;
                            if (count[v] > n) return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    public static unsafe class FloydWarshall
    {
        public static void Run(int n, long* dist, int* parent)
        {
            for (int k = 0; k < n; k++)
            {
                int knBase = k * n;
                for (int i = 0; i < n; i++)
                {
                    int inBase = i * n;
                    long dik = dist[inBase + k];
                    if (dik == long.MaxValue) continue;
                    for (int j = 0; j < n; j++)
                    {
                        long dkj = dist[knBase + j];
                        if (dkj == long.MaxValue) continue;
                        long nd = dik + dkj;
                        if (nd < dist[inBase + j])
                        {
                            dist[inBase + j] = nd;
                            parent[inBase + j] = k;
                        }
                    }
                }
            }
        }
    }

    public static unsafe class Johnson
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildAugmentedGraph(int* eu, int* ev, int* ew, int m, int n, long* augU, long* augV, long* augW)
        {
            for (int i = 0; i < m; i++)
            {
                augU[i] = eu[i];
                augV[i] = ev[i];
                augW[i] = ew[i];
            }
            for (int i = 0; i < n; i++)
            {
                augU[m + i] = n;
                augV[m + i] = i;
                augW[m + i] = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool RelaxAugmentedOnce(long* augU, long* augV, long* augW, int edgeCount, long* bfDist)
        {
            bool changed = false;
            for (int i = 0; i < edgeCount; i++)
            {
                long uu = augU[i], vv = augV[i];
                if (bfDist[uu] != long.MaxValue && bfDist[uu] + augW[i] < bfDist[vv])
                {
                    bfDist[vv] = bfDist[uu] + augW[i];
                    changed = true;
                }
            }
            return changed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasAugmentedNegativeCycle(long* augU, long* augV, long* augW, int edgeCount, long* bfDist)
        {
            for (int i = 0; i < edgeCount; i++)
            {
                long uu = augU[i], vv = augV[i];
                if (bfDist[uu] != long.MaxValue && bfDist[uu] + augW[i] < bfDist[vv])
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RunSingleSource(int n, int m, int* eu, int* ev, int* ew, long* dist, long* h, int s)
        {
            long* dBase = dist + s * n;
            for (int i = 0; i < n; i++) dBase[i] = long.MaxValue;
            dBase[s] = 0;
            SpMinHeap pq = new SpMinHeap(n);
            pq.PushOrUpdate(s, 0);
            while (pq.Size > 0)
            {
                int u = pq.Pop(out long d);
                if (d != dBase[u]) continue;
                long hu = h[u];
                for (int i = 0; i < m; i++)
                {
                    if (eu[i] != u) continue;
                    int v = ev[i];
                    long nd = d + ew[i] + hu - h[v];
                    if (nd < dBase[v])
                    {
                        dBase[v] = nd;
                        pq.PushOrUpdate(v, nd);
                    }
                }
            }
            pq.Dispose();
            for (int t = 0; t < n; t++)
            {
                if (dBase[t] != long.MaxValue)
                    dBase[t] = dBase[t] - h[s] + h[t];
            }
        }

        public static bool Run(int n, int start, int m, int* eu, int* ev, int* ew, long* dist)
        {
            int edgeCount = m + n;
            long* augU = stackalloc long[edgeCount];
            long* augV = stackalloc long[edgeCount];
            long* augW = stackalloc long[edgeCount];
            BuildAugmentedGraph(eu, ev, ew, m, n, augU, augV, augW);
            long* bfDist = stackalloc long[n + 1];
            for (int i = 0; i <= n; i++) bfDist[i] = 0;
            for (int iter = 0; iter < n; iter++)
            {
                if (!RelaxAugmentedOnce(augU, augV, augW, edgeCount, bfDist)) break;
            }
            if (HasAugmentedNegativeCycle(augU, augV, augW, edgeCount, bfDist)) return false;
            long* h = stackalloc long[n];
            for (int i = 0; i < n; i++) h[i] = bfDist[i];
            for (int s = 0; s < n; s++) RunSingleSource(n, m, eu, ev, ew, dist, h, s);
            return true;
        }
    }

    public static unsafe class ZeroOneShortestPath
    {
        private const int MaxLiveDequeEntriesPerVertex = 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DequePushFront(int* dq, long* dqDist, int cap, ref int head, ref int cnt, int v, long nd)
        {
            head--;
            if (head < 0) head = cap - 1;
            dq[head] = v;
            dqDist[head] = nd;
            cnt++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DequePushBack(int* dq, long* dqDist, int cap, ref int tail, ref int cnt, int v, long nd)
        {
            dq[tail] = v;
            dqDist[tail] = nd;
            tail++;
            if (tail >= cap) tail = 0;
            cnt++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DequePopFront(int* dq, long* dqDist, int cap, ref int head, ref int cnt, out long nd)
        {
            int v = dq[head];
            nd = dqDist[head];
            head++;
            if (head >= cap) head = 0;
            cnt--;
            return v;
        }

        public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist)
        {
            SpArrays.InitDist(dist, n);
            dist[start] = 0;
            int cap = n * MaxLiveDequeEntriesPerVertex;
            int* dq = stackalloc int[cap];
            long* dqDist = stackalloc long[cap];
            int dh = 0, dt = 0, cnt = 0;
            DequePushBack(dq, dqDist, cap, ref dt, ref cnt, start, 0);
            while (cnt > 0)
            {
                int u = DequePopFront(dq, dqDist, cap, ref dh, ref cnt, out long du);
                if (du != dist[u]) continue;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long nd = dist[u] + weight[e];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        if (weight[e] == 0)
                            DequePushFront(dq, dqDist, cap, ref dh, ref cnt, v, nd);
                        else
                            DequePushBack(dq, dqDist, cap, ref dt, ref cnt, v, nd);
                    }
                }
            }
        }
    }

    public static unsafe class PotentialDijkstra
    {
        public static void Run(int n, int start, int* head, int* to, int* next, long* weight, long* dist, int* parent, long* potential)
        {
            SpArrays.InitDist(dist, n);
            SpArrays.InitParent(parent, n);
            dist[start] = 0;
            SpMinHeap pq = new SpMinHeap(n);
            pq.PushOrUpdate(start, 0);
            while (pq.Size > 0)
            {
                int u = pq.Pop(out long d);
                if (d != dist[u]) continue;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    long reduced = weight[e] + potential[u] - potential[v];
                    long nd = dist[u] + reduced;
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        parent[v] = u;
                        pq.PushOrUpdate(v, nd);
                    }
                }
            }
            pq.Dispose();
            for (int i = 0; i < n; i++)
            {
                if (dist[i] != long.MaxValue)
                    dist[i] = dist[i] - potential[start] + potential[i];
            }
        }
    }
}
