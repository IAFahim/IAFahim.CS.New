namespace IAFahim.String.SuffixArray
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Locate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Find(int* sa, int saLen, byte* text, int textLen, byte* pattern, int patLen)
        {
            int l = 0, r = saLen - 1;
            while (l <= r)
            {
                int mid = (l + r) >> 1;
                int suffixLen = textLen - sa[mid];
                int cmp = Compare(text + sa[mid], pattern, Math.Min(patLen, suffixLen));
                if (cmp == 0)
                {
                    if (patLen <= suffixLen) return sa[mid];
                    cmp = -1;
                }
                if (cmp < 0) l = mid + 1;
                else r = mid - 1;
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Compare(byte* a, byte* b, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (a[i] != b[i]) return a[i] - b[i];
            }
            return 0;
        }
    }
}
