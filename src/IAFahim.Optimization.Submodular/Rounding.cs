namespace IAFahim.Optimization.Submodular
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Rounding
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Random(int n, double* frac, int* result, Random rng)
        {
            for (int i = 0; i < n; i++)
            {
                result[i] = rng.NextDouble() < frac[i] ? 1 : 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dependent(int n, double* frac, int* result)
        {
            double sum = 0;
            for (int i = 0; i < n; i++) sum += frac[i];
            int k = (int)Math.Round(sum);
            for (int i = 0; i < n; i++) result[i] = 0;
            Random rng = new Random(42);
            for (int i = 0; i < k; i++)
            {
                double r = rng.NextDouble() * sum;
                double acc = 0;
                for (int j = 0; j < n; j++)
                {
                    acc += frac[j];
                    if (r < acc) { result[j]++; sum -= frac[j]; frac[j] = 0; break; }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pipage(int n, double* frac, int* result)
        {
            for (int i = 0; i < n; i++) result[i] = (int)Math.Round(frac[i]);
        }
    }
}
