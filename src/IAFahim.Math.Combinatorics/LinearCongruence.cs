namespace IAFahim.Math.Combinatorics
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LinearCongruence
    {
        public static bool Run(long a, long b, long m, out long x, out long g)
        {
            g = Gcd(a, m);
            if (b % g != 0) { x = 0; g = -1; return false; }
            long a_ = a / g;
            long m_ = m / g;
            long b_ = b / g;
            long inv = ModInverse(a_, m_);
            x = (inv * b_) % m_;
            if (x < 0) x += m_;
            return true;
        }

        private static long Gcd(long a, long b)
        {
            if (a < 0) a = -a;
            if (b < 0) b = -b;
            while (b != 0) { long t = b; b = a % b; a = t; }
            return a;
        }

        private static long ModInverse(long a, long mod)
        {
            long x, y;
            long g = ExtGcd(a, mod, out x, out y);
            if (g != 1) return -1;
            return (x % mod + mod) % mod;
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
}