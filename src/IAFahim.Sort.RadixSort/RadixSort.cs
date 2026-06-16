namespace IAFahim.Sort.RadixSort
{
    using System.Runtime.CompilerServices;

    public static unsafe class RadixSortLsd
    {
        private const int Radix = 256;
        private const int ByteMask = 0xFF;
        private const int SignBias = 0x80;

        public static void Run(int* ptr, int len)
        {
            if (len <= 1) return;
            int* temp = stackalloc int[len];

            // Combined histogram: one count pass builds all 4 byte histograms.
            int* cnt = stackalloc int[Radix * 4];
            for (int j = 0; j < Radix * 4; j++) cnt[j] = 0;
            for (int i = 0; i < len; i++)
            {
                int v = ptr[i];
                cnt[(v & ByteMask)]++;
                cnt[Radix + ((v >> 8) & ByteMask)]++;
                cnt[(Radix * 2) + ((v >> 16) & ByteMask)]++;
                cnt[(Radix * 3) + (((v >> 24) & ByteMask) ^ SignBias)]++;
            }
            for (int b = 0; b < 4; b++)
            {
                int* block = cnt + (b * Radix);
                int sum = 0;
                for (int j = 0; j < Radix; j++) { int c = block[j]; block[j] = sum; sum += c; }
            }

            int* src = ptr;
            int* dst = temp;
            for (int b = 0; b < 4; b++)
            {
                int shift = b * 8;
                int* block = cnt + (b * Radix);
                if (b == 3)
                {
                    for (int i = 0; i < len; i++)
                    {
                        int v = src[i];
                        dst[block[((v >> shift) & ByteMask) ^ SignBias]++] = v;
                    }
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        int v = src[i];
                        dst[block[(v >> shift) & ByteMask]++] = v;
                    }
                }
                int* t = src; src = dst; dst = t;
            }
            if (src != ptr) { for (int i = 0; i < len; i++) ptr[i] = src[i]; }
        }

        public static void RunWithResult(int* ptr, int len, int* result)
        {
            if (len <= 1) { for (int i = 0; i < len; i++) result[i] = ptr[i]; return; }
            int* temp = stackalloc int[len];

            int* cnt = stackalloc int[Radix * 4];
            for (int j = 0; j < Radix * 4; j++) cnt[j] = 0;
            for (int i = 0; i < len; i++)
            {
                int v = ptr[i];
                cnt[(v & ByteMask)]++;
                cnt[Radix + ((v >> 8) & ByteMask)]++;
                cnt[(Radix * 2) + ((v >> 16) & ByteMask)]++;
                cnt[(Radix * 3) + (((v >> 24) & ByteMask) ^ SignBias)]++;
            }
            for (int b = 0; b < 4; b++)
            {
                int* block = cnt + (b * Radix);
                int sum = 0;
                for (int j = 0; j < Radix; j++) { int c = block[j]; block[j] = sum; sum += c; }
            }

            // First pass reads ptr (read-only); ping-pong between temp and result thereafter.
            int* src = ptr;
            int* dst = temp;
            for (int b = 0; b < 4; b++)
            {
                int shift = b * 8;
                int* block = cnt + (b * Radix);
                if (b == 3)
                {
                    for (int i = 0; i < len; i++)
                    {
                        int v = src[i];
                        dst[block[((v >> shift) & ByteMask) ^ SignBias]++] = v;
                    }
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        int v = src[i];
                        dst[block[(v >> shift) & ByteMask]++] = v;
                    }
                }
                int* t = src;
                src = dst;
                // After the first pass, never write back into ptr; alternate temp/result.
                dst = (t == ptr) ? result : t;
            }
            if (src != result) { for (int i = 0; i < len; i++) result[i] = src[i]; }
        }

        public static void RunLong(long* ptr, int len)
        {
            if (len <= 1) return;
            long* temp = stackalloc long[len];

            int* cnt = stackalloc int[Radix * 8];
            for (int j = 0; j < Radix * 8; j++) cnt[j] = 0;
            for (int i = 0; i < len; i++)
            {
                long v = ptr[i];
                cnt[(int)(v & ByteMask)]++;
                cnt[Radix + (int)((v >> 8) & ByteMask)]++;
                cnt[(Radix * 2) + (int)((v >> 16) & ByteMask)]++;
                cnt[(Radix * 3) + (int)((v >> 24) & ByteMask)]++;
                cnt[(Radix * 4) + (int)((v >> 32) & ByteMask)]++;
                cnt[(Radix * 5) + (int)((v >> 40) & ByteMask)]++;
                cnt[(Radix * 6) + (int)((v >> 48) & ByteMask)]++;
                cnt[(Radix * 7) + ((int)((v >> 56) & ByteMask) ^ SignBias)]++;
            }
            for (int b = 0; b < 8; b++)
            {
                int* block = cnt + (b * Radix);
                int sum = 0;
                for (int j = 0; j < Radix; j++) { int c = block[j]; block[j] = sum; sum += c; }
            }

            long* src = ptr;
            long* dst = temp;
            for (int b = 0; b < 8; b++)
            {
                int shift = b * 8;
                int* block = cnt + (b * Radix);
                if (b == 7)
                {
                    for (int i = 0; i < len; i++)
                    {
                        long v = src[i];
                        dst[block[((int)((v >> shift) & ByteMask)) ^ SignBias]++] = v;
                    }
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        long v = src[i];
                        dst[block[(int)((v >> shift) & ByteMask)]++] = v;
                    }
                }
                long* t = src; src = dst; dst = t;
            }
            if (src != ptr) { for (int i = 0; i < len; i++) ptr[i] = src[i]; }
        }
    }
}
