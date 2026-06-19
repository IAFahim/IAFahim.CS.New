namespace IAFahim.String.Match
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PatternMatch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Abelian(byte* a, int lenA, byte* b, int lenB, int* cntA, int* cntB)
        {
            if (lenA != lenB) return false;
            for (int i = 0; i < 256; i++) cntA[i] = cntB[i] = 0;
            for (int i = 0; i < lenA; i++) cntA[a[i]]++;
            for (int i = 0; i < lenB; i++) cntB[b[i]]++;
            for (int i = 0; i < 256; i++)
                if (cntA[i] != cntB[i])
                {
                    return false;
                }
            return true;
        }

        public static bool Parameterized(byte* a, int lenA, byte* b, int lenB, int* mapA, int* mapB)
        {
            if (lenA != lenB) return false;
            int* lastA = stackalloc int[256];
            int* lastB = stackalloc int[256];
            for (int i = 0; i < 256; i++) { lastA[i] = -1; lastB[i] = -1; }
            int nextId = 1;
            for (int i = 0; i < lenA; i++)
            {
                int prev = lastA[a[i]];
                mapA[i] = prev >= 0 ? mapA[prev] : nextId++;
                lastA[a[i]] = i;
            }
            nextId = 1;
            for (int i = 0; i < lenB; i++)
            {
                int prev = lastB[b[i]];
                mapB[i] = prev >= 0 ? mapB[prev] : nextId++;
                lastB[b[i]] = i;
            }
            bool match = true;
            for (int i = 0; i < lenA; i++)
                if (mapA[i] != mapB[i])
                {
                    match = false;
                    break;
                }
            return match;
        }
    }
}
