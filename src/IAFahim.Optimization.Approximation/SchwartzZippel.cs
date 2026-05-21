namespace IAFahim.Optimization.Approximation
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SchwartzZippel
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Test(int* poly, int n, int* eval, int* points, int m, int prime)
        {
            long lhs = 0;
            for (int i = 0; i < n; i++)
            {
                long pow = 1;
                for (int j = 0; j < n; j++)
                {
                    if ((poly[i * n + j] & (1 << j)) != 0)
                    {
                        long p = points[j] % prime;
                        pow = (pow * p) % prime;
                    }
                }
                lhs = (lhs + pow) % prime;
            }
            long rhs = 0;
            for (int i = 0; i < n; i++)
            {
                long pow = 1;
                for (int j = 0; j < n; j++)
                {
                    if ((eval[i * n + j] & (1 << j)) != 0)
                    {
                        long p = points[j] % prime;
                        pow = (pow * p) % prime;
                    }
                }
                rhs = (rhs + pow) % prime;
            }
            return lhs == rhs;
        }
    }
}
