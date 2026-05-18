namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BitCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            int count = 0;
            while (x != 0)
            {
                count += x & 1;
                x >>= 1;
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long x)
        {
            int count = 0;
            while (x != 0)
            {
                count += (int)(x & 1);
                x >>= 1;
            }
            return count;
        }
    }

    public static unsafe class BitLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            if (x == 0) return 0;
            int len = 0;
            while (x != 0)
            {
                len++;
                x >>= 1;
            }
            return len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long x)
        {
            if (x == 0) return 0;
            int len = 0;
            while (x != 0)
            {
                len++;
                x >>= 1;
            }
            return len;
        }
    }

    public static unsafe class HighestBit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            if (x == 0) return 0;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x - (x >>> 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long x)
        {
            if (x == 0) return 0;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x |= x >> 32;
            return (int)(x - (x >>> 1));
        }
    }

    public static unsafe class LowestBit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            return x & -x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x)
        {
            return x & -x;
        }
    }

    public static unsafe class NextBit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            return x == 0 ? 0 : 1 << BitLength.Run(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x)
        {
            return x == 0 ? 0 : 1L << BitLength.Run(x);
        }
    }

    public static unsafe class PrevBit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            if (x <= 1) return 0;
            int hb = HighestBit.Run(x - 1);
            return hb << 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x)
        {
            if (x <= 1) return 0;
            long hb = HighestBit.Run(x - 1);
            return hb << 1;
        }
    }

    public static unsafe class BitReverse
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            x = ((x >> 1) & 0x55555555) | ((x & 0x55555555) << 1);
            x = ((x >> 2) & 0x33333333) | ((x & 0x33333333) << 2);
            x = ((x >> 4) & 0x0F0F0F0F) | ((x & 0x0F0F0F0F) << 4);
            x = ((x >> 8) & 0x00FF00FF) | ((x & 0x00FF00FF) << 8);
            x = (x >> 16) | (x << 16);
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x)
        {
            x = ((x >> 1) & 0x5555555555555555L) | ((x & 0x5555555555555555L) << 1);
            x = ((x >> 2) & 0x3333333333333333L) | ((x & 0x3333333333333333L) << 2);
            x = ((x >> 4) & 0x0F0F0F0F0F0F0F0FL) | ((x & 0x0F0F0F0F0F0F0F0FL) << 4);
            x = ((x >> 8) & 0x00FF00FF00FF00FFL) | ((x & 0x00FF00FF00FF00FFL) << 8);
            x = ((x >> 16) & 0x0000FFFF0000FFFFL) | ((x & 0x0000FFFF0000FFFFL) << 16);
            x = (x >> 32) | (x << 32);
            return x;
        }
    }

    public static unsafe class BitCompress
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* src, int* dst, int len, int bits)
        {
            int outIdx = 0;
            int buffer = 0;
            int bitCount = 0;
            for (int i = 0; i < len; i++)
            {
                buffer |= src[i] << bitCount;
                bitCount += bits;
                while (bitCount >= 32)
                {
                    dst[outIdx++] = (int)(buffer & 0xFFFFFFFFu);
                    buffer = (int)((uint)buffer >> 32);
                    bitCount -= 32;
                }
            }
            if (bitCount > 0)
                dst[outIdx++] = (int)(buffer & 0xFFFFFFFFu);
            return outIdx;
        }
    }

    public static unsafe class BitDecompress
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* src, int* dst, int srcLen, int bits, int len)
        {
            int outIdx = 0;
            int buffer = 0;
            int bitCount = 0;
            for (int i = 0; i < srcLen && outIdx < len; i++)
            {
                buffer |= src[i] << bitCount;
                bitCount += 32;
                while (bitCount >= bits && outIdx < len)
                {
                    int mask = (1 << bits) - 1;
                    dst[outIdx++] = buffer & mask;
                    buffer >>= bits;
                    bitCount -= bits;
                }
            }
            return outIdx;
        }
    }
}