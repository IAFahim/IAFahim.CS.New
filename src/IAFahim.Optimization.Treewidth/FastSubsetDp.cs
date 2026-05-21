namespace IAFahim.Optimization.Treewidth
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FastSubsetDp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* f, long* g, int n, int k)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << k); mask++)
                {
                    int sub = (mask - 1) & mask;
                    while (sub > 0)
                    {
                        if (sub < mask)
                        {
                            long val = f[i * (1 << k) + sub] + g[sub * (1 << k) + mask];
                            if (val < f[i * (1 << k) + mask])
                                f[i * (1 << k) + mask] = val;
                        }
                        sub = (sub - 1) & mask;
                    }
                }
            }
        }
    }
}
