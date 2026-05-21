namespace IAFahim.String.Match
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PrefixFunction
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(byte* ptr, int len, int* piPtr)
        {
            piPtr[0] = 0;
            for (int i = 1; i < len; i++)
            {
                int j = piPtr[i - 1];
                while (j > 0 && ptr[i] != ptr[j])
                    j = piPtr[j - 1];
                if (ptr[i] == ptr[j])
                    j++;
                piPtr[i] = j;
            }
        }
    }
}
