namespace IAFahim.String.Compress
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MoveToFront
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Encode(byte* input, int len, byte* output, int sigma)
        {
            byte* list = (byte*)Marshal.AllocHGlobal(sizeof(byte) * sigma);
            for (int i = 0; i < sigma; i++) list[i] = (byte)i;
            for (int i = 0; i < len; i++)
            {
                int pos = 0;
                while (list[pos] != input[i]) pos++;
                output[i] = (byte)pos;
                for (int j = pos; j > 0; j--) list[j] = list[j - 1];
                list[0] = input[i];
            }
            Marshal.FreeHGlobal((nint)list);
        }

        public static void Decode(byte* input, int len, byte* output, int sigma)
        {
            byte* list = (byte*)Marshal.AllocHGlobal(sizeof(byte) * sigma);
            for (int i = 0; i < sigma; i++) list[i] = (byte)i;
            for (int i = 0; i < len; i++)
            {
                int pos = input[i];
                output[i] = list[pos];
                for (int j = pos; j > 0; j--) list[j] = list[j - 1];
                list[0] = output[i];
            }
            Marshal.FreeHGlobal((nint)list);
        }
    }
}
