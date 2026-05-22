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
        private static int Find(int* parent, int x)
        {
            while (parent[x] != x) x = parent[x];
            return x;
        }

        private static void Union(int* parent, int* size, int a, int b)
        {
            int ra = Find(parent, a);
            int rb = Find(parent, b);
            if (ra == rb) return;
            if (size[ra] < size[rb]) { int t = ra; ra = rb; rb = t; }
            parent[rb] = ra;
            size[ra] += size[rb];
        }

        public static long Run(int n, int m, int* eu, int* ev, int* ew, int* mstEdges)
        {
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            for (int i = 0; i < n; i++) { parent[i] = i; size[i] = 1; }
            int* idx = stackalloc int[m];
            for (int i = 0; i < m; i++) idx[i] = i;
            for (int i = 1; i < m; i++)
            {
                int key = ew[i];
                int j = i - 1;
                while (j >= 0 && ew[idx[j]] > key)
                {
                    idx[j + 1] = idx[j];
                    j--;
                }
                idx[j + 1] = i;
            }
            long mstWeight = 0;
            int edgeCount = 0;
            for (int i = 0; i < m && edgeCount < n - 1; i++)
            {
                int e = idx[i];
                int u = eu[e], v = ev[e];
                if (Find(parent, u) != Find(parent, v))
                {
                    mstEdges[edgeCount++] = e;
                    mstWeight += ew[e];
                    Union(parent, size, u, v);
                }
            }
            return edgeCount == n - 1 ? mstWeight : -1;
        }
    }

    public static unsafe class MinimumSpanningTreePrim
    {
        public static long Run(int n, int* head, int* to, int* next, int* weight, int* mstEdges)
        {
            bool* used = stackalloc bool[n];
            for (int i = 0; i < n; i++) used[i] = false;
            long mstWeight = 0;
            int edgeCount = 0;
            long* bestW = stackalloc long[n];
            for (int i = 0; i < n; i++) bestW[i] = long.MaxValue;
            int* parentEdge = stackalloc int[n];
            for (int i = 0; i < n; i++) parentEdge[i] = -1;

            var pq = new MstMinHeap(n);
            try
            {
                bestW[0] = 0;
                pq.PushOrUpdate(0, 0);

                while (pq.Size > 0 && edgeCount < n - 1)
                {
                    int u = pq.Pop(out long d);
                    if (used[u]) continue;
                    used[u] = true;
                    
                    if (u != 0 && parentEdge[u] != -1)
                    {
                        mstEdges[edgeCount++] = parentEdge[u];
                        mstWeight += d;
                    }

                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (!used[v] && weight[e] < bestW[v])
                        {
                            bestW[v] = weight[e];
                            parentEdge[v] = e;
                            pq.PushOrUpdate(v, weight[e]);
                        }
                    }
                }
            }
            finally { pq.Dispose(); }

            return edgeCount == n - 1 ? mstWeight : -1;
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
            long best = 0;
            {
                int* parent = stackalloc int[n];
                int* size = stackalloc int[n];
                for (int i = 0; i < n; i++) { parent[i] = i; size[i] = 1; }
                int* idx = stackalloc int[m];
                for (int i = 0; i < m; i++) idx[i] = i;
                for (int i = 1; i < m; i++)
                {
                    int key = ew[i];
                    int j = i - 1;
                    while (j >= 0 && ew[idx[j]] > key) { idx[j + 1] = idx[j]; j--; }
                    idx[j + 1] = i;
                }
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
                for (int j = 0; j < m; j++)
                {
                    if (j == skip) continue;
                    int pu = Find(parent, eu[j]);
                    int pv = Find(parent, ev[j]);
                    if (pu != pv) { parent[pu] = pv; alt += ew[j]; count++; }
                }
                if (count == n - 1) second = Math.Min(second, alt);
            }
            return second == long.MaxValue ? -1 : second;
        }
    }
}