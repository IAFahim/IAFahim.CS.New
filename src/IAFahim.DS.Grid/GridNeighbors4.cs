using System;
using System.Runtime.CompilerServices;

namespace IAFahim.DS.Grid
{
    public static unsafe class GridNeighbors4
    {
        public const int MaxNeighbors = 4;

        private static readonly int[] DR = { -1, 1, 0, 0 };
        private static readonly int[] DC = { 0, 0, -1, 1 };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Collect(int r, int c, int height, int width, int* nr, int* nc)
        {
            int count = 0;
            for (int d = 0; d < 4; d++)
            {
                int tr = r + DR[d];
                int tc = c + DC[d];
                if ((uint)tr < (uint)height && (uint)tc < (uint)width)
                {
                    nr[count] = tr;
                    nc[count] = tc;
                    count++;
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CollectFlat(int r, int c, int height, int width, int* outIndices)
        {
            int count = 0;
            for (int d = 0; d < 4; d++)
            {
                int tr = r + DR[d];
                int tc = c + DC[d];
                if ((uint)tr < (uint)height && (uint)tc < (uint)width)
                {
                    outIndices[count++] = tr * width + tc;
                }
            }
            return count;
        }
    }
}
