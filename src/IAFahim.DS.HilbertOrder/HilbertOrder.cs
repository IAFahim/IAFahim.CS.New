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
        public static long Encode(long x, long y, int w, int h)
        {
            if (w <= 0 || h <= 0) return 0;
            if (w >= h) return EncodeRecursive(x, y, 0, 0, w, 0, 0, h);
            return EncodeRecursive(x, y, 0, 0, 0, h, w, 0);
        }

        private static long EncodeRecursive(long x, long y, long ax, long ay, long bx, long by, long cx, long cy)
        {
            long w = Math.Abs(bx + cx), h = Math.Abs(by + cy);
            if (w <= 1 && h <= 1) return 0;
            // Placeholder for real Gilbert curve logic
            return x * h + y;
        }
    }

    public static unsafe class BlockOrder
    {
        public static long Encode(int l, int r, int blockSize)
        {
            int block = l / blockSize;
            long off = (block & 1) != 0 ? (long)(int.MaxValue - 1 - r) : r;
            return (long)block * int.MaxValue + off;
        }

        public static void Decode(long code, int n, int blockSize, int* l, int* r)
        {
            int block = (int)(code / int.MaxValue);
            long off = code % int.MaxValue;
            *l = block * blockSize;
            *r = (block & 1) != 0 ? (int)(int.MaxValue - 1 - off) : (int)off;
        }
    }
}
