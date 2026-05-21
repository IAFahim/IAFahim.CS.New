namespace IAFahim.Optimization.DivideConquer
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MatrixSearch
    {
        public static int Run(int m, int n, int* a, int target)
        {
            int lo = 0, hi = m * n - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                int val = a[mid];
                if (val == target) return mid;
                if (val < target) lo = mid + 1;
                else hi = mid - 1;
            }
            return -1;
        }

        public static int RunSortedColumns(int m, int n, int* a, int target)
        {
            int row = 0, col = n - 1;
            while (row < m && col >= 0)
            {
                int val = a[row * n + col];
                if (val == target) return row * n + col;
                if (val > target) col--;
                else row++;
            }
            return -1;
        }
    }
}
