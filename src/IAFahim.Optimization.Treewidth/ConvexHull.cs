namespace IAFahim.Optimization.Treewidth
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ConvexHull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckMonge(long* a, int m, int n)
        {
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    long lhs = a[i * n + j] + a[(i + 1) * n + (j + 1)];
                    long rhs = a[i * n + (j + 1)] + a[(i + 1) * n + j];
                    if (lhs > rhs) return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckQuadrangle(long* a, int m, int n)
        {
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    long d = a[i * n + j] - a[(i + 1) * n + j] - a[i * n + (j + 1)] + a[(i + 1) * n + (j + 1)];
                    if (d < 0) return false;
                }
            }
            return true;
        }
    }
}
