namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinkowskiSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Reorder(double* x, double* y, int n)
        {
            int minIdx = 0;
            for (int i = 1; i < n; i++)
            {
                if (y[i] < y[minIdx] || (y[i] == y[minIdx] && x[i] < x[minIdx]))
                    minIdx = i;
            }
            if (minIdx == 0) return;
            double* tx = stackalloc double[n];
            double* ty = stackalloc double[n];
            for (int i = 0; i < n; i++)
            {
                tx[i] = x[(minIdx + i) % n];
                ty[i] = y[(minIdx + i) % n];
            }
            for (int i = 0; i < n; i++)
            {
                x[i] = tx[i];
                y[i] = ty[i];
            }
        }

        public static int Convex(double* ax, double* ay, int an, double* bx, double* by, int bn, double* outX, double* outY)
        {
            if (an == 0 || bn == 0) return 0;
            Reorder(ax, ay, an);
            Reorder(bx, by, bn);
            
            int i = 0, j = 0, k = 0;
            outX[k] = ax[0] + bx[0];
            outY[k] = ay[0] + by[0];
            k++;
            
            int limit = an + bn;
            while (i < an || j < bn)
            {
                double dx1 = ax[(i + 1) % an] - ax[i % an];
                double dy1 = ay[(i + 1) % an] - ay[i % an];
                double dx2 = bx[(j + 1) % bn] - bx[j % bn];
                double dy2 = by[(j + 1) % bn] - by[j % bn];

                double cross = dx1 * dy2 - dy1 * dx2;

                double stepx, stepy;
                if (i < an && j < bn)
                {
                    if (cross >= 0) { stepx = dx1; stepy = dy1; i++; }
                    else { stepx = dx2; stepy = dy2; j++; }
                }
                else if (i < an) { stepx = dx1; stepy = dy1; i++; }
                else { stepx = dx2; stepy = dy2; j++; }

                // The closing edge reproduces the start vertex; do not write that
                // duplicate (it would overflow a buffer sized to the return value).
                if (k < limit)
                {
                    outX[k] = outX[k - 1] + stepx;
                    outY[k] = outY[k - 1] + stepy;
                    k++;
                }
            }
            return k - 1;
        }

        public static int Difference(double* ax, double* ay, int an, double* bx, double* by, int bn, double* outX, double* outY)
        {
            double* nbx = stackalloc double[bn];
            double* nby = stackalloc double[bn];
            for (int i = 0; i < bn; i++)
            {
                nbx[i] = -bx[i];
                nby[i] = -by[i];
            }
            // Need to reverse the order so it remains counter-clockwise
            for (int i = 0; i < bn / 2; i++)
            {
                int opp = bn - 1 - i;
                double tx = nbx[i]; nbx[i] = nbx[opp]; nbx[opp] = tx;
                double ty = nby[i]; nby[i] = nby[opp]; nby[opp] = ty;
            }
            return Convex(ax, ay, an, nbx, nby, bn, outX, outY);
        }
    }
}
