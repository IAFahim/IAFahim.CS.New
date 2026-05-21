namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinkowskiSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Convex(double* ax, double* ay, int an, double* bx, double* by, int bn, double* outX, double* outY)
        {
            int n = an + bn;
            for (int i = 0; i < an; i++) outX[i] = ax[i];
            for (int i = 0; i < bn; i++) outX[an + i] = bx[i];
            for (int i = 0; i < an; i++) outY[i] = ay[i];
            for (int i = 0; i < bn; i++) outY[an + i] = by[i];
            for (int i = 0; i < n - 1; i++)
            for (int j = i + 1; j < n; j++)
            {
                if (outX[i] > outX[j] || (outX[i] == outX[j] && outY[i] > outY[j]))
                {
                    double tx = outX[i]; outX[i] = outX[j]; outX[j] = tx;
                    double ty = outY[i]; outY[i] = outY[j]; outY[j] = ty;
                }
            }
            return n;
        }
    }
}
