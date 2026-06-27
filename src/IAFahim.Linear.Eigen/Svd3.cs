namespace IAFahim.Linear.Eigen
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Svd3
    {
        private const double Eps = 1e-12;

        public static void Run(double* a, double* u, double* sigma, double* v)
        {
            double* b = stackalloc double[9];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    double s = 0;
                    for (int k = 0; k < 3; k++) s += a[k * 3 + i] * a[k * 3 + j];
                    b[i * 3 + j] = s;
                }

            double* vals = stackalloc double[3];
            double* vecs = stackalloc double[9];
            SymmetricEigen3.Run(b, vals, vecs);

            for (int i = 0; i < 3; i++)
            {
                sigma[i] = Math.Sqrt(vals[i] < 0 ? 0 : vals[i]);
                for (int r = 0; r < 3; r++) v[r * 3 + i] = vecs[r * 3 + i];
            }

            int valid = 0;
            int[] validIdx = { -1, -1, -1 };
            for (int i = 0; i < 3; i++)
            {
                if (sigma[i] > Eps)
                {
                    validIdx[valid++] = i;
                    for (int r = 0; r < 3; r++)
                    {
                        double s = 0;
                        for (int k = 0; k < 3; k++) s += a[r * 3 + k] * v[k * 3 + i];
                        u[r * 3 + i] = s / sigma[i];
                    }
                }
            }

            if (valid == 0)
            {
                u[0] = 1; u[1] = 0; u[2] = 0;
                u[3] = 0; u[4] = 1; u[5] = 0;
                u[6] = 0; u[7] = 0; u[8] = 1;
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                if (sigma[i] > Eps) continue;
                CompleteColumn(u, validIdx, valid, i);
                validIdx[valid++] = i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CompleteColumn(double* u, int[] valid, int count, int target)
        {
            if (count >= 2)
            {
                double ax = u[valid[0]], ay = u[3 + valid[0]], az = u[6 + valid[0]];
                double bx = u[valid[1]], by = u[3 + valid[1]], bz = u[6 + valid[1]];
                double cx = ay * bz - az * by;
                double cy = az * bx - ax * bz;
                double cz = ax * by - ay * bx;
                double n = Math.Sqrt(cx * cx + cy * cy + cz * cz);
                if (n < Eps) { FillOrthogonal(u, valid, target, 0); return; }
                u[target] = cx / n; u[3 + target] = cy / n; u[6 + target] = cz / n;
            }
            else
            {
                FillOrthogonal(u, valid, target, Math.Abs(u[valid[0]]) < 0.9 ? 0 : 1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FillOrthogonal(double* u, int[] valid, int target, int seedAxis)
        {
            double ax = u[valid[0]], ay = u[3 + valid[0]], az = u[6 + valid[0]];
            double bx = seedAxis == 0 ? 1.0 : 0.0;
            double by = seedAxis == 0 ? 0.0 : 1.0;
            double bz = 0.0;
            double dot = ax * bx + ay * by + az * bz;
            bx -= ax * dot; by -= ay * dot; bz -= az * dot;
            double n = Math.Sqrt(bx * bx + by * by + bz * bz);
            if (n < Eps) { FillOrthogonal(u, valid, target, seedAxis == 0 ? 1 : 2); return; }
            u[target] = bx / n; u[3 + target] = by / n; u[6 + target] = bz / n;
        }
    }
}
