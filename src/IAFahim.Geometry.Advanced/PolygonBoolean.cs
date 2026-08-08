namespace IAFahim.Geometry.Advanced
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class PolygonBoolean
    {
        private const double Eps = 1e-12;
        private const double EpsLoose = 1e-9;

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
                        if (!qin)
                        {
                            Emit(nxtX, nxtY, ref nxtN, px, py);
                            Intersect(ax, ay, bx, by, px, py, qx, qy, out double ix, out double iy);
                            Emit(nxtX, nxtY, ref nxtN, ix, iy);
                        }
                        else Emit(nxtX, nxtY, ref nxtN, qx, qy);
                    }
                    else if (qin)
                    {
                        Intersect(ax, ay, bx, by, px, py, qx, qy, out double ix, out double iy);
                        Emit(nxtX, nxtY, ref nxtN, ix, iy);
                        Emit(nxtX, nxtY, ref nxtN, qx, qy);
                    }
                }
                double* tX = curX; curX = nxtX; nxtX = tX;
                double* tY = curY; curY = nxtY; nxtY = tY;
                curN = nxtN;
                if (curN == 0)
                {
                    Marshal.FreeHGlobal((nint)curX); Marshal.FreeHGlobal((nint)curY);
                    Marshal.FreeHGlobal((nint)nxtX); Marshal.FreeHGlobal((nint)nxtY);
                    return 0;
                }
            }
            for (int i = 0; i < curN; i++) { ox[i] = curX[i]; oy[i] = curY[i]; }
            int ret = curN;
            Marshal.FreeHGlobal((nint)curX); Marshal.FreeHGlobal((nint)curY);
            Marshal.FreeHGlobal((nint)nxtX); Marshal.FreeHGlobal((nint)nxtY);
            return ret;
        }

        public static int Union(double* ax, double* ay, int an, double* bx, double* by, int bn,
                                double* ox, double* oy, int outCap)
        {
            if (an < 3 || bn < 3 || outCap < 3) return 0;
            if (PolysEqual(ax, ay, an, bx, by, bn)) return CopyPoly(ax, ay, an, ox, oy, outCap);
            if (AllInside(ax, ay, an, bx, by, bn)) return CopyPoly(bx, by, bn, ox, oy, outCap);
            if (AllInside(bx, by, bn, ax, ay, an)) return CopyPoly(ax, ay, an, ox, oy, outCap);
            if (!EdgesCross(ax, ay, an, bx, by, bn)) return CopyPoly(ax, ay, an, ox, oy, outCap);
            return ConvexUnionRing(ax, ay, an, bx, by, bn, ox, oy, outCap);
        }

        public static int Difference(double* ax, double* ay, int an, double* bx, double* by, int bn,
                                     double* ox, double* oy, int outCap)
        {
            if (an < 3 || bn < 3 || outCap < 3) return 0;
            if (PolysEqual(ax, ay, an, bx, by, bn)) return 0;
            if (AllInside(ax, ay, an, bx, by, bn)) return 0;
            if (AllInside(bx, by, bn, ax, ay, an)) return CopyPoly(ax, ay, an, ox, oy, outCap);
            if (!EdgesCross(ax, ay, an, bx, by, bn)) return CopyPoly(ax, ay, an, ox, oy, outCap);
            return ConvexDifferenceRing(ax, ay, an, bx, by, bn, ox, oy, outCap);
        }

        public static int Xor(double* ax, double* ay, int an, double* bx, double* by, int bn,
                              double* ox, double* oy, int outCap)
        {
            if (an < 3 || bn < 3 || outCap < 3) return 0;
            if (PolysEqual(ax, ay, an, bx, by, bn)) return 0;
            if (AllInside(ax, ay, an, bx, by, bn)) return ConvexDifferenceRing(bx, by, bn, ax, ay, an, ox, oy, outCap);
            if (AllInside(bx, by, bn, ax, ay, an)) return ConvexDifferenceRing(ax, ay, an, bx, by, bn, ox, oy, outCap);
            if (!EdgesCross(ax, ay, an, bx, by, bn)) return CopyPoly(ax, ay, an, ox, oy, outCap);
            return ConvexDifferenceRing(ax, ay, an, bx, by, bn, ox, oy, outCap);
        }

        private static int ConvexUnionRing(
            double* ax, double* ay, int an, double* bx, double* by, int bn,
            double* ox, double* oy, int outCap)
        {
            int maxV = an + bn + 2 * (an + bn) + 8;
            double* vx = (double*)Marshal.AllocHGlobal(maxV * sizeof(double));
            double* vy = (double*)Marshal.AllocHGlobal(maxV * sizeof(double));
            int vc = 0;
            for (int i = 0; i < an; i++)
                if (!PointInConvexStrict(bx, by, bn, ax[i], ay[i]))
                    PushUnique(vx, vy, ref vc, maxV, ax[i], ay[i]);
            for (int i = 0; i < bn; i++)
                if (!PointInConvexStrict(ax, ay, an, bx[i], by[i]))
                    PushUnique(vx, vy, ref vc, maxV, bx[i], by[i]);
            CollectIntersections(ax, ay, an, bx, by, bn, vx, vy, ref vc, maxV);
            if (vc < 3)
            {
                Marshal.FreeHGlobal((nint)vx); Marshal.FreeHGlobal((nint)vy);
                return 0;
            }

            if (!TryCentroidInIntersection(ax, ay, an, bx, by, bn, out double cx, out double cy))
            {
                cx = 0; cy = 0;
                for (int i = 0; i < vc; i++) { cx += vx[i]; cy += vy[i]; }
                cx /= vc; cy /= vc;
            }

            int n = PolarOrderEmit(vx, vy, vc, cx, cy, ox, oy, outCap);
            Marshal.FreeHGlobal((nint)vx); Marshal.FreeHGlobal((nint)vy);
            return n;
        }

        private static int ConvexDifferenceRing(
            double* ax, double* ay, int an, double* bx, double* by, int bn,
            double* ox, double* oy, int outCap)
        {
            int maxV = an + bn + 2 * (an + bn) + 8;
            double* vx = (double*)Marshal.AllocHGlobal(maxV * sizeof(double));
            double* vy = (double*)Marshal.AllocHGlobal(maxV * sizeof(double));
            int vc = 0;
            for (int i = 0; i < an; i++)
                if (!PointInConvexStrict(bx, by, bn, ax[i], ay[i]))
                    PushUnique(vx, vy, ref vc, maxV, ax[i], ay[i]);
            for (int i = 0; i < bn; i++)
                if (PointInConvexStrict(ax, ay, an, bx[i], by[i]))
                    PushUnique(vx, vy, ref vc, maxV, bx[i], by[i]);
            CollectIntersections(ax, ay, an, bx, by, bn, vx, vy, ref vc, maxV);
            if (vc < 3)
            {
                Marshal.FreeHGlobal((nint)vx); Marshal.FreeHGlobal((nint)vy);
                return 0;
            }

            double cx = 0, cy = 0;
            int outCnt = 0;
            for (int i = 0; i < an; i++)
            {
                if (!PointInConvex(bx, by, bn, ax[i], ay[i]))
                {
                    cx += ax[i]; cy += ay[i]; outCnt++;
                }
            }
            if (outCnt > 0)
            {
                cx /= outCnt; cy /= outCnt;
            }
            else
            {
                for (int i = 0; i < an; i++) { cx += ax[i]; cy += ay[i]; }
                cx /= an; cy /= an;
            }
            if (PointInConvex(bx, by, bn, cx, cy) || outCnt == 1)
            {
                double acx = 0, acy = 0;
                for (int i = 0; i < an; i++) { acx += ax[i]; acy += ay[i]; }
                acx /= an; acy /= an;
                cx = 0.5 * cx + 0.5 * acx;
                cy = 0.5 * cy + 0.5 * acy;
                if (PointInConvex(bx, by, bn, cx, cy))
                {
                    for (int i = 0; i < vc; i++)
                    {
                        double mx = 0.5 * (vx[i] + acx), my = 0.5 * (vy[i] + acy);
                        if (PointInConvex(ax, ay, an, mx, my) && !PointInConvex(bx, by, bn, mx, my))
                        { cx = mx; cy = my; break; }
                    }
                }
            }

            int n = PolarOrderEmit(vx, vy, vc, cx, cy, ox, oy, outCap);
            Marshal.FreeHGlobal((nint)vx); Marshal.FreeHGlobal((nint)vy);
            return n;
        }

        private static bool TryCentroidInIntersection(
            double* ax, double* ay, int an, double* bx, double* by, int bn,
            out double cx, out double cy)
        {
            cx = cy = 0;
            int cap = 2 * (an + bn) + 8;
            double* ix = (double*)Marshal.AllocHGlobal(cap * sizeof(double));
            double* iy = (double*)Marshal.AllocHGlobal(cap * sizeof(double));
            int n = Intersection(ax, ay, an, bx, by, bn, ix, iy);
            if (n < 3)
            {
                Marshal.FreeHGlobal((nint)ix); Marshal.FreeHGlobal((nint)iy);
                for (int i = 0; i < an; i++)
                {
                    if (PointInConvex(bx, by, bn, ax[i], ay[i]))
                    { cx = ax[i]; cy = ay[i]; return true; }
                }
                for (int i = 0; i < bn; i++)
                {
                    if (PointInConvex(ax, ay, an, bx[i], by[i]))
                    { cx = bx[i]; cy = by[i]; return true; }
                }
                return false;
            }
            for (int i = 0; i < n; i++) { cx += ix[i]; cy += iy[i]; }
            cx /= n; cy /= n;
            Marshal.FreeHGlobal((nint)ix); Marshal.FreeHGlobal((nint)iy);
            return true;
        }

        private static void CollectIntersections(
            double* ax, double* ay, int an, double* bx, double* by, int bn,
            double* vx, double* vy, ref int vc, int maxV)
        {
            for (int i = 0; i < an; i++)
            {
                int i2 = (i + 1) % an;
                for (int j = 0; j < bn; j++)
                {
                    int j2 = (j + 1) % bn;
                    if (SegIntersect(ax[i], ay[i], ax[i2], ay[i2], bx[j], by[j], bx[j2], by[j2],
                        out double ix, out double iy, out double ta, out double tb) &&
                        ta > Eps && ta < 1 - Eps && tb > Eps && tb < 1 - Eps)
                        PushUnique(vx, vy, ref vc, maxV, ix, iy);
                }
            }
        }

        private static int PolarOrderEmit(
            double* vx, double* vy, int vc, double cx, double cy,
            double* ox, double* oy, int outCap)
        {
            int* idx = (int*)Marshal.AllocHGlobal(vc * sizeof(int));
            for (int i = 0; i < vc; i++) idx[i] = i;
            for (int i = 1; i < vc; i++)
            {
                int key = idx[i];
                double ka = Math.Atan2(vy[key] - cy, vx[key] - cx);
                int j = i - 1;
                while (j >= 0 && Math.Atan2(vy[idx[j]] - cy, vx[idx[j]] - cx) > ka)
                {
                    idx[j + 1] = idx[j];
                    j--;
                }
                idx[j + 1] = key;
            }
            int n = vc < outCap ? vc : outCap;
            for (int i = 0; i < n; i++) { ox[i] = vx[idx[i]]; oy[i] = vy[idx[i]]; }
            Marshal.FreeHGlobal((nint)idx);
            return n >= 3 ? n : 0;
        }

        private static void PushUnique(double* vx, double* vy, ref int vc, int maxV, double x, double y)
        {
            if (vc >= maxV) return;
            for (int i = 0; i < vc; i++)
                if (Dist2(vx[i], vy[i], x, y) < 1e-20) return;
            vx[vc] = x; vy[vc] = y; vc++;
        }

        private static bool PointInConvex(double* px, double* py, int n, double x, double y)
        {
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                if ((px[j] - px[i]) * (y - py[i]) - (py[j] - py[i]) * (x - px[i]) < -EpsLoose) return false;
            }
            return true;
        }

        private static bool PointInConvexStrict(double* px, double* py, int n, double x, double y)
        {
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                if ((px[j] - px[i]) * (y - py[i]) - (py[j] - py[i]) * (x - px[i]) < EpsLoose) return false;
            }
            return true;
        }

        private static bool AllInside(double* ax, double* ay, int an, double* bx, double* by, int bn)
        {
            for (int i = 0; i < an; i++)
                if (!PointInConvex(bx, by, bn, ax[i], ay[i])) return false;
            return true;
        }

        private static bool EdgesCross(double* ax, double* ay, int an, double* bx, double* by, int bn)
        {
            for (int i = 0; i < an; i++)
            {
                int i2 = (i + 1) % an;
                for (int j = 0; j < bn; j++)
                {
                    int j2 = (j + 1) % bn;
                    if (SegIntersect(ax[i], ay[i], ax[i2], ay[i2], bx[j], by[j], bx[j2], by[j2],
                        out _, out _, out double ta, out double tb) &&
                        ta > Eps && ta < 1 - Eps && tb > Eps && tb < 1 - Eps)
                        return true;
                }
            }
            return false;
        }

        private static bool SegIntersect(
            double ax, double ay, double bx, double by,
            double cx, double cy, double dx, double dy,
            out double ix, out double iy, out double alphaA, out double alphaB)
        {
            ix = iy = alphaA = alphaB = 0;
            double rx = bx - ax, ry = by - ay;
            double sx = dx - cx, sy = dy - cy;
            double den = rx * sy - ry * sx;
            if (Math.Abs(den) < Eps) return false;
            double qpx = cx - ax, qpy = cy - ay;
            double t = (qpx * sy - qpy * sx) / den;
            double u = (qpx * ry - qpy * rx) / den;
            if (t < -Eps || t > 1 + Eps || u < -Eps || u > 1 + Eps) return false;
            alphaA = t; alphaB = u;
            ix = ax + t * rx;
            iy = ay + t * ry;
            return true;
        }

        private static bool PolysEqual(double* ax, double* ay, int an, double* bx, double* by, int bn)
        {
            if (an != bn) return false;
            for (int s = 0; s < an; s++)
            {
                bool ok = true;
                for (int i = 0; i < an; i++)
                {
                    int j = (s + i) % an;
                    if (Math.Abs(ax[i] - bx[j]) > EpsLoose || Math.Abs(ay[i] - by[j]) > EpsLoose) { ok = false; break; }
                }
                if (ok) return true;
            }
            return false;
        }

        private static int CopyPoly(double* px, double* py, int n, double* ox, double* oy, int outCap)
        {
            int m = n < outCap ? n : outCap;
            for (int i = 0; i < m; i++) { ox[i] = px[i]; oy[i] = py[i]; }
            return m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Dist2(double x0, double y0, double x1, double y1)
        {
            double dx = x0 - x1, dy = y0 - y1;
            return dx * dx + dy * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LeftOf(double ax, double ay, double bx, double by, double px, double py)
            => (bx - ax) * (py - ay) - (by - ay) * (px - ax) >= -1e-12;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Emit(double* ox, double* oy, ref int n, double xv, double yv)
        {
            ox[n] = xv; oy[n] = yv; n++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Intersect(
            double ax, double ay, double bx, double by, double px, double py, double qx, double qy,
            out double ix, out double iy)
        {
            double r1 = bx - ax, r2 = by - ay;
            double s1 = qx - px, s2 = qy - py;
            double denom = r1 * s2 - r2 * s1;
            double t = ((px - ax) * s2 - (py - ay) * s1) / denom;
            ix = ax + t * r1;
            iy = ay + t * r2;
        }
    }
}
