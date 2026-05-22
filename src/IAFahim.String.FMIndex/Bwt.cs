namespace IAFahim.String.FMIndex
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BurrowsWheeler
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(int* text, int len, int* bwt, int* sa)
        {
            for (int i = 0; i < len; i++)
                bwt[i] = text[(sa[i] - 1 + len) % len];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Inverse(int* bwt, int len, int* text, int* temp, int* count, int* LF)
        {
            int maxChar = 256;
            for (int i = 0; i < maxChar; i++) count[i] = 0;
            for (int i = 0; i < len; i++) count[bwt[i]]++;

            int sum = 0;
            for (int c = 0; c < maxChar; c++)
            {
                int t = count[c];
                count[c] = sum;
                sum += t;
            }
            for (int i = 0; i < len; i++)
            {
                LF[i] = count[bwt[i]]++;
            }
            int pos = 0;
            for (int i = len - 1; i >= 0; i--)
            {
                text[i] = bwt[pos];
                pos = LF[pos];
            }
        }
    }
}