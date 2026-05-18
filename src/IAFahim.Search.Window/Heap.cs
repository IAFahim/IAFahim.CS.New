namespace IAFahim.Search.Window
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Heap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Parent(int i) => (i - 1) >> 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Left(int i) => (i << 1) + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Right(int i) => (i << 1) + 2;

        public static void PushInt32(int* ptr, int len, int val)
        {
            int i = len;
            ptr[i] = val;
            while (i > 0)
            {
                int p = Parent(i);
                if (ptr[p] <= ptr[i]) break;
                int tmp = ptr[p];
                ptr[p] = ptr[i];
                ptr[i] = tmp;
                i = p;
            }
        }

        public static int PopInt32(int* ptr, int len)
        {
            int result = ptr[0];
            ptr[0] = ptr[len - 1];
            FixInt32(ptr, 0, len - 1);
            return result;
        }

        public static void FixInt32(int* ptr, int i, int len)
        {
            int smallest = i;
            int l = Left(i);
            int r = Right(i);
            if (l < len && ptr[l] < ptr[smallest]) smallest = l;
            if (r < len && ptr[r] < ptr[smallest]) smallest = r;
            if (smallest != i)
            {
                int tmp = ptr[i];
                ptr[i] = ptr[smallest];
                ptr[smallest] = tmp;
                FixInt32(ptr, smallest, len);
            }
        }

        public static void HeapifyInt32(int* ptr, int len)
        {
            for (int i = Parent(len - 1); i >= 0; i--)
                FixInt32(ptr, i, len);
        }
    }
}