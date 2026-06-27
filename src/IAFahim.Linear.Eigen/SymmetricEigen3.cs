namespace IAFahim.Linear.Eigen
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SymmetricEigen3
    {
        private const double Eps = 1e-15;
        private const int MaxSweeps = 60;

        public static int Run(double* a, double* values, double* vectors)
        {
            double* m = stackalloc double[9];
            double* v = stackalloc double[9];
            for (int i = 0; i < 9; i++) { m[i] = a[i]; v[i] = 0; }
            v[0] = 1; v[4] = 1; v[8] = 1;

            int sweep;
            for (sweep = 0; sweep < MaxSweeps; sweep++)
            {
                double off = Math.Abs(m[1]) + Math.Abs(m[2]) + Math.Abs(m[5]);
                double diag = Math.Abs(m[0]) + Math.Abs(m[4]) + Math.Abs(m[8]);
                if (off <= Eps * (diag + Eps)) break;
                Rotate(m, v, 0, 1);
                Rotate(m, v, 0, 2);
                Rotate(m, v, 1, 2);
            }

            int[] idx = { 0, 1, 2 };
            for (int i = 0; i < 3; i++)
                for (int j = i + 1; j < 3; j++)
                    if (m[idx[i] * 3 + idx[i]] > m[idx[j] * 3 + idx[j]]) { int t = idx[i]; idx[i] = idx[j]; idx[j] = t; }
            for (int i = 0; i < 3; i++)
            {
                int e = idx[i];
                values[i] = m[e * 3 + e];
                for (int r = 0; r < 3; r++) vectors[r * 3 + i] = v[r * 3 + e];
            }
            return sweep;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Rotate(double* m, double* v, int p, int q)
        {
            double app = m[p * 3 + p];
            double aqq = m[q * 3 + q];
            double apq = m[p * 3 + q];
            if (Math.Abs(apq) <= double.Epsilon) return;
            double tau = (aqq - app) / (2.0 * apq);
            double t = tau >= 0 ? 1.0 / (tau + Math.Sqrt(1.0 + tau * tau)) : -1.0 / (-tau + Math.Sqrt(1.0 + tau * tau));
            double c = 1.0 / Math.Sqrt(1.0 + t * t);
            double s = t * c;

            m[p * 3 + p] = app - t * apq;
            m[q * 3 + q] = aqq + t * apq;
            m[p * 3 + q] = 0;
            m[q * 3 + p] = 0;
            for (int r = 0; r < 3; r++)
            {
                if (r == p || r == q) continue;
                double mrp = m[r * 3 + p];
                double mrq = m[r * 3 + q];
                m[r * 3 + p] = c * mrp - s * mrq;
                m[p * 3 + r] = m[r * 3 + p];
                m[r * 3 + q] = s * mrp + c * mrq;
                m[q * 3 + r] = m[r * 3 + q];
            }
            for (int r = 0; r < 3; r++)
            {
                double vrp = v[r * 3 + p];
                double vrq = v[r * 3 + q];
                v[r * 3 + p] = c * vrp - s * vrq;
                v[r * 3 + q] = s * vrp + c * vrq;
            }
        }
    }
}
