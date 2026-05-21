namespace IAFahim.String.SuffixArray
{
    using System;

    public static unsafe class LcpInterval
    {
        public static int Find(int* sa, int* lcp, int len, int* lcpIntv, int queryStart, int queryLen)
        {
            int l = 0, r = len - 1;
            while (l < r)
            {
                int mid = (l + r + 1) >> 1;
                if (lcp[mid] >= queryLen) l = mid;
                else r = mid - 1;
            }
            int interval = 0;
            for (int i = l; i < len; i++)
            {
                if (lcp[i] >= queryLen) interval++;
                else break;
            }
            return interval;
        }
    }
}
