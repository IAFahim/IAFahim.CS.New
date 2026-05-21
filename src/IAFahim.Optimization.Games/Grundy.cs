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
        public static int[] SpragueGrundy(int* moves, int* counts, int n)
        {
            int* g = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                int* vals = stackalloc int[counts[i]];
                for (int j = 0; j < counts[i]; j++)
                    vals[j] = g[moves[i * 10 + j]];
                g[i] = Mex(vals, counts[i]);
            }
            int[] result = new int[n];
            for (int i = 0; i < n; i++) result[i] = g[i];
            return result;
        }
    }
}
