namespace IAFahim.String.Parse
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Earley
    {
        public struct State
        {
            public int Rule;
            public int Dot;
            public int Origin;
        }

        public static bool Parse(byte* input, int len, int* rules, int ruleCount, int startRule)
        {
            int maxStates = len * ruleCount * 4;
            State* S = stackalloc State[maxStates * (len + 1)];
            int* counts = stackalloc int[len + 1];
            for (int i = 0; i <= len; i++) counts[i] = 0;

            S[0].Rule = startRule;
            S[0].Dot = 0;
            S[0].Origin = 0;
            counts[0] = 1;

            for (int k = 0; k <= len; k++)
            {
                for (int i = 0; i < counts[k]; i++)
                {
                    int rule = S[k * maxStates + i].Rule;
                    int dot = S[k * maxStates + i].Dot;
                    int origin = S[k * maxStates + i].Origin;

                    int symbol = rules[rule * 3 + dot + 1];
                    if (symbol == -1) continue;

                    if (symbol >= 256)
                    {
                        for (int r = 0; r < ruleCount; r++)
                        {
                            if (rules[r * 3] == symbol && counts[k] < maxStates)
                            {
                                S[k * maxStates + counts[k]].Rule = r;
                                S[k * maxStates + counts[k]].Dot = 0;
                                S[k * maxStates + counts[k]].Origin = k;
                                counts[k]++;
                            }
                        }
                    }
                    else if (k < len && input[k] == symbol)
                    {
                        if (counts[k + 1] < maxStates)
                        {
                            S[(k + 1) * maxStates + counts[k + 1]].Rule = rule;
                            S[(k + 1) * maxStates + counts[k + 1]].Dot = dot + 1;
                            S[(k + 1) * maxStates + counts[k + 1]].Origin = origin;
                            counts[k + 1]++;
                        }
                    }
                }
            }

            for (int i = 0; i < counts[len]; i++)
            {
                int rule = S[len * maxStates + i].Rule;
                int dot = S[len * maxStates + i].Dot;
                if (rules[rule * 3 + dot + 1] == -1 && S[len * maxStates + i].Origin == 0)
                    return true;
            }
            return false;
        }
    }
}
