namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PushRelabelGap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int* height, int* gap)
        {
            // Just gap initialization for push relabel
            for (int i = 0; i <= n; i++) gap[i] = 0;
            for (int i = 0; i < n; i++) gap[height[i]]++;
        }
    }
}