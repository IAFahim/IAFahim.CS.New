namespace IAFahim.Graph.RandomWalk
{
    using System.Runtime.CompilerServices;

    public static unsafe class SimpleRandomWalk
    {
        // Walk steps hops from start using LCG RNG. Writes visited nodes to outPath (length steps+1).
        // Graph undirected/directed via head/to/next (sentinel 0). Returns true if completed.
        public static bool Run(int n, int start, int steps, int* head, int* to, int* next, uint* rngState, int* outPath)
        {
            if (n <= 0 || steps < 0) return false;
            if ((uint)start >= (uint)n) return false;
            int cur = start;
            outPath[0] = cur;
            for (int s = 0; s < steps; s++)
            {
                int deg = 0;
                for (int e = head[cur]; e != 0; e = next[e]) deg++;
                if (deg == 0) { outPath[s + 1] = cur; continue; }
                uint r = Next(rngState);
                int pick = (int)(r % (uint)deg);
                int e2 = head[cur];
                for (int k = 0; k < pick; k++) e2 = next[e2];
                cur = to[e2];
                outPath[s + 1] = cur;
            }
            return true;
        }

        // Power-iteration PageRank-like scores (not normalized to sum 1 exactly each step).
        // scores length n, initialized by caller (e.g. 1/n).
        public static void PageRankIterate(
            int n, int* head, int* to, int* next, double* scores, double* nextScores,
            double damping, int iterations)
        {
            if (n <= 0) return;
            for (int it = 0; it < iterations; it++)
            {
                double baseShare = (1.0 - damping) / n;
                for (int i = 0; i < n; i++) nextScores[i] = baseShare;
                for (int u = 0; u < n; u++)
                {
                    int deg = 0;
                    for (int e = head[u]; e != 0; e = next[e]) deg++;
                    if (deg == 0)
                    {
                        double share = damping * scores[u] / n;
                        for (int v = 0; v < n; v++) nextScores[v] += share;
                    }
                    else
                    {
                        double share = damping * scores[u] / deg;
                        for (int e = head[u]; e != 0; e = next[e])
                            nextScores[to[e]] += share;
                    }
                }
                for (int i = 0; i < n; i++) scores[i] = nextScores[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Next(uint* state)
        {
            uint x = *state;
            if (x == 0) x = 1;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            *state = x;
            return x;
        }
    }
}
