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
            int seg = (int)((((x >= hpow) ? 1 : 0) << 1) | ((y >= hpow) ? 1 : 0));
            seg = (seg ^ (rot & 2)) ^ ((rot & 1) * ((x >= hpow) == (y >= hpow) ? 0 : 1));
            if (seg == 0)
            {
                return Run(x, y, pow - 1, rot);
            }
            if (seg == 1)
            {
                return Run(y, x, pow - 1, (rot + 1) & 3);
            }
            if (seg == 2)
            {
                return Run(x, y, pow - 1, (rot + 1) & 3);
            }
            long subSize = 1L << (2 * pow - 2);
            long dx = x >= hpow ? x - hpow : x;
            long dy = y >= hpow ? y - hpow : y;
            return subSize + Run(dx, dy, pow - 1, (rot + 2) & 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Encode(long x, long y, int logN)
        {
            x &= (1L << logN) - 1;
            y &= (1L << logN) - 1;
            int i = logN;
            int rx, ry;
            long d = 0;
            for (; i > 0; i--)
            {
                rx = (int)((x >> (i - 1)) & 1);
                ry = (int)((y >> (i - 1)) & 1);
                d += 1L << (2 * i - 2);
                if (ry == 0)
                {
                    if (rx == 1) { x = (1 << i) - 1 - x; y = (1 << i) - 1 - y; }
                    long t = x; x = y; y = t;
                }
                d += (rx << 1) + ry;
            }
            return d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SwappedEncode(long x, long y, int logN)
        {
            if (x > y) { long t = x; x = y; y = t; }
            return Encode(x, y, logN);
        }
    }

    public static unsafe class GilbertOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Encode(long x, long y, long width, long height)
        {
            if (height == 0) return 0;
            if (width == 1)
            {
                return y;
            }
            if (height == 1)
            {
                return width + x;
            }
            if (width >= height)
            {
                long half = width >> 1;
                if (x < half)
                {
                    long subD = Encode(x, y, half, height);
                    return subD;
                }
                else
                {
                    long subD = Encode(x - half, y, width - half, height);
                    return (height << 1) * half + subD;
                }
            }
            long hHalf = height >> 1;
            if (y < hHalf)
            {
                if (x >= height)
                {
                    long subD = Encode(x - height, y, width - height, hHalf);
                    return (height << 2) * (width - height) / 2 + (height * 3) + subD;
                }
                long subD2 = Encode(x, y, height, hHalf);
                return (height << 2) * width / 2 + subD2;
            }
            else
            {
                long subD = Encode(x, y - hHalf, width, height - hHalf);
                return (height * hHalf) + subD;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decode(long d, long width, long height, long* outX, long* outY)
        {
            *outX = 0;
            *outY = 0;
            long w = width;
            long h = height;
            while (w > 0 && h > 0)
            {
                if (h == 1)
                {
                    *outX += d - h;
                    return;
                }
                if (w >= h)
                {
                    long half = w >> 1;
                    if (d < (h << 1) * half)
                    {
                    }
                    else
                    {
                        d -= (h << 1) * half;
                        *outX += half;
                    }
                    w = half;
                }
                else
                {
                    long half = h >> 1;
                    if (d >= h * half)
                    {
                        d -= h * half;
                        *outY += half;
                    }
                    h = half;
                }
            }
        }
    }

    public static unsafe class BlockOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Encode(int l, int r, int blockSize)
        {
            int block = l / blockSize;
            int pos = block * (block + 1) / 2;
            return (long)pos + l % blockSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Decode(long code, int n, int blockSize, int* outL, int* outR)
        {
            int block = 0;
            long sum = 0;
            while (sum + block + 1 <= code)
            {
                sum += block + 1;
                block++;
            }
            *outL = block * blockSize + (int)(code - sum);
            int offset = (*outL) % blockSize;
            *outR = *outL + blockSize - offset - 1;
            if (*outR >= n) *outR = n - 1;
        }
    }
}