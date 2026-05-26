namespace IAFahim.Memory.Allocators
{
    using System;
    using System.Diagnostics;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public unsafe struct UnsafePoolAllocator<T> : IDisposable
        where T : unmanaged
    {
        private UnsafeSlabAllocator<T> slabAllocator;
        private UnsafeParallelHashSet<Ptr> free;

        public UnsafePoolAllocator(int countPerChunk, Allocator allocator)
        {
            Debug.Assert(countPerChunk > 0);

            this.slabAllocator = new UnsafeSlabAllocator<T>(countPerChunk, allocator);
            this.free = new UnsafeParallelHashSet<Ptr>(0, allocator);
        }

        public bool IsCreated => this.slabAllocator.IsCreated;

        public void Dispose()
        {
            this.slabAllocator.Dispose();
            this.free.Dispose();
            this.slabAllocator = default;
            this.free = default;
        }

        public T* Alloc()
        {
            using (NativeHashSet<Ptr>.Enumerator e = this.free.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    return this.slabAllocator.Alloc();
                }

                Ptr ptr = e.Current;
                this.free.Remove(ptr);
                return (T*)ptr;
            }
        }

        public void Free(T* p)
        {
            this.free.Add(new Ptr(p));
        }

        public int Allocated()
        {
            return this.slabAllocator.Allocated();
        }
    }
}
