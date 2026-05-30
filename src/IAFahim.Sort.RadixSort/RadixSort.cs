namespace IAFahim.Sort.RadixSort
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RadixSortLsd
    {
        public static void Run(int* ptr, int len)
        {
            if (len <= 1) return;
            int* temp = stackalloc int[len];
            int* src = ptr;
            int* dst = temp;
            for (int shift = 0; shift < 32; shift += 8)
            {
                int* cnt = stackalloc int[256];
                for (int j = 0; j < 256; j++) cnt[j] = 0;
                for (int i = 0; i < len; i++) cnt[(src[i] >> shift) & 0xFF]++;
                int sum = 0;
                for (int j = 0; j < 256; j++) { int c = cnt[j]; cnt[j] = sum; sum += c; }
                for (int i = 0; i < len; i++) dst[cnt[(src[i] >> shift) & 0xFF]++] = src[i];
                int* t = src; src = dst; dst = t;
            }
            if (src != ptr) { for (int i = 0; i < len; i++) ptr[i] = src[i]; }
        }

        public static void RunWithResult(int* ptr, int len, int* result)
        {
            if (len <= 1) { for (int i = 0; i < len; i++) result[i] = ptr[i]; return; }
            int* temp = stackalloc int[len];
            int* src = ptr;
            int* dst = result;
            for (int shift = 0; shift < 32; shift += 8)
            {
                int* cnt = stackalloc int[256];
                for (int j = 0; j < 256; j++) cnt[j] = 0;
                for (int i = 0; i < len; i++) cnt[(src[i] >> shift) & 0xFF]++;
                int sum = 0;
                for (int j = 0; j < 256; j++) { int c = cnt[j]; cnt[j] = sum; sum += c; }
                for (int i = 0; i < len; i++) dst[cnt[(src[i] >> shift) & 0xFF]++] = src[i];
                int* t = src; src = dst; dst = t;
            }
        }

        public static void RunLong(long* ptr, int len)
        {
            if (len <= 1) return;
            long* temp = stackalloc long[len];
            int* src = (int*)ptr;
            int* dst = (int*)temp;
            for (int shift = 0; shift < 64; shift += 8)
            {
                int* cnt = stackalloc int[256];
                for (int j = 0; j < 256; j++) cnt[j] = 0;
                for (int i = 0; i < len; i++) cnt[(int)((((int*)ptr)[i] >> shift) & 0xFF)]++;
                int sum = 0;
                for (int j = 0; j < 256; j++) { int c = cnt[j]; cnt[j] = sum; sum += c; }
                for (int i = 0; i < len; i++) dst[cnt[(int)((((int*)ptr)[i] >> shift) & 0xFF)]++] = ((int*)ptr)[i];
                int* t = (int*)ptr; int* tt = dst; dst = t; t = tt;
            }
        }
    }
}
