namespace IAFahim.Graph.Matching
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class StableMarriage
    {
        public static void Run(int n, int* manPref, int* womanPref, int* manMatch, int* womanMatch)
        {
            for (int i = 0; i < n; i++) manMatch[i] = -1;
            for (int i = 0; i < n; i++) womanMatch[i] = -1;
            int* manNext = stackalloc int[n];
            for (int i = 0; i < n; i++) manNext[i] = 0;
            int* womanRank = stackalloc int[n * n];
            for (int w = 0; w < n; w++)
            {
                for (int r = 0; r < n; r++)
                {
                    int mID = womanPref[w * n + r];
                    womanRank[w * n + mID] = r;
                }
            }
            int* stack = stackalloc int[n];
            int top = 0;
            for (int i = 0; i < n; i++) stack[top++] = i;
            while (top > 0)
            {
                int m = stack[--top];
                if (manNext[m] >= n) continue;
                int w = manPref[m * n + manNext[m]++];
                if (womanMatch[w] == -1)
                {
                    manMatch[m] = w;
                    womanMatch[w] = m;
                }
                else
                {
                    int m2 = womanMatch[w];
                    if (womanRank[w * n + m] < womanRank[w * n + m2])
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
        }

        public static bool IsStable(int n, int* manPref, int* womanPref, int* manMatch, int* womanMatch)
        {
            int* womanRank = stackalloc int[n * n];
            for (int w = 0; w < n; w++)
            {
                for (int r = 0; r < n; r++)
                {
                    int mID = womanPref[w * n + r];
                    womanRank[w * n + mID] = r;
                }
            }
            for (int m = 0; m < n; m++)
            {
                int w = manMatch[m];
                if (w == -1) return false;
                for (int i = 0; i < n; i++)
                {
                    int w2 = manPref[m * n + i];
                    if (w2 == w) break;
                    int m2 = womanMatch[w2];
                    if (m2 == -1) continue;
                    if (womanRank[w2 * n + m] < womanRank[w2 * n + m2])
                        return false;
                }
            }
            return true;
        }
    }

    public static unsafe class GaleShapley
    {
        public static void Run(int n, int* proposerPref, int* receiverPref, int* proposerMatch, int* receiverMatch)
        {
            StableMarriage.Run(n, proposerPref, receiverPref, proposerMatch, receiverMatch);
        }

        public static void RunWithHistory(int n, int* manPref, int* womanPref, int* manMatch, int* womanMatch, int* history, int* histSize)
        {
            for (int i = 0; i < n; i++) manMatch[i] = -1;
            for (int i = 0; i < n; i++) womanMatch[i] = -1;
            int* manNext = stackalloc int[n];
            for (int i = 0; i < n; i++) manNext[i] = 0;
            int* womanRank = stackalloc int[n * n];
            for (int w = 0; w < n; w++)
            {
                for (int r = 0; r < n; r++)
                {
                    int mID = womanPref[w * n + r];
                    womanRank[w * n + mID] = r;
                }
            }
            int* stack = stackalloc int[n];
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
                if (womanMatch[w] == -1)
                {
                    manMatch[m] = w;
                    womanMatch[w] = m;
                }
                else
                {
                    int m2 = womanMatch[w];
                    if (womanRank[w * n + m] < womanRank[w * n + m2])
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
        }
    }
}