namespace IAFahim.Graph.Decomposition
{
    using System.Runtime.InteropServices;

    public static unsafe class CentroidDecomposition
    {
        // Centroid tree parents for an undirected tree. head/to/next use 0 as empty sentinel.
        // centParent[centroid_root] = -1.
        public static void Build(int n, int* head, int* to, int* next, int* centParent)
        {
            for (int i = 0; i < n; i++) centParent[i] = -1;
            if (n <= 0) return;

            byte* removed = (byte*)Marshal.AllocHGlobal(n);
            int* sz = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* stack = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* comp = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            for (int i = 0; i < n; i++) removed[i] = 0;

            Decompose(0, -1, head, to, next, removed, sz, parent, stack, comp, centParent);

            Marshal.FreeHGlobal((nint)comp);
            Marshal.FreeHGlobal((nint)stack);
            Marshal.FreeHGlobal((nint)parent);
            Marshal.FreeHGlobal((nint)sz);
            Marshal.FreeHGlobal((nint)removed);
        }

        private static void Decompose(
            int entry, int centPar, int* head, int* to, int* next,
            byte* removed, int* sz, int* parent, int* stack, int* comp, int* centParent)
        {
            int total = CalcSize(entry, -1, head, to, next, removed, sz, parent, stack, comp);
            int centroid = FindCentroid(entry, -1, total, head, to, next, removed, sz);
            centParent[centroid] = centPar;
            removed[centroid] = 1;
            for (int e = head[centroid]; e != 0; e = next[e])
            {
                int v = to[e];
                if (removed[v] != 0) continue;
                Decompose(v, centroid, head, to, next, removed, sz, parent, stack, comp, centParent);
            }
        }

        private static int CalcSize(
            int root, int par, int* head, int* to, int* next,
            byte* removed, int* sz, int* parent, int* stack, int* comp)
        {
            int count = 0;
            int ss = 0;
            stack[ss++] = root;
            parent[root] = par;
            while (ss > 0)
            {
                int u = stack[--ss];
                comp[count++] = u;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v == parent[u] || removed[v] != 0) continue;
                    parent[v] = u;
                    stack[ss++] = v;
                }
            }
            for (int i = count - 1; i >= 0; i--)
            {
                int u = comp[i];
                sz[u] = 1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v == parent[u] || removed[v] != 0) continue;
                    sz[u] += sz[v];
                }
            }
            return sz[root];
        }

        private static int FindCentroid(
            int root, int par, int total, int* head, int* to, int* next, byte* removed, int* sz)
        {
            int u = root;
            int p = par;
            while (true)
            {
                int heavy = -1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v == p || removed[v] != 0) continue;
                    if (sz[v] > total / 2) { heavy = v; break; }
                }
                if (heavy < 0) return u;
                p = u;
                u = heavy;
            }
        }
    }
}
