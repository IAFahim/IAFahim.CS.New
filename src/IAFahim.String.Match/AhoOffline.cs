namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AhoOffline
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query(byte* text, int textLen, int* states, int stateCount, int* matches)
        {
            int count = 0;
            int v = 0;
            for (int i = 0; i < textLen; i++)
            {
                while (v > 0 && states[v * 256 + text[i]] == -1)
                    v = states[v * 256 + 255];
                if (states[v * 256 + text[i]] != -1)
                    v = states[v * 256 + text[i]];
                if (states[v * 256 + 254] != -1)
                    matches[count++] = i;
            }
            return count;
        }
    }
}
