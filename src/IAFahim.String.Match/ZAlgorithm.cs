namespace IAFahim.String.Match
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ZAlgorithm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(byte* ptr, int len, int* zPtr)
        {
            zPtr[0] = len;
            int l = 0, r = 0;
            for (int i = 1; i < len; i++)
            {
                if (i <= r)
                {
                    int remaining = r - i + 1;
                    if (zPtr[i - l] < remaining)
                        zPtr[i] = zPtr[i - l];
                    else
                    {
                        zPtr[i] = remaining;
                        while (i + zPtr[i] < len && ptr[zPtr[i]] == ptr[i + zPtr[i]])
                            zPtr[i]++;
                        zPtr[i] = Math.Min(zPtr[i], remaining);
                    }
                }
                else
                {
                    zPtr[i] = 0;
                    while (i + zPtr[i] < len && ptr[zPtr[i]] == ptr[i + zPtr[i]])
                        zPtr[i]++;
                }
                if (i + zPtr[i] - 1 > r)
                {
                    l = i;
                    r = i + zPtr[i] - 1;
                }
            }
        }
    }
}
