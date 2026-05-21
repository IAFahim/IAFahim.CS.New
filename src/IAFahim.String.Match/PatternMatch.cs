namespace IAFahim.String.Match
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PatternMatch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Abelian(byte* a, int lenA, byte* b, int lenB)
        {
            if (lenA != lenB) return false;
            int* cntA = (int*)Marshal.AllocHGlobal(sizeof(int) * 256);
            int* cntB = (int*)Marshal.AllocHGlobal(sizeof(int) * 256);
            for (int i = 0; i < 256; i++) cntA[i] = cntB[i] = 0;
            for (int i = 0; i < lenA; i++) cntA[a[i]]++;
            for (int i = 0; i < lenB; i++) cntB[b[i]]++;
            for (int i = 0; i < 256; i++)
                if (cntA[i] != cntB[i])
                {
                    Marshal.FreeHGlobal((nint)cntA);
                    Marshal.FreeHGlobal((nint)cntB);
                    return false;
                }
            Marshal.FreeHGlobal((nint)cntA);
            Marshal.FreeHGlobal((nint)cntB);
            return true;
        }

        public static bool Parameterized(byte* a, int lenA, byte* b, int lenB)
        {
            if (lenA != lenB) return false;
            int* mapA = (int*)Marshal.AllocHGlobal(sizeof(int) * lenA);
            int* mapB = (int*)Marshal.AllocHGlobal(sizeof(int) * lenB);
            int nextId = 1;
            for (int i = 0; i < lenA; i++)
            {
                int j;
                for (j = 0; j < i; j++)
                    if (a[j] == a[i]) break;
                mapA[i] = j < i ? mapA[j] : nextId++;
            }
            nextId = 1;
            for (int i = 0; i < lenB; i++)
            {
                int j;
                for (j = 0; j < i; j++)
                    if (b[j] == b[i]) break;
                mapB[i] = j < i ? mapB[j] : nextId++;
            }
            bool match = true;
            for (int i = 0; i < lenA; i++)
                if (mapA[i] != mapB[i])
                {
                    match = false;
                    break;
                }
            Marshal.FreeHGlobal((nint)mapA);
            Marshal.FreeHGlobal((nint)mapB);
            return match;
        }
    }
}
