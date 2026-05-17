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
        [NativeDisableUnsafePtrRestriction] public T* Ptr;

        public int Length;

        public UnsafeArray(int length, Allocator allocator)
        {
            this = default;
            this.Length = length;
            this.Allocator = allocator;
            this.Ptr = (T*)AllocatorManager.Allocate(allocator, length * UnsafeUtility.SizeOf<T>(),
                UnsafeUtility.AlignOf<T>());
            UnsafeUtility.MemClear(this.Ptr, length * UnsafeUtility.SizeOf<T>());
        }

        public Allocator Allocator { get; }

        public void Dispose()
        {
            if (this.Ptr != null)
            {
                AllocatorManager.Free(this.Allocator, this.Ptr);
                this.Ptr = null;
            }

            this = default;
        }
    }
}