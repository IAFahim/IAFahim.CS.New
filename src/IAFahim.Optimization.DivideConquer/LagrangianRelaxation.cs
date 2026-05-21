namespace IAFahim.Optimization.DivideConquer
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LagrangianRelaxation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Search(long* w, int n, int k, long lo, long hi)
        {
            while (hi - lo > 1)
            {
                long mid = (lo + hi) >> 1;
                long sum = 0;
                for (int i = 0; i < n; i++)
                {
                    long v = w[i] - mid;
                    if (v > 0) sum += v;
                }
                if (sum >= k) lo = mid;
                else hi = mid;
            }
            return lo;
        }
    }
}
