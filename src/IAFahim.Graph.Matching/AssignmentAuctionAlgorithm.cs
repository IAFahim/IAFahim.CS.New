namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class AssignmentAuctionAlgorithm
    {
        // Bertsekas auction for the square min-cost assignment problem.
        // cost[i * n + j] = cost of assigning row i to column j (integers).
        // match[i]   = column assigned to row i (always assigned for n > 0).
        // prices[j]  = auction (dual) price of column j.
        //
        // Costs are scaled by (n + 1) and the bid uses epsilon = 1, so the
        // effective epsilon is 1/(n+1) < 1/n: with integer costs this yields
        // the exact minimum-cost assignment (prices are integer scaled duals).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* cost, int n, int* match, int* prices)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            for (int j = 0; j < n; j++) prices[j] = 0;
            if (n <= 0) return;

            const int Epsilon = 1;
            int scale = n + 1;

            int* matchObj = stackalloc int[n];
            for (int j = 0; j < n; j++) matchObj[j] = -1;

            // Scan-based control loop: each pass bids for every still-unassigned
            // row. A row becomes unassigned again when another row outbids it for
            // its column, so we rescan until a full pass assigns everyone. No
            // growable queue -> no stackalloc overflow.
            long cap = 8L * n * n + 16;
            long iter = 0;
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < n; i++)
                {
                    if (match[i] != -1) continue;
                    if (++iter > cap) { changed = false; break; }

                    long bestNet = long.MinValue;
                    long secondNet = long.MinValue;
                    int bestJ = 0;
                    for (int j = 0; j < n; j++)
                    {
                        long net = -(long)cost[i * n + j] * scale - prices[j];
                        if (net > bestNet) { secondNet = bestNet; bestNet = net; bestJ = j; }
                        else if (net > secondNet) secondNet = net;
                    }

                    long bid = bestNet - secondNet + Epsilon;
                    prices[bestJ] += (int)bid;

                    int prev = matchObj[bestJ];
                    match[i] = bestJ;
                    matchObj[bestJ] = i;
                    if (prev != -1) match[prev] = -1;
                    changed = true;
                }
            }
        }
    }
}
