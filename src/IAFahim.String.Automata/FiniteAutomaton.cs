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
        public static Dfa* BuildDfa(int** nfaTrans, bool* nfaAccept, int nfaStates, int sigma, int* stateMap, int* queue)
        {
            int stateCount = 1;
            for (int i = 0; i < (1 << nfaStates); i++) stateMap[i] = -1;

            stateMap[1] = 0;
            int head = 0, tail = 0;
            queue[tail++] = 1;
            while (head < tail)
            {
                int subset = queue[head++];
                for (int c = 0; c < sigma; c++)
                {
                    int next = 0;
                    for (int s = 0; s < nfaStates; s++)
                    {
                        if ((subset & (1 << s)) != 0 && nfaTrans[s][c] >= 0)
                            next |= (1 << nfaTrans[s][c]);
                    }
                    if (stateMap[next] == -1)
                    {
                        stateMap[next] = stateCount++;
                        queue[tail++] = next;
                    }
                }
            }

            Dfa* dfa = (Dfa*)Marshal.AllocHGlobal(sizeof(Dfa));
            dfa->StateCount = stateCount;
            dfa->AlphabetSize = sigma;
            dfa->Transitions = (int**)Marshal.AllocHGlobal(stateCount * sizeof(int*));
            dfa->IsAccept = (bool*)Marshal.AllocHGlobal(stateCount * sizeof(bool));

            for (int i = 0; i < stateCount; i++)
            {
                dfa->Transitions[i] = (int*)Marshal.AllocHGlobal(sigma * sizeof(int));
                for (int c = 0; c < sigma; c++) dfa->Transitions[i][c] = 0;
                dfa->IsAccept[i] = false;
            }

            for (int mask = 0; mask < (1 << nfaStates); mask++)
            {
                if (stateMap[mask] == -1) continue;
                int dfaState = stateMap[mask];
                for (int c = 0; c < sigma; c++)
                {
                    int next = 0;
                    for (int s = 0; s < nfaStates; s++)
                    {
                        if ((mask & (1 << s)) != 0 && nfaTrans[s][c] >= 0)
                            next |= (1 << nfaTrans[s][c]);
                    }
                    dfa->Transitions[dfaState][c] = stateMap[next];
                }
                for (int s = 0; s < nfaStates; s++)
                {
                    if ((mask & (1 << s)) != 0 && nfaAccept[s])
                    {
                        dfa->IsAccept[dfaState] = true;
                        break;
                    }
                }
            }
            return dfa;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FreeDfa(Dfa* dfa)
        {
            if (dfa == null) return;
            if (dfa->Transitions != null)
            {
                for (int i = 0; i < dfa->StateCount; i++)
                {
                    if (dfa->Transitions[i] != null)
                        Marshal.FreeHGlobal((nint)dfa->Transitions[i]);
                }
                Marshal.FreeHGlobal((nint)dfa->Transitions);
            }
            if (dfa->IsAccept != null)
                Marshal.FreeHGlobal((nint)dfa->IsAccept);
            Marshal.FreeHGlobal((nint)dfa);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Minimize(Dfa* dfa)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dfa* Complement(Dfa* dfa)
        {
            return dfa;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dfa* Union(Dfa* a, Dfa* b)
        {
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dfa* Intersection(Dfa* a, Dfa* b)
        {
            return a;
        }
    }
}