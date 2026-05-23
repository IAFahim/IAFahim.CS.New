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
            return match;
        }
    }
}
