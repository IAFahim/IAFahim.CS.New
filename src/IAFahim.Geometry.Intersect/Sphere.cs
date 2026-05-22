namespace IAFahim.Geometry.Intersect
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Sphere
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LineIntersection(
            double cx, double cy, double cz, double r, 
            double lx, double ly, double lz, 
            double ldx, double ldy, double ldz, 
            double* t1, double* t2)
        {
            double fx = lx - cx;
            double fy = ly - cy;
            double fz = lz - cz;

            double a = ldx * ldx + ldy * ldy + ldz * ldz;
            if (a < 1e-24) return 0;

            double b = 2.0 * (fx * ldx + fy * ldy + fz * ldz);
            double c = fx * fx + fy * fy + fz * fz - r * r;

            double disc = b * b - 4.0 * a * c;
            if (disc < -1e-12) return 0;
            
            if (disc < 1e-12)
            {
                *t1 = -b / (2.0 * a);
                *t2 = *t1;
                return 1;
            }

            double sq = Math.Sqrt(disc);
            double q = (b > 0) ? -0.5 * (b + sq) : -0.5 * (b - sq);
            
            double r1 = q / a;
            double r2 = c / q;
            
            if (r1 > r2)
            {
                *t1 = r2;
                *t2 = r1;
            }
            else
            {
                *t1 = r1;
                *t2 = r2;
            }

            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SphereIntersection(
            double x1, double y1, double z1, double r1, 
            double x2, double y2, double z2, double r2, 
            double* cx, double* cy, double* cz,
            double* circleRadius,
            double* nx, double* ny, double* nz)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            double dz = z2 - z1;
            double dSq = dx * dx + dy * dy + dz * dz;
            double d = Math.Sqrt(dSq);

            if (d > r1 + r2 + 1e-12) return false;
            if (d < Math.Abs(r1 - r2) - 1e-12) return false;
            if (d < 1e-12) return false;

            double a = (r1 * r1 - r2 * r2 + dSq) / (2.0 * d);
            double hSq = r1 * r1 - a * a;
            double h = hSq > 0 ? Math.Sqrt(hSq) : 0.0;

            *cx = x1 + a * (dx / d);
            *cy = y1 + a * (dy / d);
            *cz = z1 + a * (dz / d);

            *circleRadius = h;

            *nx = dx / d;
            *ny = dy / d;
            *nz = dz / d;

            return true;
        }
    }
}
