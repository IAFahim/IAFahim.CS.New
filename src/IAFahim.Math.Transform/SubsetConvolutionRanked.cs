namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class SubsetConvolutionRanked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopCount(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public static void Run(long* a, long* b, long* c, int logN, long mod)
        {
            int n = 1 << logN;
            int numRanks = logN + 1;
            long totalSize = (long)numRanks * n;
            long* f = null;
            long* g = null;
            long* h = null;
            bool allocated = false;

            if (totalSize > 1024)
            {
                f = (long*)Marshal.AllocHGlobal((nint)(totalSize * sizeof(long)));
                g = (long*)Marshal.AllocHGlobal((nint)(totalSize * sizeof(long)));
                h = (long*)Marshal.AllocHGlobal((nint)(totalSize * sizeof(long)));
                allocated = true;
            }
            else
            {
                long* tempF = stackalloc long[(int)totalSize];
                long* tempG = stackalloc long[(int)totalSize];
                long* tempH = stackalloc long[(int)totalSize];
                f = tempF;
                g = tempG;
                h = tempH;
            }

            try
            {
                for (int i = 0; i < totalSize; i++)
                {
                    f[i] = 0;
                    g[i] = 0;
                    h[i] = 0;
                }

                for (int mask = 0; mask < n; mask++)
                {
                    int pc = PopCount(mask);
                    f[(long)pc * n + mask] = a[mask] % mod;
                    g[(long)pc * n + mask] = b[mask] % mod;
                }

                for (int r = 0; r < numRanks; r++)
                {
                    long* fRow = f + (long)r * n;
                    long* gRow = g + (long)r * n;
                    for (int i = 0; i < logN; i++)
                    {
                        for (int mask = 0; mask < n; mask++)
                        {
                            if ((mask & (1 << i)) != 0)
                            {
                                fRow[mask] = (fRow[mask] + fRow[mask ^ (1 << i)]) % mod;
                                gRow[mask] = (gRow[mask] + gRow[mask ^ (1 << i)]) % mod;
                            }
                        }
                    }
                }

                for (int mask = 0; mask < n; mask++)
                {
                    for (int i = 0; i < numRanks; i++)
                    {
                        long sum = 0;
                        for (int j = 0; j <= i; j++)
                        {
                            long valF = f[(long)j * n + mask];
                            long valG = g[(long)(i - j) * n + mask];
                            sum = (sum + valF * valG) % mod;
                        }
                        h[(long)i * n + mask] = sum;
                    }
                }

                for (int r = 0; r < numRanks; r++)
                {
                    long* hRow = h + (long)r * n;
                    for (int i = 0; i < logN; i++)
                    {
                        for (int mask = 0; mask < n; mask++)
                        {
                            if ((mask & (1 << i)) != 0)
                            {
                                hRow[mask] = (hRow[mask] - hRow[mask ^ (1 << i)] + mod) % mod;
                            }
                        }
                    }
                }

                for (int mask = 0; mask < n; mask++)
                {
                    int pc = PopCount(mask);
                    c[mask] = h[(long)pc * n + mask];
                }
            }
            finally
            {
                if (allocated)
                {
                    Marshal.FreeHGlobal((nint)f);
                    Marshal.FreeHGlobal((nint)g);
                    Marshal.FreeHGlobal((nint)h);
                }
            }
        }
    }
}
