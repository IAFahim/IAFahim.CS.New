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
                mids[b] = 0;
                int ones = 0;
                for (int i = 0; i < n; i++)
                {
                    bitmaps[b * (n + 1) + i] = ((data[i] >> b) & 1);
                    if (bitmaps[b * (n + 1) + i] == 1) ones++;
                }
                bitmaps[b * (n + 1) + n] = 0;
                ranks[b * (n + 1) + 0] = 0;
                for (int i = 1; i <= n; i++)
                    ranks[b * (n + 1) + i] = ranks[b * (n + 1) + i - 1] + bitmaps[b * (n + 1) + i - 1];
                mids[b] = ones;
            }
            return log;
        }

        public static void RunFrom(int* data, int n, int* mids, int* bitmapPtr, int* rankPtr, int log, int* tempZeros, int* tempOnes)
        {
            for (int b = 0; b < log; b++)
            {
                int ones = 0;
                int* bm = bitmapPtr + b * (n + 1);
                bm[n] = 0;
                for (int i = 0; i < n; i++)
                {
                    int bit = (data[i] >> b) & 1;
                    bm[i] = bit;
                    if (bit == 1) ones++;
                }
                mids[b] = ones;
                int* rk = rankPtr + b * (n + 1);
                rk[0] = 0;
                for (int i = 1; i <= n; i++)
                    rk[i] = rk[i - 1] + bm[i - 1];
            }
        }
    }

    public static unsafe class WaveletMatrixKth
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int* tempL, int* tempR, int l, int r, int k, int log)
        {
            int li = l, ri = r;
            int val = 0;
            for (int b = 0; b < log; b++)
            {
                int* bm = bitmapPtr + b * (li + 1);
                int* rk = rankPtr + b * (li + 1);
                int bit = bm[li];
                int leftCount = rk[ri + 1] - rk[li];
                if (k < leftCount)
                {
                    int newL = rk[li];
                    int newR = rk[ri + 1] - 1;
                    li = newL;
                    ri = newR;
                }
                else
                {
                    k -= leftCount;
                    int mid = mids[b];
                    int newL = mid + (li - rk[li]);
                    int newR = mid + (ri - rk[ri + 1] + 1) - 1;
                    li = newL;
                    ri = newR;
                    val |= 1 << b;
                }
            }
            return val;
        }

        public static int RunWithMid(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int k, int log)
        {
            int li = l, ri = r;
            int val = 0;
            for (int b = 0; b < log; b++)
            {
                int* rk = rankPtr + b * (li + 1 + 1);
                int* bm = bitmapPtr + b * (li + 1 + 1);
                int leftInRange = rk[ri + 1] - rk[li];
                if (k < leftInRange)
                {
                    li = rk[li];
                    ri = rk[ri + 1] - 1;
                }
                else
                {
                    k -= leftInRange;
                    int mid = mids[b];
                    li = mid + (li - rk[li]);
                    ri = mid + (ri - rk[ri + 1] + 1) - 1;
                    val |= 1 << b;
                }
            }
            return val;
        }
    }

    public static unsafe class WaveletMatrixQuantile
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int k, int log)
        {
            int li = l, ri = r;
            int val = 0;
            for (int b = 0; b < log; b++)
            {
                int* rk = rankPtr + b;
                int* bm = bitmapPtr + b;
                int leftInRange = rk[ri + 1] - rk[li];
                if (k < leftInRange)
                {
                    li = rk[li];
                    ri = rk[ri + 1] - 1;
                }
                else
                {
                    k -= leftInRange;
                    li = mids[b] + (li - rk[li]);
                    ri = mids[b] + (ri - rk[ri + 1] + 1) - 1;
                    val |= 1 << b;
                }
            }
            return val;
        }
    }

    public static unsafe class WaveletMatrixPrevValue
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int val, int log)
        {
            int li = l, ri = r;
            for (int b = 0; b < log; b++)
            {
                int* rk = rankPtr + b;
                int* bm = bitmapPtr + b;
                int bit = (val >> b) & 1;
                if (bit == 0)
                {
                    int rightInRange = (ri - li + 1) - (rk[ri + 1] - rk[li]);
                    if (rightInRange > 0)
                    {
                        ri = mids[b] + (ri - rk[ri + 1] + 1) - 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    li = rk[li];
                    ri = rk[ri + 1] - 1;
                }
            }
            int lo = l, hi = r;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                int q = WaveletMatrixQuantile.Run(bitmapPtr, rankPtr, mids, l, r, mid - l + 1, log);
                if (q < val) lo = mid + 1;
                else hi = mid;
            }
            if (lo > l) return lo - 1;
            return -1;
        }
    }

    public static unsafe class WaveletMatrixNextValue
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int val, int log)
        {
            int li = l, ri = r;
            for (int b = 0; b < log; b++)
            {
                int* rk = rankPtr + b;
                int* bm = bitmapPtr + b;
                int bit = (val >> b) & 1;
                if (bit == 1)
                {
                    int leftInRange = rk[ri + 1] - rk[li];
                    if (leftInRange > 0)
                    {
                        li = rk[li];
                        ri = rk[ri + 1] - 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    ri = mids[b] + (ri - rk[ri + 1] + 1) - 1;
                }
            }
            int lo = l, hi = r;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                int q = WaveletMatrixQuantile.Run(bitmapPtr, rankPtr, mids, l, r, mid - l + 1, log);
                if (q > val) hi = mid;
                else lo = mid + 1;
            }
            if (lo < r) return lo + 1;
            return -1;
        }
    }

    public static unsafe class WaveletMatrixIntersect
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int log,
            int* al, int* ar, int* bl, int* br, int* outL, int* outR)
        {
            int l1 = *al, r1 = *ar, l2 = *bl, r2 = *br;
            int li = l1, ri = r1;
            for (int b = 0; b < log; b++)
            {
                int* rk = rankPtr + b;
                int leftIn1 = rk[ri + 1] - rk[li];
                int leftIn2 = rk[r2 + 1] - rk[l2];
                if (leftIn1 == 0 || leftIn2 == 0)
                {
                    *outL = 0;
                    *outR = -1;
                    return 0;
                }
                int newL1 = rk[li];
                int newR1 = rk[ri + 1] - 1;
                int newL2 = mids[b] + (l2 - rk[l2]);
                int newR2 = mids[b] + (r2 - rk[r2 + 1] + 1) - 1;
                li = newL1;
                ri = newR1;
                l2 = newL2;
                r2 = newR2;
            }
            *outL = li;
            *outR = ri;
            return ri - li + 1;
        }
    }

    public static unsafe class WaveletMatrixRectangleCount
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int log,
            int l, int r, int x1, int x2, int y1, int y2, int logX, int logY)
        {
            int li = l, ri = r;
            for (int b = 0; b < log; b++)
            {
                int* rk = rankPtr + b;
                int bit = (y1 >> b) & 1;
                if (bit == 0)
                {
                    int leftCount = rk[ri + 1] - rk[li];
                    ri = mids[b] + (ri - rk[ri + 1] + 1) - 1;
                    li = mids[b] + (li - rk[li]);
                    int nextBit = (y2 >> b) & 1;
                    if (nextBit == 1)
                    {
                        int rightCount = (ri - li + 1) - leftCount;
                    }
                }
                else
                {
                    li = rk[li];
                    ri = rk[ri + 1] - 1;
                }
            }
            return ri - li + 1;
        }
    }

    public static unsafe class WaveletMatrixRectangleSum
    {
        public static long Run(int* bitmapPtr, int* rankPtr, int* mids, int* data, int log,
            int l, int r, int x1, int x2, int y1, int y2)
        {
            return 0;
        }
    }
}
