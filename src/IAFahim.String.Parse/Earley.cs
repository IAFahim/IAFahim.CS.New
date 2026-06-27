namespace IAFahim.String.Parse
{
    using System;
    using System.Runtime.InteropServices;

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
            int maxStates = (len + 1) * ruleCount * 4;
            if (maxStates < 4) maxStates = 4;
            long byteCount = (long)maxStates * (len + 1) * sizeof(State);
            State* S = (State*)Marshal.AllocHGlobal((nint)byteCount);
            long countsByteCount = (long)(len + 1) * sizeof(int);
            int* counts = (int*)Marshal.AllocHGlobal((nint)countsByteCount);
            for (int i = 0; i <= len; i++) counts[i] = 0;

            AddState(S, counts, maxStates, 0, startRule, 0, 0);

            for (int k = 0; k <= len; k++)
            {
                int i = 0;
                while (i < counts[k])
                {
                    int idx = k * maxStates + i;
                    i++;
                    int rule = S[idx].Rule;
                    int dot = S[idx].Dot;
                    int origin = S[idx].Origin;
                    int symbol = rules[rule * 3 + dot + 1];

                    if (symbol == -1)
                    {
                        Complete(S, counts, maxStates, k, rules, ruleCount, rule, dot, origin, symbol);
                    }
                    else if (symbol >= 256)
                    {
                        Predict(S, counts, maxStates, k, rules, ruleCount, rule, dot, origin, symbol);
                    }
                    else if (k < len && input[k] == symbol)
                    {
                        Scan(S, counts, maxStates, k, rule, dot, origin);
                    }
                }
            }

            bool result = false;
            for (int i = 0; i < counts[len]; i++)
            {
                State sp = S[len * maxStates + i];
                if (sp.Rule == startRule && sp.Origin == 0 && rules[sp.Rule * 3 + sp.Dot + 1] == -1)
                { result = true; break; }
            }
            Marshal.FreeHGlobal((nint)S);
            Marshal.FreeHGlobal((nint)counts);
            return result;
        }

        private static void AddState(State* S, int* counts, int maxStates, int setIdx, int rule, int dot, int origin)
        {
            if (counts[setIdx] >= maxStates) return;
            int baseIdx = setIdx * maxStates;
            for (int j = 0; j < counts[setIdx]; j++)
            {
                if (S[baseIdx + j].Rule == rule && S[baseIdx + j].Dot == dot && S[baseIdx + j].Origin == origin)
                    return;
            }
            S[baseIdx + counts[setIdx]].Rule = rule;
            S[baseIdx + counts[setIdx]].Dot = dot;
            S[baseIdx + counts[setIdx]].Origin = origin;
            counts[setIdx]++;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void Complete(State* S, int* counts, int maxStates, int k, int* rules, int ruleCount, int rule, int dot, int origin, int symbol)
        {
            int lhs = rules[rule * 3];
            int originBase = origin * maxStates;
            for (int j = 0; j < counts[origin]; j++)
            {
                State wp = S[originBase + j];
                if (rules[wp.Rule * 3 + wp.Dot + 1] == lhs)
                    AddState(S, counts, maxStates, k, wp.Rule, wp.Dot + 1, wp.Origin);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void Predict(State* S, int* counts, int maxStates, int k, int* rules, int ruleCount, int rule, int dot, int origin, int symbol)
        {
            for (int r = 0; r < ruleCount; r++)
            {
                if (rules[r * 3] == symbol)
                    AddState(S, counts, maxStates, k, r, 0, k);
            }
            int kBase = k * maxStates;
            for (int j = 0; j < counts[k]; j++)
            {
                State cp = S[kBase + j];
                if (rules[cp.Rule * 3 + cp.Dot + 1] == -1 && rules[cp.Rule * 3] == symbol && cp.Origin == k)
                {
                    AddState(S, counts, maxStates, k, rule, dot + 1, origin);
                    break;
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void Scan(State* S, int* counts, int maxStates, int k, int rule, int dot, int origin)
        {
            AddState(S, counts, maxStates, k + 1, rule, dot + 1, origin);
        }
    }
}
