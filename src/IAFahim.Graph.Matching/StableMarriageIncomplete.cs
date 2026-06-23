namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class StableMarriageIncomplete
    {
        // Gale-Shapley deferred acceptance for INCOMPLETE preference lists.
        //
        // Layout (caller-guaranteed, by design for the unchecked Run):
        //   prefMen     : n rows of stride m. prefMen[i * m + k] = id of the k-th woman man i prefers.
        //   prefWomen   : m rows of stride n. prefWomen[j * n + k] = id of the k-th man woman j prefers.
        //   numPrefMen  : length n. numPrefMen[i]   = number of valid entries in man i's list.
        //   numPrefWomen: length m. numPrefWomen[j] = number of valid entries in woman j's list.
        //   matchMen    : length n (output). matchMen[i]   = woman matched to man i, or -1.
        //   matchWomen  : length m (output). matchWomen[j] = man matched to woman j, or -1.
        //
        // Men propose. A woman accepts a proposer only if he appears on her list, and
        // prefers, among the men on her list, the one of lower rank (earlier index).
        // Agents whose acceptable partners are all taken by someone better stay at -1.
        private const int Unmatched = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HandleProposal(int woman, int man, int* prefWomen, int* numPrefWomen, int n, int* matchMen, int* matchWomen, int* freeStack, ref int top)
        {
            int current = matchWomen[woman];
            int rankNew = RankOf(prefWomen, numPrefWomen, n, woman, man);
            if (current == Unmatched)
            {
                if (rankNew >= 0)
                {
                    matchWomen[woman] = man;
                    matchMen[man] = woman;
                    return true;
                }
                return false;
            }
            if (rankNew < 0) return false;
            int rankCur = RankOf(prefWomen, numPrefWomen, n, woman, current);
            if (rankNew < rankCur)
            {
                matchMen[current] = Unmatched;
                freeStack[top++] = current;
                matchWomen[woman] = man;
                matchMen[man] = woman;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* prefMen, int* prefWomen, int* numPrefMen, int* numPrefWomen, int n, int m, int* matchMen, int* matchWomen)
        {
            for (int i = 0; i < n; i++) matchMen[i] = Unmatched;
            for (int j = 0; j < m; j++) matchWomen[j] = Unmatched;
            if (n <= 0 || m <= 0) return;

            // scratch: nextProposal[n] (per-man cursor into his list) + freeStack[n].
            int* nextProposal = stackalloc int[n];
            int* freeStack = stackalloc int[n];

            int top = 0;
            for (int i = 0; i < n; i++)
            {
                nextProposal[i] = 0;
                freeStack[top++] = i;
            }

            while (top > 0)
            {
                int man = freeStack[--top];
                int listLen = numPrefMen[man];
                int* manList = prefMen + (long)man * m;

                // Propose down the man's list until accepted or list exhausted.
                while (nextProposal[man] < listLen)
                {
                    int woman = manList[nextProposal[man]++];
                    if (HandleProposal(woman, man, prefWomen, numPrefWomen, n, matchMen, matchWomen, freeStack, ref top)) break;
                }
            }
        }

        // Returns the rank (index) of man within woman's preference list, or -1 if absent.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RankOf(int* prefWomen, int* numPrefWomen, int n, int woman, int man)
        {
            int len = numPrefWomen[woman];
            int* womanList = prefWomen + (long)woman * n;
            for (int r = 0; r < len; r++)
                if (womanList[r] == man) return r;
            return -1;
        }
    }
}
