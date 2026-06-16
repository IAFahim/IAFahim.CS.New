namespace IAFahim.Geometry.Intersect
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Plane
    {
        // Signed distance for a possibly non-unit normal (n is not assumed normalized).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PointPlaneDistance(double px, double py, double pz, double nx, double ny, double nz, double d)
        {
            return (px * nx + py * ny + pz * nz + d) / Math.Sqrt(nx * nx + ny * ny + nz * nz);
        }

        // Signed distance when the caller guarantees (nx,ny,nz) is unit length; skips the sqrt.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PointPlaneDistanceNormalized(double px, double py, double pz, double nx, double ny, double nz, double d)
        {
            return px * nx + py * ny + pz * nz + d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LinePlaneIntersection(double lx, double ly, double lz, double ldx, double ldy, double ldz, double nx, double ny, double nz, double d, double* t)
        {
            double denom = nx * ldx + ny * ldy + nz * ldz;
            if (Math.Abs(denom) < 1e-12) return false;
            *t = -(nx * lx + ny * ly + nz * lz + d) / denom;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SegmentPlaneIntersection(double x1, double y1, double z1, double x2, double y2, double z2, double nx, double ny, double nz, double d, double* t)
        {
            double dx = x2 - x1, dy = y2 - y1, dz = z2 - z1;
            double denom = nx * dx + ny * dy + nz * dz;
            if (Math.Abs(denom) < 1e-12) return false;
            double param = -(nx * x1 + ny * y1 + nz * z1 + d) / denom;
            if (param < 0.0 || param > 1.0) return false;
            *t = param; return true;
        }

        public static bool PlaneIntersection(double n1x, double n1y, double n1z, double d1, double n2x, double n2y, double n2z, double d2,
                                             double* lpx, double* lpy, double* lpz, double* ldx, double* ldy, double* ldz)
        {
            double dx = n1y * n2z - n1z * n2y;
            double dy = n1z * n2x - n1x * n2z;
            double dz = n1x * n2y - n1y * n2x;
            *ldx = dx; *ldy = dy; *ldz = dz;
            double dirSq = dx * dx + dy * dy + dz * dz;
            if (dirSq < 1e-24) return false;
            double ax = Math.Abs(dx);
            double ay = Math.Abs(dy);
            double az = Math.Abs(dz);
            if (ax >= ay && ax >= az) SolvePoint(lpx, lpy, lpz, n1y, n1z, d1, n2y, n2z, d2, dx);
            else if (ay >= ax && ay >= az) SolvePoint(lpy, lpz, lpx, n1z, n1x, d1, n2z, n2x, d2, dy);
            else SolvePoint(lpz, lpx, lpy, n1x, n1y, d1, n2x, n2y, d2, dz);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SolvePoint(double* pFixed, double* p1, double* p2, double a1, double b1, double d1, double a2, double b2, double d2, double det)
        {
            *pFixed = 0.0;
            *p1 = (-d1 * b2 + d2 * b1) / det;
            *p2 = (-a1 * d2 + a2 * d1) / det;
        }
    }
}
