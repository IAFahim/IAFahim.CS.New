namespace Unity.Collections
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class AllocatorManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Allocate(Allocator allocator, int sizeInBytes, int alignInBytes)
        {
            return (void*)Marshal.AllocHGlobal(sizeInBytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(Allocator allocator, void* pointer)
        {
            if (pointer != null)
                Marshal.FreeHGlobal((IntPtr)pointer);
        }
    }
}
