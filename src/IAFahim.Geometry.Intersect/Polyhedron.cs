namespace IAFahim.Geometry.Intersect
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Polyhedron
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Volume(double* xs, double* ys, double* zs, int* faces, int faceCount)
        {
            double vol = 0.0;
            for (int i = 0; i < faceCount; i++)
            {
                int a = faces[i * 3];
                int b = faces[i * 3 + 1];
                int c = faces[i * 3 + 2];

                double v1x = xs[b] - xs[a];
                double v1y = ys[b] - ys[a];
                double v1z = zs[b] - zs[a];

                double v2x = xs[c] - xs[a];
                double v2y = ys[c] - ys[a];
                double v2z = zs[c] - zs[a];

                double cx = v1y * v2z - v1z * v2y;
                double cy = v1z * v2x - v1x * v2z;
                double cz = v1x * v2y - v1y * v2x;

                vol += xs[a] * cx + ys[a] * cy + zs[a] * cz;
            }
            return Math.Abs(vol) / 6.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Faces(int vertexCount, int edgeCount)
        {
            return 2 - vertexCount + edgeCount;
        }
    }
}
