namespace IAFahim.Geometry.Delaunay
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class BowyerWatson
    {
        public struct Triangle
        {
            public int A, B, C;
        }

        // 2D Delaunay triangulation of n points (xs,ys). Writes triangles to outTris (cap outCap).
        // Returns triangle count. Uses Bowyer–Watson with a large super-triangle.
        public static int Triangulate(double* xs, double* ys, int n, Triangle* outTris, int outCap)
        {
            if (n < 3 || outCap <= 0) return 0;

            double minX = xs[0], maxX = xs[0], minY = ys[0], maxY = ys[0];
            for (int i = 1; i < n; i++)
            {
                if (xs[i] < minX) minX = xs[i];
                if (xs[i] > maxX) maxX = xs[i];
                if (ys[i] < minY) minY = ys[i];
                if (ys[i] > maxY) maxY = ys[i];
            }
            double dx = maxX - minX, dy = maxY - minY;
            double dmax = (dx > dy ? dx : dy) * 20 + 10;
            double midx = (minX + maxX) * 0.5, midy = (minY + maxY) * 0.5;

            // Super triangle vertices n, n+1, n+2
            double* px = (double*)Marshal.AllocHGlobal((n + 3) * sizeof(double));
            double* py = (double*)Marshal.AllocHGlobal((n + 3) * sizeof(double));
            for (int i = 0; i < n; i++) { px[i] = xs[i]; py[i] = ys[i]; }
            px[n] = midx - 2 * dmax; py[n] = midy - dmax;
            px[n + 1] = midx; py[n + 1] = midy + 2 * dmax;
            px[n + 2] = midx + 2 * dmax; py[n + 2] = midy - dmax;

            int maxT = n * 4 + 16;
            if (maxT < 16) maxT = 16;
            Triangle* tris = (Triangle*)Marshal.AllocHGlobal(maxT * sizeof(Triangle));
            byte* bad = (byte*)Marshal.AllocHGlobal(maxT);
            int tCount = 0;
            tris[tCount++] = new Triangle { A = n, B = n + 1, C = n + 2 };

            int* edgeA = (int*)Marshal.AllocHGlobal(maxT * 3 * sizeof(int));
            int* edgeB = (int*)Marshal.AllocHGlobal(maxT * 3 * sizeof(int));

            for (int pi = 0; pi < n; pi++)
            {
                int eCount = 0;
                for (int t = 0; t < tCount; t++) bad[t] = 0;
                for (int t = 0; t < tCount; t++)
                {
                    if (InCircumcircle(px, py, tris[t].A, tris[t].B, tris[t].C, pi))
                    {
                        bad[t] = 1;
                        AddEdge(edgeA, edgeB, ref eCount, tris[t].A, tris[t].B);
                        AddEdge(edgeA, edgeB, ref eCount, tris[t].B, tris[t].C);
                        AddEdge(edgeA, edgeB, ref eCount, tris[t].C, tris[t].A);
                    }
                }
                // Compact good triangles
                int w = 0;
                for (int t = 0; t < tCount; t++)
                    if (bad[t] == 0) tris[w++] = tris[t];
                tCount = w;

                // Unique boundary edges
                for (int e = 0; e < eCount; e++)
                {
                    bool dup = false;
                    for (int f = 0; f < eCount; f++)
                    {
                        if (e == f) continue;
                        if (edgeA[e] == edgeB[f] && edgeB[e] == edgeA[f]) { dup = true; break; }
                        if (edgeA[e] == edgeA[f] && edgeB[e] == edgeB[f] && e > f) { dup = true; break; }
                    }
                    if (dup) continue;
                    if (tCount < maxT)
                        tris[tCount++] = new Triangle { A = edgeA[e], B = edgeB[e], C = pi };
                }
            }

            int outN = 0;
            for (int t = 0; t < tCount && outN < outCap; t++)
            {
                int a = tris[t].A, b = tris[t].B, c = tris[t].C;
                if (a >= n || b >= n || c >= n) continue;
                outTris[outN++] = tris[t];
            }

            Marshal.FreeHGlobal((nint)edgeB);
            Marshal.FreeHGlobal((nint)edgeA);
            Marshal.FreeHGlobal((nint)bad);
            Marshal.FreeHGlobal((nint)tris);
            Marshal.FreeHGlobal((nint)py);
            Marshal.FreeHGlobal((nint)px);
            return outN;
        }

        private static void AddEdge(int* ea, int* eb, ref int n, int a, int b)
        {
            ea[n] = a; eb[n] = b; n++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InCircumcircle(double* x, double* y, int a, int b, int c, int p)
        {
            double ax = x[a] - x[p], ay = y[a] - y[p];
            double bx = x[b] - x[p], by = y[b] - y[p];
            double cx = x[c] - x[p], cy = y[c] - y[p];
            double det = (ax * ax + ay * ay) * (bx * cy - by * cx)
                       - (bx * bx + by * by) * (ax * cy - ay * cx)
                       + (cx * cx + cy * cy) * (ax * by - ay * bx);
            // Orientation of abc
            double orient = (x[b] - x[a]) * (y[c] - y[a]) - (y[b] - y[a]) * (x[c] - x[a]);
            if (orient < 0) det = -det;
            return det > 1e-12;
        }
    }
}
