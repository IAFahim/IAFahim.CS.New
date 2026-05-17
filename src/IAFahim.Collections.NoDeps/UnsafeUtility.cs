namespace Unity.Collections.LowLevel.Unsafe
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class UnsafeUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>() where T : unmanaged
        {
            return sizeof(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignOf<T>() where T : unmanaged
        {
            return sizeof(AlignOfHelper<T>) - sizeof(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemCpy(void* destination, void* source, long size)
        {
            Buffer.MemoryCopy(source, destination, size, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemClear(void* destination, long size)
        {
            byte* p = (byte*)destination;
            for (long i = 0; i < size; i++)
                p[i] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemSet(void* destination, byte value, long size)
        {
            byte* p = (byte*)destination;
            for (long i = 0; i < size; i++)
                p[i] = value;
        }

        private struct AlignOfHelper<T> where T : unmanaged
        {
#pragma warning disable CS0169
            private byte Dummy;
#pragma warning restore CS0169
            public T Data;
        }
    }
}
