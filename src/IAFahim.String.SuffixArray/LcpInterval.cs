namespace IAFahim.String.SuffixArray
{
    using System;

    public static unsafe class LcpInterval
    {
        public static int Find(int* sa, int* lcp, int len, int* lcpIntv, int queryStart, int queryLen)
        {
            if ((uint)queryStart >= (uint)len || queryLen < 0)
            {
                lcpIntv[0] = -1;
                lcpIntv[1] = -1;
                return 0;
            }

            if (queryLen == 0)
            {
                lcpIntv[0] = 0;
                lcpIntv[1] = len - 1;
                return len;
            }

            int left = queryStart;
            while (left > 0 && lcp[left] >= queryLen) left--;

            int right = queryStart;
            while (right + 1 < len && lcp[right + 1] >= queryLen) right++;

            lcpIntv[0] = left;
            lcpIntv[1] = right;
            return right - left + 1;
        }
    }
}
