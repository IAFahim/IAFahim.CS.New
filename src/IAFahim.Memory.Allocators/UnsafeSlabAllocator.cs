namespace IAFahim.Memory.Allocators
{
    using System;
    using System.Diagnostics;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using BovineLabs.Core.Memory;

    public unsafe struct UnsafeSlabAllocator<T> : IDisposable
        where T : unmanaged
    {
        private readonly int countPerSlab;
        private readonly AllocatorManager.AllocatorHandle allocator;

        private UnsafeList<Ptr>* slabs;

        [NativeDisableUnsafePtrRestriction]
        private int* count;

        public UnsafeSlabAllocator(int countPerSlab, AllocatorManager.AllocatorHandle allocator)
        {
            Debug.Assert(countPerSlab > 0);

            this.slabs = UnsafeList<Ptr>.Create(0, allocator);
            this.allocator = allocator;
            this.countPerSlab = countPerSlab;

            this.count = (int*)Unmanaged.Allocate((long)UnsafeUtility.SizeOf<int>(), UnsafeUtility.AlignOf<int>(), allocator);
            *this.count = countPerSlab;
        }

        public int AllocationCount => (int)((long)this.countPerSlab * (this.slabs->Length - 1)) + *this.count;

        public bool IsCreated => this.count != null;

        public T* Alloc()
        {
            if (*this.count == this.countPerSlab)
            {
                *this.count = 0;
                Ptr ptr = (Ptr)Unmanaged.Allocate((long)this.countPerSlab * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), this.allocator);
                this.slabs->Add(ptr);
            }

            T* lastSlab = (T*)(*this.slabs)[this.slabs->Length - 1];
            return lastSlab + (*this.count)++;
        }

        public void Clear()
        {
            for (int i = 0; i < this.slabs->Length; i++)
            {
                Unmanaged.Free((*this.slabs)[i], this.allocator);
            }

            this.slabs->Clear();
            *this.count = this.countPerSlab;
        }

        public void Dispose()
        {
            this.Clear();
            UnsafeList<Ptr>.Destroy(this.slabs);

            Unmanaged.Free(this.count, this.allocator);

            this.count = default;
            this.slabs = default;
        }

        public int Allocated()
        {
            return (int)((long)this.slabs->Length * this.countPerSlab * UnsafeUtility.SizeOf<T>());
        }
    }
}
