namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DynamicKmp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(byte* pattern, int len, int* fail)
        {
            fail[0] = 0;
            for (int i = 1; i < len; i++)
            {
                int j = fail[i - 1];
                while (j > 0 && pattern[i] != pattern[j])
                    j = fail[j - 1];
                if (pattern[i] == pattern[j]) j++;
                fail[i] = j;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Search(byte* text, int textLen, byte* pattern, int patLen, int* fail, int* matches)
        {
            int count = 0;
            int j = 0;
            for (int i = 0; i < textLen; i++)
            {
                while (j > 0 && text[i] != pattern[j])
                    j = fail[j - 1];
                if (text[i] == pattern[j]) j++;
                if (j == patLen)
                {
                    matches[count++] = i - patLen + 1;
                    j = fail[j - 1];
                }
            }
            return count;
        }
    }
}
