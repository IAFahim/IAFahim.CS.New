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
            return (q.A * x + q.B) * x + q.C;
        }

        // Crossing abscissa of two quadratics that share the same leading
        // coefficient A (the x^2 terms cancel, leaving a linear equation):
        //   p.B*x + p.C = q.B*x + q.C  =>  x = (q.C - p.C) / (p.B - q.B).
        // Caller (Run) maintains a lower envelope where consecutive deque
        // members differ in B, so the denominator is non-zero by design.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long IntersectX(Quad p, Quad q)
        {
            return (q.C - p.C) / (p.B - q.B);
        }

        public static void Run(long* dp, int n, Quad* quads, int* deque, int* head, int* tail)
        {
            int h = 0;
            int t = 0;
            for (int i = 0; i < n; i++)
            {
                long frontVal = Eval(quads[deque[h]], i);
                while (t - h > 1)
                {
                    long nextVal = Eval(quads[deque[h + 1]], i);
                    if (frontVal < nextVal) break;
                    frontVal = nextVal;
                    h++;
                }
                dp[i] = frontVal;
                Quad newQ = quads[i + 1];
                while (t - h > 1 && IntersectX(quads[deque[t - 1]], quads[deque[t - 2]]) >= IntersectX(quads[deque[t - 1]], newQ))
                    t--;
                deque[t++] = i + 1;
            }
            *head = h;
            *tail = t;
        }
    }
}
