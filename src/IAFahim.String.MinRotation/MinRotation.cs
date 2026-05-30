namespace IAFahim.String.MinRotation
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Booth
    {
        public static int Run(byte* s, int len)
        {
            if (len <= 1) return 0;
            int* f = stackalloc int[len * 2];
            for (int i = 0; i < len * 2; i++) f[i] = 0;
            int i2 = 0, j = 1, k = 0;
            while (i2 < len && j < len && k < len)
            {
                byte a = s[(i2 + k) % len];
                byte b = s[(j + k) % len];
                if (a == b) { k++; continue; }
                if (a > b) { i2 = i2 + k + 1; if (i2 <= j) i2 = j + 1; }
                else { j = j + k + 1; if (j <= i2) j = i2 + 1; }
                k = 0;
            }
            return i2 < j ? i2 : j;
        }

        public static int Run(int* s, int len)
        {
            if (len <= 1) return 0;
            int i2 = 0, j = 1, k = 0;
            while (i2 < len && j < len && k < len)
            {
                int a = s[(i2 + k) % len];
                int b = s[(j + k) % len];
                if (a == b) { k++; continue; }
                if (a > b) { i2 = i2 + k + 1; if (i2 <= j) i2 = j + 1; }
                else { j = j + k + 1; if (j <= i2) j = i2 + 1; }
                k = 0;
            }
            return i2 < j ? i2 : j;
        }
    }
}
