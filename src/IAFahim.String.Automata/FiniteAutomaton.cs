namespace IAFahim.String.Automata
{
using System.Runtime.InteropServices;
    using System;
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

        public static Dfa* BuildDfa(int** nfaTrans, bool* nfaAccept, int nfaStates, int sigma)
        {
            int stateCount = 1;
            int* powerSet = (int*)Marshal.AllocHGlobal(sizeof(int) * (1 << nfaStates));
            int* visited = (int*)Marshal.AllocHGlobal(sizeof(int) * (1 << nfaStates));
            for (int i = 0; i < (1 << nfaStates); i++)
            {
                powerSet[i] = -1;
                visited[i] = -1;
            }
            powerSet[1] = 0;
            visited[1] = 0;
            int* queue = (int*)Marshal.AllocHGlobal(sizeof(int) * (1 << nfaStates));
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
                    if (powerSet[next] == -1)
                    {
                        powerSet[next] = stateCount++;
                        queue[tail++] = next;
                    }
                }
            }
            Marshal.FreeHGlobal((nint)queue);
            Marshal.FreeHGlobal((nint)powerSet);
            Marshal.FreeHGlobal((nint)visited);
            return null;
        }

        public static bool Minimize(Dfa* dfa)
        {
            return true;
        }

        public static Dfa* Complement(Dfa* dfa)
        {
            return dfa;
        }

        public static Dfa* Union(Dfa* a, Dfa* b)
        {
            return a;
        }

        public static Dfa* Intersection(Dfa* a, Dfa* b)
        {
            return a;
        }
    }
}
