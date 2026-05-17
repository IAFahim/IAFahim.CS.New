using System;
using System.Runtime.CompilerServices;

namespace IAFahim.DS.Grid
{
    public static unsafe class MakeGrid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* ptr, int len, int width, int height)
        {
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ptr[index++] = y * width + x;
                }
            }
        }
    }
}