namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TspBitonic
    {
        private static long Dist(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2, dy = y1 - y2;
            return (long)Math.Sqrt(dx * dx + dy * dy);
        }

        public static long Run(int n, double* xs, double* ys, long* dp)
        {
            if (n < 2) return 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                dp[i * n + j] = -1;
            dp[0 * n + 1] = Dist(xs[0], ys[0], xs[1], ys[1]);
            dp[1 * n + 0] = dp[0 * n + 1];
            for (int i = 2; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    long prev = dp[j * n + i - 1];
                    if (j == i - 1)
                    {
                        long best = long.MaxValue;
                        for (int k = 0; k < i - 1; k++)
                        {
                            long cand = dp[k * n + i - 1] + Dist(xs[k], ys[k], xs[i], ys[i]);
                            if (cand < best) best = cand;
                        }
                        dp[j * n + i] = best;
                    }
                    else
                    {
                        dp[j * n + i] = prev + Dist(xs[i - 1], ys[i - 1], xs[i], ys[i]);
                    }
                    dp[i * n + j] = dp[j * n + i];
                }
            }
            long result = dp[0 * n + n - 1] + Dist(xs[0], ys[0], xs[n - 1], ys[n - 1]);
            return result;
        }
    }
}
