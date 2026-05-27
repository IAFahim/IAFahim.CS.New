namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubsetZeta
    {
        public static void Run(long* f, int n)
        {
            int size = 1 << n;
            // ⚡ Bolt: Restructured into a branchless butterfly loop for cache locality
            // and to avoid heavy branch misprediction overhead on inner loop bitwise checks.
            for (int len = 1; len < size; len <<= 1)
            {
                for (int i = 0; i < size; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j + len] += f[i + j];
                    }
                }
            }
        }

        public static void RunInt32(int* f, int n)
        {
            int size = 1 << n;
            // ⚡ Bolt: Branchless butterfly loop for Fast Zeta
            for (int len = 1; len < size; len <<= 1)
            {
                for (int i = 0; i < size; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j + len] += f[i + j];
                    }
                }
            }
        }
    }

    public static unsafe class SubsetMobius
    {
        public static void Run(long* f, int n)
        {
            int size = 1 << n;
            // ⚡ Bolt: Branchless butterfly loop for Fast Mobius
            for (int len = 1; len < size; len <<= 1)
            {
                for (int i = 0; i < size; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j + len] -= f[i + j];
                    }
                }
            }
        }

        public static void RunInt32(int* f, int n)
        {
            int size = 1 << n;
            // ⚡ Bolt: Branchless butterfly loop for Fast Mobius
            for (int len = 1; len < size; len <<= 1)
            {
                for (int i = 0; i < size; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j + len] -= f[i + j];
                    }
                }
            }
        }
    }

    public static unsafe class SupersetZeta
    {
        public static void Run(long* f, int n)
        {
            int size = 1 << n;
            // ⚡ Bolt: Branchless butterfly loop for Superset Zeta
            for (int len = 1; len < size; len <<= 1)
            {
                for (int i = 0; i < size; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j] += f[i + j + len];
                    }
                }
            }
        }
    }

    public static unsafe class SupersetMobius
    {
        public static void Run(long* f, int n)
        {
            int size = 1 << n;
            // ⚡ Bolt: Branchless butterfly loop for Superset Mobius
            for (int len = 1; len < size; len <<= 1)
            {
                for (int i = 0; i < size; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j] -= f[i + j + len];
                    }
                }
            }
        }
    }

    public static unsafe class SubsetConvolution
    {
        public static void Run(long* a, long* b, long* c, int n)
        {
            int size = 1 << n;
            // ⚡ Bolt: Branchless butterfly loops for Subset Convolution (Zeta -> Pointwise -> Mobius)
            for (int len = 1; len < size; len <<= 1)
            {
                for (int i = 0; i < size; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        a[i + j + len] += a[i + j];
                        b[i + j + len] += b[i + j];
                    }
                }
            }
            for (int mask = 0; mask < size; mask++)
                c[mask] = a[mask] * b[mask] % 1000000007L;
            for (int len = 1; len < size; len <<= 1)
            {
                for (int i = 0; i < size; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        c[i + j + len] = (c[i + j + len] - c[i + j] + 1000000007L) % 1000000007L;
                    }
                }
            }
        }
    }
}