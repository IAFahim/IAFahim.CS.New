namespace IAFahim.String.Automata
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class FiniteAutomaton
    {
        public struct Dfa
        {
            public int** Transitions;
            public bool* IsAccept;
            public int StateCount;
            public int AlphabetSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Accepts(Dfa* dfa, int startState, byte* input, int len)
        {
            int state = startState;
            for (int i = 0; i < len; i++)
            {
                int c = input[i];
                if (c >= dfa->AlphabetSize) return false;
                state = dfa->Transitions[state][c];
            }
            return dfa->IsAccept[state];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildDfa(int** nfaTrans, bool* nfaAccept, int nfaStates, int sigma, int* stateMap, Dfa* dfa)
        {
            int maxDfaStates = 1 << nfaStates;
            if (nfaStates >= 31) maxDfaStates = int.MaxValue;
            long queueBytes = (long)maxDfaStates * sizeof(int);
            long tempBytes = (long)maxDfaStates * sizeof(int);
            int* queue = (int*)Marshal.AllocHGlobal((nint)queueBytes);
            int* tempNext = (int*)Marshal.AllocHGlobal((nint)tempBytes);
            
            for (int i = 0; i < maxDfaStates; i++) { stateMap[i] = -1; }
            int stateCount = 1;
            stateMap[1] = 0;
            int head = 0, tail = 0;
            queue[tail++] = 1;

            try
            {
                while (head < tail)
                {
                    int subset = queue[head++];
                    for (int c = 0; c < sigma; c++)
                    {
                        int next = SubsetMove(nfaTrans, nfaStates, subset, c);
                        if (stateMap[next] == -1)
                        {
                            stateMap[next] = stateCount++;
                            queue[tail++] = next;
                        }
                    }
                }

                dfa->StateCount = stateCount;
                dfa->AlphabetSize = sigma;

                for (int i = 0; i < stateCount; i++)
                {
                    for (int c = 0; c < sigma; c++) dfa->Transitions[i][c] = 0;
                    dfa->IsAccept[i] = false;
                }

                for (int mask = 0; mask < maxDfaStates; mask++)
                {
                    if (stateMap[mask] == -1) continue;
                    int dfaState = stateMap[mask];
                    for (int c = 0; c < sigma; c++)
                    {
                        int next = SubsetMove(nfaTrans, nfaStates, mask, c);
                        dfa->Transitions[dfaState][c] = stateMap[next];
                    }
                    dfa->IsAccept[dfaState] = SubsetAccepts(nfaAccept, nfaStates, mask);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)queue);
                Marshal.FreeHGlobal((nint)tempNext);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SubsetMove(int** nfaTrans, int nfaStates, int subset, int c)
        {
            int next = 0;
            for (int s = 0; s < nfaStates; s++)
            {
                if ((subset & (1 << s)) != 0 && nfaTrans[s][c] >= 0)
                    next |= (1 << nfaTrans[s][c]);
            }
            return next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SubsetAccepts(bool* nfaAccept, int nfaStates, int subset)
        {
            for (int s = 0; s < nfaStates; s++)
            {
                if ((subset & (1 << s)) != 0 && nfaAccept[s])
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Minimize(Dfa* dfa)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Complement(Dfa* dfa, Dfa* result)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Union(Dfa* a, Dfa* b, Dfa* result)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Intersection(Dfa* a, Dfa* b, Dfa* result)
        {
        }
    }
}
