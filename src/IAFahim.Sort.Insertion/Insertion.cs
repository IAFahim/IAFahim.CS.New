using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Sort.Insertion
{
    public static unsafe class Insertion
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            for (int i = 1; i < len; i++)
            {
                T key = ptr[i];
                int j = i - 1;
                while (j >= 0 && ptr[j].CompareTo(key) > 0)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = key;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunDescending<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            for (int i = 1; i < len; i++)
            {
                T key = ptr[i];
                int j = i - 1;
                while (j >= 0 && ptr[j].CompareTo(key) < 0)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = key;
            }
        }
    }
}
