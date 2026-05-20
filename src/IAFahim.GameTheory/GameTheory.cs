namespace IAFahim.GameTheory
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Grundy
    {
        public static long Run(int n, long* moves, int moveCount)
        {
            if (moveCount == 0) return 0;
            long mex = 0;
            long seen = 0;
            for (int i = 0; i < moveCount; i++)
            {
                long g = Grundy.Run((int)(n - moves[i]), moves, moveCount);
                if (g >= 0 && g < 64)
                    seen |= 1L << (int)g;
            }
            while ((seen & (1L << (int)mex)) != 0) mex++;
            return mex;
        }
    }

    public static unsafe class GrundyDAG
    {
        public static int Run(int n, int* to, int* grundy, int* indeg, int* queue)
        {
            int front = 0, rear = 0;
            for (int i = 0; i < n; i++)
                if (indeg[i] == 0)
                    queue[rear++] = i;
            int* seen = stackalloc int[n];
            for (int i = 0; i < n; i++) seen[i] = 0;
            while (front < rear)
            {
                int v = queue[front++];
                int mex = 0;
                while ((seen[v] & (1 << mex)) != 0 && mex < 32) mex++;
                grundy[v] = mex;
                if (to[v] >= 0)
                {
                    indeg[to[v]]--;
                    seen[to[v]] |= 1 << mex;
                    if (indeg[to[v]] == 0)
                        queue[rear++] = to[v];
                }
            }
            return rear;
        }
    }

    public static unsafe class NimSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long* piles)
        {
            long xor = 0;
            for (int i = 0; i < n; i++)
                xor ^= piles[i];
            return xor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWinning(long xor)
        {
            return xor != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextMove(long xor, long* piles, int n)
        {
            for (int i = 0; i < n; i++)
            {
                long target = xor ^ piles[i];
                if (target < piles[i])
                    return i;
            }
            return -1;
        }
    }

    public static unsafe class Minimax
    {
        public static long Run(int depth, bool isMax, long alpha, long beta, long* gameState, int player)
        {
            if (depth == 0)
                return Evaluate(gameState, player);
            if (isMax)
            {
                long best = long.MinValue;
                for (int i = 0; i < 64; i++)
                {
                    if (MakeMove(gameState, player, i))
                    {
                        long val = Run(depth - 1, false, alpha, beta, gameState, 1 - player);
                        UndoMove(gameState, i);
                        if (val > best) best = val;
                        if (best > alpha) alpha = best;
                        if (beta <= alpha) break;
                    }
                }
                return best;
            }
            else
            {
                long best = long.MaxValue;
                for (int i = 0; i < 64; i++)
                {
                    if (MakeMove(gameState, player, i))
                    {
                        long val = Run(depth - 1, true, alpha, beta, gameState, 1 - player);
                        UndoMove(gameState, i);
                        if (val < best) best = val;
                        if (best < beta) beta = best;
                        if (beta <= alpha) break;
                    }
                }
                return best;
            }
        }

        private static long Evaluate(long* state, int player)
        {
            return state[0];
        }

        private static bool MakeMove(long* state, int player, int move)
        {
            return state[move] > 0;
        }

        private static void UndoMove(long* state, int move)
        {
        }
    }

    public static unsafe class AlphaBeta
    {
        public static long Run(int depth, bool isMax, long alpha, long beta, long* gameState)
        {
            if (depth == 0)
                return Evaluate(gameState);
            if (isMax)
            {
                long best = long.MinValue;
                for (int i = 0; i < 64; i++)
                {
                    if (MakeMove(gameState, i))
                    {
                        long val = Run(depth - 1, false, alpha, beta, gameState);
                        UndoMove(gameState, i);
                        if (val > best) best = val;
                        if (best > alpha) alpha = best;
                        if (beta <= alpha) break;
                    }
                }
                return best;
            }
            else
            {
                long best = long.MaxValue;
                for (int i = 0; i < 64; i++)
                {
                    if (MakeMove(gameState, i))
                    {
                        long val = Run(depth - 1, true, alpha, beta, gameState);
                        UndoMove(gameState, i);
                        if (val < best) best = val;
                        if (best < beta) beta = best;
                        if (beta <= alpha) break;
                    }
                }
                return best;
            }
        }

        private static long Evaluate(long* state)
        {
            return state[0];
        }

        private static bool MakeMove(long* state, int move)
        {
            return state[move] > 0;
        }

        private static void UndoMove(long* state, int move)
        {
        }
    }

    public static unsafe class RetrogradeAnalysis
    {
        public static int Run(int n, int* state, int* outDegree, int* result, int* queue)
        {
            int front = 0, rear = 0;
            for (int i = 0; i < n; i++)
            {
                if (outDegree[i] == 0)
                    queue[rear++] = i;
            }
            while (front < rear)
            {
                int v = queue[front++];
                result[v] = 1;
                for (int i = 0; i < n; i++)
                {
                    if (state[i * n + v] == 1)
                    {
                        outDegree[i]--;
                        if (outDegree[i] == 0)
                            queue[rear++] = i;
                    }
                }
            }
            return rear;
        }
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
                    int prev = i - moves[j];
                    if (prev >= 0 && prev < n)
                        seen |= 1L << (int)dp[prev];
                }
                int g = 0;
                while ((seen & (1L << g)) != 0) g++;
                dp[i] = g;
            }
            return n;
        }
    }
}