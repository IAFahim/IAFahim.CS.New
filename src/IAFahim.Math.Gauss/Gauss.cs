namespace IAFahim.Math.Gauss
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GaussEliminationDouble
    {
        public static int Run(double* a, double* b, double* x, int n, int m)
        {
            int* where = stackalloc int[m];
            for (int i = 0; i < m; i++) where[i] = -1;
            int row = 0;
            for (int col = 0; col < m && row < n; col++)
            {
                int sel = row;
                for (int i = row; i < n; i++) if (Math.Abs(a[i * m + col]) > Math.Abs(a[sel * m + col])) sel = i;
                if (Math.Abs(a[sel * m + col]) < 1e-9) continue;
                for (int j = 0; j < m; j++) { double t = a[sel * m + j]; a[sel * m + j] = a[row * m + j]; a[row * m + j] = t; }
                { double t = b[sel]; b[sel] = b[row]; b[row] = t; }
                where[col] = row;
                double div = a[row * m + col];
                for (int j = col; j < m; j++) a[row * m + j] /= div;
                b[row] /= div;
                for (int i = 0; i < n; i++)
                {
                    if (i == row) continue;
                    double coef = a[i * m + col];
                    if (Math.Abs(coef) < 1e-9) continue;
                    for (int j = col; j < m; j++) a[i * m + j] -= a[row * m + j] * coef;
                    b[i] -= b[row] * coef;
                }
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

        public static bool Run(long* a, long* b, long* x, int n, int m, long mod)
        {
            int* where = stackalloc int[m];
            for (int i = 0; i < m; i++) where[i] = -1;
            int row = 0;
            for (int col = 0; col < m && row < n; col++)
            {
                int sel = -1;
                for (int i = row; i < n; i++) if ((a[i * m + col] % mod + mod) % mod != 0) { sel = i; break; }
                if (sel == -1) continue;
                if (sel != row)
                {
                    for (int j = 0; j < m; j++) { long t = a[sel * m + j]; a[sel * m + j] = a[row * m + j]; a[row * m + j] = t; }
                    { long t = b[sel]; b[sel] = b[row]; b[row] = t; }
                }
                where[col] = row;
                long inv = ModInv(a[row * m + col], mod);
                for (int j = col; j < m; j++) a[row * m + j] = a[row * m + j] * inv % mod;
                b[row] = b[row] * inv % mod;
                for (int i = 0; i < n; i++)
                {
                    if (i == row) continue;
                    long coef = (a[i * m + col] % mod + mod) % mod;
                    if (coef == 0) continue;
                    for (int j = col; j < m; j++) a[i * m + j] = (a[i * m + j] - coef * a[row * m + j]) % mod;
                    b[i] = (b[i] - coef * b[row]) % mod;
                }
                row++;
            }
            for (int i = 0; i < m; i++) x[i] = 0;
            for (int i = 0; i < m; i++) x[i] = where[i] != -1 ? ((b[where[i]] % mod + mod) % mod) : 0;
            for (int i = row; i < n; i++) if ((b[i] % mod + mod) % mod != 0) return false;
            return true;
        }

        public static long Determinant(long* a, int n, long mod)
        {
            long det = 1;
            for (int i = 0; i < n; i++)
            {
                int sel = -1;
                for (int j = i; j < n; j++) if ((a[j * n + i] % mod + mod) % mod != 0) { sel = j; break; }
                if (sel == -1) return 0;
                if (sel != i)
                {
                    for (int j = 0; j < n; j++) { long t = a[sel * n + j]; a[sel * n + j] = a[i * n + j]; a[i * n + j] = t; }
                    det = mod - det;
                }
                det = det * ((a[i * n + i] % mod + mod) % mod) % mod;
                long inv = ModInv(a[i * n + i], mod);
                for (int j = i; j < n; j++) a[i * n + j] = a[i * n + j] * inv % mod;
                for (int k = i + 1; k < n; k++)
                {
                    long coef = (a[k * n + i] % mod + mod) % mod;
                    if (coef == 0) continue;
                    for (int j = i; j < n; j++) a[k * n + j] = (a[k * n + j] - coef * a[i * n + j]) % mod;
                }
            }
            return (det % mod + mod) % mod;
        }
    }
}
