namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AhoOffline
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query(byte* text, int textLen, int* go, int* fail, int* out_, int outCount, int* matches)
        {
            int count = 0;
            int v = 0;
            for (int i = 0; i < textLen; i++)
            {
                while (v > 0 && go[v * 256 + text[i]] == -1)
                    v = fail[v];
                if (go[v * 256 + text[i]] != -1)
                    v = go[v * 256 + text[i]];
                if (out_[v] >= 0 && out_[v] < outCount)
                    matches[count++] = i;
            }
            return count;
        }
    }
}
