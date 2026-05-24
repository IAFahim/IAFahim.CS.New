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
            int wordShift = shift >> 6, bitShift = shift & 63, wordCount = (n + 63) >> 6;
            ClearBitset(dst, wordCount);
            if (bitShift == 0) ShiftAligned(src, dst, wordShift, wordCount);
            else ShiftUnaligned(src, dst, wordShift, bitShift, wordCount);
        }

        private static void ClearBitset(long* dst, int count)
        {
            for (int i = 0; i < count; i++) dst[i] = 0;
        }

        private static void ShiftAligned(long* src, long* dst, int wordShift, int wordCount)
        {
            for (int i = wordCount - 1; i >= wordShift; i--) dst[i] = src[i - wordShift];
        }

        private static void ShiftUnaligned(long* src, long* dst, int wordShift, int bitShift, int wordCount)
        {
            int invShift = 64 - bitShift;
            for (int i = wordCount - 1; i >= wordShift; i--)
            {
                dst[i] = src[i - wordShift] << bitShift;
                if (i > wordShift) dst[i] |= src[i - wordShift - 1] >> invShift;
            }
        }
    }

    public static unsafe class BitsetSet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int pos, long* bitset, int wordsPerRow) { bitset[pos >> 6] |= 1L << (pos & 63); }
    }

    public static unsafe class BitsetGet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int pos, long* bitset, int wordsPerRow) { return (int)((bitset[pos >> 6] >> (pos & 63)) & 1); }
    }

    public static unsafe class BitsetCount
    {
        public static int Run(int n, long* bitset, int wordsPerRow)
        {
            int count = 0, wordCount = (n + 63) >> 6;
            for (int i = 0; i < wordCount; i++) count += PopCountWord(bitset[i]);
            return count;
        }

        private static int PopCountWord(long w)
        {
            int c = 0; while (w != 0) { w &= w - 1; c++; } return c;
        }
    }

    public static unsafe class BitsetNextSet
    {
        public static int Run(int from, int n, long* bitset, int wordsPerRow)
        {
            int wordCount = (n + 63) >> 6, word = from >> 6, bit = from & 63;
            long w = bitset[word] >> bit;
            if (w != 0) return from + BitIndex(w);
            for (int i = word + 1; i < wordCount; i++)
                if (bitset[i] != 0) return (i << 6) + BitIndex(bitset[i]);
            return -1;
        }

        private static int BitIndex(long x)
        {
            int n = 0;
            if ((x & 0xFFFFFFFF) == 0) { n += 32; x >>= 32; }
            if ((x & 0xFFFF) == 0) { n += 16; x >>= 16; }
            if ((x & 0xFF) == 0) { n += 8; x >>= 8; }
            if ((x & 0xF) == 0) { n += 4; x >>= 4; }
            if ((x & 0x3) == 0) { n += 2; x >>= 2; }
            if ((x & 0x1) == 0) n += 1;
            return n;
        }
    }

    public static unsafe class BitsetPrevSet
    {
        public static int Run(int from, int n, long* bitset, int wordsPerRow)
        {
            int word = from >> 6, bit = from & 63;
            if (bit > 0)
            {
                long w = bitset[word] & ((1L << bit) - 1);
                if (w != 0) return (word << 6) + BitIndexRev(w);
            }
            for (int i = word - 1; i >= 0; i--)
                if (bitset[i] != 0) return (i << 6) + BitIndexRev(bitset[i]);
            return -1;
        }

        private static int BitIndexRev(long x)
        {
            int n = 63;
            if ((x & unchecked((long)0xFFFFFFFF00000000UL)) == 0) { n -= 32; x <<= 32; }
            if ((x & unchecked((long)0xFFFF000000000000UL)) == 0) { n -= 16; x <<= 16; }
            if ((x & unchecked((long)0xFF00000000000000UL)) == 0) { n -= 8; x <<= 8; }
            if ((x & unchecked((long)0xF000000000000000UL)) == 0) { n -= 4; x <<= 4; }
            if ((x & unchecked((long)0xC000000000000000UL)) == 0) { n -= 2; x <<= 2; }
            if ((x & unchecked((long)0x8000000000000000UL)) == 0) n -= 1;
            return n;
        }
    }
}