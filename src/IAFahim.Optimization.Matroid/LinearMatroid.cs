namespace IAFahim.Optimization.Matroid
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LinearMatroid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rank(int n, int m, int* a, int* basis)
        {
            int rank = 0;
            for (int col = 0; col < m; col++)
            {
                int row = rank;
                while (row < n && a[row * m + col] == 0) row++;
                if (row == n) continue;
                if (row != rank)
                {
                    for (int j = 0; j < m; j++)
                    {
                        int tmp = a[row * m + j];
                        a[row * m + j] = a[rank * m + j];
                        a[rank * m + j] = tmp;
                    }
                }
                for (int r = 0; r < n; r++)
                {
                    if (r != rank && a[r * m + col] != 0)
                    {
                        for (int j = 0; j < m; j++)
                            a[r * m + j] ^= a[rank * m + j];
                    }
                }
                basis[rank++] = col;
            }
            return rank;
        }
    }
}
