namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class WalshHadamardXor
    {
        public static void Forward(long* f, int n)
        {
            for (int len = 1; len < n; len <<= 1)
            {
                for (int i = 0; i < n; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        long u = f[i + j];
                        long v = f[i + j + len];
                        f[i + j] = u + v;
                        f[i + j + len] = u - v;
                    }
                }
            }
        }

        public static void Inverse(long* f, int n)
        {
            Forward(f, n);
            long invN = ModInverse(n);
            for (int i = 0; i < n; i++)
                f[i] = f[i] * invN % 1000000007L;
        }

        private static long ModInverse(long a)
        {
            long b = 1000000007L;
            long x = 0, y = 0;
            long g = ExtGcd(a, b, out x, out y);
            if (g != 1) return 1;
            return (x % b + b) % b;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
        }
    }

    public static unsafe class WalshHadamardOr
    {
        public static void Forward(long* f, int n)
        {
            for (int len = 1; len < n; len <<= 1)
            {
                for (int i = 0; i < n; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j + len] = (f[i + j + len] + f[i + j]) % 1000000007L;
                    }
                }
            }
        }

        public static void Inverse(long* f, int n)
        {
            for (int len = n >> 1; len > 0; len >>= 1)
            {
                for (int i = 0; i < n; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j + len] = (f[i + j + len] - f[i + j] + 1000000007L) % 1000000007L;
                    }
                }
            }
        }
    }

    public static unsafe class WalshHadamardAnd
    {
        public static void Forward(long* f, int n)
        {
            for (int len = 1; len < n; len <<= 1)
            {
                for (int i = 0; i < n; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j] = (f[i + j] + f[i + j + len]) % 1000000007L;
                    }
                }
            }
        }

        public static void Inverse(long* f, int n)
        {
            for (int len = n >> 1; len > 0; len >>= 1)
            {
                for (int i = 0; i < n; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        f[i + j] = (f[i + j] - f[i + j + len] + 1000000007L) % 1000000007L;
                    }
                }
            }
        }
    }

    public static unsafe class FwhtConvolution
    {
        public static void Run(long* a, long* b, long* c, int n, FwhtType type)
        {
            int size = 1 << n;
            if (type == FwhtType.Xor)
            {
                WalshHadamardXor.Forward(a, size);
                WalshHadamardXor.Forward(b, size);
                for (int i = 0; i < size; i++)
                    c[i] = a[i] * b[i] % 1000000007L;
                WalshHadamardXor.Inverse(c, size);
            }
            else if (type == FwhtType.Or)
            {
                WalshHadamardOr.Forward(a, size);
                WalshHadamardOr.Forward(b, size);
                for (int i = 0; i < size; i++)
                    c[i] = a[i] * b[i] % 1000000007L;
                WalshHadamardOr.Inverse(c, size);
            }
            else
            {
                WalshHadamardAnd.Forward(a, size);
                WalshHadamardAnd.Forward(b, size);
                for (int i = 0; i < size; i++)
                    c[i] = a[i] * b[i] % 1000000007L;
                WalshHadamardAnd.Inverse(c, size);
            }
        }

        public enum FwhtType { Xor, Or, And }
    }
}