namespace IAFahim.Memory.Allocators
{
    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs.LowLevel.Unsafe;
    using BovineLabs.Core.Memory;

    public unsafe struct UnsafeParallelPoolAllocator<T> : IDisposable
        where T : unmanaged
    {
        private readonly Allocator allocator;

        [NativeDisableUnsafePtrRestriction]
        private UnsafePoolAllocator<T>* pools;

        [NativeSetThreadIndex]
        private int threadIndex;

        public UnsafeParallelPoolAllocator(int countPerChunk, Allocator allocator)
        {
            this.allocator = allocator;
            long totalSize = (long)UnsafeUtility.SizeOf<UnsafePoolAllocator<T>>() * JobsUtility.ThreadIndexCount;
            this.pools = (UnsafePoolAllocator<T>*)Unmanaged.Allocate(totalSize, UnsafeUtility.AlignOf<UnsafePoolAllocator<T>>(), allocator);

            for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
            {
                this.pools[i] = new UnsafePoolAllocator<T>(countPerChunk, allocator);
            }

            this.threadIndex = 0;
        }

        public bool IsCreated => this.pools != null;

        public void Dispose()
        {
            for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
            {
                this.pools[i].Dispose();
            }

            Unmanaged.Free(this.pools, this.allocator);
            this.pools = null;
        }

        public T* Alloc()
        {
            return this.pools[this.threadIndex].Alloc();
        }

        public void Free(T* p)
        {
            this.pools[this.threadIndex].Free(p);
        }

        public int Allocated()
        {
            int allocated = 0;

            for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
            {
                allocated += this.pools[i].Allocated();
            }

            return allocated;
        }
    }
}
