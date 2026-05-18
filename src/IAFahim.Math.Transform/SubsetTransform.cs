namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubsetZeta
    {
        public static void Run(long* f, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                        f[mask] += f[mask ^ (1 << i)];
                }
            }
        }

        public static void RunInt32(int* f, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                        f[mask] += f[mask ^ (1 << i)];
                }
            }
        }
    }

    public static unsafe class SubsetMobius
    {
        public static void Run(long* f, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                        f[mask] -= f[mask ^ (1 << i)];
                }
            }
        }

        public static void RunInt32(int* f, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                        f[mask] -= f[mask ^ (1 << i)];
                }
            }
        }
    }

    public static unsafe class SupersetZeta
    {
        public static void Run(long* f, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) == 0)
                        f[mask] += f[mask | (1 << i)];
                }
            }
        }
    }

    public static unsafe class SupersetMobius
    {
        public static void Run(long* f, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) == 0)
                        f[mask] -= f[mask | (1 << i)];
                }
            }
        }
    }

    public static unsafe class SubsetConvolution
    {
        public static void Run(long* a, long* b, long* c, int n)
        {
            int size = 1 << n;
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < size; mask++)
                {
                    if ((mask & (1 << i)) == 0)
                    {
                        a[mask | (1 << i)] += a[mask];
                        b[mask | (1 << i)] += b[mask];
                    }
                }
            }
            for (int mask = 0; mask < size; mask++)
                c[mask] = a[mask] * b[mask] % 1000000007L;
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < size; mask++)
                {
                    if ((mask & (1 << i)) != 0)
                        c[mask] = (c[mask] - c[mask ^ (1 << i)] + 1000000007L) % 1000000007L;
                }
            }
        }
    }
}