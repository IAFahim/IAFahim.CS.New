namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaximumInscribedCircle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Run(double* xs, double* ys, int n)
        {
            if (n < 3) return 0;
            double minR = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                int ni = (i + 1) % n;
                double dx = ys[ni] - ys[i], dy = -(xs[ni] - xs[i]);
                double len = Math.Sqrt(dx * dx + dy * dy);
                if (len < 1e-12) continue;
                double cx = (xs[i] + xs[ni]) / 2;
                double cy = (ys[i] + ys[ni]) / 2;
                double dist = Math.Abs(cx * dx + cy * dy) / len;
                if (dist < minR) minR = dist;
            }
            return minR == double.MaxValue ? 0 : minR;
        }
    }
}
