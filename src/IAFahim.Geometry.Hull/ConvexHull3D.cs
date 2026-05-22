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
            int faceCount = 0;
            for (int i = 0; i < n && faceCount < n * 2; i++)
            for (int j = i + 1; j < n && faceCount < n * 2; j++)
            for (int k = j + 1; k < n && faceCount < n * 2; k++)
            {
                double ux = (ys[j] - ys[i]) * (zs[k] - zs[i]) - (zs[j] - zs[i]) * (ys[k] - ys[i]);
                double uy = (zs[j] - zs[i]) * (xs[k] - xs[i]) - (xs[j] - xs[i]) * (zs[k] - zs[i]);
                double uz = (xs[j] - xs[i]) * (ys[k] - ys[i]) - (ys[j] - ys[i]) * (xs[k] - xs[i]);
                double ulen = Math.Sqrt(ux * ux + uy * uy + uz * uz);
                if (ulen < 1e-12) continue;
                ux /= ulen; uy /= ulen; uz /= ulen;
                bool allPos = true, allNeg = true;
                for (int t = 0; t < n; t++)
                {
                    if (t == i || t == j || t == k) continue;
                    double dot = (xs[t] - xs[i]) * ux + (ys[t] - ys[i]) * uy + (zs[t] - zs[i]) * uz;
                    if (dot > 1e-9) allPos = false;
                    if (dot < -1e-9) allNeg = false;
                }
                if (allPos || allNeg)
                {
                    outFaces[faceCount].A = i; outFaces[faceCount].B = j; outFaces[faceCount].C = k;
                    faceCount++;
                }
            }
            return faceCount;
        }
    }
}
