namespace IAFahim.Graph.Matching
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class StableMarriage
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int* manPref, int* womanPref, int* manMatch, int* womanMatch, int* scratch)
        {
            InitializeMatches(n, manMatch, womanMatch);
            int* manNext = scratch;
            int* womanRank = scratch + n;
            InitializeRanks(n, womanPref, manNext, womanRank);

            int* stack = scratch + n + n * n;
            int top = 0;
            for (int i = 0; i < n; i++) stack[top++] = i;
            while (top > 0)
            {
                int m = stack[--top];
                if (manNext[m] < n) ProcessProposal(n, m, manPref, womanRank, manMatch, womanMatch, manNext, stack, ref top);
            }
        }

        private static void InitializeMatches(int n, int* manMatch, int* womanMatch)
        {
            for (int i = 0; i < n; i++) { manMatch[i] = -1; womanMatch[i] = -1; }
        }

        private static void InitializeRanks(int n, int* womanPref, int* manNext, int* womanRank)
        {
            for (int i = 0; i < n; i++) manNext[i] = 0;
            for (int w = 0; w < n; w++)
                for (int r = 0; r < n; r++)
                    womanRank[w * n + womanPref[w * n + r]] = r;
        }

        private static void ProcessProposal(int n, int m, int* manPref, int* womanRank, int* manMatch, int* womanMatch, int* manNext, int* stack, ref int top)
        {
            int w = manPref[m * n + manNext[m]++];
            if (womanMatch[w] == -1) { manMatch[m] = w; womanMatch[w] = m; }
            else
            {
                int m2 = womanMatch[w];
                int* wRow = womanRank + w * n;
                if (wRow[m] < wRow[m2])
                {
                    manMatch[m2] = -1; manMatch[m] = w; womanMatch[w] = m;
                    stack[top++] = m2;
                }
                else stack[top++] = m;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsStable(int n, int* manPref, int* womanPref, int* manMatch, int* womanMatch, int* scratch)
        {
            int* womanRank = scratch;
            for (int w = 0; w < n; w++)
                for (int r = 0; r < n; r++)
                    womanRank[w * n + womanPref[w * n + r]] = r;

            for (int m = 0; m < n; m++)
                if (!CheckStabilityForMan(n, m, manPref, womanRank, manMatch, womanMatch)) return false;
            return true;
        }

        private static bool CheckStabilityForMan(int n, int m, int* manPref, int* womanRank, int* manMatch, int* womanMatch)
        {
            int w = manMatch[m];
            if (w == -1) return false;
            for (int i = 0; i < n; i++)
            {
                int w2 = manPref[m * n + i];
                if (w2 == w) break;
                int m2 = womanMatch[w2];
                if (m2 != -1)
                {
                    int* w2Row = womanRank + w2 * n;
                    if (w2Row[m] < w2Row[m2]) return false;
                }
            }
            return true;
        }
    }

    public static unsafe class GaleShapley
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int* proposerPref, int* receiverPref, int* proposerMatch, int* receiverMatch, int* scratch)
        {
            StableMarriage.Run(n, proposerPref, receiverPref, proposerMatch, receiverMatch, scratch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ProposeStep(int n, int m, int w, int* womanRank, int* manMatch, int* womanMatch, int* stack, ref int top)
        {
            if (womanMatch[w] == -1)
            {
                manMatch[m] = w;
                womanMatch[w] = m;
            }
            else
            {
                int m2 = womanMatch[w];
                int* wRow = womanRank + w * n;
                if (wRow[m] < wRow[m2])
                {
                    manMatch[m2] = -1;
                    manMatch[m] = w;
                    womanMatch[w] = m;
                    stack[top++] = m2;
                }
                else
                {
                    stack[top++] = m;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunWithHistory(int n, int* manPref, int* womanPref, int* manMatch, int* womanMatch, int* history, int* histSize, int* scratch)
        {
            for (int i = 0; i < n; i++) manMatch[i] = -1;
            for (int i = 0; i < n; i++) womanMatch[i] = -1;
            int* manNext = scratch;
            int* womanRank = scratch + n;
            for (int i = 0; i < n; i++) manNext[i] = 0;
            for (int w = 0; w < n; w++)
            {
                for (int r = 0; r < n; r++)
                {
                    int mID = womanPref[w * n + r];
                    womanRank[w * n + mID] = r;
                }
            }
            int* stack = scratch + n + n * n;
            int top = 0;
            for (int i = 0; i < n; i++) stack[top++] = i;
            *histSize = 0;
            while (top > 0)
            {
                int m = stack[--top];
                if (manNext[m] >= n) continue;
                history[(*histSize)++] = m;
                int w = manPref[m * n + manNext[m]++];
                history[(*histSize)++] = w;
                ProposeStep(n, m, w, womanRank, manMatch, womanMatch, stack, ref top);
            }
        }
    }
}