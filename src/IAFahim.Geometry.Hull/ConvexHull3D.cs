namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ConvexHull3D
    {
        public struct Face { public int A, B, C; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Build(double* xs, double* ys, double* zs, int n, Face* outFaces)
        {
            if (n < 4) return 0;
            bool* taken = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) taken[i] = false;
            int faceCount = 0;
            for (int i = 0; i < n && faceCount < n * 2; i++)
            for (int j = i + 1; j < n && faceCount < n * 2; j++)
            for (int k = j + 1; k < n && faceCount < n * 2; k++)
            {
                if (taken[i * n + j] || taken[i * n + k] || taken[j * n + k]) continue;
                double ux = ys[i] * (zs[j] - zs[k]) - ys[j] * (zs[i] - zs[k]) + ys[k] * (zs[i] - zs[j]);
                double uy = -(xs[i] * (zs[j] - zs[k]) - xs[j] * (zs[i] - zs[k]) + xs[k] * (zs[i] - zs[j]));
                double uz = xs[i] * (ys[j] - ys[k]) - xs[j] * (ys[i] - ys[k]) + xs[k] * (ys[i] - ys[j]);
                double cx = (xs[i] + xs[j] + xs[k]) / 3.0;
                double cy = (ys[i] + ys[j] + ys[k]) / 3.0;
                double cz = (zs[i] + zs[j] + zs[k]) / 3.0;
                bool allPos = true, allNeg = true;
                for (int t = 0; t < n; t++)
                {
                    if (t == i || t == j || t == k) continue;
                    double dx = xs[t] - cx, dy = ys[t] - cy, dz = zs[t] - cz;
                    double dot = dx * ux + dy * uy + dz * uz;
                    if (dot > 1e-9) allNeg = false;
                    if (dot < -1e-9) allPos = false;
                }
                if (allPos || allNeg)
                {
                    taken[i * n + j] = taken[i * n + k] = taken[j * n + k] = true;
                    outFaces[faceCount].A = i; outFaces[faceCount].B = j; outFaces[faceCount].C = k;
                    faceCount++;
                }
            }
            return faceCount;
        }
    }
}
