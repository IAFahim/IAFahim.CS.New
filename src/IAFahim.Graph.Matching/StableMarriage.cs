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
            int* queue = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int i = 0; i < n; i++) queue[qt++] = i;
            while (qh < qt)
            {
                int m = queue[qh++];
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
                    int wPrefM = womanPref[w * n + m];
                    int wPrefM2 = womanPref[w * n + m2];
                    if (wPrefM < wPrefM2)
                    {
                        manMatch[m] = w;
                        womanMatch[w] = m;
                        queue[qt++] = m2;
                    }
                    else
                    {
                        queue[qt++] = m;
                    }
                }
            }
        }

        public static bool IsStable(int n, int* manPref, int* womanPref, int* manMatch, int* womanMatch)
        {
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
                    int mPrefW = manPref[m * n + w];
                    int mPrefW2 = manPref[m * n + w2];
                    int wPrefM = womanPref[w2 * n + m];
                    int wPrefM2 = womanPref[w2 * n + m2];
                    if (mPrefW2 < mPrefW && wPrefM < wPrefM2)
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
            int* queue = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int i = 0; i < n; i++) queue[qt++] = i;
            *histSize = 0;
            while (qh < qt)
            {
                int m = queue[qh++];
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
                    int wPrefM = womanPref[w * n + m];
                    int wPrefM2 = womanPref[w * n + m2];
                    if (wPrefM < wPrefM2)
                    {
                        manMatch[m] = w;
                        womanMatch[w] = m;
                        queue[qt++] = m2;
                    }
                    else
                    {
                        queue[qt++] = m;
                    }
                }
            }
        }
    }
}