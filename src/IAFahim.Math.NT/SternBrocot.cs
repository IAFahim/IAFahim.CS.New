namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SternBrocot
    {
        public static void Run(long n, long d, long* num, long* den, int* depth)
        {
            long a = 0, b = 1, c = 1, dd = 0;
            *depth = 0;
            while (true)
            {
                long medNum = a + c;
                long medDen = b + dd;
                long cmp = medNum * d - n * medDen;
                if (cmp == 0) break;
                if (cmp < 0)
                {
                    long steps = (n * b - a * d) / (c * d - n * dd);
                    if (steps <= 0) steps = 1;
                    a += steps * c;
                    b += steps * dd;
                }
                else
                {
                    long steps = (c * d - n * dd) / (n * b - a * d);
                    if (steps <= 0) steps = 1;
                    c += steps * a;
                    dd += steps * b;
                }
                (*depth)++;
            }
            *num = n;
            *den = d;
        }
    }
}
