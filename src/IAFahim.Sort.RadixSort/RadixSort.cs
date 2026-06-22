namespace IAFahim.Sort.RadixSort
{
    using System.Runtime.CompilerServices;

    public static unsafe class RadixSortLsd
    {
        private const int Radix = 256;

        private const int ByteMask = 0xFF;

        private const int SignBias = 0x80;

        private const int BitsPerByte = 8;

        private const int IntPasses = 4;

        private const int LongPasses = 8;

        private const int IntSignByte = 3;

        private const int LongSignByte = 7;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildIntHistogram(int* ptr, int len, int* cnt)
        {
            for (int j = 0; j < Radix * IntPasses; j++) cnt[j] = 0;
            for (int i = 0; i < len; i++)
            {
                int v = ptr[i];
                cnt[v & ByteMask]++;
                cnt[Radix + ((v >> BitsPerByte) & ByteMask)]++;
                cnt[Radix * 2 + ((v >> (BitsPerByte * 2)) & ByteMask)]++;
                cnt[Radix * 3 + (((v >> (BitsPerByte * 3)) & ByteMask) ^ SignBias)]++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildLongHistogram(long* ptr, int len, int* cnt)
        {
            for (int j = 0; j < Radix * LongPasses; j++) cnt[j] = 0;
            for (int i = 0; i < len; i++)
            {
                long v = ptr[i];
                cnt[(int)(v & ByteMask)]++;
                cnt[Radix + (int)((v >> BitsPerByte) & ByteMask)]++;
                cnt[Radix * 2 + (int)((v >> (BitsPerByte * 2)) & ByteMask)]++;
                cnt[Radix * 3 + (int)((v >> (BitsPerByte * 3)) & ByteMask)]++;
                cnt[Radix * 4 + (int)((v >> (BitsPerByte * 4)) & ByteMask)]++;
                cnt[Radix * 5 + (int)((v >> (BitsPerByte * 5)) & ByteMask)]++;
                cnt[Radix * 6 + (int)((v >> (BitsPerByte * 6)) & ByteMask)]++;
                cnt[Radix * 7 + ((int)((v >> (BitsPerByte * 7)) & ByteMask) ^ SignBias)]++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PrefixSumBlocks(int* cnt, int passCount)
        {
            for (int b = 0; b < passCount; b++)
            {
                int* block = cnt + b * Radix;
                int sum = 0;
                for (int j = 0; j < Radix; j++) { int c = block[j]; block[j] = sum; sum += c; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ScatterIntPass(int* src, int* dst, int len, int* block, int shift, bool isSignByte)
        {
            if (isSignByte)
            {
                for (int i = 0; i < len; i++) { int v = src[i]; dst[block[((v >> shift) & ByteMask) ^ SignBias]++] = v; }
            }
            else
            {
                for (int i = 0; i < len; i++) { int v = src[i]; dst[block[(v >> shift) & ByteMask]++] = v; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ScatterLongPass(long* src, long* dst, int len, int* block, int shift, bool isSignByte)
        {
            if (isSignByte)
            {
                for (int i = 0; i < len; i++) { long v = src[i]; dst[block[((int)((v >> shift) & ByteMask)) ^ SignBias]++] = v; }
            }
            else
            {
                for (int i = 0; i < len; i++) { long v = src[i]; dst[block[(int)((v >> shift) & ByteMask)]++] = v; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyInt(int* src, int* dst, int len)
        {
            for (int i = 0; i < len; i++) dst[i] = src[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyLong(long* src, long* dst, int len)
        {
            for (int i = 0; i < len; i++) dst[i] = src[i];
        }

        public static void Run(int* ptr, int len)
        {
            if (len <= 1) return;
            int* temp = stackalloc int[len];
            int* cnt = stackalloc int[Radix * IntPasses];
            BuildIntHistogram(ptr, len, cnt);
            PrefixSumBlocks(cnt, IntPasses);
            int* src = ptr;
            int* dst = temp;
            for (int b = 0; b < IntPasses; b++)
            {
                ScatterIntPass(src, dst, len, cnt + b * Radix, b * BitsPerByte, b == IntSignByte);
                int* t = src; src = dst; dst = t;
            }
            if (src != ptr) CopyInt(src, ptr, len);
        }

        public static void RunWithResult(int* ptr, int len, int* result)
        {
            if (len <= 1) { CopyInt(ptr, result, len); return; }
            int* temp = stackalloc int[len];
            int* cnt = stackalloc int[Radix * IntPasses];
            BuildIntHistogram(ptr, len, cnt);
            PrefixSumBlocks(cnt, IntPasses);
            int* src = ptr;
            int* dst = temp;
            for (int b = 0; b < IntPasses; b++)
            {
                ScatterIntPass(src, dst, len, cnt + b * Radix, b * BitsPerByte, b == IntSignByte);
                int* t = src;
                src = dst;
                dst = (t == ptr) ? result : t;
            }
            if (src != result) CopyInt(src, result, len);
        }

        public static void RunLong(long* ptr, int len)
        {
            if (len <= 1) return;
            long* temp = stackalloc long[len];
            int* cnt = stackalloc int[Radix * LongPasses];
            BuildLongHistogram(ptr, len, cnt);
            PrefixSumBlocks(cnt, LongPasses);
            long* src = ptr;
            long* dst = temp;
            for (int b = 0; b < LongPasses; b++)
            {
                ScatterLongPass(src, dst, len, cnt + b * Radix, b * BitsPerByte, b == LongSignByte);
                long* t = src; src = dst; dst = t;
            }
            if (src != ptr) CopyLong(src, ptr, len);
        }
    }
}
