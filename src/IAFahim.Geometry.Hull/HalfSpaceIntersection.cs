namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class HalfSpaceIntersection
    {
        public struct HalfPlane
        {
            public double Nx, Ny, D, Angle;
            public int Id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Intersect(HalfPlane a, HalfPlane b, out double x, out double y)
        {
            double det = a.Nx * b.Ny - a.Ny * b.Nx;
            x = (a.D * b.Ny - b.D * a.Ny) / det;
            y = (a.Nx * b.D - b.Nx * a.D) / det;
        }

        public static int Run(double* nx, double* ny, double* d, int m, double* outX, double* outY, int* outSize, HalfPlane* scratchPlanes, int* scratchQ)
        {
            if (m == 0) { *outSize = 0; return 0; }
            HalfPlane* planes = scratchPlanes;
            int* q = scratchQ;

            for (int i = 0; i < m; i++)
            {
                planes[i].Nx = nx[i];
                planes[i].Ny = ny[i];
                planes[i].D = d[i];
                planes[i].Angle = Math.Atan2(ny[i], nx[i]);
                planes[i].Id = i;
            }
            
            // Add bounding box
            int total = m;
            double B = 1e9;
            planes[total++] = new HalfPlane { Nx = -1, Ny = 0, D = B, Angle = Math.Atan2(0, -1), Id = -1 };
            planes[total++] = new HalfPlane { Nx = 1, Ny = 0, D = B, Angle = Math.Atan2(0, 1), Id = -1 };
            planes[total++] = new HalfPlane { Nx = 0, Ny = -1, D = B, Angle = Math.Atan2(-1, 0), Id = -1 };
            planes[total++] = new HalfPlane { Nx = 0, Ny = 1, D = B, Angle = Math.Atan2(1, 0), Id = -1 };

            // Sort by angle
            for (int i = 0; i < total - 1; i++)
            for (int j = i + 1; j < total; j++)
            {
                if (planes[i].Angle > planes[j].Angle)
                {
                    HalfPlane tmp = planes[i]; planes[i] = planes[j]; planes[j] = tmp;
                }
            }

            int k = 0;
            for (int i = 1; i < total; i++)
            {
                if (Math.Abs(planes[i].Angle - planes[k].Angle) > 1e-9)
                {
                    planes[++k] = planes[i];
                }
                else
                {
                    // keep the more restrictive one
                    if (planes[i].Nx * 0 + planes[i].Ny * 0 - planes[i].D < planes[k].Nx * 0 + planes[k].Ny * 0 - planes[k].D) // wait, not correct check
                    {
                        // we need to keep the one that is further in the normal direction.
                        // normal is (Nx, Ny). D is the offset.
                        // Eq: Nx * x + Ny * y <= D. Smaller D is more restrictive!
                        if (planes[i].D < planes[k].D) planes[k] = planes[i];
                    }
                }
            }
            total = k + 1;

            int head = 0, tail = 0;
            q[tail++] = 0;
            q[tail++] = 1;

            for (int i = 2; i < total; i++)
            {
                while (head + 1 < tail)
                {
                    Intersect(planes[q[tail - 2]], planes[q[tail - 1]], out double px, out double py);
                    if (planes[i].Nx * px + planes[i].Ny * py > planes[i].D + 1e-9) tail--;
                    else break;
                }
                while (head + 1 < tail)
                {
                    Intersect(planes[q[head]], planes[q[head + 1]], out double px, out double py);
                    if (planes[i].Nx * px + planes[i].Ny * py > planes[i].D + 1e-9) head++;
                    else break;
                }
                q[tail++] = i;
            }

            while (head + 1 < tail)
            {
                Intersect(planes[q[tail - 2]], planes[q[tail - 1]], out double px, out double py);
                if (planes[q[head]].Nx * px + planes[q[head]].Ny * py > planes[q[head]].D + 1e-9) tail--;
                else break;
            }

            if (tail - head < 3)
            {
                *outSize = 0;
                return 0;
            }

            int outCount = 0;
            for (int i = head; i < tail; i++)
            {
                int next = (i + 1 == tail) ? head : i + 1;
                Intersect(planes[q[i]], planes[q[next]], out double px, out double py);
                outX[outCount] = px;
                outY[outCount] = py;
                outCount++;
            }
            *outSize = outCount;
            return outCount;
        }
    }
}
