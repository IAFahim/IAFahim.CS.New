namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class LcmConvolution
    {
        public static void Forward(long* f, int n)
        {
            for (int d = 1; d <= n; d++)
                for (int j = 2 * d; j <= n; j += d)
                    f[j] += f[d];
        }

        public static void Inverse(long* f, int n, int* mu)
        {
            for (int d = n; d >= 1; d--)
                // k tracks j / d to avoid expensive division in the inner loop
                for (int j = 2 * d, k = 2; j <= n; j += d, k++)
                    f[j] -= mu[k] * f[d];
        }

        public static void Run(long* a, long* b, long* result, int n, int* mu)
        {
            Forward(a, n);
            Forward(b, n);
            for (int i = 1; i <= n; i++) result[i] = a[i] * b[i];
            Inverse(result, n, mu);
        }
    }
}
