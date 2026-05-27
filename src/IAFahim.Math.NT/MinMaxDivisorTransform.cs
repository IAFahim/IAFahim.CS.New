namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class MinMaxDivisorTransform
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunMax(long* f, int n)
        {
            for (int d = 1; d <= n; d++)
                for (int j = 2 * d; j <= n; j += d)
                    if (f[d] > f[j]) f[j] = f[d];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunMin(long* f, int n)
        {
            for (int d = 1; d <= n; d++)
                for (int j = 2 * d; j <= n; j += d)
                    if (f[d] < f[j]) f[j] = f[d];
        }

        public static void RunMaxWithCounts(long* f, int* cnt, int n)
        {
            for (int d = 1; d <= n; d++)
            {
                for (int j = 2 * d; j <= n; j += d)
                {
                    if (f[d] > f[j]) { f[j] = f[d]; cnt[j] = cnt[d]; }
                    else if (f[d] == f[j]) cnt[j] += cnt[d];
                }
            }
        }

        public static void RunMinWithCounts(long* f, int* cnt, int n)
        {
            for (int d = 1; d <= n; d++)
            {
                for (int j = 2 * d; j <= n; j += d)
                {
                    if (f[d] < f[j]) { f[j] = f[d]; cnt[j] = cnt[d]; }
                    else if (f[d] == f[j]) cnt[j] += cnt[d];
                }
            }
        }
    }
}
