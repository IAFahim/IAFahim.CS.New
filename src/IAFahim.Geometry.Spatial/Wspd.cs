namespace IAFahim.Geometry.Spatial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Wspd
    {
        public struct Pair { public int A, B; public double S; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Pair* pairs, double s)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double dx = xs[i] - xs[j], dy = ys[i] - ys[j];
                    double d = Math.Sqrt(dx * dx + dy * dy);
                    pairs[count].A = i;
                    pairs[count].B = j;
                    pairs[count].S = d;
                    count++;
                }
            }
            return count;
        }
    }
}
