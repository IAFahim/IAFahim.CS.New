namespace IAFahim.Geometry.Intersect
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Plane
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PointPlaneDistance(
            double px, double py, double pz, 
            double nx, double ny, double nz, double d)
        {
            double num = px * nx + py * ny + pz * nz + d;
            double denSq = nx * nx + ny * ny + nz * nz;
            return num / Math.Sqrt(denSq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LinePlaneIntersection(
            double lx, double ly, double lz, 
            double ldx, double ldy, double ldz, 
            double nx, double ny, double nz, double d, 
            double* t)
        {
            double denom = nx * ldx + ny * ldy + nz * ldz;
            if (denom >= -1e-12 && denom <= 1e-12) return false;
            *t = -(nx * lx + ny * ly + nz * lz + d) / denom;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SegmentPlaneIntersection(
            double x1, double y1, double z1, 
            double x2, double y2, double z2, 
            double nx, double ny, double nz, double d, 
            double* t)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            double dz = z2 - z1;
            double denom = nx * dx + ny * dy + nz * dz;
            if (denom >= -1e-12 && denom <= 1e-12) return false;
            double param = -(nx * x1 + ny * y1 + nz * z1 + d) / denom;
            if (param < 0.0 || param > 1.0) return false;
            *t = param;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PlaneIntersection(
            double n1x, double n1y, double n1z, double d1,
            double n2x, double n2y, double n2z, double d2,
            double* lpx, double* lpy, double* lpz,
            double* ldx, double* ldy, double* ldz)
        {
            double dx = n1y * n2z - n1z * n2y;
            double dy = n1z * n2x - n1x * n2z;
            double dz = n1x * n2y - n1y * n2x;

            double dirSq = dx * dx + dy * dy + dz * dz;
            if (dirSq < 1e-24) return false;

            *ldx = dx;
            *ldy = dy;
            *ldz = dz;

            double absDx = Math.Abs(dx);
            double absDy = Math.Abs(dy);
            double absDz = Math.Abs(dz);

            if (absDx >= absDy && absDx >= absDz)
            {
                *lpx = 0;
                double det = n1y * n2z - n1z * n2y;
                *lpy = (-d1 * n2z + d2 * n1z) / det;
                *lpz = (-n1y * d2 + n2y * d1) / det;
            }
            else if (absDy >= absDx && absDy >= absDz)
            {
                *lpy = 0;
                double det = n1z * n2x - n1x * n2z;
                *lpz = (-d1 * n2x + d2 * n1x) / det;
                *lpx = (-n1z * d2 + n2z * d1) / det;
            }
            else
            {
                *lpz = 0;
                double det = n1x * n2y - n1y * n2x;
                *lpx = (-d1 * n2y + d2 * n1y) / det;
                *lpy = (-n1x * d2 + n2x * d1) / det;
            }

            return true;
        }
    }
}
