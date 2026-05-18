namespace IAFahim.DS.UnsafeArray
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    public unsafe struct UnsafeArray<T> : IDisposable where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction]
        public T* Ptr;

        public readonly int Length;

        public readonly Allocator Allocator;

        public UnsafeArray(int length, Allocator allocator)
        {
            long byteCount = (long)length * UnsafeUtility.SizeOf<T>();
            this = default;
            Length = length;
            Allocator = allocator;
            Ptr = (T*)AllocatorManager.Allocate(allocator, byteCount, UnsafeUtility.AlignOf<T>());
            UnsafeUtility.MemClear(Ptr, byteCount);
        }

        public void Dispose()
        {
            if (Ptr != null)
            {
                AllocatorManager.Free(Allocator, Ptr);
            }
            this = default;
        }
    }
}