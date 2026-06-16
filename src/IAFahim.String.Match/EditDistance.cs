namespace IAFahim.String.Match
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class EditDistance
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hamming(byte* a, byte* b, int len)
        {
            int dist = 0;
            for (int i = 0; i < len; i++) dist += a[i] != b[i] ? 1 : 0;
            return dist;
        }

        public static int Levenshtein(byte* a, int lenA, byte* b, int lenB, int maxDist)
        {
            if (Math.Abs(lenA - lenB) > maxDist) return maxDist + 1;
            int* prev = (int*)Marshal.AllocHGlobal(sizeof(int) * (lenB + 1)), curr = (int*)Marshal.AllocHGlobal(sizeof(int) * (lenB + 1));
            try
            {
                InitializeLevenshtein(lenB, prev);
                for (int i = 1; i <= lenA; i++)
                {
                    if (UpdateLevenshteinRow(i, lenB, a, b, prev, curr) > maxDist) break;
                    SwapBuffers(ref prev, ref curr);
                }
                return prev[lenB];
            }
            finally { Marshal.FreeHGlobal((nint)prev); Marshal.FreeHGlobal((nint)curr); }
        }

        private static void InitializeLevenshtein(int lenB, int* prev)
        {
            for (int j = 0; j <= lenB; j++) prev[j] = j;
        }

        private static int UpdateLevenshteinRow(int i, int lenB, byte* a, byte* b, int* prev, int* curr)
        {
            curr[0] = i; int min = i;
            byte ai = a[i - 1];
            for (int j = 1; j <= lenB; j++)
            {
                int cost = ai == b[j - 1] ? 0 : 1;
                curr[j] = Math.Min(Math.Min(prev[j] + 1, curr[j - 1] + 1), prev[j - 1] + cost);
                if (curr[j] < min) min = curr[j];
            }
            return min;
        }

        private static void SwapBuffers(ref int* a, ref int* b) { int* t = a; a = b; b = t; }

        /// <summary>
        /// Returns true when the Levenshtein edit distance between <paramref name="a"/>
        /// (length <paramref name="lenA"/>) and <paramref name="b"/> (length
        /// <paramref name="lenB"/>) is at most <paramref name="k"/>, using a rolling-row
        /// dynamic program with Ukkonen's whole-row pruning. The caller supplies
        /// <paramref name="v"/> as scratch of size lenB+1 (one DP row). When
        /// <paramref name="trace"/> is non-null it must point to a buffer of
        /// (lenA+1)*(lenB+1) bools and receives dp[i][j] &lt;= k reachability flags indexed
        /// as trace[i*(lenB+1)+j].
        /// </summary>
        public static bool Ukkonen(byte* a, int lenA, byte* b, int lenB, int k, int* v, bool* trace)
        {
            // The edit distance is at least the length difference, so reject early.
            if (lenA - lenB > k || lenB - lenA > k) return false;

            int stride = lenB + 1;

            // Row 0: dp[0][j] = j.
            for (int j = 0; j <= lenB; j++)
            {
                v[j] = j;
                if (trace != null) trace[j] = j <= k;
            }

            for (int i = 1; i <= lenA; i++)
            {
                int rowBase = i * stride;
                int diag = v[0]; // dp[i-1][0] before it is overwritten
                v[0] = i;        // dp[i][0]
                if (trace != null) trace[rowBase] = i <= k;

                byte ai = a[i - 1];
                int rowMin = i;
                for (int j = 1; j <= lenB; j++)
                {
                    int up = v[j];   // dp[i-1][j]
                    int cost = ai == b[j - 1] ? 0 : 1;
                    int best = diag + cost;
                    int delUp = up + 1;
                    if (delUp < best) best = delUp;
                    int delLeft = v[j - 1] + 1;
                    if (delLeft < best) best = delLeft;

                    diag = up;
                    v[j] = best;
                    if (best < rowMin) rowMin = best;
                    if (trace != null) trace[rowBase + j] = best <= k;
                }

                // Ukkonen pruning: once every cell of a row exceeds k, the minimum of each
                // following row stays > k, so the final distance is certainly > k.
                if (rowMin > k) return false;
            }

            return v[lenB] <= k;
        }
    }
}
