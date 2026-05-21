namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MeanPayoff
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Solve(int n, int m, int* from, int* to, long* w, long* potential)
        {
            for (int i = 0; i < n; i++) potential[i] = 0;
            for (int iter = 0; iter < n; iter++)
            {
                bool updated = false;
                for (int e = 0; e < m; e++)
                {
                    int u = from[e], v = to[e];
                    long nxt = potential[v] + w[e];
                    if (potential[u] < nxt)
                    {
                        potential[u] = nxt;
                        updated = true;
                    }
                }
                if (!updated) break;
            }
            for (int e = 0; e < m; e++)
            {
                int u = from[e], v = to[e];
                if (potential[u] < potential[v] + w[e]) return false;
            }
            return true;
        }
    }
}
