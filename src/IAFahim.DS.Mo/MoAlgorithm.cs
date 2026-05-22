namespace IAFahim.DS.Mo
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MoAdd
    {
        public static void Run(int* currentL, int* currentR, int* freq, int val)
        {
            freq[val]++;
        }
    }

    public static unsafe class MoRemove
    {
        public static void Run(int* currentL, int* currentR, int* freq, int val)
        {
            freq[val]--;
        }
    }

    public static unsafe class MoAnswer
    {
        public static int Run(int* freq, int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (freq[i] > 0) return i;
            }
            return -1;
        }

        public static int DistinctCount(int* freq, int n)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
                if (freq[i] > 0) count++;
            return count;
        }
    }

    public static unsafe class MoSort
    {
        public static void Run(int* queries, int* l, int* r, int* block, int q, int blockSize)
        {
            for (int i = 1; i < q; i++)
            {
                int keyL = l[i];
                int keyR = r[i];
                int keyBlock = block[i];
                int j = i - 1;
                while (j >= 0 && (block[j] > keyBlock || (block[j] == keyBlock && (r[j] > keyR || (r[j] == keyR && l[j] > keyL)))))
                {
                    l[j + 1] = l[j];
                    r[j + 1] = r[j];
                    block[j + 1] = block[j];
                    queries[j + 1] = queries[j];
                    j--;
                }
                l[j + 1] = keyL;
                r[j + 1] = keyR;
                block[j + 1] = keyBlock;
                queries[j + 1] = queries[j];
            }
        }
    }

    public static unsafe class MoRollback
    {
        public static void Run(int* freq, int n)
        {
            for (int i = 0; i < n; i++)
                freq[i] = 0;
        }
    }

    public static unsafe class MoDistinctCounter
    {
        public static void AddInt(int* freq, int* curDistinct, int val)
        {
            if (freq[val] == 0) (*curDistinct)++;
            freq[val]++;
        }

        public static void RemoveInt(int* freq, int* curDistinct, int val)
        {
            freq[val]--;
            if (freq[val] == 0) (*curDistinct)--;
        }
    }

    public static unsafe class WaveletTreeRangeSum
    {
        public static int Run(int* left, int* right, int node, int l, int r, int ql, int qr, int a, int b, int* data)
        {
            if (ql > r || qr < l) return 0;
            if (l >= ql && r <= qr)
            {
                int count = 0;
                for (int i = l; i <= r; i++)
                    if (a <= data[i] && data[i] <= b) count++;
                return count;
            }
            int mid = (l + r) >> 1;
            return Run(left, right, node * 2, l, mid, ql, qr, a, b, data) +
                   Run(left, right, node * 2 + 1, mid + 1, r, ql, qr, a, b, data);
        }
    }
}