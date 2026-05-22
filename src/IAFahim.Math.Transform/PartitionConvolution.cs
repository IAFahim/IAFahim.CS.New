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
                    long g1 = (long)j * (3 * j - 1) / 2;
                    long g2 = (long)j * (3 * j + 1) / 2;
                    if (g1 > i && g2 > i)
                    {
                        break;
                    }
                    if (g1 <= i)
                    {
                        if (j % 2 == 1)
                        {
                            val = (val + c[i - g1]) % mod;
                        }
                        else
                        {
                            val = (val - c[i - g1] + mod) % mod;
                        }
                    }
                    if (g2 <= i)
                    {
                        if (j % 2 == 1)
                        {
                            val = (val + c[i - g2]) % mod;
                        }
                        else
                        {
                            val = (val - c[i - g2] + mod) % mod;
                        }
                    }
                }
                c[i] = val;
            }
        }

        public static void ConvolveWithPentagonal(long* a, long* c, int n, long mod)
        {
            for (int i = 0; i < n; i++)
            {
                long val = a[i] % mod;
                for (int j = 1; ; j++)
                {
                    long g1 = (long)j * (3 * j - 1) / 2;
                    long g2 = (long)j * (3 * j + 1) / 2;
                    if (g1 > i && g2 > i)
                    {
                        break;
                    }
                    if (g1 <= i)
                    {
                        if (j % 2 == 1)
                        {
                            val = (val - a[i - g1] + mod) % mod;
                        }
                        else
                        {
                            val = (val + a[i - g1]) % mod;
                        }
                    }
                    if (g2 <= i)
                    {
                        if (j % 2 == 1)
                        {
                            val = (val - a[i - g2] + mod) % mod;
                        }
                        else
                        {
                            val = (val + a[i - g2]) % mod;
                        }
                    }
                }
                c[i] = val;
            }
        }
    }
}
