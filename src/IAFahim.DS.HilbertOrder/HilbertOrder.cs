namespace IAFahim.DS.HilbertOrder
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HilbertOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long x, long y, int pow, int rot)
        {
            if (pow == 0) return 0;
            int hpow = 1 << (pow - 1);
            int seg = GetHilbertSegment(x, y, hpow, rot);
            if (seg == 0) return Run(x, y, pow - 1, rot);
            if (seg == 1) return Run(y, x, pow - 1, (rot + 1) & 3);
            if (seg == 2) return Run(x, y, pow - 1, (rot + 1) & 3);
            long subSize = 1L << (2 * pow - 2);
            return subSize + Run(x >= hpow ? x - hpow : x, y >= hpow ? y - hpow : y, pow - 1, (rot + 2) & 3);
        }

        private static int GetHilbertSegment(long x, long y, int hpow, int rot)
        {
            int seg = (int)((((x >= hpow) ? 1 : 0) << 1) | ((y >= hpow) ? 1 : 0));
            return (seg ^ (rot & 2)) ^ ((rot & 1) * ((x >= hpow) == (y >= hpow) ? 0 : 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Encode(long x, long y, int logN)
        {
            x &= (1L << logN) - 1; y &= (1L << logN) - 1;
            long d = 0;
            for (int i = logN; i > 0; i--)
            {
                int rx = (int)((x >> (i - 1)) & 1), ry = (int)((y >> (i - 1)) & 1);
                d += 1L << (2 * i - 2); if (ry == 0) RotateHilbert(ref x, ref y, rx, i); d += (rx << 1) + ry;
            }
            return d;
        }

        public static long SwappedEncode(long x, long y, int logN) => Encode(Math.Min(x, y), Math.Max(x, y), logN);

        private static void RotateHilbert(ref long x, ref long y, int rx, int i)
        {
            if (rx == 1) { x = (1L << i) - 1 - x; y = (1L << i) - 1 - y; }
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
        public static long Encode(int l, int r, int blockSize) => (long)(l / blockSize) * int.MaxValue + (((l / blockSize) & 1) != 0 ? -r : r);
        public static void Decode(long code, int n, int blockSize, int* l, int* r) { *l = (int)(code / int.MaxValue) * blockSize; *r = (int)(Math.Abs(code % int.MaxValue)); }
    }
}
