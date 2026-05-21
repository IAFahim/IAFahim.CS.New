namespace IAFahim.Geometry.Intersect
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Plane
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double PointPlaneDistance(double px, double py, double pz, double nx, double ny, double nz, double d)
        {
            return (px * nx + py * ny + pz * nz - d) / Math.Sqrt(nx * nx + ny * ny + nz * nz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LinePlane(double lx, double ly, double lz, double ldx, double ldy, double ldz, double nx, double ny, double nz, double d, double* t)
        {
            double denom = nx * ldx + ny * ldy + nz * ldz;
            if (Math.Abs(denom) < 1e-12) return false;
            *t = (d - nx * lx - ny * ly - nz * lz) / denom;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SegmentPlane(double x1, double y1, double z1, double x2, double y2, double z2, double nx, double ny, double nz, double d, double* t)
        {
            double dx = x2 - x1, dy = y2 - y1, dz = z2 - z1;
            double denom = nx * dx + ny * dy + nz * dz;
            if (Math.Abs(denom) < 1e-12) return false;
            *t = (d - nx * x1 - ny * y1 - nz * z1) / denom;
            return *t >= 0 && *t <= 1;
        }
    }
}
