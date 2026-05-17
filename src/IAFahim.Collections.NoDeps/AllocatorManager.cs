namespace Unity.Collections
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public struct AllocatorHandle
    {
        public int Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AllocatorHandle(Allocator allocator)
        {
            return new AllocatorHandle { Value = (int)allocator };
        }
    }

    public static unsafe class AllocatorManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Allocate(AllocatorHandle allocator, int sizeInBytes, int alignInBytes)
        {
            return (void*)Marshal.AllocHGlobal(sizeInBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Allocate(AllocatorHandle allocator, void* ptr, int sizeInBytes, int alignInBytes)
        {
            // no-op resize hint variant
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(AllocatorHandle allocator, void* pointer)
        {
            if (pointer != null)
                Marshal.FreeHGlobal((IntPtr)pointer);
        }
    }
}
