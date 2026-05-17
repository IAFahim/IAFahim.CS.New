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

        public static T Run<T>(T* ptr, int len) where T : unmanaged
        {
            if (len <= 1)
                return len == 1 ? ptr[0] : default;
            dynamic a = ptr[0], b = ptr[1];
            ptr[1] = (T)(object)((int)(object)a + (int)(object)b);
            for (int i = 2; i < len; i++)
            {
                dynamic prev = ptr[i - 1];
                dynamic cur = ptr[i];
                ptr[i] = (T)(object)((int)(object)prev + (int)(object)cur);
            }
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
            if (l == 0)
                return ptr[r];
            dynamic left = ptr[l - 1];
            dynamic right = ptr[r];
            return (T)(object)((long)(object)right ^ (long)(object)left);
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
            return ptr[0];
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