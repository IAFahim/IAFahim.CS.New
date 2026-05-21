namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class NearestNeighbor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FromPoints(double* xs, double* ys, int n, double qx, double qy)
        {
            int best = 0;
            double bd = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                double dx = xs[i] - qx, dy = ys[i] - qy;
                double d = dx * dx + dy * dy;
                if (d < bd) { bd = d; best = i; }
            }
            return best;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FromVoronoi(double qx, double qy, double* vc, int* idx, int n)
        {
            return FromPoints(vc, idx, n, qx, qy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Range(double qx, double qy, double r, double* xs, double* ys, int n, int* outIdx)
        {
            int c = 0;
            for (int i = 0; i < n; i++)
            {
                double dx = xs[i] - qx, dy = ys[i] - qy;
                if (dx * dx + dy * dy <= r * r) outIdx[c++] = i;
            }
            return c;
        }
    }
}
