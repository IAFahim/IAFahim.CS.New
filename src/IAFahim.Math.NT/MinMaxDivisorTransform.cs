namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class MinMaxDivisorTransform
    {
        public static void RunMax(long* f, int n)
        {
            for (int d = 1; d <= n; d++)
                for (int j = 2 * d; j <= n; j += d)
                    if (f[d] > f[j]) f[j] = f[d];
        }

        public static void RunMin(long* f, int n)
        {
            for (int d = 1; d <= n; d++)
                for (int j = 2 * d; j <= n; j += d)
                    if (f[d] < f[j]) f[j] = f[d];
        }

        public static void RunMaxInverse(long* f, int n)
        {
            for (int d = n; d >= 1; d--)
                for (int j = 2 * d; j <= n; j += d)
                    if (f[d] > f[j]) f[j] = f[d];
        }

        public static void RunMinInverse(long* f, int n)
        {
            for (int d = n; d >= 1; d--)
                for (int j = 2 * d; j <= n; j += d)
                    if (f[d] < f[j]) f[j] = f[d];
        }
    }
}
