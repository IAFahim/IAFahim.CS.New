namespace IAFahim.String.Grammar
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class GrammarCompress
    {
        private const int NotFound = -1;

        private const int NoCandidate = 0;

        private const int SingletonFrequency = 1;

        private const int FirstNonTerminal = 256;

        public struct Rule
        {
            public int Left;
            public int Right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountPair(int* work, int workLen, int pair0, int pair1)
        {
            int freq = 0;
            for (int k = 0; k < workLen - 1; k++)
                if (work[k] == pair0 && work[k + 1] == pair1) freq++;
            return freq;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FindBestPair(int* work, int workLen, out int bestI, out int bestFreq)
        {
            bestI = NotFound;
            bestFreq = NoCandidate;
            for (int i = 0; i < workLen - 1; i++)
            {
                for (int j = i + 1; j < workLen - 1; j++)
                {
                    if (work[i] != work[j] || work[i + 1] != work[j + 1]) continue;
                    int freq = CountPair(work, workLen, work[i], work[i + 1]);
                    if (freq > bestFreq) { bestFreq = freq; bestI = i; }
                    break;
                }
                if (bestFreq > SingletonFrequency) break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReplacePair(int* work, int workLen, int pair0, int pair1, int newSym)
        {
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
            return newLen;
        }

        public static int Compress(byte* input, int len, Rule* rules, int maxRules, int* work)
        {
            int ruleCount = 0;
            for (int i = 0; i < len; i++) work[i] = input[i];
            int workLen = len;
            while (ruleCount < maxRules)
            {
                FindBestPair(work, workLen, out int bestI, out int bestFreq);
                if (bestFreq <= SingletonFrequency) break;
                int pair0 = work[bestI];
                int pair1 = work[bestI + 1];
                int newSym = FirstNonTerminal + ruleCount;
                workLen = ReplacePair(work, workLen, pair0, pair1, newSym);
                rules[ruleCount].Left = pair0;
                rules[ruleCount].Right = pair1;
                ruleCount++;
            }
            return ruleCount;
        }
    }
}
