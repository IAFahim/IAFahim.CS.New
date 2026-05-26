namespace IAFahim.Memory.Allocators
{
    using System;
    using System.Diagnostics;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using BovineLabs.Core.Memory;

    public unsafe struct UnsafeFixedPoolAllocator<T> : IDisposable
        where T : unmanaged
    {
        private readonly int maxItems;
        private readonly AllocatorManager.AllocatorHandle allocator;

        private Ptr buffer;
        private UnsafeParallelHashSet<Ptr> freeIndex;

        public UnsafeFixedPoolAllocator(int maxItems, Allocator allocator)
        {
            ValidateSize(maxItems);

            this.maxItems = maxItems;
            this.allocator = allocator;
            this.freeIndex = new UnsafeParallelHashSet<Ptr>(maxItems, allocator);

            long totalSize = (long)UnsafeUtility.SizeOf<T>() * maxItems;
            this.buffer = Unmanaged.Allocate(totalSize, UnsafeUtility.AlignOf<T>(), allocator);

            for (int i = 0; i < maxItems; i++)
            {
                this.freeIndex.Add((T*)this.buffer + i);
            }
        }

        public bool IsCreated => this.buffer.Value != null;

        public T* Alloc()
        {
            if (this.freeIndex.Count() == 0)
            {
                return null;
            }

            using (NativeHashSet<Ptr>.Enumerator e = this.freeIndex.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    return null;
                }

                Ptr ptr = e.Current;
                this.freeIndex.Remove(ptr);
                return (T*)ptr;
            }
        }

        public void Free(T* p)
        {
            this.ValidatePtr(p);
            this.freeIndex.Add(p);
        }

        public void Dispose()
        {
            Unmanaged.Free(this.buffer, this.allocator);
            this.freeIndex.Dispose();
            this.buffer = Ptr.Zero;
            this.freeIndex = default;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void ValidateSize(int maxItems)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (maxItems <= 0)
            {
                throw new ArgumentException("Null pointer");
            }
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void ValidatePtr(T* p)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (p == null)
            {
                throw new ArgumentException("Null pointer");
            }

            if (p < this.buffer || p >= (T*)this.buffer + this.maxItems)
            {
                throw new ArgumentException("Ptr not from this allocator");
            }

            if (this.freeIndex.Contains(p))
            {
                throw new ArgumentException("Ptr already returned");
            }

            if (this.freeIndex.Count() == this.maxItems)
            {
                throw new ArgumentException("More free than in Buffer");
            }
#endif
        }
    }
}
