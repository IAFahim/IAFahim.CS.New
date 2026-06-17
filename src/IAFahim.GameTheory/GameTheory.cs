namespace IAFahim.GameTheory
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GrundyDAG
    {
        public static int Run(int n, int* to, int* grundy, int* indeg, int* queue)
        {
            InitializeQueue(n, indeg, queue, out int rear);
            int* seen = stackalloc int[n]; for (int i = 0; i < n; i++) seen[i] = 0;
            int front = 0;
            while (front < rear)
            {
                int v = queue[front++];
                grundy[v] = FindMex(seen[v]);
                if (to[v] >= 0) UpdateNeighbor(to[v], grundy[v], indeg, seen, queue, ref rear);
            }
            return rear;
        }

        private static void InitializeQueue(int n, int* indeg, int* queue, out int rear)
        {
            rear = 0; for (int i = 0; i < n; i++) if (indeg[i] == 0) queue[rear++] = i;
        }

        private static int FindMex(int seenMask)
        {
            int mex = 0; while (mex < 31 && (seenMask & (1 << mex)) != 0) mex++;
            return mex;
        }

        private static void UpdateNeighbor(int nextV, int gv, int* indeg, int* seen, int* queue, ref int rear)
        {
            indeg[nextV]--; seen[nextV] |= 1 << gv;
            if (indeg[nextV] == 0) queue[rear++] = nextV;
        }
    }

    public static unsafe class NimSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long* piles) { long xor = 0; for (int i = 0; i < n; i++) xor ^= piles[i]; return xor; }
    }

    public static unsafe class Minimax
    {
        public static long Run(int depth, bool isMax, long alpha, long beta, long* gameState, int player)
        {
            if (depth == 0) return Evaluate(gameState, player);
            return isMax ? Maximize(depth, alpha, beta, gameState, player) : Minimize(depth, alpha, beta, gameState, player);
        }

        private static long Maximize(int depth, long alpha, long beta, long* state, int player)
        {
            long best = long.MinValue;
            for (int i = 0; i < 64; i++)
                if (MakeMove(state, player, i))
                {
                    long val = Run(depth - 1, false, alpha, beta, state, 1 - player); UndoMove(state, i);
                    if (val > best) best = val; if (best > alpha) alpha = best; if (beta <= alpha) break;
                }
            return best;
        }

        private static long Minimize(int depth, long alpha, long beta, long* state, int player)
        {
            long best = long.MaxValue;
            for (int i = 0; i < 64; i++)
                if (MakeMove(state, player, i))
                {
                    long val = Run(depth - 1, true, alpha, beta, state, 1 - player); UndoMove(state, i);
                    if (val < best) best = val; if (best < beta) beta = best; if (beta <= alpha) break;
                }
            return best;
        }

        private static long Evaluate(long* s, int p) => s[0];
        private static bool MakeMove(long* s, int p, int m) => s[m] > 0;
        private static void UndoMove(long* s, int m) { }
    }

    public static unsafe class GameDp
    {
        public static int Run(int n, long* dp, long* a, int* moves, int moveCount)
        {
            for (int i = 0; i < n; i++)
            {
                long seen = 0;
                for (int j = 0; j < moveCount; j++)
                {
                    int mv = moves[j];
                    if (mv <= 0) continue;
                    int prev = i - mv;
                    if (prev >= 0)
                    {
                        long gp = dp[prev];
                        if (gp >= 0 && gp < 64) seen |= 1L << (int)gp;
                    }
                }
                int g = 0; while (g < 64 && (seen & (1L << g)) != 0) g++;
                dp[i] = g;
            }
            return n;
        }
    }
}
