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
            timer++;
            if (timer > 100000000) return 0;
            if (candSize == 0)
            {
                if (solSize > *best) { *best = solSize; return 1; }
                return 0;
            }
            int* currentTmp = tmp + depth * n;
            for (int i = 0; i < candSize; i++) currentTmp[i] = cand[i];
            int count = 0;
            while (candSize > 0)
            {
                if (solSize + candSize <= *best) return 0;
                int v = currentTmp[0];
                int sz = 0;
                for (int i = 0; i < candSize; i++)
                {
                    if (adj[v * n + currentTmp[i]]) cand[sz++] = currentTmp[i];
                }
                sol[solSize] = v;
                Search(n, adj, cand, sz, depth + 1, sol, solSize + 1, best, dp, ref timer, tmp);
                candSize--;
                int last = currentTmp[candSize];
                for (int i = 0; i < candSize; i++) currentTmp[i] = currentTmp[i] == v ? last : currentTmp[i];
                for (int i = 0; i < count; i++) cand[i] = cand[i] == v ? cand[count] : cand[i];
                count++;
            }
            return 0;
        }
    }
}
