namespace IAFahim.String.SuffixArray
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class SuffixArray
    {
        public static void Build(byte* ptr, int len, int* sa, int* rank, int* tmpSa, int* count, int* tmpRank)
        {
            if (len == 0) return;
            if (len == 1) { sa[0] = 0; return; }

            int maxSigma = InitializeSuffixArray(ptr, len, sa, rank, count);

            for (int h = 1; h < len; h <<= 1)
            {
                SortByRank(len, h, sa, rank, tmpSa, count, maxSigma);
                maxSigma = UpdateRank(len, h, sa, rank, tmpRank);
                if (maxSigma == len) break;
            }
        }

        private static int InitializeSuffixArray(byte* ptr, int len, int* sa, int* rank, int* count)
        {
            int maxSigma = 256; if (len > 256) maxSigma = len;
            for (int i = 0; i < len * 2; i++) rank[i] = -1;
            for (int i = 0; i < maxSigma; i++) count[i] = 0;
            for (int i = 0; i < len; i++) { rank[i] = ptr[i]; count[rank[i]]++; }
            for (int i = 1; i < maxSigma; i++) count[i] += count[i - 1];
            for (int i = len - 1; i >= 0; i--) sa[--count[rank[i]]] = i;
            return maxSigma;
        }

        private static void SortByRank(int len, int h, int* sa, int* rank, int* tmpSa, int* count, int maxSigma)
        {
            int p = 0;
            for (int i = len - h; i < len; i++) tmpSa[p++] = i;
            for (int i = 0; i < len; i++) if (sa[i] >= h) tmpSa[p++] = sa[i] - h;

            for (int i = 0; i < maxSigma; i++) count[i] = 0;
            for (int i = 0; i < len; i++) count[rank[i]]++;
            for (int i = 1; i < maxSigma; i++) count[i] += count[i - 1];
            for (int i = len - 1; i >= 0; i--) sa[--count[rank[tmpSa[i]]]] = tmpSa[i];
        }

        private static int UpdateRank(int len, int h, int* sa, int* rank, int* tmpRank)
        {
            tmpRank[sa[0]] = 0;
            int p = 0;
            for (int i = 1; i < len; i++)
            {
                if (rank[sa[i]] == rank[sa[i - 1]] && rank[sa[i] + h] == rank[sa[i - 1] + h])
                    tmpRank[sa[i]] = p;
                else
                    tmpRank[sa[i]] = ++p;
            }
            for (int i = 0; i < len; i++) rank[i] = tmpRank[i];
            return p + 1;
        }
    }
}
