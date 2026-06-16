namespace IAFahim.Optimization.Geometric
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class WelzlSphere
    {
        public struct Sphere
        {
            public double X, Y, Z, R;
        }

        private const double Epsilon = 1e-9;

        public static Sphere Run(double* xs, double* ys, double* zs, int n)
        {
            if (n == 0) return default;

            ulong seed = 123456789;
            for (int i = n - 1; i > 0; i--)
            {
                seed = seed * 6364136223846793005UL + 1442695040888963407UL;
                int j = (int)(seed % (ulong)(i + 1));
                double tx = xs[i]; xs[i] = xs[j]; xs[j] = tx;
                double ty = ys[i]; ys[i] = ys[j]; ys[j] = ty;
                double tz = zs[i]; zs[i] = zs[j]; zs[j] = tz;
            }

            Sphere s = default;
            s.X = xs[0]; s.Y = ys[0]; s.Z = zs[0]; s.R = 0;
            for (int i = 1; i < n; i++)
            {
                if (!Contains(s, xs[i], ys[i], zs[i]))
                {
                    s = Solve1(xs, ys, zs, i);
                }
            }
            return s;
        }

        // Support set so far: {i}. Scan 0..i-1; on violation, point j joins the support set.
        private static Sphere Solve1(double* xs, double* ys, double* zs, int i)
        {
            Sphere s = default;
            s.X = xs[i]; s.Y = ys[i]; s.Z = zs[i]; s.R = 0;
            for (int j = 0; j < i; j++)
            {
                if (!Contains(s, xs[j], ys[j], zs[j]))
                {
                    s = Solve2(xs, ys, zs, i, j);
                }
            }
            return s;
        }

        // Support set so far: {i, j}. Scan 0..j-1; on violation, point k joins the support set.
        private static Sphere Solve2(double* xs, double* ys, double* zs, int i, int j)
        {
            Sphere s = Construct(xs[i], ys[i], zs[i], xs[j], ys[j], zs[j]);
            for (int k = 0; k < j; k++)
            {
                if (!Contains(s, xs[k], ys[k], zs[k]))
                {
                    s = Solve3(xs, ys, zs, i, j, k);
                }
            }
            return s;
        }

        // Support set so far: {i, j, k}. Scan 0..k-1; on violation, point l completes the support set.
        private static Sphere Solve3(double* xs, double* ys, double* zs, int i, int j, int k)
        {
            Sphere s = Construct(xs[i], ys[i], zs[i], xs[j], ys[j], zs[j], xs[k], ys[k], zs[k]);
            for (int l = 0; l < k; l++)
            {
                if (!Contains(s, xs[l], ys[l], zs[l]))
                {
                    s = Construct(xs[i], ys[i], zs[i], xs[j], ys[j], zs[j], xs[k], ys[k], zs[k], xs[l], ys[l], zs[l]);
                }
            }
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Contains(Sphere s, double px, double py, double pz)
        {
            double dx = px - s.X, dy = py - s.Y, dz = pz - s.Z;
            return dx * dx + dy * dy + dz * dz <= s.R * s.R + Epsilon;
        }

        // Sphere with two points on its boundary (diameter).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Sphere Construct(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            double hx = (x2 - x1) * 0.5, hy = (y2 - y1) * 0.5, hz = (z2 - z1) * 0.5;
            Sphere s;
            s.X = x1 + hx; s.Y = y1 + hy; s.Z = z1 + hz;
            s.R = Math.Sqrt(hx * hx + hy * hy + hz * hz);
            return s;
        }

        // Sphere with three points on its boundary (circumsphere of the triangle, center in its plane).
        private static Sphere Construct(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3)
        {
            // Translate so p1 is the origin.
            double bx = x2 - x1, by = y2 - y1, bz = z2 - z1;
            double cx = x3 - x1, cy = y3 - y1, cz = z3 - z1;

            // Normal of the triangle plane = b x c.
            double nx = by * cz - bz * cy;
            double ny = bz * cx - bx * cz;
            double nz = bx * cy - by * cx;
            double nn = nx * nx + ny * ny + nz * nz;

            // Degenerate (collinear) triangle: fall back to the best diameter sphere.
            if (nn < Epsilon)
            {
                return BestOfThreeDiameters(x1, y1, z1, x2, y2, z2, x3, y3, z3);
            }

            double bb = bx * bx + by * by + bz * bz;
            double cc = cx * cx + cy * cy + cz * cz;

            // Center (relative to p1): o = ( (|b|^2 (c x n) + |c|^2 (n x b)) ) / (2 nn).
            double cxn_x = cy * nz - cz * ny;
            double cxn_y = cz * nx - cx * nz;
            double cxn_z = cx * ny - cy * nx;

            double nxb_x = ny * bz - nz * by;
            double nxb_y = nz * bx - nx * bz;
            double nxb_z = nx * by - ny * bx;

            double inv = 1.0 / (2.0 * nn);
            double ox = (bb * cxn_x + cc * nxb_x) * inv;
            double oy = (bb * cxn_y + cc * nxb_y) * inv;
            double oz = (bb * cxn_z + cc * nxb_z) * inv;

            Sphere s;
            s.X = x1 + ox; s.Y = y1 + oy; s.Z = z1 + oz;
            s.R = Math.Sqrt(ox * ox + oy * oy + oz * oz);
            return s;
        }

        // Sphere with four points on its boundary (circumsphere of the tetrahedron).
        private static Sphere Construct(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3, double x4, double y4, double z4)
        {
            // Translate so p1 is the origin. Solve 3x3 system: 2 (pk . o) = |pk|^2 for k = 2,3,4.
            double a11 = x2 - x1, a12 = y2 - y1, a13 = z2 - z1;
            double a21 = x3 - x1, a22 = y3 - y1, a23 = z3 - z1;
            double a31 = x4 - x1, a32 = y4 - y1, a33 = z4 - z1;

            double r1 = (a11 * a11 + a12 * a12 + a13 * a13) * 0.5;
            double r2 = (a21 * a21 + a22 * a22 + a23 * a23) * 0.5;
            double r3 = (a31 * a31 + a32 * a32 + a33 * a33) * 0.5;

            double det = a11 * (a22 * a33 - a23 * a32)
                       - a12 * (a21 * a33 - a23 * a31)
                       + a13 * (a21 * a32 - a22 * a31);

            // Degenerate (coplanar) tetrahedron: fall back to the best triangle circumsphere.
            if (Math.Abs(det) < Epsilon)
            {
                return BestOfFourTriangles(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4);
            }

            double invDet = 1.0 / det;
            double ox = (r1 * (a22 * a33 - a23 * a32)
                       - a12 * (r2 * a33 - a23 * r3)
                       + a13 * (r2 * a32 - a22 * r3)) * invDet;
            double oy = (a11 * (r2 * a33 - a23 * r3)
                       - r1 * (a21 * a33 - a23 * a31)
                       + a13 * (a21 * r3 - r2 * a31)) * invDet;
            double oz = (a11 * (a22 * r3 - r2 * a32)
                       - a12 * (a21 * r3 - r2 * a31)
                       + r1 * (a21 * a32 - a22 * a31)) * invDet;

            Sphere s;
            s.X = x1 + ox; s.Y = y1 + oy; s.Z = z1 + oz;
            s.R = Math.Sqrt(ox * ox + oy * oy + oz * oz);
            return s;
        }

        // Smallest of the three pairwise-diameter spheres that still encloses the third point.
        private static Sphere BestOfThreeDiameters(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3)
        {
            Sphere best;
            best.X = 0; best.Y = 0; best.Z = 0; best.R = double.MaxValue;

            Sphere c12 = Construct(x1, y1, z1, x2, y2, z2);
            if (c12.R < best.R && Contains(c12, x3, y3, z3)) best = c12;

            Sphere c13 = Construct(x1, y1, z1, x3, y3, z3);
            if (c13.R < best.R && Contains(c13, x2, y2, z2)) best = c13;

            Sphere c23 = Construct(x2, y2, z2, x3, y3, z3);
            if (c23.R < best.R && Contains(c23, x1, y1, z1)) best = c23;

            return best;
        }

        // Smallest triangle circumsphere over the four faces that still encloses the remaining point.
        private static Sphere BestOfFourTriangles(double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3, double x4, double y4, double z4)
        {
            Sphere best;
            best.X = 0; best.Y = 0; best.Z = 0; best.R = double.MaxValue;

            Sphere c123 = Construct(x1, y1, z1, x2, y2, z2, x3, y3, z3);
            if (c123.R < best.R && Contains(c123, x4, y4, z4)) best = c123;

            Sphere c124 = Construct(x1, y1, z1, x2, y2, z2, x4, y4, z4);
            if (c124.R < best.R && Contains(c124, x3, y3, z3)) best = c124;

            Sphere c134 = Construct(x1, y1, z1, x3, y3, z3, x4, y4, z4);
            if (c134.R < best.R && Contains(c134, x2, y2, z2)) best = c134;

            Sphere c234 = Construct(x2, y2, z2, x3, y3, z3, x4, y4, z4);
            if (c234.R < best.R && Contains(c234, x1, y1, z1)) best = c234;

            return best;
        }
    }
}
