namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Simplex
    {
        public struct Result
        {
            public long Value;
            public int Status;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result PhaseOne(int m, int n, long* a, long* b, long* c)
        {
            Result r = default;
            r.Status = 1;
            r.Value = 0;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result PhaseTwo(int m, int n, long* a, long* b, long* c)
        {
            Result r = default;
            r.Status = 1;
            long* x = stackalloc long[m + n];
            for (int i = 0; i < m + n; i++) x[i] = 0;
            for (int j = 0; j < n; j++)
            {
                long minB = long.MaxValue;
                int minIdx = -1;
                for (int i = 0; i < m; i++)
                {
                    if (a[i * n + j] > 0 && b[i] >= 0 && b[i] / a[i * n + j] < minB)
                    {
                        minB = b[i] / a[i * n + j];
                        minIdx = i;
                    }
                }
                if (minIdx >= 0)
                {
                    for (int i = 0; i < m; i++)
                    {
                        if (i != minIdx)
                            b[i] -= a[i * n + j] * minB;
                    }
                    b[minIdx] = minB;
                }
            }
            r.Value = 0;
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < m; i++)
                {
                    if (a[i * n + j] > 0)
                    {
                        r.Value += c[j] * b[i];
                        break;
                    }
                }
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result DualSimplex(int m, int n, long* a, long* b, long* c)
        {
            return PhaseTwo(m, n, a, b, c);
        }
    }
}
