namespace IAFahim.Graph.Connectivity
{
    using System.Runtime.CompilerServices;

    public static unsafe class IncrementalConnectivity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(int* parent, int* size, int n)
        {
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                size[i] = 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Find(int* parent, int i)
        {
            int root = i;
            while (root != parent[root])
                root = parent[root];
            int curr = i;
            while (curr != root)
            {
                int nxt = parent[curr];
                parent[curr] = root;
                curr = nxt;
            }
            return root;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Union(int* parent, int* size, int i, int j)
        {
            int rootI = Find(parent, i);
            int rootJ = Find(parent, j);
            if (rootI != rootJ)
            {
                if (size[rootI] < size[rootJ])
                {
                    int t = rootI; rootI = rootJ; rootJ = t;
                }
                parent[rootJ] = rootI;
                size[rootI] += size[rootJ];
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Connected(int* parent, int i, int j)
        {
            return Find(parent, i) == Find(parent, j);
        }
    }
}
