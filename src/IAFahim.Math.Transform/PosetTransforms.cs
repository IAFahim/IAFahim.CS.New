namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class PosetTransforms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopCount(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public static void ZetaTransform(long* f, long* g, int* topOrder, bool* relation, int n, long mod)
        {
            for (int i = 0; i < n; i++)
            {
                int y = topOrder[i];
                long sum = 0;
                for (int j = 0; j <= i; j++)
                {
                    int x = topOrder[j];
                    if (relation[x * n + y])
                    {
                        sum = (sum + f[x]) % mod;
                    }
                }
                g[y] = sum;
            }
        }

        public static void MobiusTransform(long* g, long* f, int* topOrder, bool* relation, int n, long mod)
        {
            long* mu = null;
            bool allocated = false;
            long size = (long)n * n;
            if (size > 1024)
            {
                mu = (long*)Marshal.AllocHGlobal((nint)(size * sizeof(long)));
                allocated = true;
            }
            else
            {
                long* tempMu = stackalloc long[(int)size];
                mu = tempMu;
            }
            try
            {
                for (int i = 0; i < size; i++)
                {
                    mu[i] = 0;
                }
                for (int i = 0; i < n; i++)
                {
                    int x = topOrder[i];
                    mu[(long)x * n + x] = 1;
                    for (int j = i + 1; j < n; j++)
                    {
                        int y = topOrder[j];
                        if (relation[x * n + y])
                        {
                            long sum = 0;
                            for (int k = i; k < j; k++)
                            {
                                int z = topOrder[k];
                                if (relation[z * n + y])
                                {
                                    sum = (sum + mu[(long)x * n + z]) % mod;
                                }
                            }
                            mu[(long)x * n + y] = (mod - sum) % mod;
                        }
                    }
                }
                for (int i = 0; i < n; i++)
                {
                    int y = topOrder[i];
                    long sum = 0;
                    for (int j = 0; j <= i; j++)
                    {
                        int x = topOrder[j];
                        if (relation[x * n + y])
                        {
                            sum = (sum + mu[(long)x * n + y] * g[x]) % mod;
                        }
                    }
                    f[y] = sum;
                }
            }
            finally
            {
                if (allocated)
                {
                    Marshal.FreeHGlobal((nint)mu);
                }
            }
        }

        public static int LatticeMeet(int x, int y, bool* relation, int n)
        {
            for (int z = 0; z < n; z++)
            {
                if (relation[z * n + x] && relation[z * n + y])
                {
                    bool isGreatest = true;
                    for (int w = 0; w < n; w++)
                    {
                        if (relation[w * n + x] && relation[w * n + y])
                        {
                            if (!relation[w * n + z])
                            {
                                isGreatest = false;
                                break;
                            }
                        }
                    }
                    if (isGreatest)
                    {
                        return z;
                    }
                }
            }
            return -1;
        }

        public static int LatticeJoin(int x, int y, bool* relation, int n)
        {
            for (int z = 0; z < n; z++)
            {
                if (relation[x * n + z] && relation[y * n + z])
                {
                    bool isLeast = true;
                    for (int w = 0; w < n; w++)
                    {
                        if (relation[x * n + w] && relation[y * n + w])
                        {
                            if (!relation[z * n + w])
                            {
                                isLeast = false;
                                break;
                            }
                        }
                    }
                    if (isLeast)
                    {
                        return z;
                    }
                }
            }
            return -1;
        }

        public static int BooleanLatticeRank(int x)
        {
            return PopCount(x);
        }
    }
}
