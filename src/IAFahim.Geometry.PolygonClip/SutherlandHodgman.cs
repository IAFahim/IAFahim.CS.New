namespace IAFahim.Geometry.PolygonClip
{
    using System.Runtime.CompilerServices;

    public static unsafe class SutherlandHodgman
    {
        // Clip subject polygon (CCW) against convex clip polygon (CCW).
        // subject: sx,sy length sn; clip: cx,cy length cn; outputs ox,oy up to outCap.
        // Returns vertex count of clipped polygon (0 if empty).
        public static int Clip(
            double* sx, double* sy, int sn,
            double* cx, double* cy, int cn,
            double* ox, double* oy, int outCap)
        {
            if (sn < 3 || cn < 3 || outCap < 3) return 0;

            double* inX = stackalloc double[outCap];
            double* inY = stackalloc double[outCap];
            double* tmpX = stackalloc double[outCap];
            double* tmpY = stackalloc double[outCap];
            int inN = sn < outCap ? sn : outCap;
            for (int i = 0; i < inN; i++) { inX[i] = sx[i]; inY[i] = sy[i]; }

            for (int e = 0; e < cn; e++)
            {
                int e2 = (e + 1) % cn;
                double ax = cx[e], ay = cy[e], bx = cx[e2], by = cy[e2];
                int outN = 0;
                if (inN == 0) break;
                double px = inX[inN - 1], py = inY[inN - 1];
                bool pin = Inside(px, py, ax, ay, bx, by);
                for (int i = 0; i < inN; i++)
                {
                    double qx = inX[i], qy = inY[i];
                    bool qin = Inside(qx, qy, ax, ay, bx, by);
                    if (qin)
                    {
                        if (!pin)
                        {
                            Intersect(px, py, qx, qy, ax, ay, bx, by, out double ix, out double iy);
                            if (outN < outCap) { tmpX[outN] = ix; tmpY[outN] = iy; outN++; }
                        }
                        if (outN < outCap) { tmpX[outN] = qx; tmpY[outN] = qy; outN++; }
                    }
                    else if (pin)
                    {
                        Intersect(px, py, qx, qy, ax, ay, bx, by, out double ix, out double iy);
                        if (outN < outCap) { tmpX[outN] = ix; tmpY[outN] = iy; outN++; }
                    }
                    px = qx; py = qy; pin = qin;
                }
                inN = outN;
                for (int i = 0; i < inN; i++) { inX[i] = tmpX[i]; inY[i] = tmpY[i]; }
            }

            for (int i = 0; i < inN; i++) { ox[i] = inX[i]; oy[i] = inY[i]; }
            return inN;
        }

        // Left of directed edge a->b (CCW clip).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Inside(double x, double y, double ax, double ay, double bx, double by)
            => (bx - ax) * (y - ay) - (by - ay) * (x - ax) >= -1e-12;

        private static void Intersect(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4,
            out double ix, out double iy)
        {
            double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (System.Math.Abs(d) < 1e-15) { ix = x2; iy = y2; return; }
            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / d;
            ix = x1 + t * (x2 - x1);
            iy = y1 + t * (y2 - y1);
        }
    }
}
