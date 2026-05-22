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
        public static int Run(int n, int* head, int* to, int* next, int* match, int* base_, int* p, int* v, int* blossom)
        {
            for (int i = 0; i < n; i++)
            {
                match[i] = -1;
                base_[i] = i;
                p[i] = -1;
                v[i] = -1;
            }
            int result = 0;
            for (int s = 0; s < n; s++)
            {
                if (match[s] != -1) continue;
                for (int i = 0; i < n; i++) v[i] = -1;
                int* q = stackalloc int[n];
                int qh = 0, qt = 0;
                q[qt++] = s;
                v[s] = s;
                if (FindPath(n, head, to, next, match, base_, p, v, q, &qh, &qt, s))
                    result++;
            }
            return result;
        }

        public static bool FindPath(int n, int* head, int* to, int* next, int* match, int* base_, int* p, int* v, int* q, int* qh, int* qt, int s)
        {
            while (*qh < *qt)
            {
                int u = q[(*qh)++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int u2 = to[e];
                    if (match[u2] == u2 || base_[u2] == base_[u] || match[u] == u2) continue;
                    if (v[u2] == s) continue;
                    v[u2] = s;
                    p[u2] = u;
                    if (match[u2] == -1)
                    {
                        int cur = u2;
                        while (cur != -1)
                        {
                            int prev = p[cur];
                            int nextMatched = prev == -1 ? -1 : (match[prev] == cur ? prev : -1);
                            match[cur] = prev;
                            match[prev] = cur;
                            cur = nextMatched;
                        }
                        return true;
                    }
                    int u3 = match[u2];
                    v[u3] = s;
                    q[(*qt)++] = u3;
                }
            }
            return false;
        }
    }

    public static unsafe class EdmondsMatching
    {
        public static int Run(int n, int* head, int* to, int* next, int* match)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            int* base_ = stackalloc int[n];
            int* p = stackalloc int[n];
            int* v = stackalloc int[n];
            int* blossom = stackalloc int[n];
            return BlossomGeneral.Run(n, head, to, next, match, base_, p, v, blossom);
        }
    }

    public static unsafe class WeightedBlossom
    {
        public static long Run(int n, long* w, int* match)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            long* potentials = stackalloc long[n];
            for (int i = 0; i < n; i++) potentials[i] = 0;
            long result = 0;
            for (int i = 0; i < n; i++)
            {
                if (match[i] != -1) continue;
                long* dist = stackalloc long[n];
                for (int j = 0; j < n; j++) dist[j] = long.MaxValue;
                int* parent = stackalloc int[n];
                for (int j = 0; j < n; j++) parent[j] = -1;
                dist[i] = 0;
                var pq = new MinHeap(n);
                try
                {
                    pq.PushOrUpdate(i, 0);
                    while (pq.Size > 0)
                    {
                        int u = pq.Pop(out long d);
                        if (d != dist[u]) continue;
                        for (int j = 0; j < n; j++)
                        {
                            if (i == j) continue;
                            long ndist = d + w[u * n + j] - potentials[u] - potentials[j];
                            if (ndist < dist[j])
                            {
                                dist[j] = ndist;
                                parent[j] = u;
                                pq.PushOrUpdate(j, ndist);
                            }
                        }
                    }
                }
                finally { pq.Dispose(); }
                for (int j = 0; j < n; j++) potentials[j] += dist[j];
            }
            for (int i = 0; i < n; i++)
            {
                if (match[i] != -1)
                    result += w[i * n + match[i]];
            }
            return result;
        }
    }
}