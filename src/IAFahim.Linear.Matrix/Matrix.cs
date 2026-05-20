namespace IAFahim.Linear.Matrix
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MatrixNew
    {
        public static void Run(int n, int m, long* a, long val)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    a[i * m + j] = val;
        }
    }

    public static unsafe class MatrixIdentity
    {
        public static void Run(int n, long* a)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    a[i * n + j] = (i == j) ? 1 : 0;
        }
    }

    public static unsafe class MatrixAdd
    {
        public static void Run(int n, int m, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    c[i * m + j] = a[i * m + j] + b[i * m + j];
        }
    }

    public static unsafe class MatrixSub
    {
        public static void Run(int n, int m, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    c[i * m + j] = a[i * m + j] - b[i * m + j];
        }
    }

    public static unsafe class MatrixMul
    {
        public static void Run(int n, int m, int p, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < p; j++)
                {
                    long sum = 0;
                    for (int k = 0; k < m; k++)
                        sum += a[i * m + k] * b[k * p + j];
                    c[i * p + j] = sum;
                }
        }
    }

    public static unsafe class MatrixPow
    {
        public static void Run(int n, long* a, long* result, long exp)
        {
            long* temp = stackalloc long[n * n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    result[i * n + j] = (i == j) ? 1 : 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    temp[i * n + j] = a[i * n + j];
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    long* res2 = stackalloc long[n * n];
                    for (int i = 0; i < n; i++)
                        for (int j = 0; j < n; j++)
                        {
                            long sum = 0;
                            for (int k = 0; k < n; k++)
                                sum += result[i * n + k] * temp[k * n + j];
                            res2[i * n + j] = sum;
                        }
                    for (int i = 0; i < n * n; i++) result[i] = res2[i];
                }
                long* temp2 = stackalloc long[n * n];
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        long sum = 0;
                        for (int k = 0; k < n; k++)
                            sum += temp[i * n + k] * temp[k * n + j];
                        temp2[i * n + j] = sum;
                    }
                for (int i = 0; i < n * n; i++) temp[i] = temp2[i];
                exp >>= 1;
            }
        }
    }

    public static unsafe class MatrixVecMul
    {
        public static void Run(int n, int m, long* mat, long* vec, long* res)
        {
            for (int i = 0; i < n; i++)
            {
                long sum = 0;
                for (int j = 0; j < m; j++)
                    sum += mat[i * m + j] * vec[j];
                res[i] = sum;
            }
        }
    }

    public static unsafe class GaussianElimination
    {
        public static int Run(int n, int m, long* a, long* b, long* x)
        {
            for (int col = 0, row = 0; col < m && row < n; col++, row++)
            {
                int sel = row;
                for (int i = row; i < n; i++)
                    if (Math.Abs(a[i * m + col]) > Math.Abs(a[sel * m + col]))
                        sel = i;
                if (a[sel * m + col] == 0) continue;
                for (int j = 0; j < m; j++)
                {
                    long tmp = a[sel * m + j];
                    a[sel * m + j] = a[row * m + j];
                    a[row * m + j] = tmp;
                }
                long tb = b[sel]; b[sel] = b[row]; b[row] = tb;
                long div = a[row * m + col];
                for (int j = 0; j < m; j++) a[row * m + j] /= div;
                b[row] /= div;
                for (int i = 0; i < n; i++)
                {
                    if (i != row)
                    {
                        long factor = a[i * m + col];
                        for (int j = 0; j < m; j++) a[i * m + j] -= factor * a[row * m + j];
                        b[i] -= factor * b[row];
                    }
                }
            }
            for (int i = 0; i < n; i++) x[i] = b[i];
            return n;
        }
    }

    public static unsafe class GaussJordan
    {
        public static int Run(int n, int m, long* a, long* b, long* x)
        {
            for (int col = 0, row = 0; col < m && row < n; col++, row++)
            {
                int sel = row;
                for (int i = row; i < n; i++)
                    if (Math.Abs(a[i * m + col]) > Math.Abs(a[sel * m + col]))
                        sel = i;
                if (a[sel * m + col] == 0) continue;
                for (int j = 0; j < m; j++)
                {
                    long tmp = a[sel * m + j];
                    a[sel * m + j] = a[row * m + j];
                    a[row * m + j] = tmp;
                }
                long tb = b[sel]; b[sel] = b[row]; b[row] = tb;
                long div = a[row * m + col];
                for (int j = 0; j < m; j++) a[row * m + j] /= div;
                b[row] /= div;
                for (int i = 0; i < n; i++)
                {
                    if (i != row)
                    {
                        long factor = a[i * m + col];
                        for (int j = 0; j < m; j++) a[i * m + j] -= factor * a[row * m + j];
                        b[i] -= factor * b[row];
                    }
                }
            }
            for (int i = 0; i < n; i++) x[i] = b[i];
            return n;
        }
    }

    public static unsafe class MatrixRank
    {
        public static int Run(int n, int m, long* a)
        {
            int rank = 0;
            bool* used = stackalloc bool[n];
            for (int i = 0; i < n; i++) used[i] = false;
            for (int col = 0; col < m; col++)
            {
                int sel = -1;
                for (int i = 0; i < n; i++)
                {
                    if (!used[i] && a[i * m + col] != 0)
                    {
                        sel = i;
                        break;
                    }
                }
                if (sel == -1) continue;
                used[sel] = true;
                rank++;
                for (int j = col; j < m; j++) a[sel * m + j] /= a[sel * m + col];
                for (int i = 0; i < n; i++)
                {
                    if (i != sel && a[i * m + col] != 0)
                    {
                        long factor = a[i * m + col];
                        for (int j = col; j < m; j++) a[i * m + j] -= factor * a[sel * m + j];
                    }
                }
            }
            return rank;
        }
    }

    public static unsafe class MatrixDeterminant
    {
        public static long Run(int n, long* a)
        {
            long det = 1;
            int sign = 1;
            for (int i = 0; i < n; i++)
            {
                int sel = i;
                for (int j = i + 1; j < n; j++)
                    if (Math.Abs(a[j * n + i]) > Math.Abs(a[sel * n + i])) sel = j;
                if (a[sel * n + i] == 0) return 0;
                if (sel != i)
                {
                    sign *= -1;
                    for (int j = 0; j < n; j++)
                    {
                        long tmp = a[i * n + j];
                        a[i * n + j] = a[sel * n + j];
                        a[sel * n + j] = tmp;
                    }
                }
                det *= a[i * n + i];
                for (int j = i + 1; j < n; j++) a[j * n + i] /= a[i * n + i];
                for (int j = i + 1; j < n; j++)
                    for (int k = i + 1; k < n; k++)
                        a[j * n + k] -= a[j * n + i] * a[i * n + k];
            }
            return det * sign;
        }
    }

    public static unsafe class MatrixInverse
    {
        public static bool Run(int n, long* a, long* inv)
        {
            long* aug = stackalloc long[n * n * 2];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++) aug[i * 2 * n + j] = a[i * n + j];
                for (int j = 0; j < n; j++) aug[i * 2 * n + n + j] = (i == j) ? 1 : 0;
            }
            for (int i = 0; i < n; i++)
            {
                int sel = i;
                for (int j = i; j < n; j++)
                    if (Math.Abs(aug[j * 2 * n + i]) > Math.Abs(aug[sel * 2 * n + i])) sel = j;
                if (aug[sel * 2 * n + i] == 0) return false;
                if (sel != i)
                {
                    for (int j = 0; j < 2 * n; j++)
                    {
                        long tmp = aug[i * 2 * n + j];
                        aug[i * 2 * n + j] = aug[sel * 2 * n + j];
                        aug[sel * 2 * n + j] = tmp;
                    }
                }
                long div = aug[i * 2 * n + i];
                for (int j = 0; j < 2 * n; j++) aug[i * 2 * n + j] /= div;
                for (int k = 0; k < n; k++)
                {
                    if (k == i) continue;
                    long factor = aug[k * 2 * n + i];
                    for (int j = 0; j < 2 * n; j++) aug[k * 2 * n + j] -= factor * aug[i * 2 * n + j];
                }
            }
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    inv[i * n + j] = aug[i * 2 * n + n + j];
            return true;
        }
    }

    public static unsafe class LinearSystemSolve
    {
        public static int Run(int n, int m, long* a, long* b, long* x)
        {
            for (int col = 0, row = 0; col < m && row < n; col++, row++)
            {
                int sel = row;
                for (int i = row; i < n; i++)
                    if (Math.Abs(a[i * m + col]) > Math.Abs(a[sel * m + col]))
                        sel = i;
                if (a[sel * m + col] == 0) continue;
                for (int j = 0; j < m; j++)
                {
                    long tmp = a[sel * m + j];
                    a[sel * m + j] = a[row * m + j];
                    a[row * m + j] = tmp;
                }
                long tb = b[sel]; b[sel] = b[row]; b[row] = tb;
                long div = a[row * m + col];
                for (int j = 0; j < m; j++) a[row * m + j] /= div;
                b[row] /= div;
                for (int i = 0; i < n; i++)
                {
                    if (i != row)
                    {
                        long factor = a[i * m + col];
                        for (int j = 0; j < m; j++) a[i * m + j] -= factor * a[row * m + j];
                        b[i] -= factor * b[row];
                    }
                }
            }
            for (int i = 0; i < n; i++) x[i] = b[i];
            return n;
        }
    }
}
