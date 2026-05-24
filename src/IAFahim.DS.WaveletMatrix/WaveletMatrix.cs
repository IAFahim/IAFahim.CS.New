namespace IAFahim.DS.WaveletMatrix
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class WaveletMatrixBuild
    {
        public static int Run(int* data, int n, int maxVal, int* bitmaps, int* ranks, int* mids, int log)
        {
            for (int b = 0; b < log; b++)
            {
                int ones = ComputeBitmapsAndCountOnes(data, n, b, bitmaps);
                FinalizeLevel(n, b, bitmaps, ranks, mids, ones);
            }
            return log;
        }

        private static int ComputeBitmapsAndCountOnes(int* data, int n, int b, int* bitmaps)
        {
            int ones = 0;
            int offset = b * (n + 1);
            for (int i = 0; i < n; i++)
            {
                int bit = (data[i] >> b) & 1;
                bitmaps[offset + i] = bit;
                if (bit == 1) ones++;
            }
            bitmaps[offset + n] = 0;
            return ones;
        }

        private static void FinalizeLevel(int n, int b, int* bitmaps, int* ranks, int* mids, int ones)
        {
            int offset = b * (n + 1);
            ranks[offset] = 0;
            for (int i = 1; i <= n; i++)
                ranks[offset + i] = ranks[offset + i - 1] + bitmaps[offset + i - 1];
            mids[b] = ones;
        }

        public static void RunFrom(int* data, int n, int* mids, int* bitmapPtr, int* rankPtr, int log)
        {
            for (int b = 0; b < log; b++)
            {
                int ones = ComputeBitmapsAndCountOnes(data, n, b, bitmapPtr);
                FinalizeLevel(n, b, bitmapPtr, rankPtr, mids, ones);
            }
        }
    }

    public static unsafe class WaveletMatrixKth
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int k, int log)
        {
            int li = l, ri = r, val = 0;
            for (int b = 0; b < log; b++)
            {
                int leftCount = GetCountInLevel(rankPtr, li, ri);
                if (k < leftCount)
                {
                    UpdateIndicesZero(ref li, ref ri, rankPtr);
                }
                else
                {
                    k -= leftCount;
                    UpdateIndicesOne(ref li, ref ri, rankPtr, mids[b]);
                    val |= 1 << b;
                }
            }
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetCountInLevel(int* rankPtr, int li, int ri)
        {
            return rankPtr[ri + 1] - rankPtr[li];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateIndicesZero(ref int li, ref int ri, int* rankPtr)
        {
            li = rankPtr[li];
            ri = rankPtr[ri + 1] - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateIndicesOne(ref int li, ref int ri, int* rankPtr, int mid)
        {
            li = mid + (li - rankPtr[li]);
            ri = mid + (ri - rankPtr[ri + 1] + 1) - 1;
        }
    }
}
