namespace IAFahim.Search
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Interactive
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StressCompare<TInput, TOutput>(TInput* input, int inputLen, delegate*<TInput*, int, TOutput*, int*, bool> brute, delegate*<TInput*, int, TOutput*, int*, bool> opt, delegate*<TOutput*, int, TOutput*, int, bool> cmp, TOutput* sb, TOutput* so)
            where TInput : unmanaged where TOutput : unmanaged
        {
            int bs = 0, os = 0; bool bf = brute(input, inputLen, sb, &bs), of = opt(input, inputLen, so, &os);
            if (bf != of) return false; if (!bf) return true; return cmp(sb, bs, so, os);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckerCompare<T>(T* expected, int eLen, T* actual, int aLen) where T : unmanaged, IComparable<T>
        {
            if (eLen != aLen) return false;
            for (int i = 0; i < eLen; i++) if (expected[i].CompareTo(actual[i]) != 0) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool QueryCacheGet<TKey, TValue>(TKey* keys, TValue* values, byte* occ, int cap, TKey key, out TValue val)
            where TKey : unmanaged where TValue : unmanaged
        {
            val = default; TKey* kp = &key; int h = GetUnmanagedHash(kp); int idx = h % cap;
            for (int i = 0; i < cap; i++) { int c = (idx + i) % cap; if (occ[c] == 0) return false; if (CompareKeys(&keys[c], kp)) { val = values[c]; return true; } }
            return false;
        }

        private static bool CompareKeys<T>(T* a, T* b) where T : unmanaged { byte* pa = (byte*)a, pb = (byte*)b; for (int i = 0; i < sizeof(T); i++) if (pa[i] != pb[i]) return false; return true; }
        private static int GetUnmanagedHash<T>(T* k) where T : unmanaged { int h = 17; byte* p = (byte*)k; for (int i = 0; i < sizeof(T); i++) h = h * 31 + p[i]; return h & 0x7FFFFFFF; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InteractiveTreeCentroidFind(int numNodes, int* head, int* next, int* to, delegate*<int, int> queryFn)
        {
            byte* removed = stackalloc byte[numNodes]; for (int i = 0; i < numNodes; i++) removed[i] = 0;
            int* sz = stackalloc int[numNodes], parent = stackalloc int[numNodes], queue = stackalloc int[numNodes];
            int currentRoot = 0;
            while (true)
            {
                int qTail = BuildComponentQueue(currentRoot, head, next, to, removed, parent, queue);
                if (qTail == 1) return queue[0];
                ComputeSubtreeSizes(qTail, queue, parent, sz);
                int centroid = FindBestCentroid(qTail, queue, parent, sz, head, next, to, removed);
                if (centroid == -1) centroid = queue[0];
                int nextHop = queryFn(centroid); if (nextHop == centroid) return centroid;
                removed[centroid] = 1; currentRoot = nextHop;
            }
        }

        private static int BuildComponentQueue(int root, int* head, int* next, int* to, byte* removed, int* parent, int* queue)
        {
            int qh = 0, qt = 0; queue[qt++] = root; parent[root] = -1;
            while (qh < qt) { int u = queue[qh++]; for (int e = head[u]; e != -1; e = next[e]) { int v = to[e]; if (v != parent[u] && removed[v] == 0) { parent[v] = u; queue[qt++] = v; } } }
            return qt;
        }

        private static void ComputeSubtreeSizes(int qt, int* queue, int* parent, int* sz)
        {
            for (int i = 0; i < qt; i++) sz[queue[i]] = 1;
            for (int i = qt - 1; i >= 0; i--) { int u = queue[i], p = parent[u]; if (p != -1) sz[p] += sz[u]; }
        }

        private static int FindBestCentroid(int qt, int* queue, int* parent, int* sz, int* head, int* next, int* to, byte* removed)
        {
            for (int i = 0; i < qt; i++)
            {
                int u = queue[i]; bool ok = (qt - sz[u] <= qt / 2);
                if (ok) for (int e = head[u]; e != -1; e = next[e]) { int v = to[e]; if (v != parent[u] && removed[v] == 0 && sz[v] > qt / 2) { ok = false; break; } }
                if (ok) return u;
            }
            return -1;
        }
    }
}
