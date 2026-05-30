namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ZAlgorithm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(byte* ptr, int len, int* zPtr)
        {
            if (len <= 0) return;
            zPtr[0] = len;
            int l = 0, r = 0;
            for (int i = 1; i < len; i++)
            {
                if (i <= r)
                {
                    int k = i - l;
                    int rem = r - i + 1;
                    zPtr[i] = zPtr[k] < rem ? zPtr[k] : rem;
                }
                else
                {
                    zPtr[i] = 0;
                }
                while (i + zPtr[i] < len && ptr[zPtr[i]] == ptr[i + zPtr[i]])
                    zPtr[i]++;
                if (i + zPtr[i] - 1 > r)
                {
                    l = i;
                    r = i + zPtr[i] - 1;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* ptr, int len, int* zPtr)
        {
            if (len <= 0) return;
            zPtr[0] = len;
            int l = 0, r = 0;
            for (int i = 1; i < len; i++)
            {
                if (i <= r)
                {
                    int k = i - l;
                    int rem = r - i + 1;
                    zPtr[i] = zPtr[k] < rem ? zPtr[k] : rem;
                }
                else
                {
                    zPtr[i] = 0;
                }
                while (i + zPtr[i] < len && ptr[zPtr[i]] == ptr[i + zPtr[i]])
                    zPtr[i]++;
                if (i + zPtr[i] - 1 > r)
                {
                    l = i;
                    r = i + zPtr[i] - 1;
                }
            }
        }
    }
}
