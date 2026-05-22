namespace IAFahim.Graph.Connectivity
{
    using System.Runtime.CompilerServices;

    public static unsafe class DynamicTransitiveClosure
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(byte* reach, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    reach[i * n + j] = (i == j) ? (byte)1 : (byte)0;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddEdge(byte* reach, int n, int u, int v)
        {
            if (reach[u * n + v] != 0) return;

            for (int i = 0; i < n; i++)
            {
                if (reach[i * n + u] != 0)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (reach[v * n + j] != 0)
                        {
                            reach[i * n + j] = 1;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanReach(byte* reach, int n, int u, int v)
        {
            return reach[u * n + v] != 0;
        }
    }
}
