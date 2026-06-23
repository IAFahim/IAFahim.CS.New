namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Mdp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ComputeQValue(int n, int m, double* trans, double* reward, double gamma, double* v, int s, int a)
        {
            double q = 0;
            for (int sp = 0; sp < n; sp++)
                q += trans[s * m * n + a * n + sp] * (reward[s * m + a] + gamma * v[sp]);
            return q;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValueIteration(int n, int m, double* trans, double* reward, double gamma, double* v, double* newV, int iters)
        {
            for (int iter = 0; iter < iters; iter++)
            {
                for (int s = 0; s < n; s++)
                {
                    double best = double.MinValue;
                    for (int a = 0; a < m; a++)
                    {
                        double q = ComputeQValue(n, m, trans, reward, gamma, v, s, a);
                        if (q > best) best = q;
                    }
                    newV[s] = best;
                }
                for (int s = 0; s < n; s++) v[s] = newV[s];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolicyIteration(int n, int m, double* trans, double* reward, double gamma, int* policy, double* v, int* newPol)
        {
            for (int i = 0; i < n; i++) newPol[i] = 0;
            bool changed = true;
            while (changed)
            {
                for (int s = 0; s < n; s++) v[s] = ComputeQValue(n, m, trans, reward, gamma, v, s, policy[s]);
                changed = false;
                for (int s = 0; s < n; s++)
                {
                    double best = double.MinValue;
                    int bestA = 0;
                    for (int a = 0; a < m; a++)
                    {
                        double q = ComputeQValue(n, m, trans, reward, gamma, v, s, a);
                        if (q > best) { best = q; bestA = a; }
                    }
                    if (bestA != policy[s]) { newPol[s] = bestA; changed = true; }
                }
                for (int s = 0; s < n; s++) policy[s] = newPol[s];
            }
        }
    }
}