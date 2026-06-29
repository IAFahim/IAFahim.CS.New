namespace IAFahim.String.MinRotation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class Booth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AdvanceLoser(ref int loser, int winner, int k)
        {
            loser = loser + k + 1;
            if (loser <= winner) loser = winner + 1;
        }

        public static int Run(byte* s, int len)
        {
            if (len <= 1) return 0;
            int* f = (int*)Marshal.AllocHGlobal((nint)((long)len * 2 * sizeof(int)));
            for (int i = 0; i < len * 2; i++) f[i] = 0;
            int i2 = 0, j = 1, k = 0;
            while (i2 < len && j < len && k < len)
            {
                byte a = s[(i2 + k) % len];
                byte b = s[(j + k) % len];
                if (a == b) { k++; continue; }
                if (a > b) AdvanceLoser(ref i2, j, k);
                else AdvanceLoser(ref j, i2, k);
                k = 0;
            }
            int mrResult = i2 < j ? i2 : j;
            Marshal.FreeHGlobal((nint)f);
            return mrResult;
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
                if (a > b) AdvanceLoser(ref i2, j, k);
                else AdvanceLoser(ref j, i2, k);
                k = 0;
            }
            return i2 < j ? i2 : j;
        }
    }
}
