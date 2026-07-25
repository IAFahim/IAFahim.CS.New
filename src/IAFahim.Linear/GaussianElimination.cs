namespace IAFahim.Linear
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GaussianElimination
    {
        // Solve A x = b for n x n system. A is row-major n*n, destroyed in place.
        // Returns true if unique solution written to x; false if singular.
        public static bool Solve(double* a, double* b, double* x, int n)
        {
            if (n <= 0) return false;
            const double Eps = 1e-12;

            for (int col = 0; col < n; col++)
            {
                int piv = col;
                double best = Math.Abs(a[col * n + col]);
                for (int r = col + 1; r < n; r++)
                {
                    double v = Math.Abs(a[r * n + col]);
                    if (v > best) { best = v; piv = r; }
                }
                if (best < Eps) return false;
                if (piv != col)
                {
                    for (int c = col; c < n; c++)
                    {
                        double t = a[col * n + c]; a[col * n + c] = a[piv * n + c]; a[piv * n + c] = t;
                    }
                    double tb = b[col]; b[col] = b[piv]; b[piv] = tb;
                }
                double diag = a[col * n + col];
                for (int r = col + 1; r < n; r++)
                {
                    double f = a[r * n + col] / diag;
                    if (Math.Abs(f) < Eps) continue;
                    for (int c = col; c < n; c++) a[r * n + c] -= f * a[col * n + c];
                    b[r] -= f * b[col];
                }
            }

            for (int i = n - 1; i >= 0; i--)
            {
                double s = b[i];
                for (int j = i + 1; j < n; j++) s -= a[i * n + j] * x[j];
                if (Math.Abs(a[i * n + i]) < Eps) return false;
                x[i] = s / a[i * n + i];
            }
            return true;
        }

        // Determinant of n x n matrix (row-major). Matrix is destroyed.
        public static double Determinant(double* a, int n)
        {
            if (n <= 0) return 0;
            const double Eps = 1e-12;
            double det = 1;
            int sign = 1;
            for (int col = 0; col < n; col++)
            {
                int piv = col;
                double best = Math.Abs(a[col * n + col]);
                for (int r = col + 1; r < n; r++)
                {
                    double v = Math.Abs(a[r * n + col]);
                    if (v > best) { best = v; piv = r; }
                }
                if (best < Eps) return 0;
                if (piv != col)
                {
                    for (int c = col; c < n; c++)
                    {
                        double t = a[col * n + c]; a[col * n + c] = a[piv * n + c]; a[piv * n + c] = t;
                    }
                    sign = -sign;
                }
                double diag = a[col * n + col];
                det *= diag;
                for (int r = col + 1; r < n; r++)
                {
                    double f = a[r * n + col] / diag;
                    for (int c = col; c < n; c++) a[r * n + c] -= f * a[col * n + c];
                }
            }
            return sign * det;
        }
    }
}
