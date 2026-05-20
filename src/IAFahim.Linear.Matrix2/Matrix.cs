namespace IAFahim.Linear.Matrix2
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MatrixNew
    {
        public static void Run(int n, int m, long* a)
        {
            for (int i = 0; i < n * m; i++) a[i] = 0;
        }

        public static void RunSquare(int n, long* a)
        {
            for (int i = 0; i < n * n; i++) a[i] = 0;
        }
    }

    public static unsafe class MatrixIdentity
    {
        public static void Run(int n, long* a)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    a[i * n + j] = (i == j) ? 1 : 0;
                }
            }
        }
    }

    public static unsafe class MatrixAdd
    {
        public static void Run(int n, int m, long* a, long* b, long* c)
        {
            for (int i = 0; i < n * m; i++) c[i] = a[i] + b[i];
        }
    }

    public static unsafe class MatrixSub
    {
        public static void Run(int n, int m, long* a, long* b, long* c)
        {
            for (int i = 0; i < n * m; i++) c[i] = a[i] - b[i];
        }
    }

    public static unsafe class MatrixMul
    {
        public static void Run(int n, int m, int p, long* a, long* b, long* c)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < p; j++)
                {
                    long sum = 0;
                    for (int k = 0; k < m; k++)
                    {
                        sum += a[i * m + k] * b[k * p + j];
                    }
                    c[i * p + j] = sum;
                }
            }
        }
    }

    public static unsafe class MatrixPow
    {
        public static void Run(int n, long* a, long* result, long* temp, long exp)
        {
            for (int i = 0; i < n * n; i++) result[i] = 0;
            for (int i = 0; i < n; i++) result[i * n + i] = 1;
            while (exp > 0)
            {
                if ((exp & 1) != 0)
                {
                    MatrixMul.Run(n, n, n, result, a, temp);
                    for (int i = 0; i < n * n; i++) result[i] = temp[i];
                }
                MatrixMul.Run(n, n, n, a, a, temp);
                for (int i = 0; i < n * n; i++) a[i] = temp[i];
                exp >>= 1;
            }
        }
    }

    public static unsafe class MatrixVecMul
    {
        public static void Run(int n, int m, long* a, long* v, long* result)
        {
            for (int i = 0; i < n; i++)
            {
                long sum = 0;
                for (int j = 0; j < m; j++)
                {
                    sum += a[i * m + j] * v[j];
                }
                result[i] = sum;
            }
        }
    }
}