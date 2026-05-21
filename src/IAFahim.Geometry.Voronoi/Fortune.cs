namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Fortune
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, double* outX, double* outY, int* outSize)
        {
            for (int i = 0; i < n; i++) { outX[i] = xs[i]; outY[i] = ys[i]; }
            *outSize = n;
            return n;
        }
    }
}
