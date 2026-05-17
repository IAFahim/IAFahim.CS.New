using System;
using System.Runtime.CompilerServices;

namespace IAFahim.DS.Grid
{
    public static unsafe class Shuffle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int len, int seed) where T : unmanaged
        {
            if (len <= 1)
                return;
            int* rng = stackalloc int[2];
            rng[0] = seed;
            rng[1] = 1103515245;
            for (int i = len - 1; i > 0; i--)
            {
                rng[0] = (rng[1] * 1103515245 + 12345) ^ (rng[0] * 134775813 + 1);
                int j = (int)(((uint)rng[0] >> 16) % (uint)(i + 1));
                T temp = ptr[i];
                ptr[i] = ptr[j];
                ptr[j] = temp;
            }
        }
    }
}