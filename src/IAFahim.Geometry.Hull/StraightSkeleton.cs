namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class StraightSkeleton
    {
        public struct Event { public double Y, X, Dx; public int Type; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, Event* events, double* outX, double* outY)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                int p = (i - 1 + n) % n, c = i, nxt = (i + 1) % n;
                double bx = xs[nxt] - xs[p], by = ys[nxt] - ys[p];
                if (Math.Abs(bx) < 1e-12 && Math.Abs(by) < 1e-12) continue;
                double nx = -by, ny = bx;
                double len = Math.Sqrt(nx * nx + ny * ny);
                nx /= len; ny /= len;
                outX[count] = xs[i] + nx * 0.1;
                outY[count] = ys[i] + ny * 0.1;
                count++;
            }
            return count;
        }
    }
}
