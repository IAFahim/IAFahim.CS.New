namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GeneralizedSam
    {
        public static void Build(byte** strings, int* lengths, int count, int sigma, int* intText, SuffixAutomaton.State* st, SuffixAutomaton.Edge* e, ref int stSize, ref int stLast, ref int edgeCount)
        {
            int pos = 0;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < lengths[i]; j++)
                    intText[pos++] = strings[i][j];
                if (i < count - 1)
                    intText[pos++] = sigma + i;
            }
            SuffixAutomaton.Build(intText, pos, st, e, ref stSize, ref stLast, ref edgeCount);
        }
    }
}
