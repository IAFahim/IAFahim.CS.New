namespace IAFahim.String.FMIndex
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class FMIndex
    {
        public static void Build(int* text, int len, int sigma, int* occ)
        {
            for (int c = 0; c < sigma; c++) occ[c * (len + 1)] = 0;
            for (int i = 0; i < len; i++)
            {
                for (int c = 0; c < sigma; c++) occ[c * (len + 1) + i + 1] = occ[c * (len + 1) + i];
                occ[text[i] * (len + 1) + i + 1]++;
            }
        }

        public static int Count(int* text, int len, int* pattern, int patLen, int* sa)
        {
            int start = FindBound(text, len, pattern, patLen, sa, true);
            int end = FindBound(text, len, pattern, patLen, sa, false);
            return end - start;
        }

        private static int FindBound(int* text, int len, int* pattern, int patLen, int* sa, bool lower)
        {
            int l = 0, r = len;
            while (l < r)
            {
                int mid = (l + r) >> 1;
                int cmp = CompareWithSa(text, len, pattern, patLen, sa[mid]);
                if (lower ? cmp >= 0 : cmp > 0) r = mid; else l = mid + 1;
            }
            return l;
        }

        private static int CompareWithSa(int* text, int len, int* pattern, int patLen, int saPos)
        {
            int cmpLen = Math.Min(patLen, len - saPos);
            int cmp = CompareRange(text + saPos, pattern, cmpLen);
            if (cmp == 0 && patLen > cmpLen) return -1;
            return cmp;
        }

        public static void Locate(int* text, int len, int* occ, int* pattern, int patLen, int* sa, int* result, int* count)
        {
            int start = FindBound(text, len, pattern, patLen, sa, true);
            int end = FindBound(text, len, pattern, patLen, sa, false);
            *count = end - start;
            for (int i = start; i < end; i++) result[i - start] = sa[i];
        }

        private static int CompareRange(int* a, int* b, int len)
        {
            for (int i = 0; i < len; i++) if (a[i] != b[i]) return a[i] - b[i];
            return 0;
        }
    }
}
