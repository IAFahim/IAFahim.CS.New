namespace IAFahim.String.FMIndex
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FmBackwardSearch
    {
        public static void Build(int* text, int len, int sigma, int* sa, int* bwt, int* occ, int* c)
        {
            int last = len - 1;
            for (int i = 0; i < len; i++)
                bwt[i] = sa[i] == 0 ? text[last] : text[sa[i] - 1];

            for (int ch = 0; ch < sigma; ch++) occ[ch * (len + 1)] = 0;
            for (int i = 0; i < len; i++)
            {
                for (int ch = 0; ch < sigma; ch++) occ[ch * (len + 1) + i + 1] = occ[ch * (len + 1) + i];
                int sym = bwt[i];
                if (sym >= 0 && sym < sigma) occ[sym * (len + 1) + i + 1]++;
            }

            for (int k = 0; k <= sigma; k++) c[k] = 0;
            for (int i = 0; i < len; i++)
            {
                int sym = text[i];
                if (sym >= 0 && sym < sigma) c[sym + 1]++;
            }
            for (int k = 1; k <= sigma; k++) c[k] += c[k - 1];
        }

        public static int Count(int* occ, int* c, int len, int sigma, int* pattern, int patLen)
        {
            int lo = 0;
            int hi = len;
            for (int i = patLen - 1; i >= 0; i--)
            {
                int ch = pattern[i];
                if (ch < 0 || ch >= sigma) return 0;
                int rankLo = occ[ch * (len + 1) + lo];
                int rankHi = occ[ch * (len + 1) + hi];
                lo = c[ch] + rankLo;
                hi = c[ch] + rankHi;
                if (lo >= hi) return 0;
            }
            return hi - lo;
        }
    }
}
