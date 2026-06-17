namespace IAFahim.Search.Numerical
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SimulatedAnnealing
    {
        public static double Run(double* state, int dim, double initialTemp, double coolingRate, int iterations, double* bestState, double* bestValue, uint* seed, delegate*<double*, int, double> eval)
        {
            double temp = initialTemp, currentVal = eval(state, dim);
            for (int i = 0; i < dim; i++) bestState[i] = state[i];
            *bestValue = currentVal;
            double* newState = stackalloc double[dim];
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = 0; i < dim; i++) newState[i] = state[i] + (Rand(seed) * 2 - 1) * temp;
                double newVal = eval(newState, dim), delta = newVal - currentVal;
                if (delta > 0 || Rand(seed) < Math.Exp(delta / temp))
                {
                    for (int i = 0; i < dim; i++) state[i] = newState[i];
                    currentVal = newVal;
                    if (currentVal > *bestValue) { for (int i = 0; i < dim; i++) bestState[i] = state[i]; *bestValue = currentVal; }
                }
                temp *= coolingRate;
            }
            return *bestValue;
        }
        private static double Rand(uint* seed) { *seed ^= *seed << 13; *seed ^= *seed >> 17; *seed ^= *seed << 5; return (double)*seed / uint.MaxValue; }
    }

    public static unsafe class TernaryReal
    {
        public static double Run(double* func, int maxIter, double lo, double hi)
        {
            for (int iter = 0; iter < maxIter; iter++)
            {
                if (hi - lo < 1e-9) break;
                double m1 = lo + (hi - lo) / 3, m2 = hi - (hi - lo) / 3;
                if (Evaluate(func, m1) < Evaluate(func, m2)) hi = m2; else lo = m1;
            }
            return (lo + hi) / 2;
        }
        private static double Evaluate(double* f, double x) => f[0] * x + f[1];
    }

    public static unsafe class AdaptiveSimpson
    {
        public static double Run(double* func, double a, double b, double tol, int maxDepth) => AdaptiveSimpsonRecursive(func, a, b, tol, maxDepth, Evaluate(func, (a + b) / 2));

        private static double AdaptiveSimpsonRecursive(double* f, double a, double b, double tol, int depth, double fMid)
        {
            double mid = (a + b) / 2;
            double fa = Evaluate(f, a), fb = Evaluate(f, b);
            double whole = (b - a) * (fa + 4 * fMid + fb) / 6;
            if (depth <= 0) return whole;
            double fLeft = Evaluate(f, (a + mid) / 2), fRight = Evaluate(f, (mid + b) / 2);
            double left = (b - a) * (fa + 4 * fLeft + fMid) / 12;
            double right = (b - a) * (fMid + 4 * fRight + fb) / 12;
            if (Math.Abs(left + right - whole) < 15 * tol) return left + right;
            return AdaptiveSimpsonRecursive(f, a, mid, tol / 2, depth - 1, fLeft) + AdaptiveSimpsonRecursive(f, mid, b, tol / 2, depth - 1, fRight);
        }
        private static double Evaluate(double* f, double x) => f[0] * x * x + f[1] * x + f[2];
    }

    public static unsafe class SimpsonIntegral
    {
        public static double Run(double* func, double a, double b, int n)
        {
            if ((n & 1) == 1) n++;
            double h = (b - a) / n, sum = Evaluate(func, a) + Evaluate(func, b);
            for (int i = 1; i < n; i++) sum += (i % 2 == 0) ? 2 * Evaluate(func, a + i * h) : 4 * Evaluate(func, a + i * h);
            return sum * h / 3;
        }
        private static double Evaluate(double* f, double x) => f[0] * x * x + f[1] * x + f[2];
    }

    public static unsafe class GaussLegendre
    {
        public static double Run(int n, double a, double b, double* func)
        {
            double* x = stackalloc double[n], w = stackalloc double[n]; ComputeNodesWeights(n, x, w);
            double sum = 0; for (int i = 0; i < n; i++) sum += w[i] * Evaluate(func, (a + b) / 2 + (b - a) / 2 * x[i]);
            return sum * (b - a) / 2;
        }
        private static void ComputeNodesWeights(int n, double* x, double* w)
        {
            const double eps = 1e-14;
            for (int i = 1; i <= n; i++)
            {
                double root = Math.Cos(Math.PI * (i - 0.25) / (n + 0.5));
                double pder;
                x[n - i] = RefineRoot(n, root, eps, out pder);
                double xi = x[n - i];
                w[n - i] = 2.0 / ((1.0 - xi * xi) * pder * pder);
            }
        }
        private static double RefineRoot(int n, double x0, double eps, out double pder)
        {
            pder = 0;
            for (int j = 0; j < 50; j++)
            {
                double p = 1, p1 = 1, p2 = 0; for (int k = 0; k < n; k++) { p2 = p1; p1 = p; p = ((2 * k + 1) * x0 * p1 - k * p2) / (k + 1); }
                pder = n * (x0 * p - p1) / (x0 * x0 - 1); double dx = p / pder; x0 -= dx; if (Math.Abs(dx) < eps) break;
            }
            return x0;
        }
        private static double Evaluate(double* f, double x) => f[0] * x * x + f[1] * x + f[2];
    }
}
