namespace IAFahim.String.SuffixArray
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class SuffixArray
    {
        public static void Build(byte* ptr, int len, int* saPtr)
        {
            int* rank = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            int* tmp = (int*)Marshal.AllocHGlobal(sizeof(int) * len * 2);
            int* k = (int*)Marshal.AllocHGlobal(sizeof(int) * len);

            for (int i = 0; i < len; i++)
            {
                saPtr[i] = i;
                rank[i] = ptr[i];
            }

            for (int h = 1; h < len; h <<= 1)
            {
                for (int i = 0; i < len; i++)
                {
                    int j = saPtr[i] - h;
                    if (j < 0) j += len;
                    k[i] = j;
                }

                for (int i = 0; i < len; i++)
                {
                    if (i >= h) tmp[i] = rank[i - h];
                    else tmp[i] = 0;
                    tmp[len + i] = rank[i];
                }

                Sort(k, saPtr, tmp, len);

                tmp[len + saPtr[0]] = 0;
                for (int i = 1; i < len; i++)
                    tmp[len + saPtr[i]] = tmp[len + saPtr[i - 1]] + (tmp[len + saPtr[i]] != tmp[len + saPtr[i - 1]] ? 1 : 0);

                for (int i = 0; i < len; i++)
                    rank[i] = tmp[len + i];

                if (rank[saPtr[len - 1]] == len - 1) break;
            }

            Marshal.FreeHGlobal((nint)rank);
            Marshal.FreeHGlobal((nint)tmp);
            Marshal.FreeHGlobal((nint)k);
        }

        private static void Sort(int* k, int* sa, int* tmp, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (tmp[k[j]] > tmp[k[j + 1]])
                    {
                        int tk = k[j];
                        k[j] = k[j + 1];
                        k[j + 1] = tk;
                    }
                }
            }
            for (int i = 0; i < n; i++)
                sa[i] = k[i];
        }
    }
}
