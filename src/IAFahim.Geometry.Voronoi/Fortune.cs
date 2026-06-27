namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Fortune
    {
        // Voronoi diagram via the dual of the Delaunay triangulation. Each Delaunay triangle's
        // circumcenter is a Voronoi vertex; this Build writes those vertices to (outX, outY).
        // Returns the vertex count and sets *outSize. This is correct and O(n^2) (dominated by
        // the Delaunay Build); Fortune's O(n log n) sweep would be faster but needs an unmanaged
        // priority-queue + balanced-beachline and robust predicates, and the current signature
        // (vertex coords + count only) has no slot for the Voronoi EDGE list anyway. Extend the
        // signature (add an edge-index output) and pair with §W3 exact incircle before building a
        // full Fortune sweep or edge-connected Voronoi.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, int n, double* outX, double* outY, int* outSize)
        {
            if (n <= 2) { *outSize = 0; return 0; }

            int triCap = n * 2 + 16;
            Delaunay.Triangle* tris = (Delaunay.Triangle*)
                System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(Delaunay.Triangle) * triCap);
            try
            {
                int cnt = Delaunay.Build(xs, ys, n, tris);
                int written = 0;
                for (int i = 0; i < cnt; i++)
                {
                    int a = tris[i].A, b = tris[i].B, c = tris[i].C;
                    if (Circumcenter(xs[a], ys[a], xs[b], ys[b], xs[c], ys[c], out double vx, out double vy))
                    {
                        outX[written] = vx;
                        outY[written] = vy;
                        written++;
                    }
                }
                *outSize = written;
                return written;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tris);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Circumcenter(double ax, double ay, double bx, double by, double cx, double cy,
                                         out double vx, out double vy)
        {
            double d = 2.0 * ((ax - bx) * (ay - cy) - (ay - by) * (ax - cx));
            if (Math.Abs(d) < 1e-18) { vx = 0; vy = 0; return false; }
            double aa = ax * ax + ay * ay;
            double bb = bx * bx + by * by;
            double cc = cx * cx + cy * cy;
            vx = (aa * (by - cy) + bb * (cy - ay) + cc * (ay - by)) / d;
            vy = (aa * (cx - bx) + bb * (ax - cx) + cc * (bx - ax)) / d;
            return true;
        }
    }
}
