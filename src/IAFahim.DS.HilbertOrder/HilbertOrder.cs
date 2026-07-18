namespace IAFahim.DS.HilbertOrder
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HilbertOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x, long y, int pow, int rot)
        {
            x &= (1L << pow) - 1; y &= (1L << pow) - 1;
            int rotSeed = rot & 3;
            long d = 0;
            for (int i = pow; i > 0; i--)
            {
                int rx = (int)((x >> (i - 1)) & 1), ry = (int)((y >> (i - 1)) & 1);
                int seg = ((3 * rx) ^ ry) ^ rotSeed;
                d += (1L << (2 * i - 2)) * seg;
                if (ry == 0) RotateHilbert(ref x, ref y, rx, i);
            }
            return d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Encode(long x, long y, int logN)
        {
            x &= (1L << logN) - 1; y &= (1L << logN) - 1;
            long d = 0;
            for (int i = logN; i > 0; i--)
            {
                int rx = (int)((x >> (i - 1)) & 1), ry = (int)((y >> (i - 1)) & 1);
                d += (1L << (2 * i - 2)) * ((3 * rx) ^ ry);
                if (ry == 0) RotateHilbert(ref x, ref y, rx, i);
            }
            return d;
        }

        public static long SwappedEncode(long x, long y, int logN) => Encode(Math.Min(x, y), Math.Max(x, y), logN);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RotateHilbert(ref long x, ref long y, int rx, int i)
        {
            if (rx == 1) { long mask = (1L << i) - 1; x = mask - x; y = mask - y; }
            long t = x; x = y; y = t;
        }
    }

    public static unsafe class GilbertOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Encode(long x, long y, int w, int h)
        {
            if (w <= 0 || h <= 0) return 0;
            if ((ulong)x >= (ulong)w || (ulong)y >= (ulong)h) return 0;
            int m = w > h ? w : h;
            int logN = 0;
            while ((1 << logN) < m) logN++;
            if (logN == 0) return 0;
            return HilbertOrder.Encode(x, y, logN);
        }
    }

    public static unsafe class BlockOrder
    {
        private const long BlockStride = 1L << 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Encode(int l, int r, int blockSize)
        {
            if (blockSize <= 0) return 0;
            int block = l / blockSize;
            long off = (block & 1) != 0 ? (BlockStride - 1 - (uint)r) : (uint)r;
            return (long)block * BlockStride + off;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decode(long code, int n, int blockSize, int* l, int* r)
        {
            if (blockSize <= 0)
            {
                *l = 0;
                *r = 0;
                return;
            }
            int block = (int)(code / BlockStride);
            long off = code % BlockStride;
            if (off < 0) off += BlockStride;
            *l = block * blockSize;
            *r = (block & 1) != 0 ? (int)(BlockStride - 1 - off) : (int)off;
        }
    }
}
