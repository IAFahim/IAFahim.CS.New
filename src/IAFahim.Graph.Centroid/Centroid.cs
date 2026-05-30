namespace IAFahim.Graph.Centroid
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class CentroidDecomposition
    {
        public static int Build(int n, int* head, int* to, int* next, int* centroid, int* sz, byte* removed)
        {
            int tempStorage = 0;
            ComputeSizes(n, -1, 0, head, to, next, sz, removed, &tempStorage);
            int c = FindCentroid(n, -1, 0, head, to, next, sz, removed, &tempStorage);
            centroid[0] = c;
            removed[c] = 1;
            return c;
        }

        private static void ComputeSizes(int n, int p, int u, int* head, int* to, int* next, int* sz, byte* removed, int* temp)
        {
            sz[u] = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p && removed[v] == 0) { ComputeSizes(n, u, v, head, to, next, sz, removed, temp); sz[u] += sz[v]; }
            }
        }

        private static int FindCentroid(int n, int p, int u, int* head, int* to, int* next, int* sz, byte* removed, int* temp)
        {
            int total = sz[u];
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p && removed[v] == 0 && sz[v] > 0 && sz[v] > total / 2)
                    return FindCentroid(n, u, v, head, to, next, sz, removed, temp);
            }
            return u;
        }

        public static void Decompose(int n, int* head, int* to, int* next, int u, byte* removed, int* sz, int* centroids, int* centroidCount)
        {
            ComputeSizes(n, -1, u, head, to, next, sz, removed, &n);
            int c = FindCentroid(n, -1, u, head, to, next, sz, removed, &n);
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
