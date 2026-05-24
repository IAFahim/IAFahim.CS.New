namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Simplex
    {
        public struct Result { public double Value; public int Status; }

        public static Result Run(int m, int n, double* a, double* b, double* c, double* x)
        {
            double eps = 1e-9; int cols = n + m + 2, rows = m + 2;
            double* tab = stackalloc double[rows * cols]; for (int i = 0; i < rows * cols; i++) tab[i] = 0;
            
            InitializeTableau(m, n, a, b, c, cols, tab, out int minBIdx);
            int* basis = stackalloc int[m]; for (int i = 0; i < m; i++) basis[i] = n + i;

            if (minBIdx != -1)
            {
                basis[minBIdx] = cols - 2; Pivot(rows, cols, tab, minBIdx, cols - 2);
                int status = SimplexCore(rows, cols, m, tab, m + 1, basis, true, eps);
                if (status == 1) return new Result { Status = 1 };
                if (tab[(m + 1) * cols + cols - 1] < -eps) return new Result { Status = 2 };
            }

            if (SimplexCore(rows, cols, m, tab, m, basis, false, eps) == 1) return new Result { Status = 1 };
            ExtractSolution(m, n, cols, tab, basis, x);
            return new Result { Value = tab[m * cols + cols - 1], Status = 0 };
        }

        private static void InitializeTableau(int m, int n, double* a, double* b, double* c, int cols, double* tab, out int minBIdx)
        {
            minBIdx = -1; double minB = -1e-9;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) tab[i * cols + j] = a[i * n + j];
                tab[i * cols + n + i] = 1; tab[i * cols + cols - 2] = -1; tab[i * cols + cols - 1] = b[i];
                if (b[i] < minB) { minB = b[i]; minBIdx = i; }
            }
            for (int j = 0; j < n; j++) tab[m * cols + j] = -c[j];
            tab[(m + 1) * cols + cols - 2] = 1;
        }

        private static int SimplexCore(int rows, int cols, int m, double* tab, int objRow, int* basis, bool phase1, double eps)
        {
            while (true)
            {
                int pCol = FindPivotCol(cols, tab, objRow, phase1, eps); if (pCol == -1) return 0;
                int pRow = FindPivotRow(m, cols, tab, pCol, eps); if (pRow == -1) return 1;
                basis[pRow] = pCol; Pivot(rows, cols, tab, pRow, pCol);
            }
        }

        private static int FindPivotCol(int cols, double* tab, int objRow, bool phase1, double eps)
        {
            int pc = -1; double minC = -eps;
            for (int j = 0; j < cols - 1; j++)
            {
                if (!phase1 && j == cols - 2) continue;
                if (tab[objRow * cols + j] < minC) { minC = tab[objRow * cols + j]; pc = j; }
            }
            return pc;
        }

        private static int FindPivotRow(int m, int cols, double* tab, int pCol, double eps)
        {
            int pr = -1; double minR = double.MaxValue;
            for (int i = 0; i < m; i++)
                if (tab[i * cols + pCol] > eps)
                {
                    double r = tab[i * cols + cols - 1] / tab[i * cols + pCol];
                    if (r < minR) { minR = r; pr = i; }
                }
            return pr;
        }

        private static void Pivot(int rows, int cols, double* tab, int r, int c)
        {
            double v = tab[r * cols + c]; for (int j = 0; j < cols; j++) tab[r * cols + j] /= v;
            for (int i = 0; i < rows; i++)
                if (i != r) { double f = tab[i * cols + c]; for (int j = 0; j < cols; j++) tab[i * cols + j] -= f * tab[r * cols + j]; }
        }

        private static void ExtractSolution(int m, int n, int cols, double* tab, int* basis, double* x)
        {
            if (x == null) return;
            for (int j = 0; j < n; j++) x[j] = 0;
            for (int i = 0; i < m; i++) if (basis[i] < n) x[basis[i]] = tab[i * cols + cols - 1];
        }
    }
}
