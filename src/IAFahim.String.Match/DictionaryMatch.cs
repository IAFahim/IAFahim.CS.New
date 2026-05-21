namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DictionaryMatch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Match(byte* text, int textLen, byte** patterns, int* patLens, int patCount, int* matches)
        {
            int count = 0;
            for (int i = 0; i < textLen; i++)
            {
                for (int p = 0; p < patCount; p++)
                {
                    if (i + patLens[p] > textLen) continue;
                    bool found = true;
                    for (int j = 0; j < patLens[p]; j++)
                    {
                        if (text[i + j] != patterns[p][j])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found) matches[count++] = p;
                }
            }
            return count;
        }
    }
}
