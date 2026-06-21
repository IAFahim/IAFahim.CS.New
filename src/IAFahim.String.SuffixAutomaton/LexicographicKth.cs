namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LexicographicKth
    {
        public static bool Find(byte* text, int textLen, long k, byte* outBuf, int* outLen)
        {
            if (k <= 0 || textLen <= 0) { *outLen = 0; return false; }
            long totalSubs = (long)textLen * (textLen + 1) / 2;
            if (k > totalSubs) { *outLen = 0; return false; }

            int pairCount = (int)totalSubs;
            int* starts = stackalloc int[pairCount];
            int* lens = stackalloc int[pairCount];
            int idx = 0;
            for (int len = 1; len <= textLen; len++)
            {
                for (int start = 0; start + len <= textLen; start++)
                {
                    starts[idx] = start;
                    lens[idx] = len;
                    idx++;
                }
            }

            int target = (int)k;
            QuickSelect(text, starts, lens, 0, pairCount - 1, target - 1);

            int s = starts[target - 1];
            int l = lens[target - 1];
            for (int i = 0; i < l; i++) outBuf[i] = text[s + i];
            *outLen = l;
            return true;
        }

        private static void QuickSelect(byte* text, int* starts, int* lens, int lo, int hi, int k)
        {
            while (lo < hi)
            {
                int pivot = Partition(text, starts, lens, lo, hi);
                if (pivot == k) return;
                if (pivot < k) lo = pivot + 1;
                else hi = pivot - 1;
            }
        }

        private static int Partition(byte* text, int* starts, int* lens, int lo, int hi)
        {
            int ps = starts[hi], pl = lens[hi];
            int i = lo - 1;
            for (int j = lo; j < hi; j++)
            {
                if (CompareSub(text, starts[j], lens[j], ps, pl) <= 0)
                {
                    i++;
                    int ts = starts[i]; starts[i] = starts[j]; starts[j] = ts;
                    int tl = lens[i]; lens[i] = lens[j]; lens[j] = tl;
                }
            }
            int ts2 = starts[i + 1]; starts[i + 1] = starts[hi]; starts[hi] = ts2;
            int tl2 = lens[i + 1]; lens[i + 1] = lens[hi]; lens[hi] = tl2;
            return i + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareSub(byte* text, int s1, int l1, int s2, int l2)
        {
            int minLen = l1 < l2 ? l1 : l2;
            for (int i = 0; i < minLen; i++)
            {
                if (text[s1 + i] < text[s2 + i]) return -1;
                if (text[s1 + i] > text[s2 + i]) return 1;
            }
            return l1.CompareTo(l2);
        }
    }
}
