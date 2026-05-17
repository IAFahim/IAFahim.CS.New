using System;
using System.Runtime.CompilerServices;

namespace IAFahim.DS.Grid
{
    public static unsafe class Rotate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int width, int height, bool clockwise) where T : unmanaged
        {
            int newWidth = height;
            int newHeight = width;
            int len = width * height;
            T* temp = stackalloc T[len];
            for (int i = 0; i < len; i++)
            {
                temp[i] = ptr[i];
            }
            if (clockwise)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int srcIdx = y * width + x;
                        int dstIdx = x * height + (height - 1 - y);
                        ptr[dstIdx] = temp[srcIdx];
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int srcIdx = y * width + x;
                        int dstIdx = (width - 1 - x) * height + y;
                        ptr[dstIdx] = temp[srcIdx];
                    }
                }
            }
        }
    }
}