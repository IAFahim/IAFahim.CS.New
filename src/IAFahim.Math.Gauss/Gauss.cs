namespace IAFahim.Math.Gauss
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GaussEliminationDouble
    {
        private const double Epsilon = 1e-9;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SelectPivot(double* a, int n, int m, int row, int col)
        {
            int sel = row;
            for (int i = row; i < n; i++) if (Math.Abs(a[i * m + col]) > Math.Abs(a[sel * m + col])) sel = i;
            return sel;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapRows(double* a, double* b, int m, int sel, int row)
        {
            for (int j = 0; j < m; j++) { double t = a[sel * m + j]; a[sel * m + j] = a[row * m + j]; a[row * m + j] = t; }
            { double t = b[sel]; b[sel] = b[row]; b[row] = t; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void NormalizeRow(double* a, double* b, int m, int row, int col)
        {
            double div = a[row * m + col];
            for (int j = col; j < m; j++) a[row * m + j] /= div;
            b[row] /= div;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EliminateRow(double* a, double* b, int m, int row, int col, int i)
        {
            if (i == row) return;
            double coef = a[i * m + col];
            if (Math.Abs(coef) < Epsilon) return;
            for (int j = col; j < m; j++) a[i * m + j] -= a[row * m + j] * coef;
            b[i] -= b[row] * coef;
        }

        public static int Run(double* a, double* b, double* x, int n, int m)
        {
            int* where = stackalloc int[m];
            for (int i = 0; i < m; i++) where[i] = -1;
            int row = 0;
            for (int col = 0; col < m && row < n; col++)
            {
                int sel = SelectPivot(a, n, m, row, col);
                if (Math.Abs(a[sel * m + col]) < Epsilon) continue;
                SwapRows(a, b, m, sel, row);
                where[col] = row;
                NormalizeRow(a, b, m, row, col);
                for (int i = 0; i < n; i++) EliminateRow(a, b, m, row, col, i);
                row++;
            }
            for (int i = 0; i < m; i++) x[i] = 0;
            for (int i = 0; i < m; i++) if (where[i] != -1) x[i] = b[where[i]];
            return row;
        }
    }

    public static unsafe class GaussModP
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ModPow(long a, long e, long mod)
        {
            long r = 1;
            while (e > 0) { if ((e & 1) == 1) r = r * a % mod; a = a * a % mod; e >>= 1; }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ModInv(long a, long mod) => ModPow((a % mod + mod) % mod, mod - 2, mod);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindNonZeroPivot(long* a, int n, int stride, int start, int col, long mod)
        {
            for (int i = start; i < n; i++) if ((a[i * stride + col] % mod + mod) % mod != 0) return i;
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapRows(long* a, long* b, int m, int sel, int row)
        {
            for (int j = 0; j < m; j++) { long t = a[sel * m + j]; a[sel * m + j] = a[row * m + j]; a[row * m + j] = t; }
            { long t = b[sel]; b[sel] = b[row]; b[row] = t; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void NormalizeRow(long* a, long* b, int m, int row, int col, long mod)
        {
            long inv = ModInv(a[row * m + col], mod);
            for (int j = col; j < m; j++) a[row * m + j] = a[row * m + j] * inv % mod;
            b[row] = b[row] * inv % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EliminateRow(long* a, long* b, int m, int row, int col, int i, long mod)
        {
            if (i == row) return;
            long coef = (a[i * m + col] % mod + mod) % mod;
            if (coef == 0) return;
            for (int j = col; j < m; j++) a[i * m + j] = (a[i * m + j] - coef * a[row * m + j]) % mod;
            b[i] = (b[i] - coef * b[row]) % mod;
        }

        public static bool Run(long* a, long* b, long* x, int n, int m, long mod)
        {
            int* where = stackalloc int[m];
            for (int i = 0; i < m; i++) where[i] = -1;
            int row = 0;
            for (int col = 0; col < m && row < n; col++)
            {
                int sel = FindNonZeroPivot(a, n, m, row, col, mod);
                if (sel == -1) continue;
                if (sel != row) SwapRows(a, b, m, sel, row);
                where[col] = row;
                NormalizeRow(a, b, m, row, col, mod);
                for (int i = 0; i < n; i++) EliminateRow(a, b, m, row, col, i, mod);
                row++;
            }
            for (int i = 0; i < m; i++) x[i] = 0;
            for (int i = 0; i < m; i++) x[i] = where[i] != -1 ? ((b[where[i]] % mod + mod) % mod) : 0;
            for (int i = row; i < n; i++) if ((b[i] % mod + mod) % mod != 0) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapRowsSquare(long* a, int n, int sel, int row)
        {
            for (int j = 0; j < n; j++) { long t = a[sel * n + j]; a[sel * n + j] = a[row * n + j]; a[row * n + j] = t; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EliminateRowBelow(long* a, int n, int i, int k, long mod)
        {
            long coef = (a[k * n + i] % mod + mod) % mod;
            if (coef == 0) return;
            for (int j = i; j < n; j++) a[k * n + j] = (a[k * n + j] - coef * a[i * n + j]) % mod;
        }

        public static long Determinant(long* a, int n, long mod)
        {
            long det = 1;
            for (int i = 0; i < n; i++)
            {
                int sel = FindNonZeroPivot(a, n, n, i, i, mod);
                if (sel == -1) return 0;
                if (sel != i)
                {
                    SwapRowsSquare(a, n, sel, i);
                    det = mod - det;
                }
                det = det * ((a[i * n + i] % mod + mod) % mod) % mod;
                long inv = ModInv(a[i * n + i], mod);
                for (int j = i; j < n; j++) a[i * n + j] = a[i * n + j] * inv % mod;
                for (int k = i + 1; k < n; k++) EliminateRowBelow(a, n, i, k, mod);
            }
            return (det % mod + mod) % mod;
        }
    }
}
