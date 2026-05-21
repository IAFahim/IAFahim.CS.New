namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LexicographicKth
    {
        public static bool Find(byte* text, int textLen, long k, byte* outBuf, int* outLen)
        {
            if (k <= 0) { *outLen = 0; return false; }
            long count = 0;
            for (int len = 1; len <= textLen; len++)
            {
                for (int start = 0; start <= textLen - len; start++)
                {
                    count++;
                    if (count == k)
                    {
                        for (int i = 0; i < len; i++)
                            outBuf[i] = text[start + i];
                        *outLen = len;
                        return true;
                    }
                }
            }
            *outLen = 0;
            return false;
        }
    }
}
