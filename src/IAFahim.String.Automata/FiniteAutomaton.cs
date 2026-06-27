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

        // Moore partition-refinement DFA minimization, in-place.
        // Assumes a complete DFA (every state has a defined transition for every
        // symbol, as produced by BuildDfa) whose start state is index 0. States
        // are merged by equivalence class and the DFA is compacted; the start
        // state is preserved at index 0. Unreachable states are NOT removed here
        // (the subset construction in BuildDfa already yields only reachable
        // states); Moore refinement merely merges equivalent states.
        // Returns true if the DFA changed (state count reduced), false otherwise.
        public static bool Minimize(Dfa* dfa)
        {
            int n = dfa->StateCount;
            int sigma = dfa->AlphabetSize;
            if (n <= 1) return false;

            // This module has no Unity.Collections dependency (unlike Recast/Graph
            // modules), so scratch uses Marshal.AllocHGlobal to match the existing
            // BuildDfa allocator in this file rather than AllocatorManager.Temp.
            // cls[s]   = current partition id (equivalence class) of state s
            // newCls   = recomputed class id for this round
            // sig      = scratch signature buffer used to assign new class ids
            int* cls = (int*)Marshal.AllocHGlobal((nint)((long)sizeof(int) * n));
            int* newCls = (int*)Marshal.AllocHGlobal((nint)((long)sizeof(int) * n));
            // Signature per state: 1 (class) + sigma (target classes). Stored row-major.
            long sigStride = 1L + sigma;
            int* sig = (int*)Marshal.AllocHGlobal((nint)((long)sizeof(int) * n * sigStride));
            // ord[] indexes states sorted by signature so equal signatures are adjacent.
            int* ord = (int*)Marshal.AllocHGlobal((nint)((long)sizeof(int) * n));

            bool changed;
            try
            {
                // Initial partition: accepting vs non-accepting.
                int classCount = 0;
                bool seenAccept = false, seenReject = false;
                int acceptId = 0, rejectId = 0;
                for (int s = 0; s < n; s++)
                {
                    if (dfa->IsAccept[s])
                    {
                        if (!seenAccept) { seenAccept = true; acceptId = classCount++; }
                        cls[s] = acceptId;
                    }
                    else
                    {
                        if (!seenReject) { seenReject = true; rejectId = classCount++; }
                        cls[s] = rejectId;
                    }
                }

                // Refine until the partition stabilizes.
                while (true)
                {
                    // Build the signature of each state: its own class plus the
                    // class of each symbol's target.
                    for (int s = 0; s < n; s++)
                    {
                        int* row = sig + (long)s * sigStride;
                        row[0] = cls[s];
                        int* tr = dfa->Transitions[s];
                        for (int c = 0; c < sigma; c++) row[1 + c] = cls[tr[c]];
                        ord[s] = s;
                    }

                    // Sort state indices by signature (insertion sort; n is the
                    // post-subset DFA size, typically small and Burst-friendly).
                    InsertionSortBySig(ord, sig, sigStride, n);

                    // Assign new contiguous class ids to runs of equal signatures.
                    int newCount = 0;
                    for (int i = 0; i < n; i++)
                    {
                        int s = ord[i];
                        if (i > 0 && SigEqual(sig, sigStride, ord[i - 1], s))
                            newCls[s] = newCls[ord[i - 1]];
                        else
                            newCls[s] = newCount++;
                    }

                    bool stable = newCount == classCount;
                    for (int s = 0; s < n; s++) cls[s] = newCls[s];
                    classCount = newCount;
                    if (stable) break;
                }

                changed = classCount < n;
                if (changed)
                {
                    // Compact: pick one representative state per class. Keep state
                    // 0's class as id 0 so the start state stays at index 0.
                    // remap[oldClass] = new compacted state index.
                    int* remap = newCls; // reuse buffer
                    for (int i = 0; i < n; i++) remap[i] = -1;

                    int start = cls[0];
                    remap[start] = 0;
                    int next = 1;
                    for (int s = 0; s < n; s++)
                    {
                        int c = cls[s];
                        if (remap[c] == -1) remap[c] = next++;
                    }

                    // For each new state, copy transitions/accept from any state in
                    // that class, rewriting targets through the class remap.
                    // ord doubles as "representative oldState for new index".
                    int* repr = ord;
                    for (int i = 0; i < next; i++) repr[i] = -1;
                    for (int s = 0; s < n; s++)
                    {
                        int ns = remap[cls[s]];
                        if (repr[ns] == -1) repr[ns] = s;
                    }

                    for (int ns = 0; ns < next; ns++)
                    {
                        int s = repr[ns];
                        int* dst = dfa->Transitions[ns];
                        int* srcRow = dfa->Transitions[s];
                        for (int c = 0; c < sigma; c++)
                            dst[c] = remap[cls[srcRow[c]]];
                        dfa->IsAccept[ns] = dfa->IsAccept[s];
                    }
                    dfa->StateCount = next;
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)cls);
                Marshal.FreeHGlobal((nint)newCls);
                Marshal.FreeHGlobal((nint)sig);
                Marshal.FreeHGlobal((nint)ord);
            }
            return changed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SigEqual(int* sig, long stride, int a, int b)
        {
            int* ra = sig + a * stride;
            int* rb = sig + b * stride;
            for (long k = 0; k < stride; k++)
                if (ra[k] != rb[k]) return false;
            return true;
        }

        // Lexicographic compare of two signature rows; <0, 0, >0.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SigCompare(int* sig, long stride, int a, int b)
        {
            int* ra = sig + a * stride;
            int* rb = sig + b * stride;
            for (long k = 0; k < stride; k++)
            {
                int d = ra[k] - rb[k];
                if (d != 0) return d;
            }
            return 0;
        }

        private static void InsertionSortBySig(int* ord, int* sig, long stride, int n)
        {
            for (int i = 1; i < n; i++)
            {
                int key = ord[i];
                int j = i - 1;
                while (j >= 0 && SigCompare(sig, stride, ord[j], key) > 0)
                {
                    ord[j + 1] = ord[j];
                    j--;
                }
                ord[j + 1] = key;
            }
        }

        // Complement of a complete DFA: copy structure, flip accepting states.
        // 'result' must have Transitions rows [0, dfa->StateCount) and IsAccept
        // pre-allocated by the caller. Start state stays at index 0.
        public static void Complement(Dfa* dfa, Dfa* result)
        {
            int n = dfa->StateCount;
            int sigma = dfa->AlphabetSize;
            result->StateCount = n;
            result->AlphabetSize = sigma;
            for (int s = 0; s < n; s++)
            {
                int* src = dfa->Transitions[s];
                int* dst = result->Transitions[s];
                for (int c = 0; c < sigma; c++) dst[c] = src[c];
                result->IsAccept[s] = !dfa->IsAccept[s];
            }
        }

        // Union via product construction: result accepts iff a OR b accepts.
        // Both inputs must share the same AlphabetSize and be complete DFAs with
        // start state 0. 'result' must have Transitions rows and IsAccept
        // pre-allocated for a->StateCount * b->StateCount states; the product
        // start state (a0,b0) is placed at index 0.
        public static void Union(Dfa* a, Dfa* b, Dfa* result)
        {
            Product(a, b, result, true);
        }

        // Intersection via product construction: result accepts iff a AND b accept.
        // Same buffer requirements as Union.
        public static void Intersection(Dfa* a, Dfa* b, Dfa* result)
        {
            Product(a, b, result, false);
        }

        // Shared product construction. union==true -> OR accept, else AND accept.
        // Product state (i,j) maps to flat index i*nb + j, so (0,0) -> 0.
        private static void Product(Dfa* a, Dfa* b, Dfa* result, bool union)
        {
            int na = a->StateCount;
            int nb = b->StateCount;
            int sigma = a->AlphabetSize < b->AlphabetSize ? a->AlphabetSize : b->AlphabetSize;
            result->StateCount = na * nb;
            result->AlphabetSize = sigma;
            for (int i = 0; i < na; i++)
            {
                int* ai = a->Transitions[i];
                bool aAcc = a->IsAccept[i];
                for (int j = 0; j < nb; j++)
                {
                    int p = i * nb + j;
                    int* bj = b->Transitions[j];
                    int* dst = result->Transitions[p];
                    for (int c = 0; c < sigma; c++)
                        dst[c] = ai[c] * nb + bj[c];
                    bool bAcc = b->IsAccept[j];
                    result->IsAccept[p] = union ? (aAcc || bAcc) : (aAcc && bAcc);
                }
            }
        }
    }
}
