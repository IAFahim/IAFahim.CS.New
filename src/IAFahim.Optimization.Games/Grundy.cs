namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Grundy
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mex(int* values, int n)
        {
            bool* seen = stackalloc bool[n + 1];
            for (int i = 0; i <= n; i++) seen[i] = false;
            for (int i = 0; i < n; i++)
            {
                int v = values[i];
                if (v >= 0 && v <= n) seen[v] = true;
            }
            for (int i = 0; i <= n; i++)
                if (!seen[i]) return i;
            return n + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpragueGrundy(int* moves, int* counts, int n, int* g, int* scratch)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < counts[i]; j++)
                    scratch[j] = g[moves[i * 10 + j]];
                g[i] = Mex(scratch, counts[i]);
            }
        }
    }
}