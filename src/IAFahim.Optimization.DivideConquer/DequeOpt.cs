namespace IAFahim.Optimization.DivideConquer
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DequeOpt
    {
        public struct Quad
        {
            public long A, B, C;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Eval(Quad q, long x)
        {
            return q.A * x * x + q.B * x + q.C;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long IntersectX(Quad p, Quad q)
        {
            return (q.B - p.B + p.A - q.A + (q.A > p.A ? p.A - q.A : q.A - p.A)) / (p.A + q.A);
        }

        public static void Run(long* dp, int n, Quad* quads, int* deque, int* head, int* tail)
        {
            *head = *tail = 0;
            for (int i = 0; i < n; i++)
            {
                while (*tail - *head > 1 && Eval(quads[deque[*head]], i) >= Eval(quads[deque[*head + 1]], i))
                    (*head)++;
                dp[i] = Eval(quads[deque[*head]], i);
                Quad newQ = quads[i + 1];
                while (*tail - *head > 1 && IntersectX(quads[deque[*tail - 1]], quads[deque[*tail - 2]]) >= IntersectX(quads[deque[*tail - 1]], newQ))
                    (*tail)--;
                deque[(*tail)++] = i + 1;
            }
        }
    }
}
