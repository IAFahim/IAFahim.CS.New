namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaximumInscribedCircle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Cross(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeNormals(double* xs, double* ys, int n, double* nx, double* ny, double* c)
        {
            for (int i = 0; i < n; i++)
            {
                int nxt = (i + 1) % n;
                double dx = xs[nxt] - xs[i];
                double dy = ys[nxt] - ys[i];
                double len = Math.Sqrt(dx * dx + dy * dy);
                nx[i] = -dy / len;
                ny[i] = dx / len;
                c[i] = xs[i] * nx[i] + ys[i] * ny[i];
            }
        }

        private static bool IsFeasible(double mid, int n, double* nx, double* ny, double* c, int* q)
        {
            int head = 0, tail = 0;
            q[tail++] = 0;
            q[tail++] = 1;
            for (int i = 2; i < n; i++)
            {
                while (head + 1 < tail)
                {
                    int prev = q[tail - 2];
                    int curr = q[tail - 1];
                    double px_curr, py_curr;
                    Intersect(nx[prev], ny[prev], c[prev] + mid, nx[curr], ny[curr], c[curr] + mid, out px_curr, out py_curr);
                    if (nx[i] * px_curr + ny[i] * py_curr > c[i] + mid - 1e-9) tail--;
                    else break;
                }
                while (head + 1 < tail)
                {
                    int prev = q[head];
                    int curr = q[head + 1];
                    double px_curr, py_curr;
                    Intersect(nx[prev], ny[prev], c[prev] + mid, nx[curr], ny[curr], c[curr] + mid, out px_curr, out py_curr);
                    if (nx[i] * px_curr + ny[i] * py_curr > c[i] + mid - 1e-9) head++;
                    else break;
                }
                q[tail++] = i;
            }
            while (head + 1 < tail)
            {
                int prev = q[tail - 2];
                int curr = q[tail - 1];
                int first = q[head];
                double px_curr, py_curr;
                Intersect(nx[prev], ny[prev], c[prev] + mid, nx[curr], ny[curr], c[curr] + mid, out px_curr, out py_curr);
                if (nx[first] * px_curr + ny[first] * py_curr > c[first] + mid - 1e-9) tail--;
                else break;
            }
            return tail - head >= 3;
        }

        public static double Run(double* xs, double* ys, int n)
        {
            if (n < 3) return 0;
            double lo = 0, hi = 2e9;
            double* nx = stackalloc double[n];
            double* ny = stackalloc double[n];
            double* c = stackalloc double[n];
            int* q = stackalloc int[n + 5];
            ComputeNormals(xs, ys, n, nx, ny, c);
            for (int iter = 0; iter < 60; iter++)
            {
                double mid = (lo + hi) / 2;
                if (IsFeasible(mid, n, nx, ny, c, q)) lo = mid;
                else hi = mid;
            }
            return lo;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Intersect(double nx1, double ny1, double c1, double nx2, double ny2, double c2, out double x, out double y)
        {
            double det = nx1 * ny2 - ny1 * nx2;
            if (Math.Abs(det) < 1e-12) { x = y = 0; return; }
            x = (c1 * ny2 - c2 * ny1) / det;
            y = (nx1 * c2 - nx2 * c1) / det;
        }
    }
}
