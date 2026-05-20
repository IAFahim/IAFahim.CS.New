namespace IAFahim.Search.Bit
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BitsetOr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, long* a, long* b, long* res, int wordsPerRow)
        {
            int wordCount = (n + 63) >> 6;
            for (int i = 0; i < wordCount; i++)
                res[i] = a[i] | b[i];
        }
    }

    public static unsafe class BitsetAnd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, long* a, long* b, long* res, int wordsPerRow)
        {
            int wordCount = (n + 63) >> 6;
            for (int i = 0; i < wordCount; i++)
                res[i] = a[i] & b[i];
        }
    }

    public static unsafe class BitsetShift
    {
        public static void Run(int n, long* src, long* dst, int shift, int wordsPerRow)
        {
            int wordShift = shift >> 6;
            int bitShift = shift & 63;
            int wordCount = (n + 63) >> 6;
            for (int i = 0; i < wordCount; i++) dst[i] = 0;
            if (bitShift == 0)
            {
                for (int i = wordCount - 1; i >= wordShift; i--)
                    dst[i] = src[i - wordShift];
            }
            else
            {
                for (int i = wordCount - 1; i >= wordShift; i--)
                {
                    dst[i] = src[i - wordShift] << bitShift;
                    if (i > wordShift)
                        dst[i] |= src[i - wordShift - 1] >> (64 - bitShift);
                }
            }
        }
    }

    public static unsafe class BitsetSet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int pos, long* bitset, int wordsPerRow)
        {
            bitset[pos >> 6] |= 1L << (pos & 63);
        }
    }

    public static unsafe class BitsetGet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int pos, long* bitset, int wordsPerRow)
        {
            return (bitset[pos >> 6] >> (pos & 63)) & 1;
        }
    }

    public static unsafe class BitsetCount
    {
        public static int Run(int n, long* bitset, int wordsPerRow)
        {
            int count = 0;
            int wordCount = (n + 63) >> 6;
            for (int i = 0; i < wordCount; i++)
            {
                long w = bitset[i];
                while (w != 0)
                {
                    w &= w - 1;
                    count++;
                }
            }
            return count;
        }
    }

    public static unsafe class BitsetNextSet
    {
        public static int Run(int from, int n, long* bitset, int wordsPerRow)
        {
            int word = from >> 6;
            int bit = from & 63;
            long w = bitset[word] >> bit;
            if (w != 0) return from + (int)BitIndex(w);
            word++;
            int wordCount = (n + 63) >> 6;
            while (word < wordCount)
            {
                if (bitset[word] != 0)
                    return (word << 6) + (int)BitIndex(bitset[word]);
                word++;
            }
            return -1;
        }

        private static int BitIndex(long x)
        {
            int n = 0;
            if (x <= 0xFFFFFFFF) { n += 32; x >>= 32; }
            if (x <= 0xFFFF) { n += 16; x >>= 16; }
            if (x <= 0xFF) { n += 8; x >>= 8; }
            if (x <= 0xF) { n += 4; x >>= 4; }
            if (x <= 0x3) { n += 2; x >>= 2; }
            if (x <= 0x1) { n += 1; }
            return n;
        }
    }

    public static unsafe class BitsetPrevSet
    {
        public static int Run(int from, int n, long* bitset, int wordsPerRow)
        {
            int word = from >> 6;
            int bit = from & 63;
            if (bit > 0)
            {
                long w = bitset[word] & ((1L << bit) - 1);
                if (w != 0) return (word << 6) + (int)BitIndexRev(w);
            }
            word--;
            while (word >= 0)
            {
                if (bitset[word] != 0)
                    return (word << 6) + (int)BitIndexRev(bitset[word]);
                word--;
            }
            return -1;
        }

        private static int BitIndexRev(long x)
        {
            int n = 63;
            if ((x & 0xFFFFFFFF00000000L) == 0) { n -= 32; x <<= 32; }
            if ((x & 0xFFFF0000FFFF0000L) == 0) { n -= 16; x <<= 16; }
            if ((x & 0xFF00FF00FF00FF00L) == 0) { n -= 8; x <<= 8; }
            if ((x & 0xF0F0F0F0F0F0F0F0L) == 0) { n -= 4; x <<= 4; }
            if ((x & 0xCCCCCCCCCCCCCCCCL) == 0) { n -= 2; x <<= 2; }
            if ((x & 0xAAAAAAAAAAAAAAAAL) == 0) { n -= 1; }
            return n;
        }
    }
}