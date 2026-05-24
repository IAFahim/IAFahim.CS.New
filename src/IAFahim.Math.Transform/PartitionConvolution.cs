namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PartitionConvolution
    {
        public static void ConvolveWithPartition(long* a, long* c, int n, long mod)
        {
            for (int i = 0; i < n; i++)
            {
                long val = a[i] % mod;
                for (int j = 1; ; j++)
                {
                    long g1 = (long)j * (3 * j - 1) / 2, g2 = (long)j * (3 * j + 1) / 2;
                    if (g1 > i && g2 > i) break;
                    val = UpdateVal(val, i, j, g1, g2, c, mod, true);
                }
                c[i] = val;
            }
        }

        private static long UpdateVal(long val, int i, int j, long g1, long g2, long* c, long mod, bool isPart)
        {
            long sign = (j % 2 == 1) ? 1 : mod - 1;
            if (!isPart) sign = (mod - sign) % mod;
            if (g1 <= i) val = (val + sign * c[i - g1]) % mod;
            if (g2 <= i) val = (val + sign * c[i - g2]) % mod;
            return val;
        }

        public static void ConvolveWithPentagonal(long* a, long* c, int n, long mod)
        {
            for (int i = 0; i < n; i++)
            {
                long val = a[i] % mod;
                for (int j = 1; ; j++)
                {
                    long g1 = (long)j * (3 * j - 1) / 2, g2 = (long)j * (3 * j + 1) / 2;
                    if (g1 > i && g2 > i) break;
                    val = UpdateVal(val, i, j, g1, g2, a, mod, false);
                }
                c[i] = val;
            }
        }
    }
}
