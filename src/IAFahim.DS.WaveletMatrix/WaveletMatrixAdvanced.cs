namespace IAFahim.DS.WaveletMatrix
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class WaveletMatrixQuantile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int k, int log)
        {
            if (k < 1) return 0;
            return WaveletMatrixKth.Run(bitmapPtr, rankPtr, mids, l, r, k - 1, log);
        }
    }

    public static unsafe class WaveletMatrixRectangleCount
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int vLo, int vHi, int log)
        {
            if (l > r || vLo >= vHi) return 0;
            int stride = bitmapPtr[0] + 1;
            return Rec(rankPtr, mids, stride, log, 0, l, r + 1, vLo, vHi, 0, 1 << log);
        }

        private static int Rec(int* rankPtr, int* mids, int stride, int log, int level,
                               int li, int ri, int vLo, int vHi, int vLoBound, int vHiBound)
        {
            if (li >= ri) return 0;
            if (vLo <= vLoBound && vHiBound <= vHi) return ri - li;
            if (vHi <= vLoBound || vHiBound <= vLo) return 0;
            if (level == log) return 0;
            int bit = log - 1 - level;
            int* lr = rankPtr + level * stride;
            int r0 = lr[li], r1 = lr[ri];
            int zerosLo = li - r0, zerosHi = ri - r1;
            int mid = mids[level];
            int onesLo = mid + r0, onesHi = mid + r1;
            int half = 1 << bit;
            int cnt = Rec(rankPtr, mids, stride, log, level + 1, zerosLo, zerosHi, vLo, vHi, vLoBound, vLoBound + half);
            cnt += Rec(rankPtr, mids, stride, log, level + 1, onesLo, onesHi, vLo, vHi, vLoBound + half, vHiBound);
            return cnt;
        }
    }

    public static unsafe class WaveletMatrixPrevValue
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int upperBound, int log)
        {
            if (l > r) return int.MinValue;
            int stride = bitmapPtr[0] + 1;
            return Rec(rankPtr, mids, stride, log, 0, l, r + 1, 0, upperBound);
        }

        private static int Rec(int* rankPtr, int* mids, int stride, int log, int level,
                               int li, int ri, int vLo, int upperBound)
        {
            if (li >= ri) return int.MinValue;
            if (level == log) return vLo < upperBound ? vLo : int.MinValue;
            int* lr = rankPtr + level * stride;
            int r0 = lr[li], r1 = lr[ri];
            int mid = mids[level];
            int zLo = li - r0, zHi = ri - r1;
            int oLo = mid + r0, oHi = mid + r1;
            int half = 1 << (log - 1 - level);
            int ones = Rec(rankPtr, mids, stride, log, level + 1, oLo, oHi, vLo + half, upperBound);
            if (ones != int.MinValue) return ones;
            return Rec(rankPtr, mids, stride, log, level + 1, zLo, zHi, vLo, upperBound);
        }
    }

    public static unsafe class WaveletMatrixNextValue
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int lowerBound, int log)
        {
            if (l > r) return int.MaxValue;
            int stride = bitmapPtr[0] + 1;
            return Rec(rankPtr, mids, stride, log, 0, l, r + 1, 0, lowerBound);
        }

        private static int Rec(int* rankPtr, int* mids, int stride, int log, int level,
                               int li, int ri, int vLo, int lowerBound)
        {
            if (li >= ri) return int.MaxValue;
            if (level == log) return vLo > lowerBound ? vLo : int.MaxValue;
            int* lr = rankPtr + level * stride;
            int r0 = lr[li], r1 = lr[ri];
            int mid = mids[level];
            int zLo = li - r0, zHi = ri - r1;
            int oLo = mid + r0, oHi = mid + r1;
            int half = 1 << (log - 1 - level);
            int zeros = Rec(rankPtr, mids, stride, log, level + 1, zLo, zHi, vLo, lowerBound);
            if (zeros != int.MaxValue) return zeros;
            return Rec(rankPtr, mids, stride, log, level + 1, oLo, oHi, vLo + half, lowerBound);
        }
    }

    public static unsafe class WaveletMatrixIntersect
    {
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l1, int r1, int l2, int r2, int log)
        {
            int stride = bitmapPtr[0] + 1;
            return Rec(rankPtr, mids, stride, log, 0, l1, r1 + 1, l2, r2 + 1);
        }

        private static int Rec(int* rankPtr, int* mids, int stride, int log, int level,
                               int l1i, int r1i, int l2i, int r2i)
        {
            if (l1i >= r1i || l2i >= r2i) return 0;
            if (level == log) return 1;
            int* lr = rankPtr + level * stride;
            int r10 = lr[l1i], r11 = lr[r1i];
            int r20 = lr[l2i], r21 = lr[r2i];
            int mid = mids[level];
            int z1Lo = l1i - r10, z1Hi = r1i - r11;
            int z2Lo = l2i - r20, z2Hi = r2i - r21;
            int o1Lo = mid + r10, o1Hi = mid + r11;
            int o2Lo = mid + r20, o2Hi = mid + r21;
            int cnt = Rec(rankPtr, mids, stride, log, level + 1, z1Lo, z1Hi, z2Lo, z2Hi);
            cnt += Rec(rankPtr, mids, stride, log, level + 1, o1Lo, o1Hi, o2Lo, o2Hi);
            return cnt;
        }
    }

    public static unsafe class WaveletMatrixBuildSums
    {
        public static void Run(int* data, int n, int* bitmaps, int* ranks, int* mids, long* valSums, int log)
        {
            int* cur = bitmaps;
            int* next = bitmaps + (n + 1);
            for (int i = 0; i < n; i++) cur[i] = data[i];
            int last = log - 1;
            for (int b = 0; b < log; b++)
            {
                int offset = b * (n + 1);
                long* sumRow = valSums + offset;
                int* rankRow = ranks + offset;
                int bit = last - b;
                int ones = 0;
                for (int i = 0; i < n; i++) ones += (cur[i] >> bit) & 1;
                int zeros = n - ones;
                int acc = 0;
                long accSum = 0;
                int onePos = zeros;
                rankRow[0] = 0;
                sumRow[0] = 0;
                int* scratch = b < last ? next : null;
                for (int i = 0; i < n; i++)
                {
                    int v = cur[i];
                    int isOne = (v >> bit) & 1;
                    if (isOne == 0)
                    {
                        if (scratch != null) scratch[i - acc] = v;
                    }
                    else
                    {
                        if (scratch != null) scratch[onePos] = v;
                        onePos++;
                    }
                    acc += isOne;
                    accSum += v;
                    rankRow[i + 1] = acc;
                    sumRow[i + 1] = accSum;
                }
                mids[b] = zeros;
                if (b < last) { int* tmp = cur; cur = next; next = tmp; }
            }
            bitmaps[0] = n;
        }
    }

    public static unsafe class WaveletMatrixRectangleSum
    {
        public static long Run(int* bitmapPtr, int* rankPtr, int* mids, long* valSums, int l, int r, int vLo, int vHi, int log)
        {
            if (l > r || vLo >= vHi) return 0;
            int stride = bitmapPtr[0] + 1;
            return Rec(rankPtr, mids, valSums, stride, log, 0, l, r + 1, vLo, vHi, 0, 1 << log);
        }

        private static long Rec(int* rankPtr, int* mids, long* valSums, int stride, int log, int level,
                                int li, int ri, int vLo, int vHi, int vLoBound, int vHiBound)
        {
            if (li >= ri) return 0;
            if (vLo <= vLoBound && vHiBound <= vHi)
            {
                long* sumRow = valSums + level * stride;
                return sumRow[ri] - sumRow[li];
            }
            if (vHi <= vLoBound || vHiBound <= vLo) return 0;
            if (level == log) return 0;
            int bit = log - 1 - level;
            int* lr = rankPtr + level * stride;
            int r0 = lr[li], r1 = lr[ri];
            int zerosLo = li - r0, zerosHi = ri - r1;
            int mid = mids[level];
            int onesLo = mid + r0, onesHi = mid + r1;
            int half = 1 << bit;
            long s = Rec(rankPtr, mids, valSums, stride, log, level + 1, zerosLo, zerosHi, vLo, vHi, vLoBound, vLoBound + half);
            s += Rec(rankPtr, mids, valSums, stride, log, level + 1, onesLo, onesHi, vLo, vHi, vLoBound + half, vHiBound);
            return s;
        }
    }

    public static unsafe class SuccinctWaveletBuild
    {
        public static void Run(ulong* bits, int wordCount, long* prefix)
        {
            long acc = 0;
            prefix[0] = 0;
            for (int w = 0; w < wordCount; w++)
            {
                acc += Popcount(bits[w]);
                prefix[w + 1] = acc;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Popcount(ulong x)
        {
            x = x - ((x >> 1) & 0x5555555555555555UL);
            x = (x & 0x3333333333333333UL) + ((x >> 2) & 0x3333333333333333UL);
            x = (x + (x >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
            return (long)((x * 0x0101010101010101UL) >> 56);
        }
    }

    public static unsafe class SuccinctWaveletRank
    {
        public static int Run(ulong* bits, long* prefix, int i)
        {
            if (i <= 0) return 0;
            int word = i >> 6;
            int bit = i & 63;
            long baseCount = prefix[word];
            if (bit == 0) return (int)baseCount;
            ulong mask = (1UL << bit) - 1UL;
            return (int)(baseCount + Popcount(bits[word] & mask));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Popcount(ulong x)
        {
            x = x - ((x >> 1) & 0x5555555555555555UL);
            x = (x & 0x3333333333333333UL) + ((x >> 2) & 0x3333333333333333UL);
            x = (x + (x >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
            return (long)((x * 0x0101010101010101UL) >> 56);
        }
    }

    public static unsafe class SuccinctWaveletSelect
    {
        public static int Run(ulong* bits, long* prefix, int wordCount, int k)
        {
            if (k < 1) return -1;
            int lo = 0, hi = wordCount;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (prefix[mid + 1] >= k) hi = mid;
                else lo = mid + 1;
            }
            int word = lo;
            int need = k - (int)prefix[word];
            ulong w = bits[word];
            int pos = -1;
            for (int b = 0; b < 64; b++)
            {
                if (((w >> b) & 1UL) != 0UL)
                {
                    need--;
                    if (need == 0) { pos = b; break; }
                }
            }
            return (word << 6) + pos;
        }
    }
}
