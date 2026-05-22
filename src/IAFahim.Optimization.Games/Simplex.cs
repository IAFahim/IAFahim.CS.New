namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Simplex
    {
        public struct Result
        {
            public double Value;
            public int Status; // 0: Optimal, 1: Unbounded, 2: Infeasible
        }

        public static Result Run(int m, int n, double* a, double* b, double* c, double* x)
        {
            double epsilon = 1e-9;
            int cols = n + m + 2;
            int rows = m + 2;
            double* tab = stackalloc double[rows * cols];
            for (int i = 0; i < rows * cols; i++) tab[i] = 0;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++) tab[i * cols + j] = a[i * n + j];
                tab[i * cols + n + i] = 1;
                tab[i * cols + cols - 2] = -1;
                tab[i * cols + cols - 1] = b[i];
            }
            for (int j = 0; j < n; j++) tab[m * cols + j] = -c[j];
            tab[(m + 1) * cols + cols - 2] = 1;

            int* basis = stackalloc int[m];
            for (int i = 0; i < m; i++) basis[i] = n + i;

            int minBIdx = -1;
            double minB = -epsilon;
            for (int i = 0; i < m; i++)
            {
                if (tab[i * cols + cols - 1] < minB)
                {
                    minB = tab[i * cols + cols - 1];
                    minBIdx = i;
                }
            }

            void Pivot(int r, int cIdx)
            {
                double pivotVal = tab[r * cols + cIdx];
                for (int j = 0; j < cols; j++) tab[r * cols + j] /= pivotVal;
                for (int i = 0; i < rows; i++)
                {
                    if (i != r)
                    {
                        double factor = tab[i * cols + cIdx];
                        for (int j = 0; j < cols; j++) tab[i * cols + j] -= factor * tab[r * cols + j];
                    }
                }
            }

            if (minBIdx != -1)
            {
                basis[minBIdx] = cols - 2;
                Pivot(minBIdx, cols - 2);
            }

            int SimplexCore(int objRow, bool isPhase1)
            {
                while (true)
                {
                    int pivotCol = -1;
                    double minC = -epsilon;
                    for (int j = 0; j < cols - 1; j++)
                    {
                        if (!isPhase1 && j == cols - 2) continue;
                        if (tab[objRow * cols + j] < minC)
                        {
                            minC = tab[objRow * cols + j];
                            pivotCol = j;
                        }
                    }
                    if (pivotCol == -1) return 0;

                    int pivotRow = -1;
                    double minRatio = double.MaxValue;
                    for (int i = 0; i < m; i++)
                    {
                        if (tab[i * cols + pivotCol] > epsilon)
                        {
                            double ratio = tab[i * cols + cols - 1] / tab[i * cols + pivotCol];
                            if (ratio < minRatio)
                            {
                                minRatio = ratio;
                                pivotRow = i;
                            }
                        }
                    }

                    if (pivotRow == -1) return 1;

                    basis[pivotRow] = pivotCol;
                    Pivot(pivotRow, pivotCol);
                }
            }

            if (minBIdx != -1)
            {
                int status = SimplexCore(m + 1, true);
                if (status == 1) return new Result { Status = 1 };
                if (tab[(m + 1) * cols + cols - 1] < -epsilon)
                    return new Result { Status = 2 };
            }

            int phase2Status = SimplexCore(m, false);
            if (phase2Status == 1) return new Result { Status = 1 };

            if (x != null)
            {
                for (int j = 0; j < n; j++) x[j] = 0;
                for (int i = 0; i < m; i++)
                {
                    if (basis[i] < n) x[basis[i]] = tab[i * cols + cols - 1];
                }
            }

            return new Result { Value = tab[m * cols + cols - 1], Status = 0 };
        }
    }
}
