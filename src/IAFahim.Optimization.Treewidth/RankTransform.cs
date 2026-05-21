namespace IAFahim.Optimization.Treewidth
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RankTransform
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* x, int n, int* rank)
        {
            int* sorted = stackalloc int[n];
            for (int i = 0; i < n; i++) sorted[i] = x[i];
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (sorted[i] > sorted[j])
                    {
                        int tmp = sorted[i];
                        sorted[i] = sorted[j];
                        sorted[j] = tmp;
                    }
                }
            }
            int r = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == 0 || sorted[i] != sorted[i - 1]) r++;
                rank[sorted[i]] = r;
            }
        }
    }
}
