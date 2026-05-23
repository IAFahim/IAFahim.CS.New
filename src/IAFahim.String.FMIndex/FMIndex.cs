namespace IAFahim.String.FMIndex
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class FMIndex
    {
        public static void Build(int* text, int len, int sigma, int* occ)
        {
            for (int c = 0; c < sigma; c++)
                occ[c * (len + 1)] = 0;
            for (int i = 0; i < len; i++)
            {
                for (int c = 0; c < sigma; c++)
                    occ[c * (len + 1) + i + 1] = occ[c * (len + 1) + i];
                occ[text[i] * (len + 1) + i + 1]++;
            }
        }

        public static int Count(int* text, int len, int* pattern, int patLen, int* sa)
        {
            int l = 0, r = len;
            while (l < r)
            {
                int mid = (l + r) >> 1;
                int pos = sa[mid];
                int cmpLen = Math.Min(patLen, len - pos);
                if (CompareRange(text, pos, pattern, 0, cmpLen) >= 0)
                    r = mid;
                else
                    l = mid + 1;
            }
            int start = l;
            l = 0; r = len;
            while (l < r)
            {
                int mid = (l + r) >> 1;
                int pos = sa[mid];
                int cmpLen = Math.Min(patLen, len - pos);
                if (CompareRange(text, pos, pattern, 0, cmpLen) > 0)
                    r = mid;
                else
                    l = mid + 1;
            }
            return l - start;
        }

        public static void Locate(int* text, int len, int* occ, int* pattern, int patLen, int* sa, int* result, int* count)
        {
            int l = 0, r = len;
            while (l < r)
            {
                int mid = (l + r) >> 1;
                int pos = sa[mid];
                int cmpLen = Math.Min(patLen, len - pos);
                if (CompareRange(text, pos, pattern, 0, cmpLen) >= 0)
                    r = mid;
                else
                    l = mid + 1;
            }
            int start = l;
            while (r < len && (occ[r + 1] - occ[start]) < patLen) r++;
            *count = r - start;
            for (int i = start; i <= r; i++)
                result[i - start] = sa[i];
        }

        private static int CompareRange(int* a, int aOff, int* b, int bOff, int len)
        {
            for (int i = 0; i < len; i++)
                if (a[aOff + i] != b[bOff + i])
                    return a[aOff + i] - b[bOff + i];
            return 0;
        }
    }
}
