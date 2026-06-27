namespace IAFahim.Geometry.Voronoi
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class Delaunay
    {
        private const double FlipEpsilon = 1e-12;

        public struct Triangle { public int A, B, C; }

        // O(n^4) global-check Delaunay: for every CCW triple, emit it iff no other point
        // lies inside its circumcircle. Correct and maximal for any input (the global check
        // is free of the incremental inconsistency that plagues Bowyer-Watson). Use this when
        // correctness matters; prefer BuildFast for large well-separated inputs.
        public static int Build(double* xs, double* ys, int n, Triangle* outTri)
        {
            if (n < 3) return 0;
            int cnt = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    for (int k = j + 1; k < n; k++)
                        if (TryAddTriangle(i, j, k, xs, ys, n, outTri, ref cnt)) { }
            return cnt;
        }

        private static bool TryAddTriangle(int i, int j, int k, double* xs, double* ys, int n, Triangle* outTri, ref int cnt)
        {
            double cross = (xs[j] - xs[i]) * (ys[k] - ys[i]) - (ys[j] - ys[i]) * (xs[k] - xs[i]);
            if (Math.Abs(cross) < 1e-9) return false;
            int u = i, v = j, w = k; if (cross < 0) { u = j; v = i; }
            if (IsDelaunay(u, v, w, xs, ys, n)) { outTri[cnt++] = new Triangle { A = u, B = v, C = w }; return true; }
            return false;
        }

        private static bool IsDelaunay(int u, int v, int w, double* xs, double* ys, int n)
        {
            for (int t = 0; t < n; t++)
                if (t != u && t != v && t != w && InCircle(xs[u], ys[u], xs[v], ys[v], xs[w], ys[w], xs[t], ys[t])) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InCircle(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            double adx = ax - dx, ady = ay - dy, bdx = bx - dx, bdy = by - dy, cdx = cx - dx, cdy = cy - dy;
            double abdet = adx * bdy - bdx * ady, bcdet = bdx * cdy - cdx * bdy, cadet = cdx * ady - adx * cdy;
            return (adx * adx + ady * ady) * bcdet + (bdx * bdx + bdy * bdy) * cadet + (cdx * cdx + cdy * cdy) * abdet > 1e-9;
        }

        // O(n^2) incremental Bowyer-Watson (super-triangle + bad-cavity retriangulation).
        // Much faster than Build for large n. ROBUSTNESS CAVEAT: uses the inexact double
        // InCircle predicate, so on near-cocircular / degenerate inputs an adjacent triangle
        // can be misclassified, leaving the result a valid (every triangle locally Delaunay)
        // but NON-MAXIMAL triangulation (a coverage hole). For guaranteed-maximal output on
        // arbitrary input use Build, or extend §W3 with an exact 192-bit incircle predicate.
        public static int BuildFast(double* xs, double* ys, int n, Triangle* outTri)
        {
            return BowyerWatson.Build(xs, ys, n, outTri);
        }

        // Edge flip with adjacency update. triangles[t1],triangles[t2] share an interior edge;
        // adj encodes triangle adjacency as adj[t*3+s] = neighbor across side s, where side s is the
        // edge OPPOSITE vertex s of triangle t (-1 = boundary). If the shared edge violates the
        // Delaunay condition (the opposite vertex of one triangle lies strictly inside the other's
        // circumcircle), the shared diagonal is flipped to the other diagonal of the enclosing quad.
        // Returns true iff a flip was performed. Both triangles are rewritten CCW and adj is patched
        // for t1, t2 and their (up to) four outer neighbors. Caller guarantees t1,t2 share exactly
        // one edge and both adj entries are mutual.
        public static bool Flip(double* xs, double* ys, Triangle* triangles, int* adj, int t1, int t2)
        {
            Triangle T1 = triangles[t1];
            Triangle T2 = triangles[t2];

            int s1 = SharedSide(adj, t1, t2);   // side of t1 whose neighbor is t2 (opposite vertex A)
            int s2 = SharedSide(adj, t2, t1);   // side of t2 whose neighbor is t1 (opposite vertex B)
            if (s1 < 0 || s2 < 0) return false;

            int A = VertexAt(T1, s1);                       // opposite vertex in t1
            int U = VertexAt(T1, (s1 + 1) % 3);
            int V = VertexAt(T1, (s1 + 2) % 3);             // shared edge endpoints (t1's order)
            int B = VertexAt(T2, s2);                       // opposite vertex in t2
            // match U,V against t2's shared-edge endpoints (may be swapped)
            int bU = VertexAt(T2, (s2 + 1) % 3);
            int bV = VertexAt(T2, (s2 + 2) % 3);
            bool swap = (bU == V && bV == U);

            double xa = xs[A], ya = ys[A], xu = xs[U], yu = ys[U], xv = xs[V], yv = ys[V], xb = xs[B], yb = ys[B];

            // Flip iff B is strictly inside circumcircle of triangle (U,V,A) — orient-corrected.
            double orient = (xu - xa) * (yv - ya) - (yu - ya) * (xv - xa);
            double inc = IncircleSign(xu, yu, xv, yv, xa, ya, xb, yb);
            double signed = orient > 0 ? inc : -inc;
            if (signed <= FlipEpsilon) return false;

            // Outer neighbors (before rewrite): edges that survive into the new triangles.
            // nAU = neighbor of t1 across edge (A,U) = side opposite V in t1.
            int posV1 = (s1 + 2) % 3;
            int posU1 = (s1 + 1) % 3;
            int nAU = adj[t1 * 3 + posV1];   // edge (A,U) opposite V
            int nAV = adj[t1 * 3 + posU1];   // edge (A,V) opposite U
            int posV2 = (s2 + 2) % 3;
            int posU2 = (s2 + 1) % 3;
            int nBU = adj[t2 * 3 + (swap ? posU2 : posV2)]; // edge (B,U)
            int nBV = adj[t2 * 3 + (swap ? posV2 : posU2)]; // edge (B,V)

            // Rewrite triangles with new diagonal (A,B), both CCW.
            triangles[t1] = MakeCcw(xs, ys, A, B, U);
            triangles[t2] = MakeCcw(xs, ys, A, B, V);

            // Rebuild adj by matching each triangle's actual edges to its neighbors (order-independent).
            AssignAdj(adj, t1, triangles[t1], B, U, nBU, U, A, nAU, A, B, t2);
            AssignAdj(adj, t2, triangles[t2], B, V, nBV, V, A, nAV, A, B, t1);

            // Patch the four outer neighbors to point at their new partner.
            PatchBack(adj, nBU, t2, t1);
            PatchBack(adj, nAU, t2, t1);
            PatchBack(adj, nBV, t1, t2);
            PatchBack(adj, nAV, t1, t2);
            return true;
        }

        // For triangle `ti` with stored vertices, set adj[ti*3+side] for three edges given as
        // (eU,eV,neighbor) triples. side = index of the vertex NOT on that edge.
        private static void AssignAdj(int* adj, int ti, Triangle T,
                                      int e0u, int e0v, int n0,
                                      int e1u, int e1v, int n1,
                                      int e2u, int e2v, int n2)
        {
            int[] v = { T.A, T.B, T.C };
            SetEdge(adj, ti, v, e0u, e0v, n0);
            SetEdge(adj, ti, v, e1u, e1v, n1);
            SetEdge(adj, ti, v, e2u, e2v, n2);
        }

        private static void SetEdge(int* adj, int ti, int[] v, int eu, int ev, int nb)
        {
            for (int s = 0; s < 3; s++)
            {
                int o1 = v[(s + 1) % 3], o2 = v[(s + 2) % 3];
                if ((o1 == eu && o2 == ev) || (o1 == ev && o2 == eu))
                { adj[ti * 3 + s] = nb; return; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SharedSide(int* adj, int from, int to)
        {
            for (int s = 0; s < 3; s++) if (adj[from * 3 + s] == to) return s;
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int VertexAt(Triangle t, int i)
            => i == 0 ? t.A : (i == 1 ? t.B : t.C);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PatchBack(int* adj, int neighbor, int oldPartner, int newPartner)
        {
            if (neighbor < 0) return;
            for (int s = 0; s < 3; s++)
                if (adj[neighbor * 3 + s] == oldPartner) { adj[neighbor * 3 + s] = newPartner; return; }
        }

        private static Triangle MakeCcw(double* xs, double* ys, int p, int q, int r)
        {
            double cross = (xs[q] - xs[p]) * (ys[r] - ys[p]) - (ys[q] - ys[p]) * (xs[r] - xs[p]);
            return cross < 0 ? new Triangle { A = p, B = r, C = q } : new Triangle { A = p, B = q, C = r };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double IncircleSign(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            double adx = ax - dx, ady = ay - dy, bdx = bx - dx, bdy = by - dy, cdx = cx - dx, cdy = cy - dy;
            double abdet = adx * bdy - bdx * ady;
            double bcdet = bdx * cdy - cdx * bdy;
            double cadet = cdx * ady - adx * cdy;
            return (adx * adx + ady * ady) * bcdet + (bdx * bdx + bdy * bdy) * cadet + (cdx * cdx + cdy * cdy) * abdet;
        }
    }

    public static unsafe class BowyerWatson
    {
        public static int Build(double* xs, double* ys, int n, Delaunay.Triangle* outTri)
        {
            if (n < 3) return 0;
            if (!HasNonCollinearTriple(xs, ys, n)) return 0;
            BoundingBox(xs, ys, n, out double minX, out double minY, out double maxX, out double maxY);
            double dx = maxX - minX, dy = maxY - minY;
            double dmax = dx > dy ? dx : dy;
            if (dmax < 1e-9) dmax = 1.0;
            double midX = (minX + maxX) * 0.5, midY = (minY + maxY) * 0.5;
            double big = dmax * 20.0;
            int total = n + 3;
            double* px = (double*)Marshal.AllocHGlobal(sizeof(double) * total);
            double* py = (double*)Marshal.AllocHGlobal(sizeof(double) * total);
            int cap = 2 * n + 32;
            int* ta = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
            int* tb = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
            int* tc = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
            byte* alive = (byte*)Marshal.AllocHGlobal(sizeof(byte) * cap);
            int* bad = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
            int* edgeU = (int*)Marshal.AllocHGlobal(sizeof(int) * cap * 3);
            int* edgeV = (int*)Marshal.AllocHGlobal(sizeof(int) * cap * 3);
            try
            {
                for (int i = 0; i < n; i++) { px[i] = xs[i]; py[i] = ys[i]; }
                px[n] = midX - big; py[n] = minY - big;
                px[n + 1] = midX + big; py[n + 1] = minY - big * 0.5;
                px[n + 2] = midX; py[n + 2] = maxY + big;
                int cnt = 1;
                ta[0] = n; tb[0] = n + 1; tc[0] = n + 2; alive[0] = 1;
                for (int p = 0; p < n; p++)
                {
                    int badCnt = 0;
                    for (int t = 0; t < cnt; t++)
                    {
                        if (alive[t] == 0) continue;
                        if (InCircle(px[ta[t]], py[ta[t]], px[tb[t]], py[tb[t]], px[tc[t]], py[tc[t]], px[p], py[p]))
                        { bad[badCnt++] = t; alive[t] = 0; }
                    }
                    int edgeCnt = 0;
                    for (int bi = 0; bi < badCnt; bi++)
                    {
                        int t = bad[bi];
                        AddBoundaryEdge(ta[t], tb[t], t, bad, badCnt, ta, tb, tc, edgeU, edgeV, ref edgeCnt);
                        AddBoundaryEdge(tb[t], tc[t], t, bad, badCnt, ta, tb, tc, edgeU, edgeV, ref edgeCnt);
                        AddBoundaryEdge(tc[t], ta[t], t, bad, badCnt, ta, tb, tc, edgeU, edgeV, ref edgeCnt);
                    }
                    for (int e = 0; e < edgeCnt; e++)
                    {
                        if (cnt >= cap) { cap *= 2; GrowAll(ref ta, ref tb, ref tc, ref alive, ref bad, ref edgeU, ref edgeV, cap); }
                        ta[cnt] = edgeU[e]; tb[cnt] = edgeV[e]; tc[cnt] = p; alive[cnt] = 1; cnt++;
                    }
                }
                int outCount = 0;
                for (int t = 0; t < cnt; t++)
                {
                    if (alive[t] == 0) continue;
                    if (ta[t] >= n || tb[t] >= n || tc[t] >= n) continue;
                    outTri[outCount++] = new Delaunay.Triangle { A = ta[t], B = tb[t], C = tc[t] };
                }
                return outCount;
            }
            finally
            {
                Marshal.FreeHGlobal((System.IntPtr)px);
                Marshal.FreeHGlobal((System.IntPtr)py);
                Marshal.FreeHGlobal((System.IntPtr)ta);
                Marshal.FreeHGlobal((System.IntPtr)tb);
                Marshal.FreeHGlobal((System.IntPtr)tc);
                Marshal.FreeHGlobal((System.IntPtr)alive);
                Marshal.FreeHGlobal((System.IntPtr)bad);
                Marshal.FreeHGlobal((System.IntPtr)edgeU);
                Marshal.FreeHGlobal((System.IntPtr)edgeV);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddBoundaryEdge(int u, int v, int self, int* bad, int badCnt,
                                            int* ta, int* tb, int* tc, int* edgeU, int* edgeV, ref int edgeCnt)
        {
            bool shared = false;
            for (int bi = 0; bi < badCnt; bi++)
            {
                int t = bad[bi];
                if (t == self) continue;
                int a = ta[t], b = tb[t], c = tc[t];
                int cnt = 0;
                if (a == u || a == v) cnt++;
                if (b == u || b == v) cnt++;
                if (c == u || c == v) cnt++;
                if (cnt == 2) { shared = true; break; }
            }
            if (!shared) { edgeU[edgeCnt] = u; edgeV[edgeCnt] = v; edgeCnt++; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasNonCollinearTriple(double* xs, double* ys, int n)
        {
            int p0 = 0, p1 = 1;
            while (p1 < n && xs[p0] == xs[p1] && ys[p0] == ys[p1]) p1++;
            if (p1 >= n) return false;
            for (int i = p1 + 1; i < n; i++)
            {
                double cross = (xs[p1] - xs[p0]) * (ys[i] - ys[p0]) - (ys[p1] - ys[p0]) * (xs[i] - xs[p0]);
                if (cross != 0.0) return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BoundingBox(double* xs, double* ys, int n, out double minX, out double minY, out double maxX, out double maxY)
        {
            minX = maxX = xs[0]; minY = maxY = ys[0];
            for (int i = 1; i < n; i++)
            {
                if (xs[i] < minX) minX = xs[i]; else if (xs[i] > maxX) maxX = xs[i];
                if (ys[i] < minY) minY = ys[i]; else if (ys[i] > maxY) maxY = ys[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InCircle(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            double adx = ax - dx, ady = ay - dy, bdx = bx - dx, bdy = by - dy, cdx = cx - dx, cdy = cy - dy;
            double abdet = adx * bdy - bdx * ady;
            double bcdet = bdx * cdy - cdx * bdy;
            double cadet = cdx * ady - adx * cdy;
            double det = (adx * adx + ady * ady) * bcdet + (bdx * bdx + bdy * bdy) * cadet + (cdx * cdx + cdy * cdy) * abdet;
            double orient = (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
            if (orient > 0) return det > 1e-18;
            if (orient < 0) return det < -1e-18;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GrowAll(ref int* ta, ref int* tb, ref int* tc, ref byte* alive,
                                    ref int* bad, ref int* edgeU, ref int* edgeV, int cap)
        {
            int oldCap = cap >> 1;
            ta = ReallocInt(ta, oldCap, cap);
            tb = ReallocInt(tb, oldCap, cap);
            tc = ReallocInt(tc, oldCap, cap);
            alive = ReallocByte(alive, oldCap, cap);
            bad = ReallocInt(bad, oldCap, cap);
            edgeU = ReallocInt(edgeU, oldCap * 3, cap * 3);
            edgeV = ReallocInt(edgeV, oldCap * 3, cap * 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int* ReallocInt(int* old, int oldCount, int newCount)
        {
            int* block = (int*)Marshal.AllocHGlobal(sizeof(int) * newCount);
            for (int i = 0; i < oldCount; i++) block[i] = old[i];
            Marshal.FreeHGlobal((System.IntPtr)old);
            return block;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte* ReallocByte(byte* old, int oldCount, int newCount)
        {
            byte* block = (byte*)Marshal.AllocHGlobal(sizeof(byte) * newCount);
            for (int i = 0; i < oldCount; i++) block[i] = old[i];
            Marshal.FreeHGlobal((System.IntPtr)old);
            return block;
        }
    }
}

