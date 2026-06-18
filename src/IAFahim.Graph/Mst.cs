namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    internal unsafe struct MstMinHeap
    {
        public long* Dist;
        public int* V;
        public int* Pos;
        public int Size;

        public MstMinHeap(int n)
        {
            Dist = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
            V = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
            Pos = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
            for (int i = 0; i < n; i++) Pos[i] = -1;
            Size = 0;
        }

        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)Dist);
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)V);
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)Pos);
        }

        public void PushOrUpdate(int v, long d)
        {
            int idx = Pos[v];
            if (idx == -1)
            {
                idx = Size++;
                V[idx] = v;
            }
            Dist[idx] = d;
            while (idx > 0)
            {
                int p = (idx - 1) / 2;
                if (Dist[p] <= Dist[idx]) break;
                long tmpD = Dist[p]; Dist[p] = Dist[idx]; Dist[idx] = tmpD;
                int tmpV = V[p]; V[p] = V[idx]; V[idx] = tmpV;
                Pos[V[p]] = p;
                Pos[V[idx]] = idx;
                idx = p;
            }
        }

        public int Pop(out long d)
        {
            int u = V[0];
            d = Dist[0];
            Pos[u] = -1;
            Size--;
            if (Size > 0)
            {
                Dist[0] = Dist[Size];
                V[0] = V[Size];
                Pos[V[0]] = 0;
                int idx = 0;
                while (idx * 2 + 1 < Size)
                {
                    int left = idx * 2 + 1;
                    int right = idx * 2 + 2;
                    int smallest = left;
                    if (right < Size && Dist[right] < Dist[left]) smallest = right;
                    if (Dist[idx] <= Dist[smallest]) break;
                    long tmpD = Dist[idx]; Dist[idx] = Dist[smallest]; Dist[smallest] = tmpD;
                    int tmpV = V[idx]; V[idx] = V[smallest]; V[smallest] = tmpV;
                    Pos[V[idx]] = idx;
                    Pos[V[smallest]] = smallest;
                    idx = smallest;
                }
            }
            return u;
        }
    }

    public static unsafe class MinimumSpanningTreeKruskal
    {
        public static long Run(int n, int m, int* eu, int* ev, int* ew, int* mstEdges)
        {
            int* parent = stackalloc int[n], size = stackalloc int[n];
            InitializeKruskal(n, parent, size);
            
            int* idx = stackalloc int[m];
            SortEdges(m, ew, idx);
            
            long mstWeight = 0;
            int edgeCount = 0;
            for (int i = 0; i < m && edgeCount < n - 1; i++)
            {
                if (ProcessKruskalEdge(idx[i], eu, ev, ew, parent, size, mstEdges, ref edgeCount, ref mstWeight)) { }
            }
            return edgeCount == n - 1 ? mstWeight : -1;
        }

        private static void InitializeKruskal(int n, int* parent, int* size)
        {
            for (int i = 0; i < n; i++) { parent[i] = i; size[i] = 1; }
        }

        private static void SortEdges(int m, int* ew, int* idx)
        {
            for (int i = 0; i < m; i++) idx[i] = i;
            for (int i = 1; i < m; i++)
            {
                int key = ew[idx[i]], ki = idx[i], j = i - 1;
                while (j >= 0 && ew[idx[j]] > key) { idx[j + 1] = idx[j]; j--; }
                idx[j + 1] = ki;
            }
        }

        private static bool ProcessKruskalEdge(int e, int* eu, int* ev, int* ew, int* parent, int* size, int* mstEdges, ref int edgeCount, ref long mstWeight)
        {
            int ra = Find(parent, eu[e]), rb = Find(parent, ev[e]);
            if (ra != rb)
            {
                mstEdges[edgeCount++] = e;
                mstWeight += ew[e];
                Union(parent, size, ra, rb);
                return true;
            }
            return false;
        }

        private static int Find(int* parent, int x)
        {
            while (parent[x] != x) x = parent[x];
            return x;
        }

        private static void Union(int* parent, int* size, int ra, int rb)
        {
            if (size[ra] < size[rb]) { int t = ra; ra = rb; rb = t; }
            parent[rb] = ra; size[ra] += size[rb];
        }
    }

    public static unsafe class MinimumSpanningTreePrim
    {
        public static long Run(int n, int* head, int* to, int* next, int* weight, int* mstEdges)
        {
            bool* used = stackalloc bool[n];
            long* bestW = stackalloc long[n];
            int* parentEdge = stackalloc int[n];
            InitializePrim(n, used, bestW, parentEdge);

            long mstWeight = 0;
            int edgeCount = 0;
            var pq = new MstMinHeap(n);
            try
            {
                bestW[0] = 0; pq.PushOrUpdate(0, 0);
                while (pq.Size > 0 && edgeCount < n - 1)
                {
                    int u = pq.Pop(out long d);
                    if (used[u]) continue;
                    used[u] = true;
                    if (u != 0) { mstEdges[edgeCount++] = parentEdge[u]; mstWeight += d; }
                    PrimRelax(u, head, to, next, weight, used, bestW, parentEdge, &pq);
                }
            }
            finally { pq.Dispose(); }
            return edgeCount == n - 1 ? mstWeight : -1;
        }

        private static void InitializePrim(int n, bool* used, long* bestW, int* parentEdge)
        {
            for (int i = 0; i < n; i++) { used[i] = false; bestW[i] = long.MaxValue; parentEdge[i] = -1; }
        }

        private static void PrimRelax(int u, int* head, int* to, int* next, int* weight, bool* used, long* bestW, int* parentEdge, MstMinHeap* pq)
        {
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (!used[v] && weight[e] < bestW[v])
                {
                    bestW[v] = weight[e]; parentEdge[v] = e;
                    pq->PushOrUpdate(v, weight[e]);
                }
            }
        }
    }

    public static unsafe class SecondBestMst
    {
        private static int Find(int* parent, int x)
        {
            while (parent[x] != x) x = parent[x];
            return x;
        }

        public static long Run(int n, int m, int* eu, int* ev, int* ew)
        {
            int* idx = stackalloc int[m];
            for (int i = 0; i < m; i++) idx[i] = i;
            for (int i = 1; i < m; i++)
            {
                int key = ew[idx[i]], ki = idx[i], j = i - 1;
                while (j >= 0 && ew[idx[j]] > key) { idx[j + 1] = idx[j]; j--; }
                idx[j + 1] = ki;
            }
            long best = 0;
            {
                int* parent = stackalloc int[n];
                int* size = stackalloc int[n];
                for (int i = 0; i < n; i++) { parent[i] = i; size[i] = 1; }
                int count = 0;
                for (int i = 0; i < m && count < n - 1; i++)
                {
                    int e = idx[i];
                    int pu = Find(parent, eu[e]);
                    int pv = Find(parent, ev[e]);
                    if (pu != pv) { parent[pu] = pv; best += ew[e]; count++; }
                }
                if (count < n - 1) return -1;
            }
            long second = long.MaxValue;
            for (int skip = 0; skip < m; skip++)
            {
                int* parent = stackalloc int[n];
                for (int i = 0; i < n; i++) parent[i] = i;
                int count = 0;
                long alt = 0;
                for (int p = 0; p < m; p++)
                {
                    int e = idx[p];
                    if (e == skip) continue;
                    int pu = Find(parent, eu[e]);
                    int pv = Find(parent, ev[e]);
                    if (pu != pv) { parent[pu] = pv; alt += ew[e]; count++; }
                }
                if (count == n - 1) second = Math.Min(second, alt);
            }
            return second == long.MaxValue ? -1 : second;
        }
    }
}