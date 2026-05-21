namespace IAFahim.Geometry.Intersect
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Sphere
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LineIntersection(double cx, double cy, double cz, double r, double lx, double ly, double lz, double ldx, double ldy, double ldz, double* t1, double* t2)
        {
            double fx = lx - cx, fy = ly - cy, fz = lz - cz;
            double a = ldx * ldx + ldy * ldy + ldz * ldz;
            double b = 2 * (fx * ldx + fy * ldy + fz * ldz);
            double c = fx * fx + fy * fy + fz * fz - r * r;
            double disc = b * b - 4 * a * c;
            if (disc < 0) return 0;
            double sq = Math.Sqrt(disc);
            *t1 = (-b - sq) / (2 * a);
            *t2 = (-b + sq) / (2 * a);
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SphereIntersection(double x1, double y1, double z1, double r1, double x2, double y2, double z2, double r2, double* cx, double* cy, double* cz)
        {
            double dx = x2 - x1, dy = y2 - y1, dz = z2 - z1;
            double d = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (d > r1 + r2) return false;
            if (d < Math.Abs(r1 - r2)) return false;
            double a = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            double h = Math.Sqrt(r1 * r1 - a * a);
            *cx = x1 + a * dx / d;
            *cy = y1 + a * dy / d;
            *cz = z1 + a * dz / d;
            return true;
        }
    }
}
