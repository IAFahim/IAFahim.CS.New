using System;
using System.Runtime.CompilerServices;

namespace IAFahim.DS.Grid
{
    public static unsafe class FillGrid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int width, int height, T value) where T : unmanaged
        {
            int len = width * height;
            for (int i = 0; i < len; i++)
            {
                ptr[i] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunXY<T, F>(T* ptr, int width, int height, T value, F getValue) where T : unmanaged where F : struct, IFillGridXY<T>
        {
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ptr[index++] = getValue.Get(x, y);
                }
            }
        }
    }

    public interface IFillGridXY<T> where T : unmanaged
    {
        T Get(int x, int y);
    }
}