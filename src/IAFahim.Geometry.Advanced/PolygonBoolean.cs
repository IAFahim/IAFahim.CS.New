namespace IAFahim.Geometry.Advanced
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class PolygonBoolean
    {
        // Convex-polygon intersection via Sutherland-Hodgman clipping. Subject (sx,sy,sn) and clip
        // (cx,cy,cn) must both be convex and CCW-oriented. Writes the intersection (a convex polygon)
        // to (ox,oy) and returns its vertex count (0 if empty). Caller sizes ox,oy to >= sn+cn.
        // CAVEAT: uses the inexact double orient2d; degenerate (collinear vertex-on-edge) inputs may
        // drop a boundary vertex. For guaranteed output route through §W3 OrientationExact.
        public static int Intersection(double* sx, double* sy, int sn,
                                       double* cx, double* cy, int cn,
                                       double* ox, double* oy)
        {
            if (sn < 3 || cn < 3) return 0;
            int cap = 2 * (sn + cn) + 8;
            double* curX = (double*)Marshal.AllocHGlobal(sizeof(double) * cap);
            double* curY = (double*)Marshal.AllocHGlobal(sizeof(double) * cap);
            double* nxtX = (double*)Marshal.AllocHGlobal(sizeof(double) * cap);
            double* nxtY = (double*)Marshal.AllocHGlobal(sizeof(double) * cap);
            try
            {
                int curN = sn;
                for (int i = 0; i < sn; i++) { curX[i] = sx[i]; curY[i] = sy[i]; }
                for (int e = 0; e < cn; e++)
                {
                    double ax = cx[e], ay = cy[e];
                    double bx = cx[(e + 1) % cn], by = cy[(e + 1) % cn];
                    int nxtN = 0;
                    for (int i = 0; i < curN; i++)
                    {
                        double px = curX[i], py = curY[i];
                        double qx = curX[(i + 1) % curN], qy = curY[(i + 1) % curN];
                        bool pin = LeftOf(ax, ay, bx, by, px, py);
                        bool qin = LeftOf(ax, ay, bx, by, qx, qy);
                        if (pin)
                        {
                            if (!qin) { Emit(nxtX, nxtY, ref nxtN, px, py); Emit(nxtX, nxtY, ref nxtN, Intersect(ax, ay, bx, by, px, py, qx, qy)); }
                            else Emit(nxtX, nxtY, ref nxtN, qx, qy);
                        }
                        else if (qin)
                        {
                            Emit(nxtX, nxtY, ref nxtN, Intersect(ax, ay, bx, by, px, py, qx, qy));
                            Emit(nxtX, nxtY, ref nxtN, qx, qy);
                        }
                    }
                    double* tX = curX; curX = nxtX; nxtX = tX;
                    double* tY = curY; curY = nxtY; nxtY = tY;
                    curN = nxtN;
                    if (curN == 0) return 0;
                }
                for (int i = 0; i < curN; i++) { ox[i] = curX[i]; oy[i] = curY[i]; }
                return curN;
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)curX); Marshal.FreeHGlobal((IntPtr)curY);
                Marshal.FreeHGlobal((IntPtr)nxtX); Marshal.FreeHGlobal((IntPtr)nxtY);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LeftOf(double ax, double ay, double bx, double by, double px, double py)
            => (bx - ax) * (py - ay) - (by - ay) * (px - ax) >= -1e-12;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (double, double) Intersect(double ax, double ay, double bx, double by, double px, double py, double qx, double qy)
        {
            double r1 = bx - ax, r2 = by - ay;
            double s1 = qx - px, s2 = qy - py;
            double denom = r1 * s2 - r2 * s1;
            double t = ((px - ax) * s2 - (py - ay) * s1) / denom;
            return (ax + t * r1, ay + t * r2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Emit(double* ox, double* oy, ref int n, double x, double y)
        { ox[n] = x; oy[n] = y; n++; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Emit(double* ox, double* oy, ref int n, (double x, double y) p)
        { ox[n] = p.x; oy[n] = p.y; n++; }

        // General (non-convex, possibly disjoint or hole-bearing) polygon boolean ops produce
        // non-convex / multi-component outputs that require a DCEL/arrangement + winding output
        // contract (Weiler-Atherton or Greiner-Hormann) and the §W3 exact predicates for the
        // degenerate collinear / vertex-on-edge cases. That topology + output-buffer contract is a
        // separate deliverable (see §W3 of FIXES_BACKLOG). Convex∩convex is handled by Intersection
        // above; Union/Difference/Xor are deferred until the DCEL output schema is fixed, because a
        // plausible-wrong partial implementation is worse than an honest throw with the contract gap.
        public static void Union()
            => throw new NotImplementedException(
                "General polygon union needs a DCEL/arrangement output contract (outer ring + holes, "
                + "winding order, multi-component) + §W3 exact predicates for degenerate cases. "
                + "Convex∩convex is available via Intersection; non-convex union/output topology is a "
                + "separate deliverable. Define the output schema first.");

        public static void Difference()
            => throw new NotImplementedException(
                "General polygon difference A\\B needs the DCEL output contract + hole handling + §W3 "
                + "predicates, same as Union. Defer until the output topology schema is fixed.");

        public static void Xor()
            => throw new NotImplementedException(
                "Polygon XOR = (A∪B)\\(A∩B); needs Union + the DCEL output contract + §W3 predicates. "
                + "Defer until the output topology schema is fixed.");
    }
}
