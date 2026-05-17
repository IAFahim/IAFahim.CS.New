using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Imos
{
    public static unsafe class Imos2D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(int* diff, int width, int height, int r1, int c1, int r2, int c2, int val)
        {
            if (r1 < 0) r1 = 0;
            if (c1 < 0) c1 = 0;
            if (r2 >= height) r2 = height - 1;
            if (c2 >= width) c2 = width - 1;
            if (r1 > r2 || c1 > c2) return;

            diff[r1 * width + c1] += val;
            if (c2 + 1 < width)
                diff[r1 * width + c2 + 1] -= val;
            if (r2 + 1 < height)
                diff[(r2 + 1) * width + c1] -= val;
            if (r2 + 1 < height && c2 + 1 < width)
                diff[(r2 + 1) * width + c2 + 1] += val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(int* dst, int* diff, int width, int height)
        {
            for (int i = 0; i < width * height; i++)
                dst[i] = diff[i];
            for (int r = 0; r < height; r++)
            {
                for (int c = 1; c < width; c++)
                    dst[r * width + c] += dst[r * width + c - 1];
            }
            for (int r = 1; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                    dst[r * width + c] += dst[(r - 1) * width + c];
            }
        }
    }
}