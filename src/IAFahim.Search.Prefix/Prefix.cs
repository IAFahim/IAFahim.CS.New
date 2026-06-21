using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Prefix
{
    public static unsafe class PrefixSums
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* ptr, int len)
        {
            if (len <= 0)
                return 0;
            for (int i = 1; i < len; i++)
                ptr[i] = ptr[i] + ptr[i - 1];
            return ptr[len - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* ptr, int len)
        {
            if (len <= 0)
                return 0;
            for (int i = 1; i < len; i++)
                ptr[i] = ptr[i] + ptr[i - 1];
            return ptr[len - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Sum<T>(T* ptr, int len) where T : unmanaged
        {
            return len > 0 ? ptr[len - 1] : default;
        }
    }

    public static unsafe class PrefixXor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* ptr, int len)
        {
            if (len <= 0)
                return 0;
            for (int i = 1; i < len; i++)
                ptr[i] = ptr[i] ^ ptr[i - 1];
            return ptr[len - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* ptr, int len)
        {
            if (len <= 0)
                return 0;
            for (int i = 1; i < len; i++)
                ptr[i] = ptr[i] ^ ptr[i - 1];
            return ptr[len - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RangeXor<T>(T* ptr, int l, int r) where T : unmanaged
        {
            if (l > r || l < 0)
                return default;
            int sz = sizeof(T);
            byte* a = (byte*)(ptr + r);
            if (l == 0)
            {
                T result = default;
                byte* dst = (byte*)&result;
                for (int k = 0; k < sz; k++) dst[k] = a[k];
                return result;
            }
            byte* b = (byte*)(ptr + (l - 1));
            {
                T result = default;
                byte* dst = (byte*)&result;
                for (int k = 0; k < sz; k++) dst[k] = (byte)(a[k] ^ b[k]);
                return result;
            }
        }
    }

    public static unsafe class PrefixMin
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 0)
                return default;
            for (int i = 1; i < len; i++)
            {
                if (ptr[i].CompareTo(ptr[i - 1]) > 0)
                    ptr[i] = ptr[i - 1];
            }
            return ptr[len - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinIndex<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 0)
                return -1;
            int idx = 0;
            for (int i = 1; i < len; i++)
            {
                if (ptr[i].CompareTo(ptr[idx]) < 0)
                    idx = i;
            }
            return idx;
        }
    }

    public static unsafe class PrefixMax
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 0)
                return default;
            for (int i = 1; i < len; i++)
            {
                if (ptr[i].CompareTo(ptr[i - 1]) < 0)
                    ptr[i] = ptr[i - 1];
            }
            return ptr[len - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxIndex<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 0)
                return -1;
            int idx = 0;
            for (int i = 1; i < len; i++)
            {
                if (ptr[i].CompareTo(ptr[idx]) > 0)
                    idx = i;
            }
            return idx;
        }
    }
}