namespace IAFahim.String.FMIndex
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class BurrowsWheeler
    {
        public static void Transform(int* text, int len, int* bwt, int* sa)
        {
            for (int i = 0; i < len; i++)
                bwt[i] = text[(sa[i] - 1 + len) % len];
        }

        public static void Inverse(int* bwt, int len, int* text)
        {
            int* F = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            int* temp = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            int* count = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            for (int i = 0; i < len; i++)
            {
                F[i] = bwt[i];
                temp[i] = bwt[i];
            }
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len - i - 1; j++)
                {
                    if (temp[j] > temp[j + 1])
                    {
                        int t = temp[j];
                        temp[j] = temp[j + 1];
                        temp[j + 1] = t;
                    }
                }
            }
            for (int i = 0; i < len; i++)
                count[i] = 0;
            for (int i = 0; i < len; i++)
                count[F[i]]++;
            int sum = 0;
            for (int c = 0; c < len; c++)
            {
                int t = count[c];
                count[c] = sum;
                sum += t;
            }
            int* LF = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            for (int i = 0; i < len; i++)
            {
                int c = F[i];
                LF[i] = count[c]++;
            }
            int pos = 0;
            for (int i = 0; i < len; i++)
            {
                text[pos] = bwt[pos];
                pos = LF[pos];
            }
            Marshal.FreeHGlobal((nint)F);
            Marshal.FreeHGlobal((nint)temp);
            Marshal.FreeHGlobal((nint)count);
            Marshal.FreeHGlobal((nint)LF);
        }
    }
}
