namespace IAFahim.Optimization.Approximation
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Freivalds
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Verify(int n, int* a, int* b, int* c, int* x, int* r, int iters)
        {
            Random rng = new Random(12345);
            for (int it = 0; it < iters; it++)
            {
                for (int i = 0; i < n; i++) r[i] = (rng.Next() & 1) * 2 - 1;
                long* br = stackalloc long[n];
                long* ar = stackalloc long[n];
                long* cr = stackalloc long[n];
                for (int i = 0; i < n; i++)
                {
                    long sum = 0;
                    for (int j = 0; j < n; j++)
                        sum += (long)b[i * n + j] * r[j];
                    br[i] = sum;
                }
                for (int i = 0; i < n; i++)
                {
                    long sum = 0;
                    for (int j = 0; j < n; j++)
                        sum += (long)a[i * n + j] * br[j];
                    ar[i] = sum;
                }
                for (int i = 0; i < n; i++)
                {
                    long sum = 0;
                    for (int j = 0; j < n; j++)
                        sum += (long)c[i * n + j] * r[j];
                    cr[i] = sum;
                }
                for (int i = 0; i < n; i++)
                    if (Math.Abs(ar[i] - cr[i]) > 1e-6) return false;
            }
            return true;
        }
    }
}
