namespace IAFahim.Search.LIS
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Lis
    {
        public static int Run(int* ptr, int len, int* result)
        {
            if (len == 0) return 0;
            int* tail = stackalloc int[len];
            int* prev = stackalloc int[len];
            int* idx = stackalloc int[len];
            for (int i = 0; i < len; i++) { tail[i] = 0; prev[i] = -1; idx[i] = 0; }
            int lisLen = 0;
            for (int i = 0; i < len; i++)
            {
                int pos = LowerBound(tail, lisLen, ptr[i]);
                if (pos == lisLen) lisLen++;
                tail[pos] = ptr[i];
                idx[pos] = i;
                prev[i] = pos > 0 ? idx[pos - 1] : -1;
            }
            if (result == null) return lisLen;
            int cur = idx[lisLen - 1];
            for (int i = lisLen - 1; i >= 0; i--) { result[i] = cur; cur = prev[cur]; }
            return lisLen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowerBound(int* tail, int len, int val)
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (tail[mid] >= val) hi = mid; else lo = mid + 1;
            }
            return lo;
        }

        public static int RunLong(long* ptr, int len, int* result)
        {
            if (len == 0) return 0;
            long* tail = stackalloc long[len];
            int* prev = stackalloc int[len];
            int* idx = stackalloc int[len];
            for (int i = 0; i < len; i++) { tail[i] = long.MaxValue; prev[i] = -1; idx[i] = 0; }
            int lisLen = 0;
            for (int i = 0; i < len; i++)
            {
                int pos = LowerBoundLong(tail, lisLen, ptr[i]);
                if (pos == lisLen) lisLen++;
                tail[pos] = ptr[i];
                idx[pos] = i;
                prev[i] = pos > 0 ? idx[pos - 1] : -1;
            }
            if (result == null) return lisLen;
            int cur = idx[lisLen - 1];
            for (int i = lisLen - 1; i >= 0; i--) { result[i] = cur; cur = prev[cur]; }
            return lisLen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowerBoundLong(long* tail, int len, long val)
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (tail[mid] >= val) hi = mid; else lo = mid + 1;
            }
            return lo;
        }
    }
}
