namespace IAFahim.Graph.Matching
{
    using System;
    using System.Runtime.CompilerServices;

    internal unsafe struct MinHeap
    {
        public long* Dist;
        public int* V;
        public int* Pos;
        public int Size;

        public MinHeap(int n)
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

    public static unsafe class BlossomGeneral
    {
        private static int GetLca(int n, int* base_, int* parent, int* match, int* inPath, int u, int v)
        {
            for (int i = 0; i < n; i++) inPath[i] = 0;
            u = FindBase(base_, parent, match, u, inPath);
            return FindLca(base_, parent, match, v, inPath);
        }

        private static int FindBase(int* base_, int* parent, int* match, int u, int* inPath)
        {
            while (true)
            {
                u = base_[u];
                inPath[u] = 1;
                if (match[u] == -1) break;
                u = base_[parent[match[u]]];
            }
            return u;
        }

        private static int FindLca(int* base_, int* parent, int* match, int v, int* inPath)
        {
            while (true)
            {
                v = base_[v];
                if (inPath[v] == 1) return v;
                v = base_[parent[match[v]]];
            }
        }

        private static void Contract(int n, int* base_, int* parent, int* match, int* color, int* q, ref int qt, int u, int v, int lca)
        {
            while (base_[u] != lca)
            {
                parent[u] = v;
                int mv = match[u];
                if (color[mv] == 1) { color[mv] = 0; q[qt++] = mv; }
                
                UpdateBases(n, base_, base_[u], base_[mv], lca);
                v = mv;
                u = parent[v];
            }
        }

        private static void UpdateBases(int n, int* base_, int oldU, int oldMv, int lca)
        {
            for (int i = 0; i < n; i++)
                if (base_[i] == oldU || base_[i] == oldMv) base_[i] = lca;
        }

        private static bool FindAugmentingPath(int n, int* head, int* to, int* next, int* match, int* parent, int* base_, int* color, int* q, int* inPath, int s)
        {
            InitializeSearch(n, s, color, parent, base_, q, out int qh, out int qt);
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (base_[u] == base_[v] || match[u] == v) continue;
                    if (ProcessNeighbor(n, head, to, next, match, parent, base_, color, q, ref qt, inPath, u, v)) return true;
                }
            }
            return false;
        }

        private static void InitializeSearch(int n, int s, int* color, int* parent, int* base_, int* q, out int qh, out int qt)
        {
            for (int i = 0; i < n; i++) { color[i] = -1; parent[i] = -1; base_[i] = i; }
            qh = 0; qt = 0;
            color[s] = 0; q[qt++] = s;
        }

        private static bool ProcessNeighbor(int n, int* head, int* to, int* next, int* match, int* parent, int* base_, int* color, int* q, ref int qt, int* inPath, int u, int v)
        {
            if (color[v] == -1)
            {
                if (match[v] == -1) { AugmentPath(match, parent, u, v); return true; }
                color[v] = 1; parent[v] = u;
                int mv = match[v];
                color[mv] = 0; parent[mv] = v;
                q[qt++] = mv;
            }
            else if (color[v] == 0)
            {
                int lca = GetLca(n, base_, parent, match, inPath, u, v);
                Contract(n, base_, parent, match, color, q, ref qt, u, v, lca);
                Contract(n, base_, parent, match, color, q, ref qt, v, u, lca);
            }
            return false;
        }

        private static void AugmentPath(int* match, int* parent, int u, int v)
        {
            parent[v] = u;
            int cur = v;
            while (cur != -1)
            {
                int pNode = parent[cur];
                int nextMatched = match[pNode];
                match[cur] = pNode;
                match[pNode] = cur;
                cur = nextMatched;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int* head, int* to, int* next, int* match, int* base_, int* p, int* v, int* blossom, int* scratch)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            int result = 0;
            for (int s = 0; s < n; s++)
                if (match[s] == -1 && FindAugmentingPath(n, head, to, next, match, p, base_, v, scratch, blossom, s)) result++;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int* head, int* to, int* next, int* match, int* base_, int* p, int* v, int* blossom)
        {
            int* scratch = stackalloc int[n];
            return Run(n, head, to, next, match, base_, p, v, blossom, scratch);
        }
    }

    public static unsafe class WeightedBlossom
    {
        public static long Run(int n, long* w, int* match)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            long* potentials = stackalloc long[n];
            for (int i = 0; i < n; i++) potentials[i] = 0;

            for (int i = 0; i < n; i++)
            {
                if (match[i] == -1) min_cost_augment(n, i, w, potentials, match);
            }
            return CalculateTotalWeight(n, w, match);
        }

        private static void min_cost_augment(int n, int i, long* w, long* potentials, int* match)
        {
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            for (int j = 0; j < n; j++) { dist[j] = long.MaxValue; parent[j] = -1; }
            dist[i] = 0;
            MinHeap pq = new MinHeap(n);
            try
            {
                pq.PushOrUpdate(i, 0);
                while (pq.Size > 0)
                {
                    int u = pq.Pop(out long d);
                    if (d != dist[u]) continue;
                    RelaxEdges(n, u, i, w, potentials, dist, parent, &pq);
                }
            }
            finally { pq.Dispose(); }
            for (int j = 0; j < n; j++) if (dist[j] != long.MaxValue) potentials[j] += dist[j];
        }

        private static void RelaxEdges(int n, int u, int i, long* w, long* potentials, long* dist, int* parent, MinHeap* pq)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j) continue;
                long ndist = dist[u] + w[u * n + j] - potentials[u] - potentials[j];
                if (ndist < dist[j]) { dist[j] = ndist; parent[j] = u; pq->PushOrUpdate(j, ndist); }
            }
        }

        private static long CalculateTotalWeight(int n, long* w, int* match)
        {
            long result = 0;
            for (int i = 0; i < n; i++)
                if (match[i] != -1) result += w[i * n + match[i]];
            return result;
        }
    }
}