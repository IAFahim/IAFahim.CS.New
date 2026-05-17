using System;
using System.Runtime.CompilerServices;

namespace IAFahim.DS.Grid
{
    public static unsafe class FillSlice
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int len, int start, int end, T value) where T : unmanaged
        {
            if ((uint)start >= (uint)len || start >= end)
                return;
            for (int i = start; i < end; i++)
            {
                ptr[i] = value;
            }
        }
    }
}