using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Subset
{
    public static unsafe class EnumerateSubsets
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(int superMask)
        {
            int count = 0;
            int sub = superMask;
            while (true)
            {
                count++;
                if (sub == 0) break;
                sub = (sub - 1) & superMask;
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int superMask, int* dst)
        {
            int count = 0;
            int sub = superMask;
            while (true)
            {
                dst[count++] = sub;
                if (sub == 0) break;
                sub = (sub - 1) & superMask;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EnumerateUntil(int superMask, int* dst, int maxResults)
        {
            int count = 0;
            int sub = superMask;
            while (count < maxResults)
            {
                dst[count++] = sub;
                if (sub == 0) break;
                sub = (sub - 1) & superMask;
            }
            return count;
        }
    }
}