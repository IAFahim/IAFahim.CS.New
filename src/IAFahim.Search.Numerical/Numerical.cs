namespace IAFahim.Search.Numerical
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SimulatedAnnealing
    {
        public static double Run(double* state, int dim, double initialTemp, double coolingRate, int iterations, double* bestState, double* bestValue, delegate*<double*, int, double> eval)
        {
            double temp = initialTemp;
            double currentVal = eval(state, dim);
            for (int i = 0; i < dim; i++) bestState[i] = state[i];
            *bestValue = currentVal;
            for (int iter = 0; iter < iterations; iter++)
            {
                double* newState = stackalloc double[dim];
                for (int i = 0; i < dim; i++)
                    newState[i] = state[i] + (Rand() * 2 - 1) * temp;
                double newVal = eval(newState, dim);
                double delta = newVal - currentVal;
                if (delta > 0 || Rand() < Math.Exp(delta / temp))
                {
                    for (int i = 0; i < dim; i++) state[i] = newState[i];
                    currentVal = newVal;
                    if (currentVal > *bestValue)
                    {
                        for (int i = 0; i < dim; i++) bestState[i] = state[i];
                        *bestValue = currentVal;
                    }
                }
                temp *= coolingRate;
            }
            return *bestValue;
        }

        private static double Rand()
        {
            return new Random().NextDouble();
        }
    }

    public static unsafe class TernaryReal
    {
        public static double Run(double* func, int maxIter, double lo, double hi)
        {
            for (int iter = 0; iter < maxIter; iter++)
            {
                if (hi - lo < 1e-9) break;
                double m1 = lo + (hi - lo) / 3;
                double m2 = hi - (hi - lo) / 3;
                double f1 = func[0] * m1 + func[1];
                double f2 = func[0] * m2 + func[1];
                if (f1 < f2) hi = m2;
                else lo = m1;
            }
            return (lo + hi) / 2;
        }
    }

    public static unsafe class GradientStep
    {
        public static double Run(double* x, int dim, double learningRate, int iterations, double* gradient, double* best)
        {
            for (int i = 0; i < dim; i++) best[i] = x[i];
            double bestVal = Eval(best, dim);
            for (int iter = 0; iter < iterations; iter++)
            {
                ComputeGradient(x, dim, gradient);
                for (int i = 0; i < dim; i++)
                    x[i] -= learningRate * gradient[i];
                double val = Eval(x, dim);
                if (val < bestVal)
                {
                    for (int i = 0; i < dim; i++) best[i] = x[i];
                    bestVal = val;
                }
            }
            return bestVal;
        }

        private static void ComputeGradient(double* x, int dim, double* grad)
        {
            double eps = 1e-8;
            for (int i = 0; i < dim; i++)
            {
                double orig = x[i];
                x[i] = orig + eps;
                double f2 = Eval(x, dim);
                x[i] = orig - eps;
                double f1 = Eval(x, dim);
                x[i] = orig;
                grad[i] = (f2 - f1) / (2 * eps);
            }
        }

        private static double Eval(double* x, int dim)
        {
            double sum = 0;
            for (int i = 0; i < dim; i++) sum += x[i] * x[i];
            return sum;
        }
    }

    public static unsafe class NewtonSolve
    {
        public static double Run(double* func, int maxIter, double x0, double* derivative)
        {
            double x = x0;
            for (int i = 0; i < maxIter; i++)
            {
                double f = func[0] * x * x + func[1] * x + func[2];
                double df = derivative[0] * x + derivative[1];
                if (Math.Abs(df) < 1e-12) break;
                x -= f / df;
            }
            return x;
        }
    }

    public static unsafe class BisectionReal
    {
        public static double Run(double* func, double lo, double hi, double tol, int maxIter)
        {
            for (int iter = 0; iter < maxIter; iter++)
            {
                if (hi - lo < tol) break;
                double mid = (lo + hi) / 2;
                double fMid = func[0] * mid + func[1];
                double fLo = func[0] * lo + func[1];
                if (fMid * fLo < 0) hi = mid;
                else lo = mid;
            }
            return (lo + hi) / 2;
        }
    }

    public static unsafe class AdaptiveSimpson
    {
        public static double Run(double* func, double a, double b, double tol, int maxDepth)
        {
            return AdaptiveSimpsonRecursive(func, a, b, tol, maxDepth, Evaluate(func, (a + b) / 2));
        }

        private static double AdaptiveSimpsonRecursive(double* f, double a, double b, double tol, int depth, double fMid)
        {
            if (depth <= 0) return (b - a) * (Evaluate(f, a) + 4 * fMid + Evaluate(f, b)) / 6;
            double fLeft = Evaluate(f, (a + b) / 2);
            double fRight = Evaluate(f, b);
            double left = (b - a) * (Evaluate(f, a) + 4 * fLeft + fMid) / 6;
            double right = (b - a) * (fMid + 4 * fLeft + Evaluate(f, b)) / 6;
            if (Math.Abs(left + right - Evaluate(f, a) - 4 * fMid - Evaluate(f, b)) < 15 * tol)
                return left + right;
            return AdaptiveSimpsonRecursive(f, a, (a + b) / 2, tol / 2, depth - 1, fLeft) +
                   AdaptiveSimpsonRecursive(f, (a + b) / 2, b, tol / 2, depth - 1, fRight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Evaluate(double* f, double x)
        {
            return f[0] * x * x + f[1] * x + f[2];
        }
    }

    public static unsafe class SimpsonIntegral
    {
        public static double Run(double* func, double a, double b, int n)
        {
            if ((n & 1) == 1) n++;
            double h = (b - a) / n;
            double sum = Evaluate(func, a) + Evaluate(func, b);
            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                sum += (i % 2 == 0) ? 2 * Evaluate(func, x) : 4 * Evaluate(func, x);
            }
            return sum * h / 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Evaluate(double* f, double x)
        {
            return f[0] * x * x + f[1] * x + f[2];
        }
    }

    public static unsafe class GaussLegendre
    {
        public static double Run(int n, double a, double b, double* func)
        {
            double* x = stackalloc double[n];
            double* w = stackalloc double[n];
            ComputeNodesWeights(n, x, w);
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                double t = (a + b) / 2 + (b - a) / 2 * x[i];
                sum += w[i] * Evaluate(func, t);
            }
            return sum * (b - a) / 2;
        }

        private static void ComputeNodesWeights(int n, double* x, double* w)
        {
            double eps = 1e-14;
            for (int i = 1; i <= n; i++)
            {
                if (i == 1) x[n - 1] = 0;
                else x[n - i] = -1 + 3.0 * (i - 1) / (n - 1);
                for (int j = 0; j < 50; j++)
                {
                    double p = 1, p1 = 1, p2 = 0;
                    for (int k = 0; k < n; k++)
                    {
                        p2 = p1;
                        p1 = p;
                        p = ((2 * k + 1) * x[n - i] * p1 - k * p2) / (k + 1);
                    }
                    double pder = n * (x[n - i] * p - p1) / (x[n - i] * x[n - i] - 1);
                    double dx = p / pder;
                    x[n - i] -= dx;
                    if (Math.Abs(dx) < eps) break;
                }
                w[n - i] = 2 / ((1 - x[n - i] * x[n - i]) * (n * (n + 1)) / (2.0 * (n + 1) * (n - 1)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Evaluate(double* f, double x)
        {
            return f[0] * x * x + f[1] * x + f[2];
        }
    }

    public static unsafe class ConvexFunctionMinimize
    {
        public static double Run(double* func, double* grad, double lo, double hi, double tol, int maxIter)
        {
            double bestX = (lo + hi) / 2;
            double bestVal = Evaluate(func, bestX);
            for (int iter = 0; iter < maxIter; iter++)
            {
                if (hi - lo < tol) break;
                double x1 = lo + (hi - lo) / 3;
                double x2 = hi - (hi - lo) / 3;
                double f1 = Evaluate(func, x1);
                double f2 = Evaluate(func, x2);
                if (f1 < f2) hi = x2;
                else lo = x1;
                double xMid = (lo + hi) / 2;
                double valMid = Evaluate(func, xMid);
                if (valMid < bestVal) { bestVal = valMid; bestX = xMid; }
            }
            return bestX;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Evaluate(double* f, double x)
        {
            return f[0] * x * x + f[1] * x + f[2];
        }
    }
}