namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static unsafe class MstShared
    {
        public const int NoEdge = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindSet(int* parent, int x)
        {
            int root = x;
            while (parent[root] != root) root = parent[root];
            while (parent[x] != root)
            {
                int next = parent[x];
                parent[x] = root;
                x = next;
            }
            return root;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HeapSortEdgeIndices(int* ew, int* idx, int m)
        {
            for (int i = 0; i < m; i++) idx[i] = i;
            for (int i = (m >> 1) - 1; i >= 0; i--) SiftDownEdge(ew, idx, i, m);
            for (int end = m - 1; end > 0; end--)
            {
                int t = idx[0]; idx[0] = idx[end]; idx[end] = t;
                SiftDownEdge(ew, idx, 0, end);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftDownEdge(int* ew, int* idx, int i, int n)
        {
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = i;
                if (l < n && ew[idx[l]] > ew[idx[m]]) m = l;
                if (r < n && ew[idx[r]] > ew[idx[m]]) m = r;
                if (m == i) break;
                int t = idx[i]; idx[i] = idx[m]; idx[m] = t;
                i = m;
            }
        }
    }

    internal unsafe struct MstMinHeap
    {
        private const int HeapRoot = 0;

        private const int NotInHeap = -1;

        public long* Dist;

        public int* V;

        public int* Pos;

        public int Size;

        public MstMinHeap(int n)
        {
            Dist = (long*)Marshal.AllocHGlobal((nint)((long)n * sizeof(long)));
            V = (int*)Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));
            Pos = (int*)Marshal.AllocHGlobal((nint)((long)n * sizeof(int)));
            for (int i = 0; i < n; i++) Pos[i] = NotInHeap;
            Size = 0;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((nint)Dist);
            Marshal.FreeHGlobal((nint)V);
            Marshal.FreeHGlobal((nint)Pos);
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

    public static unsafe class MinimumSpanningTreeKruskal
    {
        public static long Run(int n, int m, int* eu, int* ev, int* ew, int* mstEdges)
        {
            int* parent = stackalloc int[n], size = stackalloc int[n];
            InitializeUnion(n, parent, size);
            int* idx = stackalloc int[m];
            MstShared.HeapSortEdgeIndices(ew, idx, m);
            long mstWeight = 0;
            int edgeCount = 0;
            for (int i = 0; i < m && edgeCount < n - 1; i++)
                AddKruskalEdge(idx[i], eu, ev, ew, parent, size, mstEdges, ref edgeCount, ref mstWeight);
            return edgeCount == n - 1 ? mstWeight : -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeUnion(int n, int* parent, int* size)
        {
            for (int i = 0; i < n; i++) { parent[i] = i; size[i] = 1; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddKruskalEdge(int e, int* eu, int* ev, int* ew, int* parent, int* size, int* mstEdges, ref int edgeCount, ref long mstWeight)
        {
            int ra = MstShared.FindSet(parent, eu[e]);
            int rb = MstShared.FindSet(parent, ev[e]);
            if (ra != rb)
            {
                mstEdges[edgeCount++] = e;
                mstWeight += ew[e];
                UnionBySize(parent, size, ra, rb);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UnionBySize(int* parent, int* size, int ra, int rb)
        {
            if (size[ra] < size[rb]) { int t = ra; ra = rb; rb = t; }
            parent[rb] = ra;
            size[ra] += size[rb];
        }
    }

    public static unsafe class MinimumSpanningTreePrim
    {
        private const int RootVertex = 0;

        public static long Run(int n, int* head, int* to, int* next, int* weight, int* mstEdges)
        {
            bool* used = stackalloc bool[n];
            long* bestW = stackalloc long[n];
            int* parentEdge = stackalloc int[n];
            InitializePrim(n, used, bestW, parentEdge);
            long mstWeight = 0;
            int edgeCount = 0;
            MstMinHeap pq = new MstMinHeap(n);
            try
            {
                bestW[RootVertex] = 0;
                pq.PushOrUpdate(RootVertex, 0);
                while (pq.Size > 0 && edgeCount < n - 1)
                {
                    int u = pq.Pop(out long d);
                    if (used[u]) continue;
                    used[u] = true;
                    if (u != RootVertex) { mstEdges[edgeCount++] = parentEdge[u]; mstWeight += d; }
                    PrimRelax(u, head, to, next, weight, used, bestW, parentEdge, &pq);
                }
            }
            finally { pq.Dispose(); }
            return edgeCount == n - 1 ? mstWeight : -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializePrim(int n, bool* used, long* bestW, int* parentEdge)
        {
            for (int i = 0; i < n; i++) { used[i] = false; bestW[i] = long.MaxValue; parentEdge[i] = -1; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PrimRelax(int u, int* head, int* to, int* next, int* weight, bool* used, long* bestW, int* parentEdge, MstMinHeap* pq)
        {
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (!used[v] && weight[e] < bestW[v])
                {
                    bestW[v] = weight[e];
                    parentEdge[v] = e;
                    pq->PushOrUpdate(v, weight[e]);
                }
            }
        }
    }

    public static unsafe class SecondBestMst
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long BuildMstExcluding(int* idx, int m, int excludeEdge, int* eu, int* ev, int* ew, int* parent, int n, out int count)
        {
            for (int i = 0; i < n; i++) parent[i] = i;
            count = 0;
            long weight = 0;
            for (int p = 0; p < m; p++)
            {
                int e = idx[p];
                if (e == excludeEdge) continue;
                int pu = MstShared.FindSet(parent, eu[e]);
                int pv = MstShared.FindSet(parent, ev[e]);
                if (pu != pv) { parent[pu] = pv; weight += ew[e]; count++; }
            }
            return weight;
        }

        public static long Run(int n, int m, int* eu, int* ev, int* ew)
        {
            int* idx = stackalloc int[m];
            MstShared.HeapSortEdgeIndices(ew, idx, m);
            int* parent = stackalloc int[n];
            long best = BuildMstExcluding(idx, m, MstShared.NoEdge, eu, ev, ew, parent, n, out int bestCount);
            if (bestCount < n - 1) return -1;
            long second = long.MaxValue;
            for (int skip = 0; skip < m; skip++)
            {
                long alt = BuildMstExcluding(idx, m, skip, eu, ev, ew, parent, n, out int count);
                if (count == n - 1) second = Math.Min(second, alt);
            }
            return second == long.MaxValue ? -1 : second;
        }
    }
}
