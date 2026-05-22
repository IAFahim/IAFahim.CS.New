namespace IAFahim.String.SuffixArray
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class SuffixArray
    {
        public static void Build(byte* ptr, int len, int* sa)
        {
            if (len == 0) return;
            if (len == 1) { sa[0] = 0; return; }

            int maxSigma = 256;
            if (len > 256) maxSigma = len;

            int* rank = (int*)Marshal.AllocHGlobal(sizeof(int) * len * 2);
            int* tmpSa = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            int* count = (int*)Marshal.AllocHGlobal(sizeof(int) * maxSigma);
            int* tmpRank = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            
            try
            {
                for (int i = 0; i < len * 2; i++) rank[i] = -1;

                for (int i = 0; i < maxSigma; i++) count[i] = 0;
                for (int i = 0; i < len; i++)
                {
                    rank[i] = ptr[i];
                    count[rank[i]]++;
                }
                for (int i = 1; i < maxSigma; i++) count[i] += count[i - 1];
                for (int i = len - 1; i >= 0; i--) sa[--count[rank[i]]] = i;

                for (int h = 1; h < len; h <<= 1)
                {
                    int p = 0;
                    for (int i = len - h; i < len; i++) tmpSa[p++] = i;
                    for (int i = 0; i < len; i++) if (sa[i] >= h) tmpSa[p++] = sa[i] - h;

                    for (int i = 0; i < maxSigma; i++) count[i] = 0;
                    for (int i = 0; i < len; i++) count[rank[i]]++;
                    for (int i = 1; i < maxSigma; i++) count[i] += count[i - 1];
                    for (int i = len - 1; i >= 0; i--) sa[--count[rank[tmpSa[i]]]] = tmpSa[i];

                    tmpRank[sa[0]] = 0;
                    p = 0;
                    for (int i = 1; i < len; i++)
                    {
                        int curr1 = rank[sa[i]], curr2 = rank[sa[i] + h];
                        int prev1 = rank[sa[i - 1]], prev2 = rank[sa[i - 1] + h];
                        if (curr1 == prev1 && curr2 == prev2)
                            tmpRank[sa[i]] = p;
                        else
                            tmpRank[sa[i]] = ++p;
                    }

                    for (int i = 0; i < len; i++) rank[i] = tmpRank[i];

                    if (p == len - 1) break;
                    maxSigma = p + 1;
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)rank);
                Marshal.FreeHGlobal((nint)tmpSa);
                Marshal.FreeHGlobal((nint)count);
                Marshal.FreeHGlobal((nint)tmpRank);
            }
        }
    }
}
