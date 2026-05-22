namespace IAFahim.Graph.Connectivity
{
    using System.Runtime.CompilerServices;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct RollbackOp
    {
        public int U;
        public int V;
    }

    public static unsafe class OfflineDynamicMst
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
            while (i != parent[i]) i = parent[i];
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Union(int* parent, int* size, int i, int j, RollbackOp* history, ref int historyCount)
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
                history[historyCount].U = rootI;
                history[historyCount].V = rootJ;
                historyCount++;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rollback(int* parent, int* size, RollbackOp* history, ref int historyCount, int targetCount)
        {
            while (historyCount > targetCount)
            {
                historyCount--;
                int u = history[historyCount].U;
                int v = history[historyCount].V;
                size[u] -= size[v];
                parent[v] = v;
            }
        }
    }
}
