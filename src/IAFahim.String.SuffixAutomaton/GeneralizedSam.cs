namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class GeneralizedSam
    {
        public static void Build(byte** strings, int* lengths, int count, int sigma)
        {
            int totalLen = 0;
            for (int i = 0; i < count; i++) totalLen += lengths[i];
            int* intText = (int*)Marshal.AllocHGlobal(sizeof(int) * (totalLen + count));
            int pos = 0;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < lengths[i]; j++)
                    intText[pos++] = strings[i][j];
                if (i < count - 1)
                    intText[pos++] = sigma + i;
            }
            SuffixAutomaton.Build(intText, pos);
            Marshal.FreeHGlobal((nint)intText);
        }
    }
}
