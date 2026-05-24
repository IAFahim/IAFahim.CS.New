namespace IAFahim.Optimization.Matroid
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LinearMatroid
    {
        public static int Rank(int n, int m, int* a, int* basis)
        {
            int rank = 0;
            for (int col = 0; col < m; col++)
            {
                int row = FindPivotRow(n, m, col, rank, a);
                if (row == -1) continue;
                SwapRows(m, rank, row, a);
                EliminateOtherRows(n, m, col, rank, a);
                basis[rank++] = col;
            }
            return rank;
        }

        private static int FindPivotRow(int n, int m, int col, int rank, int* a)
        {
            for (int r = rank; r < n; r++) if (a[r * m + col] != 0) return r;
            return -1;
        }

        private static void SwapRows(int m, int r1, int r2, int* a)
        {
            if (r1 == r2) return;
            for (int j = 0; j < m; j++) { int t = a[r1 * m + j]; a[r1 * m + j] = a[r2 * m + j]; a[r2 * m + j] = t; }
        }

        private static void EliminateOtherRows(int n, int m, int col, int pivotRow, int* a)
        {
            for (int r = 0; r < n; r++)
                if (r != pivotRow && a[r * m + col] != 0)
                    for (int j = 0; j < m; j++) a[r * m + j] ^= a[pivotRow * m + j];
        }
    }
}
