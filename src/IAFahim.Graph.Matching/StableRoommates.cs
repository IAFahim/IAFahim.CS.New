namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class StableRoommates
    {
        private const int Unmatched = -1;
        private const int NoPosition = -1;
        private const int NotSeen = 0;

        // Number of int slots of scratch required for a problem of size n:
        //   rank(n*n) + nextPos(n*n) + prevPos(n*n) + head(n) + tail(n)
        //   + holder(n) + stack(n) + rotX(n) + rotY(n) + seenAt(n)
        //   = 3*n*n + 7*n
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ScratchSize(int n) => 3 * n * n + 7 * n;

        // Solves the Stable Roommates problem with Irving's algorithm.
        //
        // pref:  n*n row-major preference matrix. Row i lists the OTHER n-1
        //        participants in i's order of preference at positions
        //        0..n-2 (column n-1 of each row is unused). Entries must be a
        //        permutation of {0..n-1}\{i}.
        // n:     number of participants (need not be even).
        // match: output, length n. On success match[i] is i's partner and the
        //        relation is symmetric (match[match[i]] == i).
        // scratch: caller-provided working buffer of length at least
        //          ScratchSize(n) = 3*n*n + 7*n ints.
        //
        // Returns true and fills match when a stable matching exists.
        // Returns false (and sets every match[i] = -1) otherwise.
        //
        // Unchecked: the caller guarantees pref/match/scratch are non-null,
        // correctly sized, and that each row of pref is a valid ranking.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* pref, int n, int* match, int* scratch)
        {
            for (int i = 0; i < n; i++) match[i] = Unmatched;
            if (n <= 0) return true;
            if (n == 1) return false; // a single participant cannot be matched

            // Scratch layout.
            int* rank = scratch;                 // n*n : rank[i*n + j] = position of j in i's list (n if absent)
            int* nextPos = rank + n * n;         // n*n : doubly linked reduced list, next position
            int* prevPos = nextPos + n * n;      // n*n : doubly linked reduced list, prev position
            int* head = prevPos + n * n;         // n   : first valid position in i's reduced list
            int* tail = head + n;                // n   : last  valid position in i's reduced list
            int* holder = tail + n;              // n   : proposer that i currently holds (-1 = none)
            int* stack = holder + n;             // n   : work stack of free proposers
            int* rotX = stack + n;               // n   : rotation trace, x participants
            int* rotY = rotX + n;                // n   : rotation trace, y participants
            int* seenAt = rotY + n;              // n   : 1 + index of participant in current trace (0 = unseen)

            BuildRanks(pref, n, rank);
            BuildLists(n, nextPos, prevPos, head, tail);
            for (int i = 0; i < n; i++) { holder[i] = Unmatched; seenAt[i] = NotSeen; }

            if (!Phase1(pref, n, rank, nextPos, prevPos, head, tail, holder, stack))
                return false;

            if (!Phase2(pref, n, rank, nextPos, prevPos, head, tail, rotX, rotY, seenAt))
                return false;

            // Each reduced list now has exactly one entry: the partner.
            for (int i = 0; i < n; i++)
            {
                int p = head[i];
                if (p == NoPosition) return false;
                match[i] = pref[i * n + p];
            }
            return true;
        }

        // Builds rank[i*n + j] = position of j in i's preference list.
        // Self and any absent participant get the sentinel n (worst possible).
        private static void BuildRanks(int* pref, int n, int* rank)
        {
            for (int i = 0; i < n; i++)
            {
                int* row = rank + i * n;
                for (int j = 0; j < n; j++) row[j] = n;
                int* prow = pref + i * n;
                for (int r = 0; r < n - 1; r++) row[prow[r]] = r;
            }
        }

        // Builds, per participant, a doubly linked list over the n-1 valid
        // positions of its preference row so entries can be removed in O(1).
        private static void BuildLists(int n, int* nextPos, int* prevPos, int* head, int* tail)
        {
            int last = n - 2; // last valid position index
            for (int i = 0; i < n; i++)
            {
                int* nx = nextPos + i * n;
                int* pv = prevPos + i * n;
                for (int r = 0; r <= last; r++)
                {
                    nx[r] = r == last ? NoPosition : r + 1;
                    pv[r] = r == 0 ? NoPosition : r - 1;
                }
                head[i] = 0;
                tail[i] = last;
            }
        }

        // Removes participant j from i's reduced list (O(1)). j's stored
        // position is read from the rank table. Updates head/tail.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RemoveFromList(int i, int j, int n, int* rank, int* nextPos, int* prevPos, int* head, int* tail)
        {
            int p = rank[i * n + j];
            if (p == n) return; // j not present in i's list
            int* nx = nextPos + i * n;
            int* pv = prevPos + i * n;
            int prev = pv[p];
            int next = nx[p];
            if (prev == NoPosition) head[i] = next; else nx[prev] = next;
            if (next == NoPosition) tail[i] = prev; else pv[next] = prev;
        }

        // Phase 1: proposal sequence. Each participant proposes down its list;
        // a receiver holds its best proposer and rejects (mutually deleting)
        // everyone it likes less. Fails if any list becomes empty.
        private static bool Phase1(int* pref, int n, int* rank, int* nextPos, int* prevPos, int* head, int* tail, int* holder, int* stack)
        {
            int top = 0;
            for (int i = 0; i < n; i++) stack[top++] = i;

            while (top > 0)
            {
                int x = stack[--top];
                // x proposes to the head of its list until it is held by
                // someone, or its list empties (failure).
                while (head[x] != NoPosition)
                {
                    int y = pref[x * n + head[x]];
                    int cur = holder[y];
                    if (cur == Unmatched)
                    {
                        holder[y] = x;
                        break;
                    }
                    if (cur == x) break; // already y's holder

                    int* yRank = rank + y * n;
                    if (yRank[x] < yRank[cur])
                    {
                        // y prefers x: hold x, reject cur, delete pair (y, cur).
                        // holder[cur] (who proposed TO cur) is unrelated to cur's
                        // role as a proposer and must NOT be touched here.
                        holder[y] = x;
                        RemoveFromList(y, cur, n, rank, nextPos, prevPos, head, tail);
                        RemoveFromList(cur, y, n, rank, nextPos, prevPos, head, tail);
                        stack[top++] = cur; // cur must propose to its next choice
                        break;
                    }

                    // y prefers cur: y rejects x. Delete pair (x, y); x retries.
                    RemoveFromList(x, y, n, rank, nextPos, prevPos, head, tail);
                    RemoveFromList(y, x, n, rank, nextPos, prevPos, head, tail);
                }

                if (head[x] == NoPosition) return false;
            }

            // Reduce to the phase-1 table: each receiver y keeps its best
            // proposer holder[y] as the WORST entry of its list, deleting (both
            // ways) everyone y ranks strictly below that proposer. This restores
            // the invariant first(p)=q  <=>  last(q)=p needed by phase 2.
            for (int y = 0; y < n; y++)
            {
                int h = holder[y];
                if (h == Unmatched) return false; // y received no proposal: no matching
                int hPos = rank[y * n + h];
                if (hPos == n) return false; // inconsistent: proposer absent from list
                int* nx = nextPos + y * n;
                int* yrow = pref + y * n;
                int p = nx[hPos];
                while (p != NoPosition)
                {
                    int victim = yrow[p];
                    int nextP = nx[p];
                    RemoveFromList(y, victim, n, rank, nextPos, prevPos, head, tail);
                    RemoveFromList(victim, y, n, rank, nextPos, prevPos, head, tail);
                    p = nextP;
                }
            }

            for (int i = 0; i < n; i++)
                if (head[i] == NoPosition) return false;
            return true;
        }

        // Phase 2: repeatedly find and eliminate all-or-nothing rotations until
        // every reduced list has length one (success) or a list empties
        // (failure: no stable matching exists).
        //
        // Invariant maintained by phase 1 and each elimination: for every p, if
        // q = first(p) then p = last(q). A rotation is the cyclic sequence
        //   y_k = second(x_k),   x_{k+1} = last(y_k) = the unique p with first(p)=y_k.
        // Eliminating it mutually deletes each pair (y_k, x_{k+1}).
        private static bool Phase2(int* pref, int n, int* rank, int* nextPos, int* prevPos, int* head, int* tail, int* rotX, int* rotY, int* seenAt)
        {
            while (true)
            {
                // Find any participant whose reduced list still has >= 2 entries.
                int start = NoPosition;
                for (int i = 0; i < n; i++)
                {
                    if (head[i] != tail[i]) { start = i; break; }
                }
                if (start == NoPosition) return true; // all lists length one: done

                // Trace the rotation, recording x's in rotX, y's in rotY.
                // seenAt[p] = 1 + traceIndex marks p as an x already visited.
                int len = 0;
                int x = start;
                int cycleStart;
                while (true)
                {
                    int xFirst = head[x];
                    int secondPos = nextPos[x * n + xFirst];
                    if (secondPos == NoPosition) { cycleStart = NoPosition; break; } // safety
                    int y = pref[x * n + secondPos];

                    seenAt[x] = len + 1;
                    rotX[len] = x;
                    rotY[len] = y;
                    len++;

                    int nextX = pref[y * n + tail[y]]; // last(y)
                    if (seenAt[nextX] != NotSeen)
                    {
                        cycleStart = seenAt[nextX] - 1; // cycle is [cycleStart .. len-1]
                        break;
                    }
                    x = nextX;
                }

                // Clear seen marks for the traced participants.
                for (int t = 0; t < len; t++) seenAt[rotX[t]] = NotSeen;

                if (cycleStart == NoPosition) return false; // should not happen on valid input

                // The rotation has L entries. Following the canonical "lasts /
                // seconds" formulation: lasts[k] = rotX[k] for k < len and
                // lasts[len] = rotX[cycleStart] (the participant that closed the
                // cycle), seconds[k] = rotY[k]. Cycle entry i (i = 0..L-1) is
                //   right_i = seconds[cycleStart + i] = rotY[cycleStart + i]
                //   left_i  = lasts[cycleStart + 1 + i]
                // To eliminate, right_i rejects every participant it ranks
                // strictly below left_{(i-1) mod L} (mutual deletion).
                int rotLen = len - cycleStart;
                for (int i = 0; i < rotLen; i++)
                {
                    int owner = rotY[cycleStart + i];
                    int prevI = i == 0 ? rotLen - 1 : i - 1;
                    // left_{prevI} = lasts[cycleStart + 1 + prevI]; the last index
                    // (cycleStart + 1 + (rotLen-1) == len) wraps to rotX[cycleStart].
                    int lastsIdx = cycleStart + 1 + prevI;
                    int pivot = lastsIdx == len ? rotX[cycleStart] : rotX[lastsIdx];
                    if (!EliminateSuccessors(pref, owner, pivot, n, rank, nextPos, prevPos, head, tail))
                        return false;
                }
            }
        }

        // In owner's reduced list, removes every participant ranked strictly
        // below pivot (and removes owner from each of their lists). Returns
        // false if any affected list becomes empty.
        private static bool EliminateSuccessors(int* pref, int owner, int pivot, int n, int* rank, int* nextPos, int* prevPos, int* head, int* tail)
        {
            int pivotPos = rank[owner * n + pivot];
            if (pivotPos == n) return head[owner] != NoPosition; // pivot already gone
            int* nx = nextPos + owner * n;
            int* prow = pref + owner * n;
            // Walk from the entry after pivot down to the tail, deleting each.
            int p = nx[pivotPos];
            while (p != NoPosition)
            {
                int victim = prow[p];
                int nextP = nx[p];
                RemoveFromList(owner, victim, n, rank, nextPos, prevPos, head, tail);
                RemoveFromList(victim, owner, n, rank, nextPos, prevPos, head, tail);
                if (head[victim] == NoPosition) return false;
                p = nextP;
            }
            return head[owner] != NoPosition;
        }
    }
}
