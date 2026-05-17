using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Window
{
    public static unsafe class SlidingWindowMax
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* src, int* dst, int len, int windowSize)
        {
            if (len == 0 || windowSize == 0) return;
            int* deque = stackalloc int[len];
            int front = 0, back = 0;
            for (int i = 0; i < len; i++)
            {
                while (front < back && src[deque[back - 1]] <= src[i]) back--;
                deque[back++] = i;
                if (deque[front] <= i - windowSize) front++;
                if (i >= windowSize - 1)
                    dst[i - windowSize + 1] = src[deque[front]];
            }
        }
    }
}