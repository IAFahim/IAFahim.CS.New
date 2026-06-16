namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BitCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            uint v = (uint)x;
            v = v - ((v >> 1) & 0x55555555u);
            v = (v & 0x33333333u) + ((v >> 2) & 0x33333333u);
            v = (v + (v >> 4)) & 0x0F0F0F0Fu;
            return (int)((v * 0x01010101u) >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long x)
        {
            ulong v = (ulong)x;
            v = v - ((v >> 1) & 0x5555555555555555UL);
            v = (v & 0x3333333333333333UL) + ((v >> 2) & 0x3333333333333333UL);
            v = (v + (v >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
            return (int)((v * 0x0101010101010101UL) >> 56);
        }
    }

    public static unsafe class BitLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int x)
        {
            uint v = (uint)x;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v = v - ((v >> 1) & 0x55555555u);
            v = (v & 0x33333333u) + ((v >> 2) & 0x33333333u);
            v = (v + (v >> 4)) & 0x0F0F0F0Fu;
            return (int)((v * 0x01010101u) >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long x)
        {
            ulong v = (ulong)x;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v |= v >> 32;
            v = v - ((v >> 1) & 0x5555555555555555UL);
            v = (v & 0x3333333333333333UL) + ((v >> 2) & 0x3333333333333333UL);
            v = (v + (v >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
            return (int)((v * 0x0101010101010101UL) >> 56);
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
            ulong buffer = 0;
            int bitCount = 0;
            for (int i = 0; i < len; i++)
            {
                buffer |= (ulong)(uint)src[i] << bitCount;
                bitCount += bits;
                while (bitCount >= 32)
                {
                    dst[outIdx++] = (int)(uint)buffer;
                    buffer >>= 32;
                    bitCount -= 32;
                }
            }
            if (bitCount > 0)
                dst[outIdx++] = (int)(uint)buffer;
            return outIdx;
        }
    }

    public static unsafe class BitDecompress
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* src, int* dst, int srcLen, int bits, int len)
        {
            int outIdx = 0;
            ulong buffer = 0;
            int bitCount = 0;
            ulong mask = (1UL << bits) - 1UL;
            for (int i = 0; i < srcLen && outIdx < len; i++)
            {
                buffer |= (ulong)(uint)src[i] << bitCount;
                bitCount += 32;
                while (bitCount >= bits && outIdx < len)
                {
                    dst[outIdx++] = (int)(buffer & mask);
                    buffer >>= bits;
                    bitCount -= bits;
                }
            }
            return outIdx;
        }
    }
}