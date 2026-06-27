namespace IAFahim.Linear.Matrix
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class MatrixMul
    {
        public static void Run(int n, int m, int p, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < p; j++) c[i * p + j] = ComputeDotProduct(i, j, m, p, a, b);
        }

        private static long ComputeDotProduct(int i, int j, int m, int p, long* a, long* b)
        {
            long sum = 0;
            for (int k = 0; k < m; k++) sum += a[i * m + k] * b[k * p + j];
            return sum;
        }
    }

    public static unsafe class MatrixPow
    {
        public static void Run(int n, long* a, long* result, long exp)
        {
            InitializeIdentity(n, result);
            long* temp = (long*)Marshal.AllocHGlobal((IntPtr)((long)n * n * sizeof(long)));
            Buffer.MemoryCopy(a, temp, (long)n * n * sizeof(long), (long)n * n * sizeof(long));

            while (exp > 0)
            {
                if ((exp & 1) == 1) MultiplyInto(n, result, temp);
                if (exp > 1) MultiplyInto(n, temp, temp);
                exp >>= 1;
            }
            Marshal.FreeHGlobal((IntPtr)temp);
        }

        private static void InitializeIdentity(int n, long* res)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++) res[i * n + j] = (i == j) ? 1 : 0;
        }

        private static void MultiplyInto(int n, long* a, long* b)
        {
            long* res = (long*)Marshal.AllocHGlobal((IntPtr)((long)n * n * sizeof(long)));
            MatrixMul.Run(n, n, n, a, b, res);
            Buffer.MemoryCopy(res, a, (long)n * n * sizeof(long), (long)n * n * sizeof(long));
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }

    public static unsafe class GaussianElimination
    {
        public static int Run(int n, int m, long* a, long* b, long* x)
        {
            for (int i = 0; i < Math.Min(n, m); i++)
            {
                int sel = FindPivot(n, m, i, a);
                if (a[sel * m + i] == 0) continue;
                SwapRows(n, m, i, sel, a, b);
                Eliminate(n, m, i, a, b);
            }
            for (int i = 0; i < n; i++) x[i] = b[i];
            return n;
        }

        private static int FindPivot(int n, int m, int i, long* a)
        {
            int sel = i;
            for (int k = i + 1; k < n; k++)
                if (Math.Abs(a[k * m + i]) > Math.Abs(a[sel * m + i])) sel = k;
            return sel;
        }

        private static void SwapRows(int n, int m, int r1, int r2, long* a, long* b)
        {
            if (r1 == r2) return;
            for (int j = 0; j < m; j++) { long t = a[r1 * m + j]; a[r1 * m + j] = a[r2 * m + j]; a[r2 * m + j] = t; }
            long tb = b[r1]; b[r1] = b[r2]; b[r2] = tb;
        }

        private static void Eliminate(int n, int m, int row, long* a, long* b)
        {
            long div = a[row * m + row];
            for (int j = row; j < m; j++) a[row * m + j] /= div;
            b[row] /= div;
            for (int i = 0; i < n; i++)
            {
                if (i == row) continue;
                long factor = a[i * m + row];
                for (int j = row; j < m; j++) a[i * m + j] -= factor * a[row * m + j];
                b[i] -= factor * b[row];
            }
        }
    }

    public static unsafe class MatrixDeterminant
    {
        public static long Run(int n, long* a)
        {
            if (n == 0) return 1;
            long sign = 1, prevPivot = 1;
            for (int k = 0; k < n; k++)
            {
                int pr = FindPivot(n, k, a);
                if (a[pr * n + k] == 0) return 0;
                if (pr != k) { sign = -sign; SwapRows(n, k, pr, a); }
                long pivot = a[k * n + k];
                PerformElimination(n, k, pivot, prevPivot, a);
                prevPivot = pivot;
            }
            return a[(n - 1) * n + (n - 1)] * sign;
        }

        private static int FindPivot(int n, int k, long* a)
        {
            int pr = k;
            for (int i = k + 1; i < n; i++) if (Math.Abs(a[i * n + k]) > Math.Abs(a[pr * n + k])) pr = i;
            return pr;
        }

        private static void SwapRows(int n, int r1, int r2, long* a)
        {
            for (int j = 0; j < n; j++) { long t = a[r1 * n + j]; a[r1 * n + j] = a[r2 * n + j]; a[r2 * n + j] = t; }
        }

        private static void PerformElimination(int n, int k, long pivot, long prevPivot, long* a)
        {
            for (int i = k + 1; i < n; i++)
                for (int j = k + 1; j < n; j++) a[i * n + j] = (a[i * n + j] * pivot - a[i * n + k] * a[k * n + j]) / prevPivot;
        }
    }

    public static unsafe class MatrixInverse
    {
        public static bool Run(int n, long* a, long* inv)
        {
            long* aug = (long*)Marshal.AllocHGlobal((IntPtr)((long)n * n * 2 * sizeof(long)));
            InitializeAugmented(n, a, aug);
            for (int i = 0; i < n; i++)
            {
                int sel = FindPivot(n, i, aug);
                if (aug[sel * 2 * n + i] == 0) { Marshal.FreeHGlobal((IntPtr)aug); return false; }
                SwapRowsAug(n, i, sel, aug);
                EliminateAug(n, i, aug);
            }
            ExtractInverse(n, aug, inv);
            Marshal.FreeHGlobal((IntPtr)aug);
            return true;
        }

        private static void InitializeAugmented(int n, long* a, long* aug)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++) aug[i * 2 * n + j] = a[i * n + j];
                for (int j = 0; j < n; j++) aug[i * 2 * n + n + j] = (i == j) ? 1 : 0;
            }
        }

        private static int FindPivot(int n, int i, long* aug)
        {
            int sel = i;
            for (int j = i + 1; j < n; j++) if (Math.Abs(aug[j * 2 * n + i]) > Math.Abs(aug[sel * 2 * n + i])) sel = j;
            return sel;
        }

        private static void SwapRowsAug(int n, int r1, int r2, long* aug)
        {
            if (r1 == r2) return;
            for (int j = 0; j < 2 * n; j++) { long t = aug[r1 * 2 * n + j]; aug[r1 * 2 * n + j] = aug[r2 * 2 * n + j]; aug[r2 * 2 * n + j] = t; }
        }

        private static void EliminateAug(int n, int row, long* aug)
        {
            long div = aug[row * 2 * n + row];
            for (int j = 0; j < 2 * n; j++) aug[row * 2 * n + j] /= div;
            for (int k = 0; k < n; k++)
            {
                if (k == row) continue;
                long factor = aug[k * 2 * n + row];
                for (int j = 0; j < 2 * n; j++) aug[k * 2 * n + j] -= factor * aug[row * 2 * n + j];
            }
        }

        private static void ExtractInverse(int n, long* aug, long* inv)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++) inv[i * n + j] = aug[i * 2 * n + n + j];
        }
    }
}
