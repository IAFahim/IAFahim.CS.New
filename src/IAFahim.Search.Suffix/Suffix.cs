using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Suffix
{
    public static unsafe class SuffixSums
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* ptr, int len)
        {
            if (len <= 0)
                return 0;
            for (int i = len - 2; i >= 0; i--)
                ptr[i] = ptr[i] + ptr[i + 1];
            return ptr[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* ptr, int len)
        {
            if (len <= 0)
                return 0;
            for (int i = len - 2; i >= 0; i--)
                ptr[i] = ptr[i] + ptr[i + 1];
            return ptr[0];
        }

        public static T Run<T>(T* ptr, int len) where T : unmanaged
        {
            if (len <= 1)
                return len == 1 ? ptr[0] : default;
            for (int i = len - 2; i >= 0; i--)
            {
                dynamic a = ptr[i], b = ptr[i + 1];
                ptr[i] = (T)(object)((int)(object)a + (int)(object)b);
            }
            return ptr[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Sum<T>(T* ptr, int len) where T : unmanaged
        {
            return len > 0 ? ptr[0] : default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RangeSum<T>(T* ptr, int l, int r) where T : unmanaged
        {
            if (l > r || l < 0)
                return default;
            for (int i = r - 1; i >= l; i--)
            {
                dynamic a = ptr[i], b = ptr[i + 1];
                ptr[i] = (T)(object)((int)(object)a + (int)(object)b);
            }
            T lr = l > 0 ? ptr[l - 1] : default;
            dynamic result = ptr[r];
            dynamic left = lr;
            return (T)(object)((int)(object)result - (int)(object)left);
        }
    }

    public static unsafe class SuffixMin
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 0)
                return default;
            for (int i = len - 2; i >= 0; i--)
            {
                if (ptr[i].CompareTo(ptr[i + 1]) > 0)
                    ptr[i] = ptr[i + 1];
            }
            return ptr[len - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinIndex<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 0)
                return -1;
            int minIdx = len - 1;
            for (int i = len - 2; i >= 0; i--)
            {
                if (ptr[i].CompareTo(ptr[minIdx]) < 0)
                    minIdx = i;
            }
            return minIdx;
        }
    }

    public static unsafe class SuffixMax
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 0)
                return default;
            for (int i = len - 2; i >= 0; i--)
            {
                if (ptr[i].CompareTo(ptr[i + 1]) < 0)
                    ptr[i] = ptr[i + 1];
            }
            return ptr[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxIndex<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 0)
                return -1;
            int maxIdx = len - 1;
            for (int i = len - 2; i >= 0; i--)
            {
                if (ptr[i].CompareTo(ptr[maxIdx]) > 0)
                    maxIdx = i;
            }
            return maxIdx;
        }
    }
}