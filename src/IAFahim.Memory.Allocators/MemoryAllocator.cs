namespace IAFahim.Memory.Allocators
{
    using System;
    using System.Diagnostics;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Mathematics;

    public unsafe struct MemoryAllocator : IDisposable
    {
        private NativeHashSet<Ptr> allocated;

        public MemoryAllocator(Allocator allocator)
        {
            this.Allocator = allocator;
            this.allocated = new NativeHashSet<Ptr>(0, allocator);
        }

        public Allocator Allocator { get; }

        public void* Allocate(int itemSizeInBytes, int alignmentInBytes, int items = 1)
        {
            void* ptr = AllocatorManager.Allocate(this.Allocator, itemSizeInBytes, alignmentInBytes, items);
            this.allocated.Add(ptr);
            return ptr;
        }

        public T* Create<T>(int count = 1)
            where T : unmanaged
        {
            Debug.Assert(count > 0);
            return (T*)this.Allocate(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), count);
        }

        public UnsafeList<T> CreateList<T>(int capacity)
            where T : unmanaged
        {
            int newCapacity = math.max(capacity, 64 / UnsafeUtility.SizeOf<T>());
            newCapacity = math.ceilpow2(newCapacity);

            T* buffer = this.Create<T>(newCapacity);

            return new UnsafeList<T>(buffer, newCapacity, Allocator.None);
        }

        public void FreeAll()
        {
            NativeArray<Ptr> array = this.allocated.ToNativeArray(Allocator.Temp);

            for (int i = 0; i < array.Length; i++)
            {
                Ptr ptr = array[i];
                AllocatorManager.Free(this.Allocator, ptr);
            }

            array.Dispose();
            this.allocated.Clear();
        }

        public void Dispose()
        {
            this.FreeAll();
            this.allocated.Dispose();
        }
    }
}
