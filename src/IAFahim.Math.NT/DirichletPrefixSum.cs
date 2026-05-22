namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DirichletPrefixSum
    {
        public static void ConvolutionPrefixSum(int n, long* f, long* g, long* result)
        {
            if (n < 0)
            {
                return;
            }
            result[0] = 0;
            if (n == 0)
            {
                return;
            }
            for (int i = 1; i <= n; i++)
            {
                result[i] = 0;
            }
            for (int i = 1; i <= n; i++)
            {
                long fi = f[i];
                if (fi == 0)
                {
                    continue;
                }
                for (int j = 1; i * j <= n; j++)
                {
                    result[i * j] += fi * g[j];
                }
            }
            for (int i = 1; i <= n; i++)
            {
                result[i] += result[i - 1];
            }
        }

        public static long Hyperbola(long n, long* prefixF, long* prefixG)
        {
            if (n <= 0)
            {
                return 0;
            }
            long u = (long)Math.Sqrt((double)n);
            long sum = 0;
            for (long d = 1; d <= u; d++)
            {
                long fd = prefixF[d] - prefixF[d - 1];
                sum += fd * prefixG[n / d];
            }
            long limit2 = n / u;
            for (long d = 1; d <= limit2; d++)
            {
                long gd = prefixG[d] - prefixG[d - 1];
                sum += gd * prefixF[n / d];
            }
            sum -= prefixF[u] * prefixG[limit2];
            return sum;
        }
    }
}
