namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HalfSpaceIntersection
    {
        public struct HalfPlane { public double Nx, Ny, D, Angle; public int Id; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildIntersectionDeque(HalfPlane* scratchPlanes, int* scratchQ, int total, out int head, out int tail)
        {
            head = 0; tail = 0;
            scratchQ[tail++] = 0; scratchQ[tail++] = 1;
            for (int i = 2; i < total; i++)
            {
                while (head + 1 < tail && IsOutside(scratchPlanes[i], scratchPlanes[scratchQ[tail - 2]], scratchPlanes[scratchQ[tail - 1]])) tail--;
                while (head + 1 < tail && IsOutside(scratchPlanes[i], scratchPlanes[scratchQ[head]], scratchPlanes[scratchQ[head + 1]])) head++;
                scratchQ[tail++] = i;
            }
            while (head + 1 < tail && IsOutside(scratchPlanes[scratchQ[head]], scratchPlanes[scratchQ[tail - 2]], scratchPlanes[scratchQ[tail - 1]])) tail--;
        }

        public static int Run(double* nx, double* ny, double* d, int m, double* outX, double* outY, int* outSize, HalfPlane* scratchPlanes, int* scratchQ)
        {
            if (m == 0) { *outSize = 0; return 0; }
            int total = InitializePlanes(nx, ny, d, m, scratchPlanes);
            SortPlanesByAngle(scratchPlanes, total);
            total = UniquePlanes(scratchPlanes, total);

            if (total < 2) { *outSize = 0; return 0; }
            BuildIntersectionDeque(scratchPlanes, scratchQ, total, out int head, out int tail);

            if (tail - head < 3) { *outSize = 0; return 0; }
            return ExtractVertices(scratchPlanes, scratchQ, head, tail, outX, outY, outSize);
        }

        private static int InitializePlanes(double* nx, double* ny, double* d, int m, HalfPlane* planes)
        {
            for (int i = 0; i < m; i++) { planes[i] = new HalfPlane { Nx = nx[i], Ny = ny[i], D = d[i], Angle = Math.Atan2(ny[i], nx[i]), Id = i }; }
            double B = 1e9;
            planes[m + 0] = new HalfPlane { Nx = -1, Ny = 0, D = B, Angle = Math.Atan2(0, -1), Id = -1 };
            planes[m + 1] = new HalfPlane { Nx = 1, Ny = 0, D = B, Angle = Math.Atan2(0, 1), Id = -1 };
            planes[m + 2] = new HalfPlane { Nx = 0, Ny = -1, D = B, Angle = Math.Atan2(-1, 0), Id = -1 };
            planes[m + 3] = new HalfPlane { Nx = 0, Ny = 1, D = B, Angle = Math.Atan2(1, 0), Id = -1 };
            return m + 4;
        }

        private static void SortPlanesByAngle(HalfPlane* planes, int count)
        {
            for (int i = (count >> 1) - 1; i >= 0; i--) SiftDownAngle(planes, i, count);
            for (int end = count - 1; end > 0; end--)
            {
                HalfPlane t = planes[0]; planes[0] = planes[end]; planes[end] = t;
                SiftDownAngle(planes, 0, end);
            }
        }

        private static void SiftDownAngle(HalfPlane* a, int i, int n)
        {
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = i;
                if (l < n && a[l].Angle > a[m].Angle) m = l;
                if (r < n && a[r].Angle > a[m].Angle) m = r;
                if (m == i) break;
                HalfPlane t = a[i]; a[i] = a[m]; a[m] = t;
                i = m;
            }
        }

        private static int UniquePlanes(HalfPlane* planes, int count)
        {
            int k = 0;
            for (int i = 1; i < count; i++)
                if (Math.Abs(planes[i].Angle - planes[k].Angle) > 1e-9) planes[++k] = planes[i];
                else if (planes[i].D < planes[k].D) planes[k] = planes[i];
            return k + 1;
        }

        private static bool IsOutside(HalfPlane p, HalfPlane a, HalfPlane b)
        {
            Intersect(a, b, out double x, out double y);
            return p.Nx * x + p.Ny * y > p.D + 1e-9;
        }

        private static void Intersect(HalfPlane a, HalfPlane b, out double x, out double y)
        {
            double det = a.Nx * b.Ny - a.Ny * b.Nx;
            x = (a.D * b.Ny - b.D * a.Ny) / det; y = (a.Nx * b.D - b.Nx * a.D) / det;
        }

        private static int ExtractVertices(HalfPlane* planes, int* q, int h, int t, double* outX, double* outY, int* outSize)
        {
            int cnt = 0;
            for (int i = h; i < t; i++)
            {
                int next = (i + 1 == t) ? h : i + 1;
                Intersect(planes[q[i]], planes[q[next]], out outX[cnt], out outY[cnt]); cnt++;
            }
            *outSize = cnt; return cnt;
        }
    }
}
