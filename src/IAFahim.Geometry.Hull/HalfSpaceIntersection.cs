namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HalfSpaceIntersection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(double* nx, double* ny, double* d, int m, double* outX, double* outY, int* outSize)
        {
            if (m == 0) { *outSize = 0; return 0; }
            int n = 0;
            double minX = -1e9, maxX = 1e9, minY = -1e9, maxY = 1e9;
            for (int i = 0; i < m; i++)
            {
                if (Math.Abs(nx[i]) > Math.Abs(ny[i]))
                {
                    double x0 = -d[i] / nx[i];
                    if (nx[i] > 0) maxX = Math.Min(maxX, x0);
                    else minX = Math.Max(minX, x0);
                }
                else
                {
                    double y0 = -d[i] / ny[i];
                    if (ny[i] > 0) maxY = Math.Min(maxY, y0);
                    else minY = Math.Max(minY, y0);
                }
            }
            if (minX > maxX || minY > maxY) { *outSize = 0; return 0; }
            n = 0;
            outX[n] = minX; outY[n++] = minY;
            outX[n] = maxX; outY[n++] = maxY;
            outX[n] = maxX; outY[n++] = maxY;
            outX[n] = minX; outY[n++] = minY;
            *outSize = n;
            return n;
        }
    }
}
