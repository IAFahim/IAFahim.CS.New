namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphFirstMeeting
    {
        // First common node on the forward paths of u and v under f.
        // Each node has exactly one successor, so a forward path is a tail
        // that descends into a cycle. The two paths share a node iff they
        // enter the same cycle; the first shared node is then found by
        // aligning the two paths to equal remaining length and stepping
        // together (the linked-list-intersection technique generalized to
        // rho-shaped functional graphs).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* f, int n, int u, int v)
        {
            // Locate u's cycle entry and cycle length via Floyd, then measure
            // the cyclic distance from v's cycle entry to u's cycle entry; if
            // v never reaches u's cycle, the paths are disjoint.
            int cycleEntry = CycleEntry(f, u, out int cycleLen);

            // Distance from u to its cycle entry (tail length of u).
            int du = Distance(f, u, cycleEntry);

            // Find where v reaches u's cycle. Walk v until it lands on u's cycle
            // (any node within cycleLen steps that coincides with the cycle),
            // bounded by tail length so disjoint components terminate.
            // Determine v's distance to u's cycle entry, or detect disjointness.
            int dv = DistanceOnCycle(f, v, cycleEntry, cycleLen, out bool meets);
            if (!meets) return -1;

            // Advance the longer tail so both are equidistant from the merge,
            // then step in lockstep to the first common node.
            int a = u;
            int b = v;
            while (du > dv) { a = f[a]; du--; }
            while (dv > du) { b = f[b]; dv--; }
            while (a != b)
            {
                a = f[a];
                b = f[b];
            }
            return a;
        }

        // Floyd cycle detection: returns the cycle entry node and outputs the
        // cycle length.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CycleEntry(int* f, int u, out int cycleLen)
        {
            int slow = f[u];
            int fast = f[f[u]];
            while (slow != fast)
            {
                slow = f[slow];
                fast = f[f[fast]];
            }
            // slow == fast: a point on the cycle. Measure cycle length.
            cycleLen = 1;
            int p = f[slow];
            while (p != slow)
            {
                p = f[p];
                cycleLen++;
            }
            // Find entry: reset one pointer to u, advance both one step.
            slow = u;
            while (slow != fast)
            {
                slow = f[slow];
                fast = f[fast];
            }
            return slow;
        }

        // Number of f-steps from src to reach target (target is reachable and on
        // src's path). Bounded by following until we hit target.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Distance(int* f, int src, int target)
        {
            int d = 0;
            int c = src;
            while (c != target)
            {
                c = f[c];
                d++;
            }
            return d;
        }

        // Walk src forward; once it enters a cycle, check whether that cycle
        // contains 'entry' (u's cycle entry). If so, report the distance from
        // src to 'entry' and meets=true; otherwise meets=false.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DistanceOnCycle(int* f, int src, int entry, int cycleLen, out bool meets)
        {
            // Find src's own cycle entry.
            int srcEntry = CycleEntry(f, src, out int srcCycleLen);

            // Same cycle iff cycle lengths match and srcEntry is reachable from
            // entry within cycleLen steps (i.e. lies on the same cycle).
            if (srcCycleLen == cycleLen)
            {
                int probe = entry;
                for (int i = 0; i < cycleLen; i++)
                {
                    if (probe == srcEntry)
                    {
                        // Same cycle. Distance from src to 'entry' = tail of src
                        // (src->srcEntry) plus forward steps srcEntry->entry on
                        // the cycle.
                        int tail = Distance(f, src, srcEntry);
                        int around = 0;
                        int q = srcEntry;
                        while (q != entry)
                        {
                            q = f[q];
                            around++;
                        }
                        meets = true;
                        return tail + around;
                    }
                    probe = f[probe];
                }
            }

            meets = false;
            return 0;
        }
    }
}
