namespace IAFahim.Optimization.Approximation
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Freivalds
    {
        public static bool Verify(int n, int* a, int* b, int* c, int* r, int iters, uint* seed)
        {
            for (int it = 0; it < iters; it++)
            {
                InitializeRandomVector(n, r, seed);
                if (!PerformCheck(n, a, b, c, r)) return false;
            }
            return true;
        }

        private static void InitializeRandomVector(int n, int* r, uint* seed)
        {
            for (int i = 0; i < n; i++)
            {
                *seed ^= *seed << 13; *seed ^= *seed >> 17; *seed ^= *seed << 5;
                r[i] = (int)((*seed & 1) * 2 - 1);
            }
        }

        private static bool PerformCheck(int n, int* a, int* b, int* c, int* r)
        {
            long* br = stackalloc long[n], ar = stackalloc long[n], cr = stackalloc long[n];
            MatrixVectorMultiply(n, b, r, br);
            MatrixVectorMultiply(n, a, br, ar);
            MatrixVectorMultiply(n, c, r, cr);
            for (int i = 0; i < n; i++) if (ar[i] != cr[i]) return false;
            return true;
        }

        private static void MatrixVectorMultiply(int n, int* mat, int* vec, long* res)
        {
            for (int i = 0; i < n; i++)
            {
                long sum = 0;
                for (int j = 0; j < n; j++) sum += (long)mat[i * n + j] * vec[j];
                res[i] = sum;
            }
        }

        private static void MatrixVectorMultiply(int n, int* mat, long* vec, long* res)
        {
            for (int i = 0; i < n; i++)
            {
                long sum = 0;
                for (int j = 0; j < n; j++) sum += (long)mat[i * n + j] * vec[j];
                res[i] = sum;
            }
        }
    }
}
