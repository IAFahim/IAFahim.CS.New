namespace IAFahim.String.Grammar
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class GrammarCompress
    {
        public struct Rule
        {
            public int Left;
            public int Right;
        }

        public static int Compress(byte* input, int len, Rule* rules, int maxRules)
        {
            int ruleCount = 0;
            byte* work = (byte*)Marshal.AllocHGlobal(len);
            Buffer.MemoryCopy(input, work, len, len);
            int workLen = len;
            while (ruleCount < maxRules)
            {
                int bestI = -1, bestJ = -1, bestFreq = 0;
                for (int i = 0; i < workLen - 1; i++)
                {
                    for (int j = i + 1; j < workLen - 1; j++)
                    {
                        if (work[i] == work[j] && work[i + 1] == work[j + 1])
                        {
                            int freq = 0;
                            for (int k = 0; k < workLen - 1; k++)
                                if (work[k] == work[i] && work[k + 1] == work[i + 1])
                                    freq++;
                            if (freq > bestFreq)
                            {
                                bestFreq = freq;
                                bestI = i;
                                bestJ = j;
                            }
                            break;
                        }
                    }
                    if (bestFreq > 1) break;
                }
                if (bestFreq <= 1) break;
                byte pair0 = work[bestI];
                byte pair1 = work[bestI + 1];
                byte newSym = (byte)(254 - ruleCount);
                int newLen = 0;
                for (int k = 0; k < workLen; k++)
                {
                    if (k < workLen - 1 && work[k] == pair0 && work[k + 1] == pair1)
                    {
                        work[newLen++] = newSym;
                        k++;
                    }
                    else
                    {
                        work[newLen++] = work[k];
                    }
                }
                rules[ruleCount].Left = pair0;
                rules[ruleCount].Right = pair1;
                ruleCount++;
                workLen = newLen;
            }
            Marshal.FreeHGlobal((nint)work);
            return ruleCount;
        }
    }
}
