namespace IAFahim.Graph.Centroid
{
    using System.Runtime.CompilerServices;

    public static unsafe class CentroidDecomposition
    {
        public static int Build(int n, int* head, int* to, int* next, int* centroid, int* sz, byte* removed)
        {
            ComputeSizes(-1, 0, head, to, next, sz, removed);
            int c = FindCentroid(-1, 0, sz[0], head, to, next, sz, removed);
            centroid[0] = c;
            removed[c] = 1;
            return c;
        }

        private static void ComputeSizes(int p, int u, int* head, int* to, int* next, int* sz, byte* removed)
        {
            sz[u] = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p && removed[v] == 0) { ComputeSizes(u, v, head, to, next, sz, removed); sz[u] += sz[v]; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindCentroid(int p, int u, int total, int* head, int* to, int* next, int* sz, byte* removed)
        {
            int half = total / 2;
            while (true)
            {
                int heavy = -1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v != p && removed[v] == 0 && sz[v] > half)
                    {
                        heavy = v;
                        break;
                    }
                }
                if (heavy < 0) return u;
                p = u;
                u = heavy;
            }
        }

        public static void Decompose(int n, int* head, int* to, int* next, int u, byte* removed, int* sz, int* centroids, int* centroidCount)
        {
            ComputeSizes(-1, u, head, to, next, sz, removed);
            int c = FindCentroid(-1, u, sz[u], head, to, next, sz, removed);
            centroids[*centroidCount] = c;
            (*centroidCount)++;
            removed[c] = 1;
            for (int e = head[c]; e != 0; e = next[e])
            {
                int v = to[e];
                if (removed[v] == 0) Decompose(n, head, to, next, v, removed, sz, centroids, centroidCount);
            }
        }
    }
}
