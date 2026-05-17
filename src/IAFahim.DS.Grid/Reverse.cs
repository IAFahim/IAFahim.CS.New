using System;
using System.Runtime.CompilerServices;

namespace IAFahim.DS.Grid
{
    public static unsafe class Reverse
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int len) where T : unmanaged
        {
            int i = 0;
            int j = len - 1;
            while (i < j)
            {
                T temp = ptr[i];
                ptr[i] = ptr[j];
                ptr[j] = temp;
                i++;
                j--;
            }
        }
    }
}