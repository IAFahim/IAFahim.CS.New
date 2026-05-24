namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaximumClique
    {
        public static int Run(int n, bool* adj, int* cand, int candSize, int* sol, long* dp, int* best, int* tmp)
        {
            long timer = 0;
            return Search(n, adj, cand, candSize, 0, sol, 0, best, dp, ref timer, tmp);
        }

        private static int Search(int n, bool* adj, int* cand, int candSize, int depth, int* sol, int solSize, int* best, long* dp, ref long timer, int* tmp)
        {
            if (ShouldStop(ref timer, solSize, candSize, best)) return 0;
            if (candSize == 0) { if (solSize > *best) { *best = solSize; return 1; } return 0; }

            int* currentTmp = tmp + depth * n;
            CopyCandidates(candSize, cand, currentTmp);
            
            while (candSize > 0)
            {
                if (solSize + candSize <= *best) return 0;
                int v = currentTmp[0];
                int nextCandSize = FilterNeighbors(n, v, candSize, currentTmp, cand, adj);
                
                sol[solSize] = v;
                Search(n, adj, cand, nextCandSize, depth + 1, sol, solSize + 1, best, dp, ref timer, tmp);
                
                RemoveCandidate(ref candSize, currentTmp, v);
            }
            return 0;
        }

        private static bool ShouldStop(ref long timer, int solSize, int candSize, int* best)
        {
            timer++;
            return timer > 100000000 || solSize + candSize <= *best;
        }

        private static void CopyCandidates(int candSize, int* src, int* dst)
        {
            for (int i = 0; i < candSize; i++) dst[i] = src[i];
        }

        private static int FilterNeighbors(int n, int v, int candSize, int* currentTmp, int* nextCand, bool* adj)
        {
            int sz = 0;
            for (int i = 0; i < candSize; i++)
                if (adj[v * n + currentTmp[i]]) nextCand[sz++] = currentTmp[i];
            return sz;
        }

        private static void RemoveCandidate(ref int candSize, int* currentTmp, int v)
        {
            candSize--;
            int last = currentTmp[candSize];
            for (int i = 0; i < candSize; i++)
                if (currentTmp[i] == v) currentTmp[i] = last;
        }
    }
}
